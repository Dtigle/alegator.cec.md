Option Explicit On
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports Pr22.Processing
Imports Microsoft.VisualBasic

Namespace ScannerSample
    Partial Public Class Form1
        Inherits Form
        ReadOnly pr As Pr22.DocumentReaderDevice
        Private DeviceIsConnected As Boolean
        Private ScanCtrl As Pr22.Task.TaskControl
        Private AnalyzeResult As Pr22.Processing.Document

        Public Sub New()
            InitializeComponent()
            Try
                pr = New Pr22.DocumentReaderDevice()
            Catch ex As Exception
                If TypeOf ex Is DllNotFoundException Or TypeOf ex Is Pr22.Exceptions.FileOpen Then

                    Dim platform As Integer = IntPtr.Size * 8
                    Dim codepl As Integer = GetCodePlatform()

                    MessageBox.Show("This sample program" & CStr(IIf(codepl = 0, " is compiled for Any CPU and", "")) & _
                                    " is running on " & platform & " bit platform." & vbCr & _
                                    "Please check if the Passport Reader is installed correctly or compile your code for " & _
                                    (96 - platform) & " bit." & vbCr & ex.Message, _
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.[Stop])
                Else
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Stop])
                End If
            End Try
        End Sub

        Private Sub FormLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If pr Is Nothing Then
                Close() : Return
            End If

            AddHandler pr.Connection, AddressOf DeviceConnected

            AddHandler pr.PresenceStateChanged, AddressOf DocumentStateChanged
            AddHandler pr.ImageScanned, AddressOf ImageScanned
            AddHandler pr.ScanFinished, AddressOf ScanFinished
            AddHandler pr.DocFrameFound, AddressOf DocFrameFound
        End Sub

        Private Sub FormClose(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
            If DeviceIsConnected Then
                CloseScan()
                pr.Close()
            End If
        End Sub

#Region "Connection"

        ' This raises only when no device is used or when the currently used
        ' device is disconnected.
        Private Sub DeviceConnected(ByVal sender As Object, ByVal e As Pr22.Events.ConnectionEventArgs)
            UpdateDeviceList()
        End Sub

        Private Sub UpdateDeviceList()
            If InvokeRequired Then
                BeginInvoke(New MethodInvoker(AddressOf UpdateDeviceList))
                Return
            End If
            Dim Devices As List(Of String) = Pr22.DocumentReaderDevice.GetDeviceList()
            DevicesListBox.Items.Clear()
            For Each s As String In Devices : DevicesListBox.Items.Add(s) : Next
        End Sub

        Private Sub ConnectButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ConnectButton.Click
            If DevicesListBox.SelectedIndex < 0 Then Return

            ConnectButton.Enabled = False
            Cursor = Cursors.WaitCursor
            Try
                pr.UseDevice(DevicesListBox.Text)
                DeviceIsConnected = True
                pr.Scanner.StartTask(Pr22.Task.FreerunTask.Detection())
                DisconnectButton.Enabled = True
                Dim Lights As List(Of Pr22.Imaging.Light) = pr.Scanner.Info.GetLights()
                For Each light As Pr22.Imaging.Light In Lights
                    LightsCheckedListBox.Items.Add(light)
                Next
                StartButton.Enabled = True
            Catch ex As Pr22.Exceptions.General
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Stop])
                DisconnectButton_Click(sender, e)
            End Try
            Cursor = Cursors.[Default]
        End Sub

        Private Sub DisconnectButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DisconnectButton.Click
            If DeviceIsConnected Then
                CloseScan()
                Application.DoEvents()
                pr.Close()
                DeviceIsConnected = False
            End If
            ConnectButton.Enabled = True
            DisconnectButton.Enabled = False
            StartButton.Enabled = False
            LightsCheckedListBox.Items.Clear()
            FieldsTabControl.Controls.Clear()
            FieldsTabControl.Controls.Add(OcrTab)
            FieldsTabControl.Controls.Add(DataTab)
            FieldsDataGridView.Rows.Clear()
            ClearOCRData()
            ClearDataPage()
        End Sub

#End Region

#Region "Scanning"

        ' To raise this event FreerunTask.Detection() has to be started.
        Private Sub DocumentStateChanged(ByVal sender As Object, ByVal e As Pr22.Events.DetectionEventArgs)
            If e.State = Pr22.Util.PresenceState.Present Then
                BeginInvoke(New EventHandler(AddressOf StartButton_Click), sender, e)
            End If
        End Sub

        Private Sub StartButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles StartButton.Click
            FieldsTabControl.Controls.Clear()
            FieldsTabControl.Controls.Add(OcrTab)
            FieldsTabControl.Controls.Add(DataTab)
            FieldsDataGridView.Rows.Clear()
            ClearOCRData()
            ClearDataPage()
            If LightsCheckedListBox.CheckedItems.Count = 0 Then
                MessageBox.Show("No light selected to scan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            StartButton.Enabled = False
            Dim ScanTask As New Pr22.Task.DocScannerTask()
            For Each light As Pr22.Imaging.Light In LightsCheckedListBox.CheckedItems
                AddTabPage(light.ToString())
                ScanTask.Add(light)
            Next
            ScanCtrl = pr.Scanner.StartScanning(ScanTask, Pr22.Imaging.PagePosition.First)
        End Sub

        Private Sub ImageScanned(ByVal sender As Object, ByVal e As Pr22.Events.ImageEventArgs)
            DrawImage(e)
        End Sub

        ' To rotate the document to upside down direction the Analyze() should
        ' be called.
        Private Sub DocFrameFound(ByVal sender As Object, ByVal e As Pr22.Events.PageEventArgs)
            If Not DocViewCheckBox.Checked Then Return
            For Each tab As Control In FieldsTabControl.Controls
                Try
                    Dim light As Pr22.Imaging.Light = DirectCast([Enum].Parse(GetType(Pr22.Imaging.Light), tab.Text), Pr22.Imaging.Light)
                    If DirectCast(tab.Controls(0), PictureBox).Image IsNot Nothing Then
                        DrawImage(New Pr22.Events.ImageEventArgs(e.Page, light))
                    End If
                Catch ex As System.ArgumentException
                End Try
            Next
        End Sub

        Private Sub DrawImage(ByVal e As Pr22.Events.ImageEventArgs)
            If InvokeRequired Then
                BeginInvoke(New Action(Of Pr22.Events.ImageEventArgs)(AddressOf DrawImage), e)
                Return
            End If
            Dim docImage As Pr22.Imaging.DocImage = pr.Scanner.GetPage(e.Page).[Select](e.Light)
            Dim tabs As Control() = FieldsTabControl.Controls.Find(e.Light.ToString(), False)
            'If tabs.Length = 0 Then tabs = AddTabPage(e.Light.ToString())
            If tabs.Length = 0 Then Return
            Dim pb As PictureBox = DirectCast(tabs(0).Controls(0), PictureBox)
            Dim bmap As Bitmap = docImage.ToBitmap()
            If DocViewCheckBox.Checked Then
                Try
                    bmap = docImage.DocView().ToBitmap()
                Catch ex As Pr22.Exceptions.General
                End Try
            End If
            pb.SizeMode = PictureBoxSizeMode.Zoom
            pb.Image = bmap
            pb.Refresh()
        End Sub

        Private Sub ScanFinished(ByVal sender As Object, ByVal e As Pr22.Events.PageEventArgs)
            BeginInvoke(New MethodInvoker(AddressOf Analyze))
            BeginInvoke(New MethodInvoker(AddressOf CloseScan))
        End Sub

        Private Sub CloseScan()
            Try
                If ScanCtrl IsNot Nothing Then ScanCtrl.Wait()
            Catch ex As Pr22.Exceptions.General
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Stop])
            End Try
            ScanCtrl = Nothing
            StartButton.Enabled = True
        End Sub

#End Region

#Region "Analyzing"

        Private Sub Analyze()
            Dim OcrTask As New Pr22.Task.EngineTask()

            If OCRParamsCheckedListBox.GetItemCheckState(0) = CheckState.Checked Then
                OcrTask.Add(FieldSource.Mrz, FieldId.All)
            End If
            If OCRParamsCheckedListBox.GetItemCheckState(1) = CheckState.Checked Then
                OcrTask.Add(FieldSource.Viz, FieldId.All)
            End If
            If OCRParamsCheckedListBox.GetItemCheckState(2) = CheckState.Checked Then
                OcrTask.Add(FieldSource.Barcode, FieldId.All)
            End If

            Dim page As Pr22.Processing.Page
            Try
                page = pr.Scanner.GetPage(0)
            Catch ex As Pr22.Exceptions.General
                Return
            End Try
            Try
                AnalyzeResult = pr.Engine.Analyze(page, OcrTask)
            Catch ex As Pr22.Exceptions.General
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Stop])
                Return
            End Try
            FillOcrDataGrid()
            FillDataPage()
        End Sub

        Private Sub FillOcrDataGrid()
            Dim Fields As List(Of Pr22.Processing.FieldReference) = AnalyzeResult.GetFields()
            For i As Integer = 0 To Fields.Count - 1
                Try
                    Dim field As Pr22.Processing.Field = AnalyzeResult.GetField(Fields(i))
                    Dim values As String() = New String(3) {}
                    values(0) = i.ToString()
                    values(1) = Fields(i).ToString(" ") + New StrCon() + GetAmid(field)
                    Try
                        values(2) = field.GetBestStringValue()
                    Catch ex As Pr22.Exceptions.InvalidParameter
                        values(2) = PrintBinary(field.GetBinaryValue(), 0, 16)
                    Catch ex As Pr22.Exceptions.General
                    End Try
                    values(3) = field.GetStatus().ToString()

                    FieldsDataGridView.Rows.Add(values)
                Catch ex As Pr22.Exceptions.General
                End Try
            Next
        End Sub

        Private Sub FieldsDataGridView_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs) Handles FieldsDataGridView.SelectionChanged
            ClearOCRData()
            If FieldsDataGridView.SelectedCells.Count = 0 Then Return
            Dim ix As Integer = FieldsDataGridView.SelectedCells(0).RowIndex
            If AnalyzeResult Is Nothing OrElse ix < 0 OrElse AnalyzeResult.GetFields().Count <= ix _
            OrElse ix = FieldsDataGridView.Rows.Count - 1 Then Return

            ix = Integer.Parse(FieldsDataGridView.Rows(ix).Cells(0).Value.ToString())
            Dim SelectedField As Pr22.Processing.FieldReference = AnalyzeResult.GetFields()(ix)
            Dim field As Pr22.Processing.Field = AnalyzeResult.GetField(SelectedField)
            Try
                RAWValueLabel.Text = field.GetRawStringValue()
            Catch ex As Pr22.Exceptions.General
            End Try
            Try
                FormattedValueLabel.Text = field.GetFormattedStringValue()
            Catch ex As Pr22.Exceptions.General
            End Try
            Try
                StandardizedValueLabel.Text = field.GetStandardizedStringValue()
            Catch ex As Pr22.Exceptions.General
            End Try
            Try
                FieldImagePictureBox.Image = field.GetImage().ToBitmap()
            Catch ex As Pr22.Exceptions.General
            End Try
        End Sub

        Private Sub FillDataPage()
            Name1.Text = GetFieldValue(FieldId.Surname)
            If Name1.Text <> "" Then
                Name1.Text += " " + GetFieldValue(FieldId.Surname2)
                Name2.Text = GetFieldValue(FieldId.Givenname) + New StrCon() _
                + GetFieldValue(FieldId.MiddleName)
            Else
                Name1.Text = GetFieldValue(FieldId.Name)
            End If

            Birth.Text = New StrCon("on") + GetFieldValue(FieldId.BirthDate) _
            + New StrCon("in") + GetFieldValue(FieldId.BirthPlace)

            Nationality.Text = GetFieldValue(FieldId.Nationality)

            Sex.Text = GetFieldValue(FieldId.Sex)

            Issuer.Text = GetFieldValue(FieldId.IssueCountry) + New StrCon() _
            + GetFieldValue(FieldId.IssueState)

            Type.Text = GetFieldValue(FieldId.DocType) + New StrCon() _
            + GetFieldValue(FieldId.DocTypeDisc)
            If Type.Text = "" Then Type.Text = GetFieldValue(FieldId.Type)

            Page.Text = GetFieldValue(FieldId.DocPage)

            Number.Text = GetFieldValue(FieldId.DocumentNumber)

            Validity.Text = New StrCon("from") + GetFieldValue(FieldId.IssueDate) _
            + New StrCon("to") + GetFieldValue(FieldId.ExpiryDate)

            Try
                PhotoPictureBox.Image = AnalyzeResult.GetField(FieldSource.Viz, _
                FieldId.Face).GetImage().ToBitmap()
            Catch ex As Pr22.Exceptions.General
            End Try

            Try
                SignaturePictureBox.Image = AnalyzeResult.GetField(FieldSource.Viz, _
                FieldId.Signature).GetImage().ToBitmap()
            Catch ex As Pr22.Exceptions.General
            End Try
        End Sub

#End Region

#Region "General tools"

        Private Function GetAmid(ByVal field As Field) As String
            Try
                Return field.ToVariant().GetChild(Pr22.Util.VariantId.AMID, 0)
            Catch ex As Pr22.Exceptions.General
                Return ""
            End Try
        End Function

        Private Function GetFieldValue(ByVal Id As Pr22.Processing.FieldId) As String

            Dim filter As FieldReference = New FieldReference(FieldSource.All, Id)
            Dim Fields As List(Of FieldReference) = AnalyzeResult.GetFields(filter)
            For Each FR As FieldReference In Fields
                Dim value As String = AnalyzeResult.GetField(FR).GetBestStringValue()
                If value <> "" Then Return value
            Next
            Return ""
        End Function

        Private Shared Function PrintBinary(ByVal arr As Byte(), ByVal pos As Integer, ByVal sz As Integer) As String

            Dim p0 As Integer = pos
            Dim str As String = "", str2 As String = ""

            While p0 < arr.Length AndAlso p0 < pos + sz
                str += arr(p0).ToString("X2") + " "
                str2 += CChar(IIf(arr(p0) < 33 OrElse arr(p0) > 126, "."c, ChrW(arr(p0))))
                p0 += 1
            End While

            While p0 < pos + sz
                str += "   " : str2 += " " : p0 += 1
            End While

            Return str + str2
        End Function

        Private Function AddTabPage(ByVal lightName As String) As Control()
            Dim ImageTabPage As New TabPage(lightName)
            ImageTabPage.Name = lightName
            Dim PBox As New PictureBox()
            ImageTabPage.Controls.Add(PBox)
            FieldsTabControl.Controls.Add(ImageTabPage)
            PBox.Size = ImageTabPage.Size
            Return New Control(0) {ImageTabPage}
        End Function

        Private Sub ClearOCRData()
            FieldImagePictureBox.Image = Nothing
            RAWValueLabel.Text = ""
            FormattedValueLabel.Text = ""
            StandardizedValueLabel.Text = ""
        End Sub

        Private Sub ClearDataPage()
            Name1.Text = "" : Name2.Text = "" : Birth.Text = "" : Nationality.Text = "" : Sex.Text = ""
            Issuer.Text = "" : Type.Text = "" : Page.Text = "" : Number.Text = "" : Validity.Text = ""
            PhotoPictureBox.Image = Nothing
            SignaturePictureBox.Image = Nothing
        End Sub

        Private Function GetCodePlatform() As Integer
            Dim pek As System.Reflection.PortableExecutableKinds
            Dim mac As System.Reflection.ImageFileMachine
            System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.GetPEKind(pek, mac)

            If (pek And System.Reflection.PortableExecutableKinds.PE32Plus) <> 0 Then Return 64
            If (pek And System.Reflection.PortableExecutableKinds.Required32Bit) <> 0 Then Return 32
            Return 0
        End Function

#End Region

    End Class

    ''' <summary>
    ''' This class makes string concatenation with spaces and prefixes.
    ''' </summary>
    Public Class StrCon
        Private fstr As String = ""
        Private [cstr] As String = ""

        Public Sub New()
        End Sub

        Public Sub New(ByVal bounder As String)
            [cstr] = bounder + " "
        End Sub

        Public Shared Operator +(ByVal csv As StrCon, ByVal str As String) As String
            If str <> "" Then str = csv.[cstr] + str
            If csv.fstr <> "" AndAlso str <> "" AndAlso str(0) <> ","c Then csv.fstr += " "
            Return csv.fstr + str
        End Operator

        Public Shared Operator +(ByVal str As String, ByVal csv As StrCon) As StrCon
            csv.fstr = str
            Return csv
        End Operator
    End Class
End Namespace

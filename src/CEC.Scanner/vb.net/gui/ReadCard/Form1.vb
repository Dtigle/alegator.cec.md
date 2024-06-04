Option Explicit On
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports Pr22.Processing
Imports Microsoft.VisualBasic

Namespace ReadCard
    Partial Public Class Form1
        Inherits Form
        ReadOnly pr As Pr22.DocumentReaderDevice
        Private DeviceIsConnected As Boolean
        Private ReadCtrl As Pr22.Task.TaskControl
        Private Card As Pr22.ECard
        Private VizResult As Pr22.Processing.Document
        Private FaceDoc As Pr22.Processing.Document

        Delegate Sub DualParamInvoker(Of t1, t2)(ByVal item As t1, ByVal value As t2)

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

            AddHandler pr.AuthBegin, AddressOf AuthBegin
            AddHandler pr.AuthFinished, AddressOf AuthFinished
            AddHandler pr.AuthWaitForInput, AddressOf AuthWaitForInput
            AddHandler pr.ReadBegin, AddressOf ReadBegin
            AddHandler pr.ReadFinished, AddressOf ReadFinished
            AddHandler pr.FileChecked, AddressOf FileChecked

            Dim values As String() = [Enum].GetNames(GetType(Pr22.ECardHandling.FileId))
            For Each file As String In values
                FilesListView.Items.Add(file)
            Next

            For Each level As Pr22.ECardHandling.AuthLevel In [Enum].GetValues(GetType(Pr22.ECardHandling.AuthLevel))
                AuthSelector.Items.Add(level)
            Next

            AuthSelector.SelectedIndex = 1

            LoadCertificates(My.Settings.CertDir)
        End Sub

        Private Sub FormClose(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
            If ReadCtrl IsNot Nothing Then ReadCtrl.Stop().Wait()
            If DeviceIsConnected Then pr.Close()
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
                DisconnectButton.Enabled = True
                Dim Readers As List(Of Pr22.ECardReader) = pr.Readers
                For Each reader As Pr22.ECardReader In Readers
                    ReadersCheckedListBox.Items.Add(reader.Info.HwType.ToString())
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
                If ReadCtrl IsNot Nothing Then
                    ReadCtrl.Stop().Wait()
                    ReadCtrl = Nothing
                    Application.DoEvents()
                End If
                If Card IsNot Nothing Then
                    Card.Disconnect()
                    Card = Nothing
                End If
                pr.Close()
                DeviceIsConnected = False
            End If
            ConnectButton.Enabled = True
            DisconnectButton.Enabled = False
            StartButton.Enabled = False
            ReadersCheckedListBox.Items.Clear()
            textBox1.Clear()
        End Sub

#End Region

#Region "Reading"

        Private Sub StartButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles StartButton.Click
            textBox1.Clear()
            ClearControls()

            If ReadCtrl IsNot Nothing Then
                ReadCtrl.Wait()
                ReadCtrl = Nothing
            End If
            If Card IsNot Nothing Then
                Try
                    Card.Disconnect()
                Catch ex As Pr22.Exceptions.General
                End Try
                Card = Nothing
            End If

            Dim Reader As Pr22.ECardReader = Nothing
            For Each rdr As Pr22.ECardReader In pr.Readers
                If ReadersCheckedListBox.CheckedItems.Contains(rdr.Info.HwType.ToString()) Then
                    Dim Cards As List(Of String) = rdr.GetCards()
                    If Cards.Count > 0 Then
                        Card = rdr.ConnectCard(0)
                        Reader = rdr
                        Exit For
                    End If
                End If
            Next
            If Reader IsNot Nothing AndAlso Card IsNot Nothing Then
                StartReading(Reader)
            End If
        End Sub

        Private Sub StartReading(ByVal Reader As Pr22.ECardReader)
            ClearControls()
            StartButton.Enabled = False

            LogText("Scanning")
            Dim ScanTask As New Pr22.Task.DocScannerTask()
            ScanTask.Add(Pr22.Imaging.Light.Infra).Add(Pr22.Imaging.Light.White)
            Dim page As Pr22.Processing.Page = pr.Scanner.Scan(ScanTask, Pr22.Imaging.PagePosition.First)

            LogText("Analyzing")
            Dim EngineTask As New Pr22.Task.EngineTask()
            EngineTask.Add(FieldSource.Mrz, FieldId.All)
            EngineTask.Add(FieldSource.Viz, FieldId.CAN)

            Dim FaceFieldId, SignatureFieldId As Pr22.Processing.FieldReference
            FaceFieldId = New FieldReference(FieldSource.Viz, FieldId.Face)
            EngineTask.Add(FaceFieldId)
            SignatureFieldId = New FieldReference(FieldSource.Viz, FieldId.Signature)
            EngineTask.Add(SignatureFieldId)
            VizResult = pr.Engine.Analyze(page, EngineTask)
            FaceDoc = Nothing

            Try
                DrawImage(FacePictureBox2, VizResult.GetField(FaceFieldId).GetImage().ToBitmap())
            Catch ex As Pr22.Exceptions.General
            End Try
            Try
                DrawImage(SignaturePictureBox2, VizResult.GetField(SignatureFieldId).GetImage().ToBitmap())
            Catch ex As Pr22.Exceptions.General
            End Try

            Dim task As New Pr22.Task.ECardTask()
            task.AuthLevel = DirectCast(AuthSelector.SelectedItem, Pr22.ECardHandling.AuthLevel)

            For Each item As ListViewItem In FilesListView.CheckedItems
                task.Add(DirectCast([Enum].Parse(GetType(Pr22.ECardHandling.FileId), item.Text), Pr22.ECardHandling.FileId))
            Next

            Try
                ReadCtrl = Reader.StartRead(Card, task)
            Catch ex As Pr22.Exceptions.General
            End Try
        End Sub

        Private Sub AuthBegin(ByVal sender As Object, ByVal e As Pr22.Events.AuthEventArgs)
            LogText("Auth Begin: " + e.Authentication.ToString())
            ColorAuthLabel(e.Authentication, Color.Yellow)
        End Sub

        Private Sub AuthFinished(ByVal sender As Object, ByVal e As Pr22.Events.AuthEventArgs)
            Dim errstr As String = e.Result.ToString()
            If Not [Enum].IsDefined(GetType(Pr22.Exceptions.ErrorCodes), e.Result) Then
                errstr = CInt(e.Result).ToString("X4")
            End If

            LogText("Auth Done: " + e.Authentication.ToString() + " status: " + errstr)
            Dim ok As Boolean = e.Result = Pr22.Exceptions.ErrorCodes.ENOERR
            ColorAuthLabel(e.Authentication, CType(IIf(ok, Color.Green, Color.Red), Color))
        End Sub

        Private Sub AuthWaitForInput(ByVal sender As Object, ByVal e As Pr22.Events.AuthEventArgs)
            LogText("Auth Wait For Input: " + e.Authentication.ToString())
            ColorAuthLabel(e.Authentication, Color.Yellow)

            Dim AuthData As Pr22.Processing.BinData = Nothing
            Dim selector As Integer = 0

            Select Case e.Authentication
                Case Pr22.ECardHandling.AuthProcess.BAC, Pr22.ECardHandling.AuthProcess.BAP, Pr22.ECardHandling.AuthProcess.PACE

                    Dim authFields As List(Of Pr22.Processing.FieldReference)
                    Dim fr As Pr22.Processing.FieldReference
                    fr = New FieldReference(FieldSource.Mrz, FieldId.All)
                    authFields = VizResult.GetFields(fr)
                    selector = 1
                    If authFields.Count = 0 Then
                        fr = New FieldReference(FieldSource.Viz, FieldId.CAN)
                        authFields = VizResult.GetFields(fr)
                        selector = 2
                    End If
                    If authFields.Count = 0 Then Exit Select

                    AuthData = New Pr22.Processing.BinData()
                    AuthData.SetString(VizResult.GetField(fr).GetBestStringValue())
                    Exit Select
            End Select
            Try
                Card.Authenticate(e.Authentication, AuthData, selector)
            Catch ex As Pr22.Exceptions.General
            End Try
        End Sub

        Private Sub ReadBegin(ByVal sender As Object, ByVal e As Pr22.Events.FileEventArgs)
            LogText("Read Begin: " + e.FileId.ToString())
        End Sub

        Private Sub ReadFinished(ByVal sender As Object, ByVal e As Pr22.Events.FileEventArgs)
            Dim errstr As String = e.Result.ToString()
            If Not [Enum].IsDefined(GetType(Pr22.Exceptions.ErrorCodes), e.Result) Then
                errstr = CInt(e.Result).ToString("X4")
            End If

            LogText("Read End: " + e.FileId.ToString() + " result: " + errstr)

            If e.FileId.Id = CInt(Pr22.ECardHandling.FileId.All) Then
                ProcessAfterAllRead()
                SetStartButton(True)
            ElseIf e.Result <> Pr22.Exceptions.ErrorCodes.ENOERR Then
                ColorFileName(e.FileId, Color.Red)
            Else
                ColorFileName(e.FileId, Color.Blue)
                ProcessAfterFileRead(e.FileId)
            End If
        End Sub

        Private Sub ProcessAfterAllRead()
            Try
                Dim mrz As String = VizResult.GetField(FieldSource.Mrz, FieldId.All).GetRawStringValue()
                Dim dg1 As String = MRZTextBox.Text.Replace(vbCr, "")
                If dg1.Length > 40 Then
                    ColorLabel(MrzLabel, CType(IIf(mrz = dg1, Color.Green, Color.Red), Color))
                End If
            Catch ex As Pr22.Exceptions.General
            End Try
            Try
                Dim facecmp As Pr22.Processing.Document = VizResult + FaceDoc
                Dim fcl As List(Of Pr22.Processing.FieldCompare) = facecmp.GetFieldCompareList()
                For Each fc As Pr22.Processing.FieldCompare In fcl
                    If fc.field1.Id = FieldId.Face AndAlso fc.field2.Id = FieldId.Face Then
                        Dim col As Color = Color.Yellow
                        If fc.confidence < 300 Then : col = Color.Red
                        ElseIf fc.confidence > 600 Then : col = Color.Green
                        End If
                        ColorLabel(FaceLabel, col)
                    End If
                Next
            Catch ex As Pr22.Exceptions.General
            End Try
        End Sub

        Private Sub ProcessAfterFileRead(ByVal File As Pr22.ECardHandling.File)
            Try
                Dim RawFileContent As Pr22.Processing.BinData = Card.GetFile(File)
                Dim FileDoc As Pr22.Processing.Document = pr.Engine.Analyze(RawFileContent)

                Dim FaceFieldId As Pr22.Processing.FieldReference = New FieldReference(FieldSource.ECard, FieldId.Face)
                Dim MrzFieldId As Pr22.Processing.FieldReference = New FieldReference(FieldSource.ECard, FieldId.CompositeMrz)
                Dim SignatureFieldId As Pr22.Processing.FieldReference = New FieldReference(FieldSource.ECard, FieldId.Signature)
                Dim FingerFieldId As Pr22.Processing.FieldReference = New FieldReference(FieldSource.ECard, FieldId.Fingerprint)

                If FileDoc.GetFields().Contains(FaceFieldId) Then
                    FaceDoc = FileDoc
                    DrawImage(FacePictureBox1, FileDoc.GetField(FaceFieldId).GetImage().ToBitmap())
                End If
                If FileDoc.GetFields().Contains(MrzFieldId) Then
                    Dim mrz As String = FileDoc.GetField(MrzFieldId).GetRawStringValue()
                    If mrz.Length = 90 Then : mrz = mrz.Insert(60, vbCrLf).Insert(30, vbCrLf)
                    ElseIf mrz.Length > 50 Then : mrz = mrz.Insert(mrz.Length \ 2, vbCrLf)
                    End If
                    PrintMrzLines(mrz)
                End If
                If FileDoc.GetFields().Contains(SignatureFieldId) Then
                    DrawImage(SignaturePictureBox1, FileDoc.GetField(SignatureFieldId).GetImage().ToBitmap())
                End If
                If FileDoc.GetFields().Contains(FingerFieldId) Then
                    Try
                        DrawImage(FingerPictureBox1, FileDoc.GetField(FieldSource.ECard, FieldId.Fingerprint, 0).GetImage().ToBitmap())
                        DrawImage(FingerPictureBox2, FileDoc.GetField(FieldSource.ECard, FieldId.Fingerprint, 1).GetImage().ToBitmap())
                    Catch ex As Exception
                    End Try
                End If
            Catch ex As Pr22.Exceptions.General
            End Try
        End Sub

        Private Sub FileChecked(ByVal sender As Object, ByVal e As Pr22.Events.FileEventArgs)
            LogText("File Checked: " + e.FileId.ToString())
            Dim ok As Boolean = e.Result = Pr22.Exceptions.ErrorCodes.ENOERR
            ColorFileName(e.FileId, CType(IIf(ok, Color.Green, Color.Yellow), Color))
        End Sub

#End Region

#Region "General tools"

        Private Sub ColorFileName(ByVal file As Pr22.ECardHandling.File, ByVal col As Color)
            If InvokeRequired Then
                BeginInvoke(New DualParamInvoker(Of Pr22.ECardHandling.File, Color)(AddressOf ColorFileName), file, col)
                Return
            End If
            Dim fi As ListViewItem = FilesListView.FindItemWithText(file.ToString())
            If fi Is Nothing Then
                Try
                    file = Card.ConvertFileId(file)
                Catch ex As Pr22.Exceptions.General
                End Try
                fi = FilesListView.FindItemWithText(file.ToString())
            End If
            If fi IsNot Nothing Then fi.ForeColor = col
        End Sub

        Private Sub toolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripMenuItem1.Click
            If FilesListView.SelectedItems.Count = 0 OrElse Card Is Nothing Then Return

            Dim filedata As Pr22.Processing.BinData = Nothing

            For Each item As ListViewItem In FilesListView.SelectedItems
                If item.ForeColor <> Color.Black AndAlso item.ForeColor <> Color.Red Then
                    Dim file As Pr22.ECardHandling.FileId
                    file = DirectCast([Enum].Parse(GetType(Pr22.ECardHandling.FileId), item.Text), Pr22.ECardHandling.FileId)

                    saveFileDialog1.Filter = "binary file (*.bin)|*.bin|document file (*.xml)|*.xml"
                    saveFileDialog1.FileName = file.ToString()
                    If saveFileDialog1.ShowDialog() = DialogResult.OK Then
                        filedata = Card.GetFile(file)
                        If saveFileDialog1.FilterIndex = 1 Then
                            filedata.Save(saveFileDialog1.FileName)
                        ElseIf saveFileDialog1.FilterIndex = 2 Then
                            pr.Engine.Analyze(filedata).Save(Pr22.Processing.Document.FileFormat.Xml). _
                            Save(saveFileDialog1.FileName)
                        End If
                    End If
                    Exit For
                End If
            Next
        End Sub

        Private Function FileList(ByVal dirname As String, ByVal mask As String) As System.Collections.ArrayList
            Dim list As New System.Collections.ArrayList()
            Try
                Dim dir As New System.IO.DirectoryInfo(dirname)
                For Each d As System.IO.DirectoryInfo In dir.GetDirectories()
                    list.AddRange(FileList(dir.FullName + "/" + d.Name, mask))
                Next
                For Each f As System.IO.FileInfo In dir.GetFiles(mask)
                    list.Add(f.FullName)
                Next
            Catch ex As System.Exception
            End Try
            Return list
        End Function

        Private Sub LoadCertificates(ByVal dir As String)
            Dim exts As String() = {"*.cer", "*.crt", "*.der", "*.pem", "*.crl", "*.cvcert", "*.ldif", "*.ml"}

            For Each ext As String In exts
                Dim list As System.Collections.ArrayList = FileList(dir, ext)
                For Each file As String In list
                    Try
                        Dim fd As Pr22.Processing.BinData = New BinData().Load(file)
                        Dim pk As String = Nothing
                        If ext = "*.cvcert" Then
                            'Searching for private key
                            pk = file.Substring(0, file.LastIndexOf("."c) + 1) + "pkcs8"
                            If Not System.IO.File.Exists(pk) Then pk = Nothing
                        End If
                        If pk Is Nothing Then
                            pr.GlobalCertificateManager.Load(fd)
                        Else
                            pr.GlobalCertificateManager.Load(fd, New BinData().Load(pk))
                        End If
                    Catch ex As Pr22.Exceptions.General
                    End Try
                Next
            Next
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

#Region "Display"

        Private Sub LogText(ByVal s As String)
            If InvokeRequired Then
                BeginInvoke(New Action(Of String)(AddressOf LogText), s)
            Else
                textBox1.AppendText(s & vbCrLf)
            End If
        End Sub

        Private Sub PrintMrzLines(ByVal mrz As String)
            If InvokeRequired Then
                BeginInvoke(New Action(Of String)(AddressOf PrintMrzLines), mrz)
            Else
                MRZTextBox.Text = mrz
            End If
        End Sub

        Private Sub ColorAuthLabel(ByVal auth As Pr22.ECardHandling.AuthProcess, ByVal col As Color)
            Dim lbl As Label

            Select Case auth
                Case Pr22.ECardHandling.AuthProcess.BAC, Pr22.ECardHandling.AuthProcess.BAP
                    lbl = BACLabel
                    Exit Select
                Case Pr22.ECardHandling.AuthProcess.Active
                    lbl = AALabel
                    Exit Select
                Case Pr22.ECardHandling.AuthProcess.Chip
                    lbl = CALabel
                    Exit Select
                Case Pr22.ECardHandling.AuthProcess.PACE
                    lbl = PACELabel
                    Exit Select
                Case Pr22.ECardHandling.AuthProcess.Passive
                    lbl = PALabel
                    Exit Select
                Case Pr22.ECardHandling.AuthProcess.Terminal
                    lbl = TALabel
                    Exit Select
                Case Else
                    Return
            End Select

            ColorLabel(lbl, col)
        End Sub

        Private Sub ColorLabel(ByVal lbl As Label, ByVal col As Color)
            If InvokeRequired Then
                BeginInvoke(New DualParamInvoker(Of Label, Color)(AddressOf ColorLabel), lbl, col)
            Else
                lbl.ForeColor = col
            End If
        End Sub

        Private Sub DrawImage(ByVal pbox As PictureBox, ByVal bmp As Image)
            If InvokeRequired Then
                BeginInvoke(New DualParamInvoker(Of PictureBox, Image)(AddressOf DrawImage), pbox, bmp)
            Else
                pbox.Image = bmp
            End If
        End Sub

        Private Sub ClearControls()
            MRZTextBox.Clear()
            FacePictureBox1.Image = Nothing
            FacePictureBox2.Image = Nothing
            SignaturePictureBox1.Image = Nothing
            SignaturePictureBox2.Image = Nothing
            FingerPictureBox1.Image = Nothing
            FingerPictureBox2.Image = Nothing

            AALabel.ForeColor = Color.Black
            BACLabel.ForeColor = Color.Black
            PACELabel.ForeColor = Color.Black
            CALabel.ForeColor = Color.Black
            TALabel.ForeColor = Color.Black
            PALabel.ForeColor = Color.Black
            MrzLabel.ForeColor = Color.Black
            FaceLabel.ForeColor = Color.Black

            For Each item As ListViewItem In FilesListView.Items
                item.ForeColor = Color.Black
            Next
            Update()
        End Sub

        Private Sub SetStartButton(ByVal en As Boolean)
            If InvokeRequired Then
                BeginInvoke(New Action(Of Boolean)(AddressOf SetStartButton), en)
            Else
                StartButton.Enabled = en
            End If
        End Sub

#End Region

    End Class
End Namespace

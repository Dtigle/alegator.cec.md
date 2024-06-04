' This example shows how to generate document type string.

Option Explicit On
Imports Microsoft.VisualBasic
Imports Pr22
Imports Pr22.Processing

Namespace tutorial

    Partial Class MainClass

        Private pr As DocumentReaderDevice = Nothing

        '----------------------------------------------------------------------
        ''' <summary>
        ''' Opens the first document reader device.
        ''' </summary>
        ''' <returns></returns>
        Public Function Open() As Integer

            System.Console.WriteLine("Opening a device")
            System.Console.WriteLine()
            pr = New DocumentReaderDevice()

            AddHandler pr.Connection, AddressOf onDeviceConnected
            AddHandler pr.DeviceUpdate, AddressOf onDeviceUpdate

            Try
                pr.UseDevice(0)
            Catch ex As Pr22.Exceptions.NoSuchDevice
                System.Console.WriteLine("No device found!")
                Return 1
            End Try

            System.Console.WriteLine("The device " + pr.DeviceName + " is opened.")
            System.Console.WriteLine()
            Return 0
        End Function
        '----------------------------------------------------------------------

        Public Function Program() As Integer

            'Devices can be manipulated only after opening.
            If Open() <> 0 Then Return 1

            'Subscribing to scan events
            AddHandler pr.ScanStarted, AddressOf ScanStarted
            AddHandler pr.ImageScanned, AddressOf ImageScanned
            AddHandler pr.ScanFinished, AddressOf ScanFinished
            AddHandler pr.DocFrameFound, AddressOf DocFrameFound

            Dim Scanner As DocScanner = pr.Scanner
            Dim OcrEngine As Engine = pr.Engine

            System.Console.WriteLine("Scanning some images to read from.")
            Dim ScanTask As New Pr22.Task.DocScannerTask()
            'For OCR (MRZ) reading purposes, IR (infrared) image is recommended.
            ScanTask.Add(Pr22.Imaging.Light.White).Add(Pr22.Imaging.Light.Infra)
            Dim DocPage As Page = Scanner.Scan(ScanTask, Pr22.Imaging.PagePosition.First)
            System.Console.WriteLine()

            System.Console.WriteLine("Reading all the field data.")
            Dim ReadingTask As New Pr22.Task.EngineTask()
            'Specify the fields we would like to receive.
            ReadingTask.Add(FieldSource.All, FieldId.All)

            Dim OcrDoc As Document = OcrEngine.Analyze(DocPage, ReadingTask)

            System.Console.WriteLine()
            System.Console.WriteLine("Document code: " & OcrDoc.ToVariant().ToInt())
            System.Console.WriteLine("Document type: " & GetDocType(OcrDoc))
            System.Console.WriteLine("Status: " & OcrDoc.GetStatus().ToString())

            pr.Close()
            Return 0
        End Function
        '----------------------------------------------------------------------

        Public Function GetFieldValue(ByVal Doc As Pr22.Processing.Document, ByVal Id As Pr22.Processing.FieldId) As String

            Dim filter As FieldReference = New FieldReference(FieldSource.All, Id)
            Dim Fields As System.Collections.Generic.List(Of FieldReference) = Doc.GetFields(filter)
            For Each FR As FieldReference In Fields
                Dim value As String = Doc.GetField(FR).GetBestStringValue()
                If value <> "" Then Return value
            Next
            Return ""
        End Function
        '----------------------------------------------------------------------

        Public Function GetDocType(ByVal OcrDoc As Document) As String

            Dim documentTypeName As String = ""

            Dim documentCode As Integer = OcrDoc.ToVariant().ToInt()
            documentTypeName = Pr22Extension.DocumentType.GetDocumentName(documentCode)

            If documentTypeName = "" Then

                Dim issue_country As String = GetFieldValue(OcrDoc, FieldId.IssueCountry)
                Dim issue_state As String = GetFieldValue(OcrDoc, FieldId.IssueState)
                Dim doc_type As String = GetFieldValue(OcrDoc, FieldId.DocType)
                Dim doc_page As String = GetFieldValue(OcrDoc, FieldId.DocPage)
                Dim doc_subtype As String = GetFieldValue(OcrDoc, FieldId.DocTypeDisc)

                Dim tmpval As String = Pr22Extension.CountryCode.GetName(issue_country)
                If tmpval <> "" Then issue_country = tmpval

                documentTypeName = issue_country + New StrCon() + issue_state _
                    + New StrCon() + Pr22Extension.DocumentType.GetDocTypeName(doc_type) _
                    + New StrCon("-") + Pr22Extension.DocumentType.GetPageName(doc_page) _
                    + New StrCon(",") + doc_subtype

            End If
            Return documentTypeName

        End Function
        '----------------------------------------------------------------------
        ' Event handlers
        '----------------------------------------------------------------------

        Private Sub onDeviceConnected(ByVal a As Object, ByVal e As Pr22.Events.ConnectionEventArgs)

            System.Console.WriteLine("Connection event. Device number: " & e.DeviceNumber)
        End Sub
        '----------------------------------------------------------------------

        Private Sub onDeviceUpdate(ByVal a As Object, ByVal e As Pr22.Events.UpdateEventArgs)

            System.Console.WriteLine("Update event.")
            Select Case e.part
                Case 1
                    System.Console.WriteLine("  Reading calibration file from device.")
                Case 2
                    System.Console.WriteLine("  Scanner firmware update.")
                Case 4
                    System.Console.WriteLine("  RFID reader firmware update.")
                Case 5
                    System.Console.WriteLine("  License update.")
            End Select
        End Sub
        '----------------------------------------------------------------------

        Private Sub ScanStarted(ByVal a As Object, ByVal e As Pr22.Events.PageEventArgs)

            System.Console.WriteLine("Scan started. Page: " & e.Page)
        End Sub
        '----------------------------------------------------------------------

        Private Sub ImageScanned(ByVal a As Object, ByVal e As Pr22.Events.ImageEventArgs)

            System.Console.WriteLine("Image scanned. Page: " & e.Page & " Light: " & e.Light.ToString())
        End Sub
        '----------------------------------------------------------------------

        Private Sub ScanFinished(ByVal a As Object, ByVal e As Pr22.Events.PageEventArgs)

            System.Console.WriteLine("Page scanned. Page: " & e.Page & " Status: " & e.Status.ToString())
        End Sub
        '----------------------------------------------------------------------

        Private Sub DocFrameFound(ByVal a As Object, ByVal e As Pr22.Events.PageEventArgs)

            System.Console.WriteLine("Document frame found. Page: " & e.Page)
        End Sub
        '----------------------------------------------------------------------

        Public Shared Function Main(ByVal args As String()) As Integer

            Try
                Dim prog As New MainClass()
                prog.Program()
            Catch e As Pr22.Exceptions.General
                System.Console.Error.WriteLine(e.Message)
            End Try
            System.Console.WriteLine("Press any key to exit!")
            System.Console.ReadKey(True)
            Return 0
        End Function
        '----------------------------------------------------------------------
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

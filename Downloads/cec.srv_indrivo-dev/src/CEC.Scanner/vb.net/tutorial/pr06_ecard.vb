﻿' This example shows how to read and process data from ECards.
' After ECard selection before reading some authentication process have to
' be called for accessing the data files.

Option Explicit On
Imports Microsoft.VisualBasic
Imports Pr22
Imports Pr22.Processing
Imports System.Collections.Generic
Namespace tutorial

    Class MainClass

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
        ''' <summary>
        ''' Returns a list of files in a directory.
        ''' </summary>
        ''' <param name="dirname"></param>
        ''' <param name="mask"></param>
        ''' <returns></returns>
        Public Function FileList(ByVal dirname As String, ByVal mask As String) As System.Collections.ArrayList

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
        '----------------------------------------------------------------------
        ''' <summary>
        ''' Loads certificates from a directory.
        ''' </summary>
        ''' <param name="dir"></param>
        Public Sub LoadCertificates(ByVal dir As String)

            Dim exts As String() = {"*.cer", "*.crt", "*.der", "*.pem", "*.crl", "*.cvcert", "*.ldif", "*.ml"}
            Dim cnt As Integer = 0

            For Each ext As String In exts
                Dim list As System.Collections.ArrayList = FileList(dir, ext)
                For Each file As String In list
                    Try
                        Dim fd As BinData = New BinData().Load(file)
                        Dim pk As String = Nothing
                        If ext = "*.cvcert" Then
                            'Searching for private key
                            pk = file.Substring(0, file.LastIndexOf("."c) + 1) + "pkcs8"
                            If Not System.IO.File.Exists(pk) Then pk = Nothing
                        End If
                        If pk Is Nothing Then
                            pr.CertificateManager.Load(fd)
                            System.Console.WriteLine("Certificate " + file + " is loaded.")
                        Else
                            pr.CertificateManager.Load(fd, New BinData().Load(pk))
                            System.Console.WriteLine("Certificate " + file + " is loaded with private key.")
                        End If
                        cnt += 1
                    Catch ex As Pr22.Exceptions.General
                        System.Console.WriteLine("Loading certificate " + file + " is failed!")
                    End Try
                Next
            Next
            If cnt = 0 Then System.Console.WriteLine("No certificates loaded from " + dir)
            System.Console.WriteLine()
        End Sub
        '----------------------------------------------------------------------
        ''' <summary>
        ''' Does an authentication after collecting the necessary information.
        ''' </summary>
        ''' <param name="SelectedCard"></param>
        ''' <param name="CurrentAuth"></param>
        ''' <returns></returns>
        Public Function Authenticate(ByVal SelectedCard As ECard, ByVal CurrentAuth As Pr22.ECardHandling.AuthProcess) As Boolean

            Dim AdditionalAuthData As BinData = Nothing
            Dim selector As Integer = 0
            Select Case CurrentAuth
                Case Pr22.ECardHandling.AuthProcess.BAC, Pr22.ECardHandling.AuthProcess.PACE, Pr22.ECardHandling.AuthProcess.BAP
                    'Read MRZ (necessary for BAC, PACE and BAP)
                    Dim ScanTask As New Pr22.Task.DocScannerTask()
                    ScanTask.Add(Pr22.Imaging.Light.Infra)
                    Dim FirstPage As Page = pr.Scanner.Scan(ScanTask, Pr22.Imaging.PagePosition.First)

                    Dim MrzReadingTask As New Pr22.Task.EngineTask()
                    MrzReadingTask.Add(FieldSource.Mrz, FieldId.All)
                    Dim MrzDoc As Document = pr.Engine.Analyze(FirstPage, MrzReadingTask)

                    AdditionalAuthData = New BinData().SetString(MrzDoc.GetField(FieldSource.Mrz, FieldId.All).GetRawStringValue())
                    selector = 1

                Case Pr22.ECardHandling.AuthProcess.Passive, Pr22.ECardHandling.AuthProcess.Terminal

                    'Load the certificates if not done yet

                Case Pr22.ECardHandling.AuthProcess.SelectApp
                    If SelectedCard.Applications.Count > 0 Then selector = CInt(SelectedCard.Applications(0))

            End Select
            Try
                SelectedCard.Authenticate(CurrentAuth, AdditionalAuthData, selector)
                System.Console.WriteLine("- " & CurrentAuth.ToString() & " authentication succeeded")
                Return True
            Catch e As Pr22.Exceptions.General
                System.Console.WriteLine("- " & CurrentAuth.ToString() & " authentication failed: " & e.Message)
                Return False
            End Try
        End Function
        '----------------------------------------------------------------------

        Public Function Program() As Integer

            'Devices can be manipulated only after opening.
            If Open() <> 0 Then Return 1

            'Please set the appropriate path
            LoadCertificates(pr.GetProperty("rwdata_dir") + "\certs")

            Dim CardReaders As List(Of ECardReader) = pr.Readers

            'Connecting to the 1st card of any reader
            Dim SelectedCard As ECard = Nothing
            Dim CardCount As Integer = 0
            System.Console.WriteLine("Detected readers and cards:")
            For Each reader As ECardReader In CardReaders
                System.Console.WriteLine(vbTab & "Reader: " & reader.Info.HwType.ToString())
                Dim cards As List(Of String) = reader.GetCards()
                If SelectedCard Is Nothing AndAlso cards.Count > 0 Then SelectedCard = reader.ConnectCard(0)
                For Each card As String In cards
                    System.Console.WriteLine(vbTab & vbTab & "(" & CardCount & ")card: " & card)
                    CardCount += 1
                Next
                System.Console.WriteLine()
            Next
            If SelectedCard Is Nothing Then
                System.Console.WriteLine("No card selected!")
                Return 1
            End If

            System.Console.WriteLine("Executing authentications:")
            Dim CurrentAuth As Pr22.ECardHandling.AuthProcess = SelectedCard.GetNextAuthentication(False)
            Dim PassiveAuthImplemented As Boolean = False

            While CurrentAuth <> Pr22.ECardHandling.AuthProcess.None
                If CurrentAuth = Pr22.ECardHandling.AuthProcess.Passive Then PassiveAuthImplemented = True
                Dim authOk As Boolean = Authenticate(SelectedCard, CurrentAuth)
                CurrentAuth = SelectedCard.GetNextAuthentication(Not authOk)
            End While
            System.Console.WriteLine()

            System.Console.WriteLine("Reading data:")
            Dim FilesOnSelectedCard As List(Of Pr22.ECardHandling.File) = SelectedCard.Files
            If PassiveAuthImplemented Then
                FilesOnSelectedCard.Add(Pr22.ECardHandling.FileId.CertDS)
                FilesOnSelectedCard.Add(Pr22.ECardHandling.FileId.CertCSCA)
            End If
            For Each File As Pr22.ECardHandling.File In FilesOnSelectedCard
                Try
                    System.Console.Write("File: " & File.ToString() & ".")
                    Dim RawFileData As BinData = SelectedCard.GetFile(File)
                    RawFileData.Save(File.ToString() & ".dat")
                    Dim FileData As Document = pr.Engine.Analyze(RawFileData)
                    FileData.Save(Document.FileFormat.Xml).Save(File.ToString() + ".xml")

                    'Executing mandatory data integrity check for Passive Authentication
                    If PassiveAuthImplemented Then
                        Dim f As Pr22.ECardHandling.File = File
                        If f.Id >= Pr22.ECardHandling.FileId.GeneralData Then f = SelectedCard.ConvertFileId(f)
                        If f.Id >= 1 AndAlso f.Id <= 16 Then
                            System.Console.Write(" hash check...")
                            System.Console.Write(IIf(SelectedCard.CheckHash(f), "OK", "failed"))
                        End If
                    End If
                    System.Console.WriteLine()
                    PrintDocFields(FileData)
                Catch e As Pr22.Exceptions.General
                    System.Console.Write(" Reading failed: " + e.Message)
                End Try
                System.Console.WriteLine()
            Next

            System.Console.WriteLine("Authentications:")
            Dim AuthData As Document = SelectedCard.GetAuthResult()
            AuthData.Save(Document.FileFormat.Xml).Save("AuthResult.xml")
            PrintDocFields(AuthData)
            System.Console.WriteLine()

            SelectedCard.Disconnect()

            pr.Close()
            Return 0
        End Function
        '----------------------------------------------------------------------
        ''' <summary>
        ''' Prints out all fields of a document structure to console.
        ''' </summary>
        ''' <remarks>
        ''' Values are printed in three different forms: raw, formatted and standardized.
        ''' Status (checksum result) is printed together with fieldname and raw value.
        ''' At the end, images of all fields are saved into png format.
        ''' </remarks>
        ''' <param name="doc"></param>
        Private Shared Sub PrintDocFields(ByVal doc As Document)

            Dim Fields As System.Collections.Generic.List(Of FieldReference) = doc.GetFields()

            System.Console.WriteLine("  {0, -20}{1, -17}{2}", "FieldId", "Status", "Value")
            System.Console.WriteLine("  {0, -20}{1, -17}{2}", "-------", "------", "-----")
            System.Console.WriteLine()

            For Each CurrentFieldRef As FieldReference In Fields
                Try
                    Dim CurrentField As Field = doc.GetField(CurrentFieldRef)
                    Dim Value As String = "", FormattedValue As String = "", StandardizedValue As String = ""
                    Dim binValue As Byte() = Nothing
                    Try
                        Value = CurrentField.GetRawStringValue()
                    Catch ex As Pr22.Exceptions.EntryNotFound
                    Catch ex As Pr22.Exceptions.InvalidParameter
                        binValue = CurrentField.GetBinaryValue()
                    End Try
                    Try
                        FormattedValue = CurrentField.GetFormattedStringValue()
                    Catch ex As Pr22.Exceptions.EntryNotFound
                    End Try
                    Try
                        StandardizedValue = CurrentField.GetStandardizedStringValue()
                    Catch ex As Pr22.Exceptions.EntryNotFound
                    End Try
                    Dim Status As Status = Status.NoChecksum
                    Status = CurrentField.GetStatus()
                    Dim Fieldname As String = CurrentFieldRef.ToString()
                    If binValue IsNot Nothing Then
                        System.Console.WriteLine("  {0, -20}{1, -17}Binary", Fieldname, Status)
                        'For cnt As Integer = 0 To binValue.Length - 1 Step 16
                        'System.Console.WriteLine(PrintBinary(binValue, cnt, 16))
                        'Next
                    Else
                        System.Console.WriteLine("  {0, -20}{1, -17}[{2}]", Fieldname, Status, Value)
                        System.Console.WriteLine(vbTab & "{1, -31}[{0}]", FormattedValue, "   - Formatted")
                        System.Console.WriteLine(vbTab & "{1, -31}[{0}]", StandardizedValue, "   - Standardized")
                    End If

                    Dim lst As List(Of Checking) = CurrentField.GetDetailedStatus()
                    For Each chk As Checking In lst
                        System.Console.WriteLine(chk.ToString())
                    Next

                    Try
                        CurrentField.GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Png).Save(Fieldname + ".png")
                    Catch ex As Pr22.Exceptions.General
                    End Try
                Catch ex As Pr22.Exceptions.General
                End Try
            Next
            System.Console.WriteLine()

            For Each comp As FieldCompare In doc.GetFieldCompareList()
                System.Console.WriteLine("Comparing " & comp.field1.ToString() & " vs. " & _
                    comp.field2.ToString() & " results " & comp.confidence)
            Next
            System.Console.WriteLine()
        End Sub
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
End Namespace

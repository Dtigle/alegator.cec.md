' This example shows how to parametrize the image scanning process.

Option Explicit On
Imports System.Collections.Generic
Imports Pr22
Imports Pr22.Exceptions
Imports Pr22.Task
Namespace tutorial

    Class MainClass

        Private pr As DocumentReaderDevice = Nothing
        Private DocPresent As Boolean

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

            System.Console.WriteLine("This tutorial guides you through a complex image scanning process.")
            System.Console.WriteLine("This will demonstrate all possible options of page management.")
            System.Console.WriteLine("The stages of the scan process will be saved into separate zip files")
            System.Console.WriteLine("in order to provide the possibility of comparing them to each other.")
            System.Console.WriteLine()

            'Devices can be manipulated only after opening.
            If Open() <> 0 Then Return 1

            'Subscribing to scan events
            AddHandler pr.ScanStarted, AddressOf ScanStarted
            AddHandler pr.ImageScanned, AddressOf ImageScanned
            AddHandler pr.ScanFinished, AddressOf ScanFinished
            AddHandler pr.DocFrameFound, AddressOf DocFrameFound
            AddHandler pr.PresenceStateChanged, AddressOf PresentStateChanged

            Dim Scanner As DocScanner = pr.Scanner

            Dim LiveTask As TaskControl = Scanner.StartTask(FreerunTask.Detection())

            'first page
            Dim FirstTask As New DocScannerTask()

            System.Console.WriteLine("At first the device scans only a white image...")
            FirstTask.Add(Pr22.Imaging.Light.White)
            Dim page1 As Pr22.Processing.Page = Scanner.Scan(FirstTask, Pr22.Imaging.PagePosition.First)

            System.Console.WriteLine("And then the program saves it as a PNG file.")
            page1.Select(Pr22.Imaging.Light.White).GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Png).Save("original.png")

            System.Console.WriteLine("Saving stage 1.")
            pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("1stScan.zip")
            System.Console.WriteLine()

            System.Console.WriteLine("If scanning of an additional infra image of the same page is required...")
            System.Console.WriteLine("We need to scan it into the current page.")
            FirstTask.Add(Pr22.Imaging.Light.Infra)
            Scanner.Scan(FirstTask, Pr22.Imaging.PagePosition.Current)

            Try
                System.Console.WriteLine("If a cropped image of the document is available")
                System.Console.WriteLine(" then the program saves it as a PNG file.")
                Scanner.GetPage(0).Select(Pr22.Imaging.Light.White).DocView().GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Png).Save("document.png")
            Catch ex As Pr22.Exceptions.ImageProcessingFailed
                System.Console.WriteLine("Cropped image is not available!")
            End Try

            System.Console.WriteLine("Saving stage 2.")
            pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("2ndScan.zip")
            System.Console.WriteLine()

            'second page
            System.Console.WriteLine("At this point, if scanning of an additional page of the document is needed")
            System.Console.WriteLine("with all of the available lights except the infra light.")
            System.Console.WriteLine("It is recommended to execute in one scan process")
            System.Console.WriteLine(" - as it is the fastest in such a way.")
            Dim SecondTask As New DocScannerTask()
            SecondTask.Add(Pr22.Imaging.Light.All).Del(Pr22.Imaging.Light.Infra)
            System.Console.WriteLine()

            DocPresent = False
            System.Console.WriteLine("At this point, the user has to change the document on the reader.")
            While DocPresent = False
                System.Threading.Thread.Sleep(100)
            End While

            System.Console.WriteLine("Scanning the images.")
            Scanner.Scan(SecondTask, Pr22.Imaging.PagePosition.Next)

            System.Console.WriteLine("Saving stage 3.")
            pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("3rdScan.zip")
            System.Console.WriteLine()

            System.Console.WriteLine("Upon putting incorrect page on the scanner, the scanned page has to be removed.")
            Scanner.CleanUpLastPage()

            DocPresent = False
            System.Console.WriteLine("And the user has to change the document on the reader again.")
            While DocPresent = False
                System.Threading.Thread.Sleep(100)
            End While

            System.Console.WriteLine("Scanning...")
            Scanner.Scan(SecondTask, Pr22.Imaging.PagePosition.Next)

            System.Console.WriteLine("Saving stage 4.")
            pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("4thScan.zip")
            System.Console.WriteLine()

            LiveTask.Stop()

            System.Console.WriteLine("Scanning processes are finished.")
            pr.Close()
            Return 0
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
            Dim img As Pr22.Imaging.RawImage = DirectCast(a, DocumentReaderDevice).Scanner.GetPage(e.Page).Select(e.Light).GetImage()
            img.Save(Pr22.Imaging.RawImage.FileFormat.Bmp).Save("page_" & e.Page & "_light_" & e.Light.ToString() & ".bmp")
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

        Private Sub PresentStateChanged(ByVal a As Object, ByVal e As Pr22.Events.DetectionEventArgs)

            If e.State = Pr22.Util.PresenceState.Present Then DocPresent = True
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

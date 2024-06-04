﻿/* This example shows how to parametrize the image scanning process.
 */
namespace tutorial
{
    using System.Collections.Generic;
    using Pr22;
    using Pr22.Exceptions;
    using Pr22.Task;

    class MainClass
    {
        DocumentReaderDevice pr = null;
        bool DocPresent;

        //----------------------------------------------------------------------
        /// <summary>
        /// Opens the first document reader device.
        /// </summary>
        /// <returns></returns>
        public int Open()
        {
            System.Console.WriteLine("Opening a device");
            System.Console.WriteLine();
            pr = new DocumentReaderDevice();

            pr.Connection += onDeviceConnected;
            pr.DeviceUpdate += onDeviceUpdate;

            try { pr.UseDevice(0); }
            catch (Pr22.Exceptions.NoSuchDevice)
            {
                System.Console.WriteLine("No device found!");
                return 1;
            }

            System.Console.WriteLine("The device " + pr.DeviceName + " is opened.");
            System.Console.WriteLine();
            return 0;
        }
        //----------------------------------------------------------------------

        public int Program()
        {
            System.Console.WriteLine("This tutorial guides you through a complex image scanning process.");
            System.Console.WriteLine("This will demonstrate all possible options of page management.");
            System.Console.WriteLine("The stages of the scan process will be saved into separate zip files");
            System.Console.WriteLine("in order to provide the possibility of comparing them to each other.");
            System.Console.WriteLine();

            //Devices can be manipulated only after opening.
            if (Open() != 0) return 1;

            //Subscribing to scan events
            pr.ScanStarted += ScanStarted;
            pr.ImageScanned += ImageScanned;
            pr.ScanFinished += ScanFinished;
            pr.DocFrameFound += DocFrameFound;
            pr.PresenceStateChanged += PresentStateChanged;

            DocScanner Scanner = pr.Scanner;

            TaskControl LiveTask = Scanner.StartTask(FreerunTask.Detection());

            //first page
            {
                DocScannerTask FirstTask = new DocScannerTask();

                System.Console.WriteLine("At first the device scans only a white image...");
                FirstTask.Add(Pr22.Imaging.Light.White);
                Pr22.Processing.Page page1 = Scanner.Scan(FirstTask, Pr22.Imaging.PagePosition.First);

                System.Console.WriteLine("And then the program saves it as a PNG file.");
                page1.Select(Pr22.Imaging.Light.White).GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Png).Save("original.png");

                System.Console.WriteLine("Saving stage 1.");
                pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("1stScan.zip");
                System.Console.WriteLine();

                System.Console.WriteLine("If scanning of an additional infra image of the same page is required...");
                System.Console.WriteLine("We need to scan it into the current page.");
                FirstTask.Add(Pr22.Imaging.Light.Infra);
                Scanner.Scan(FirstTask, Pr22.Imaging.PagePosition.Current);

                try
                {
                    System.Console.WriteLine("If a cropped image of the document is available");
                    System.Console.WriteLine(" then the program saves it as a PNG file.");
                    Scanner.GetPage(0).Select(Pr22.Imaging.Light.White).DocView().GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Png).Save("document.png");
                }
                catch (Pr22.Exceptions.ImageProcessingFailed)
                {
                    System.Console.WriteLine("Cropped image is not available!");
                }

                System.Console.WriteLine("Saving stage 2.");
                pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("2ndScan.zip");
                System.Console.WriteLine();
            }

            //second page
            {
                System.Console.WriteLine("At this point, if scanning of an additional page of the document is needed");
                System.Console.WriteLine("with all of the available lights except the infra light.");
                System.Console.WriteLine("It is recommended to execute in one scan process");
                System.Console.WriteLine(" - as it is the fastest in such a way.");
                DocScannerTask SecondTask = new DocScannerTask();
                SecondTask.Add(Pr22.Imaging.Light.All).Del(Pr22.Imaging.Light.Infra);
                System.Console.WriteLine();

                DocPresent = false;
                System.Console.WriteLine("At this point, the user has to change the document on the reader.");
                while (DocPresent == false) System.Threading.Thread.Sleep(100);

                System.Console.WriteLine("Scanning the images.");
                Scanner.Scan(SecondTask, Pr22.Imaging.PagePosition.Next);

                System.Console.WriteLine("Saving stage 3.");
                pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("3rdScan.zip");
                System.Console.WriteLine();

                System.Console.WriteLine("Upon putting incorrect page on the scanner, the scanned page has to be removed.");
                Scanner.CleanUpLastPage();

                DocPresent = false;
                System.Console.WriteLine("And the user has to change the document on the reader again.");
                while (DocPresent == false) System.Threading.Thread.Sleep(100);

                System.Console.WriteLine("Scanning...");
                Scanner.Scan(SecondTask, Pr22.Imaging.PagePosition.Next);

                System.Console.WriteLine("Saving stage 4.");
                pr.Engine.GetRootDocument().Save(Pr22.Processing.Document.FileFormat.Zipped).Save("4thScan.zip");
                System.Console.WriteLine();
            }

            LiveTask.Stop();

            System.Console.WriteLine("Scanning processes are finished.");
            pr.Close();
            return 0;
        }
        //----------------------------------------------------------------------
        // Event handlers
        //----------------------------------------------------------------------

        void onDeviceConnected(object a, Pr22.Events.ConnectionEventArgs e)
        {
            System.Console.WriteLine("Connection event. Device number: " + e.DeviceNumber);
        }
        //----------------------------------------------------------------------

        void onDeviceUpdate(object a, Pr22.Events.UpdateEventArgs e)
        {
            System.Console.WriteLine("Update event.");
            switch (e.part)
            {
                case 1:
                    System.Console.WriteLine("  Reading calibration file from device.");
                    break;
                case 2:
                    System.Console.WriteLine("  Scanner firmware update.");
                    break;
                case 4:
                    System.Console.WriteLine("  RFID reader firmware update.");
                    break;
                case 5:
                    System.Console.WriteLine("  License update.");
                    break;
            }
        }
        //----------------------------------------------------------------------

        void ScanStarted(object a, Pr22.Events.PageEventArgs e)
        {
            System.Console.WriteLine("Scan started. Page: " + e.Page);
        }
        //----------------------------------------------------------------------

        void ImageScanned(object a, Pr22.Events.ImageEventArgs e)
        {
            System.Console.WriteLine("Image scanned. Page: " + e.Page + " Light: " + e.Light);
            Pr22.Imaging.RawImage img = ((DocumentReaderDevice)a).Scanner.GetPage(e.Page).Select(e.Light).GetImage();
            img.Save(Pr22.Imaging.RawImage.FileFormat.Bmp).Save("page_" + e.Page + "_light_" + e.Light + ".bmp");
        }
        //----------------------------------------------------------------------

        void ScanFinished(object a, Pr22.Events.PageEventArgs e)
        {
            System.Console.WriteLine("Page scanned. Page: " + e.Page + " Status: " + e.Status);
        }
        //----------------------------------------------------------------------

        void DocFrameFound(object a, Pr22.Events.PageEventArgs e)
        {
            System.Console.WriteLine("Document frame found. Page: " + e.Page);
        }
        //----------------------------------------------------------------------

        void PresentStateChanged(object a, Pr22.Events.DetectionEventArgs e)
        {
            if (e.State == Pr22.Util.PresenceState.Present) DocPresent = true;
        }
        //----------------------------------------------------------------------

        public static int Main(string[] args)
        {
            try
            {
                MainClass prog = new MainClass();
                prog.Program();
            }
            catch (Pr22.Exceptions.General e)
            {
                System.Console.Error.WriteLine(e.Message);
            }
            System.Console.WriteLine("Press any key to exit!");
            System.Console.ReadKey(true);
            return 0;
        }
        //----------------------------------------------------------------------
    }
}

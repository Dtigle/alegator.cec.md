/* This example shows how to get general information about the device capabilities.
 */
namespace tutorial
{
    using System.Collections.Generic;
    using Pr22;

    class MainClass
    {
        DocumentReaderDevice pr = null;

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
            //Devices can be manipulated only after opening.
            if (Open() != 0) return 1;

            System.Console.WriteLine("SDK versions:");
            System.Console.WriteLine("\tAssembly: " + pr.GetType().Assembly.GetName().Version);
            System.Console.WriteLine("\tInterface: " + pr.GetVersion('A'));
            System.Console.WriteLine("\tSystem: " + pr.GetVersion('S'));
            System.Console.WriteLine();

            Pr22.DocScanner.Information scannerinfo = pr.Scanner.Info;

            //Devices provide proper image quality only if they are calibrated.
            //Devices are calibrated by default. If you receive the message "not calibrated"
            //then please contact your hardware supplier.
            System.Console.WriteLine("Calibration state of the device:");
            if (scannerinfo.IsCalibrated())
                System.Console.WriteLine("\tcalibrated");
            else
                System.Console.WriteLine("\tnot calibrated");
            System.Console.WriteLine();

            System.Console.WriteLine("Available lights for image scanning:");
            List<Pr22.Imaging.Light> lights = scannerinfo.GetLights();
            foreach (Pr22.Imaging.Light light in lights)
                System.Console.WriteLine("\t" + light);
            System.Console.WriteLine();

            System.Console.WriteLine("Available object windows for image scanning:");
            for (int i = 0; i < scannerinfo.GetWindowCount(); ++i)
            {
                System.Drawing.Rectangle frame = scannerinfo.GetSize(i);
                System.Console.WriteLine("\t" + i + ": " + frame.Width / 1000.0f + " x " + frame.Height / 1000.0f + " mm");
            }
            System.Console.WriteLine();

            System.Console.WriteLine("Scanner component versions:");
            System.Console.WriteLine("\tFirmware: " + scannerinfo.GetVersion('F'));
            System.Console.WriteLine("\tHardware: " + scannerinfo.GetVersion('H'));
            System.Console.WriteLine("\tSoftware: " + scannerinfo.GetVersion('S'));
            System.Console.WriteLine();

            System.Console.WriteLine("Available card readers:");
            List<ECardReader> readers = pr.Readers;
            for (int i = 0; i < readers.Count; ++i)
            {
                System.Console.WriteLine("\t" + i + ": " + readers[i].Info.HwType);
                System.Console.WriteLine("\t\tFirmware: " + readers[i].Info.GetVersion('F'));
                System.Console.WriteLine("\t\tHardware: " + readers[i].Info.GetVersion('H'));
                System.Console.WriteLine("\t\tSoftware: " + readers[i].Info.GetVersion('S'));
            }
            System.Console.WriteLine();

            System.Console.WriteLine("Available status LEDs:");
            List<Pr22.Control.StatusLed> leds = pr.Peripherals.StatusLeds;
            for (int i = 0; i < leds.Count; ++i)
                System.Console.WriteLine("\t" + i + ": color " + leds[i].Light);
            System.Console.WriteLine();

            Pr22.Engine.Information EngineInfo = pr.Engine.Info;

            System.Console.WriteLine("Engine version: " + EngineInfo.GetVersion('E'));
            string[] licok = { "no presence info", "not available", "present", "expired" };
            string lictxt = EngineInfo.RequiredLicense.ToString();
            if (EngineInfo.RequiredLicense == Pr22.Processing.EngineLicense.MrzOcrBarcodeReading)
                lictxt = "MrzOcrBarcodeReadingL or MrzOcrBarcodeReadingF";
            System.Console.WriteLine("Required license: " + lictxt + " - " + licok[TestLicense(EngineInfo)]);
            System.Console.WriteLine("Engine release date: " + EngineInfo.RequiredLicenseDate);
            System.Console.WriteLine();

            System.Console.WriteLine("Available licenses:");
            List<Pr22.Processing.EngineLicense> licenses = EngineInfo.GetAvailableLicenses();
            foreach (Pr22.Processing.EngineLicense lic in licenses)
                System.Console.WriteLine("\t" + lic + " (" + EngineInfo.GetLicenseDate(lic) + ")");
            System.Console.WriteLine();

            System.Console.WriteLine("Closing the device.");
            pr.Close();
            return 0;
        }
        //----------------------------------------------------------------------
        /// <summary>
        /// Tests if the required OCR license is present.
        /// </summary>
        int TestLicense(Pr22.Engine.Information info)
        {
            if (info.RequiredLicense == Pr22.Processing.EngineLicense.Unknown) return 0;
            string availdate = info.GetLicenseDate(info.RequiredLicense);
            if (availdate == "-") return 1;
            if (info.RequiredLicenseDate == "-") return 2;
            if (availdate[0] != 'X' && availdate.CompareTo(info.RequiredLicenseDate) > 0) return 2;
            return 3;
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

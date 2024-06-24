/* This example shows the possibilities of opening a device.
 * The following tutorials show the easy way of opening in their Open methods.
 */
namespace tutorial
{
    using Pr22;

    class MainClass
    {
        DocumentReaderDevice pr = null;

        //----------------------------------------------------------------------

        public int Program()
        {
            /* To open more than one device simultaneously, create more DocumentReaderDevice objects */
            System.Console.WriteLine("Opening system");
            System.Console.WriteLine();
            pr = new DocumentReaderDevice();

            pr.Connection += onDeviceConnected;
            pr.DeviceUpdate += onDeviceUpdate;

            System.Collections.Generic.List<string> deviceList = DocumentReaderDevice.GetDeviceList();

            if (deviceList.Count == 0)
            {
                System.Console.WriteLine("No device found!");
                return 0;
            }

            System.Console.WriteLine(deviceList.Count + " device" + (deviceList.Count > 1 ? "s" : "") + " found.");
            foreach (string devName in deviceList)
            {
                System.Console.WriteLine("  Device: " + devName);
            }
            System.Console.WriteLine();

            System.Console.WriteLine("Connecting to the first device by its name: " + deviceList[0]);
            System.Console.WriteLine();
            System.Console.WriteLine("If this is the first usage of this device on this PC,");
            System.Console.WriteLine("the \"calibration file\" will be downloaded from the device.");
            System.Console.WriteLine("This can take a while.");
            System.Console.WriteLine();

            pr.UseDevice(deviceList[0]);

            System.Console.WriteLine("The device is opened.");

            System.Console.WriteLine("Closing the device.");
            pr.Close();
            System.Console.WriteLine();

            /* Opening the first device without using any device lists. */

            System.Console.WriteLine("Connecting to the first device by its ordinal number: 0");

            pr.UseDevice(0);

            System.Console.WriteLine("The device is opened.");

            System.Console.WriteLine("Closing the device.");
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

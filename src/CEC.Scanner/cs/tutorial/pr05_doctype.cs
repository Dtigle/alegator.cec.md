/* This example shows how to generate document type string.
 */
namespace tutorial
{
    using System;
    using System.Collections.Generic;
    using Pr22;
    using Pr22.Processing;

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

            //Subscribing to scan events
            pr.ScanStarted += ScanStarted;
            pr.ImageScanned += ImageScanned;
            pr.ScanFinished += ScanFinished;
            pr.DocFrameFound += DocFrameFound;

            DocScanner Scanner = pr.Scanner;
            Engine OcrEngine = pr.Engine;

            System.Console.WriteLine("Scanning some images to read from.");
            Pr22.Task.DocScannerTask ScanTask = new Pr22.Task.DocScannerTask();
            //For OCR (MRZ) reading purposes, IR (infrared) image is recommended.
            ScanTask.Add(Pr22.Imaging.Light.White).Add(Pr22.Imaging.Light.Infra);
            Page DocPage = Scanner.Scan(ScanTask, Pr22.Imaging.PagePosition.First);
            System.Console.WriteLine();

            System.Console.WriteLine("Reading all the field data.");
            Pr22.Task.EngineTask ReadingTask = new Pr22.Task.EngineTask();
            //Specify the fields we would like to receive.
            ReadingTask.Add(FieldSource.All, FieldId.All);

            Document OcrDoc = OcrEngine.Analyze(DocPage, ReadingTask);

            System.Console.WriteLine();
            System.Console.WriteLine("Document code: " + OcrDoc.ToVariant().ToInt());
            System.Console.WriteLine("Document type: " + GetDocType(OcrDoc));
            System.Console.WriteLine("Status: " + OcrDoc.GetStatus().ToString());

            pr.Close();
            return 0;
        }
        //----------------------------------------------------------------------

        public static string GetFieldValue(Pr22.Processing.Document Doc, Pr22.Processing.FieldId Id)
        {
            FieldReference filter = new FieldReference(FieldSource.All, Id);
            List<FieldReference> Fields = Doc.GetFields(filter);
            foreach (FieldReference FR in Fields)
            {
                string value = Doc.GetField(FR).GetBestStringValue();
                if (value != "") return value;
            }
            return "";
        }
        //----------------------------------------------------------------------

        public static string GetDocType(Document OcrDoc)
        {
            string documentTypeName;

            int documentCode = OcrDoc.ToVariant().ToInt();
            documentTypeName = Pr22.Extension.DocumentType.GetDocumentName(documentCode);

            if (documentTypeName == "")
            {
                string issue_country = GetFieldValue(OcrDoc, FieldId.IssueCountry);
                string issue_state = GetFieldValue(OcrDoc, FieldId.IssueState);
                string doc_type = GetFieldValue(OcrDoc, FieldId.DocType);
                string doc_page = GetFieldValue(OcrDoc, FieldId.DocPage);
                string doc_subtype = GetFieldValue(OcrDoc, FieldId.DocTypeDisc);

                string tmpval = Pr22.Extension.CountryCode.GetName(issue_country);
                if (tmpval != "") issue_country = tmpval;

                documentTypeName = issue_country + new StrCon() + issue_state
                    + new StrCon() + Pr22.Extension.DocumentType.GetDocTypeName(doc_type)
                    + new StrCon("-") + Pr22.Extension.DocumentType.GetPageName(doc_page)
                    + new StrCon(",") + doc_subtype;

            }
            return documentTypeName;
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

    /// <summary>
    /// This class makes string concatenation with spaces and prefixes.
    /// </summary>
    public class StrCon
    {
        string fstr = "";
        string cstr = "";

        public StrCon() { }

        public StrCon(string bounder) { cstr = bounder + " "; }

        public static string operator +(StrCon csv, string str)
        {
            if (str != "") str = csv.cstr + str;
            if (csv.fstr != "" && str != "" && str[0] != ',') csv.fstr += " ";
            return csv.fstr + str;
        }

        public static StrCon operator +(string str, StrCon csv)
        {
            csv.fstr = str;
            return csv;
        }
    }
}

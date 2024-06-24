using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Pr22.Processing;

namespace PassportScanner
{
    public partial class Form1 : Form
    {
        readonly Pr22.DocumentReaderDevice pr;
        bool DeviceIsConnected;
        Pr22.Task.TaskControl ScanCtrl;
        Pr22.Processing.Document AnalyzeResult;

        public Form1()
        {
            InitializeComponent();
            CheckElements();
            try { pr = new Pr22.DocumentReaderDevice(); }
            catch (Exception ex)
            {
                if (ex is DllNotFoundException || ex is Pr22.Exceptions.FileOpen)
                {
                    int platform = IntPtr.Size * 8;
                    int codepl = GetCodePlatform();

                    MessageBox.Show("This sample program" + (codepl == 0 ? " is compiled for Any CPU and" : "") +
                        " is running on " + platform + " bit platform.\n" +
                        "Please check if the Passport Reader is installed correctly or compile your code for "
                        + (96 - platform) + " bit.\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void CheckElements()
        {
            try
            {
                OCRParamsCheckedListBox.SetItemChecked(0, true);
                OCRParamsCheckedListBox.SetItemChecked(1, true);
                OCRParamsCheckedListBox.SetItemChecked(2, true);
            }
            catch
            {

            }
        }

        private void FormLoad(object sender, EventArgs e)
        {
            if (pr == null) { Close(); return; }

            pr.Connection += DeviceConnected;

            pr.PresenceStateChanged += DocumentStateChanged;
            pr.ImageScanned += ImageScanned;
            pr.ScanFinished += ScanFinished;
            pr.DocFrameFound += DocFrameFound;
        }

        void FormClose(object sender, FormClosingEventArgs e)
        {
            if (DeviceIsConnected)
            {
                CloseScan();
                pr.Close();
            }
        }

        #region Connection
        //----------------------------------------------------------------------

        // This raises only when no device is used or when the currently used
        // device is disconnected.
        void DeviceConnected(object sender, Pr22.Events.ConnectionEventArgs e)
        {
            UpdateDeviceList();
        }

        void UpdateDeviceList()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(UpdateDeviceList));
                return;
            }
            List<string> Devices = Pr22.DocumentReaderDevice.GetDeviceList();
            DevicesListBox.Items.Clear();
            foreach (string s in Devices) DevicesListBox.Items.Add(s);
        }

        void ConnectButton_Click(object sender, EventArgs e)
        {
            //TODO For test only
            //Thread.Sleep(1000);
            //var value = "2008033001805";
            //if (!string.IsNullOrWhiteSpace(value))
            //{
            //    if (value.Length == 13)
            //    {
            //        SendKeys.Send(value);
            //        SendKeys.Send("{Enter}");
            //    }
            //}

            if (DevicesListBox.SelectedIndex < 0) return;

            ConnectButton.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                pr.UseDevice(DevicesListBox.Text);
                DeviceIsConnected = true;
                pr.Scanner.StartTask(Pr22.Task.FreerunTask.Detection());
                DisconnectButton.Enabled = true;
                List<Pr22.Imaging.Light> Lights = pr.Scanner.Info.GetLights();
                foreach (Pr22.Imaging.Light light in Lights)
                    LightsCheckedListBox.Items.Add(light);
                StartButton.Enabled = true;
            }
            catch (Pr22.Exceptions.General ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                DisconnectButton_Click(sender, e);
            }

            try
            {
                LightsCheckedListBox.SetItemChecked(0,true);
                LightsCheckedListBox.SetItemChecked(1, true);
            }
            catch (Pr22.Exceptions.General ex)
            {
            }

            Cursor = Cursors.Default;
        }

        void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (DeviceIsConnected)
            {
                CloseScan();
                Application.DoEvents();
                pr.Close();
                DeviceIsConnected = false;
            }
            ConnectButton.Enabled = true;
            DisconnectButton.Enabled = false;
            StartButton.Enabled = false;
            LightsCheckedListBox.Items.Clear();
            FieldsTabControl.Controls.Clear();
            FieldsTabControl.Controls.Add(OcrTab);
            FieldsTabControl.Controls.Add(DataTab);
            FieldsDataGridView.Rows.Clear();
            ClearOCRData();
            ClearDataPage();
        }

        #endregion

        #region Scanning
        //----------------------------------------------------------------------

        // To raise this event FreerunTask.Detection() has to be started.
        void DocumentStateChanged(object sender, Pr22.Events.DetectionEventArgs e)
        {
            if (e.State == Pr22.Util.PresenceState.Present)
            {
                BeginInvoke(new EventHandler(StartButton_Click), sender, e);
            }
        }

        void StartButton_Click(object sender, EventArgs e)
        {
            FieldsTabControl.Controls.Clear();
            FieldsTabControl.Controls.Add(OcrTab);
            FieldsTabControl.Controls.Add(DataTab);
            FieldsDataGridView.Rows.Clear();
            ClearOCRData();
            ClearDataPage();
            if (LightsCheckedListBox.CheckedItems.Count == 0)
            {
                MessageBox.Show("No light selected to scan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            StartButton.Enabled = false;
            Pr22.Task.DocScannerTask ScanTask = new Pr22.Task.DocScannerTask();
            foreach (Pr22.Imaging.Light light in LightsCheckedListBox.CheckedItems)
            {
                AddTabPage(light.ToString());
                ScanTask.Add(light);
            }
            ScanCtrl = pr.Scanner.StartScanning(ScanTask, Pr22.Imaging.PagePosition.First);
        }

        void ImageScanned(object sender, Pr22.Events.ImageEventArgs e)
        {
            DrawImage(e);
        }

        // To rotate the document to upside down direction the Analyze() should
        // be called.
        void DocFrameFound(object sender, Pr22.Events.PageEventArgs e)
        {
            if (!DocViewCheckBox.Checked) return;
            foreach (Control tab in FieldsTabControl.Controls)
            {
                try
                {
                    Pr22.Imaging.Light light = (Pr22.Imaging.Light)Enum.Parse(typeof(Pr22.Imaging.Light), tab.Text);
                    if (((PictureBox)tab.Controls[0]).Image != null)
                        DrawImage(new Pr22.Events.ImageEventArgs(e.Page, light));
                }
                catch (System.ArgumentException) { }
            }
        }

        void DrawImage(Pr22.Events.ImageEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Pr22.Events.ImageEventArgs>(DrawImage), e);
                return;
            }
            Pr22.Imaging.DocImage docImage = pr.Scanner.GetPage(e.Page).Select(e.Light);
            Control[] tabs = FieldsTabControl.Controls.Find(e.Light.ToString(), false);
            //if (tabs.Length == 0) tabs = AddTabPage(e.Light.ToString());
            if (tabs.Length == 0) return;
            PictureBox pb = (PictureBox)tabs[0].Controls[0];
            Bitmap bmap = docImage.ToBitmap();
            if (DocViewCheckBox.Checked)
            {
                try { bmap = docImage.DocView().ToBitmap(); }
                catch (Pr22.Exceptions.General) { }
            }
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = bmap;
            pb.Refresh();
        }

        void ScanFinished(object sender, Pr22.Events.PageEventArgs e)
        {
            BeginInvoke(new MethodInvoker(Analyze));
            BeginInvoke(new MethodInvoker(CloseScan));
        }

        void CloseScan()
        {
            try { if (ScanCtrl != null) ScanCtrl.Wait(); }
            catch (Pr22.Exceptions.General ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            ScanCtrl = null;
            StartButton.Enabled = true;
        }

        #endregion

        #region Analyzing
        //----------------------------------------------------------------------

        void Analyze()
        {
            Pr22.Task.EngineTask OcrTask = new Pr22.Task.EngineTask();

            if (OCRParamsCheckedListBox.GetItemCheckState(0) == CheckState.Checked)
                OcrTask.Add(FieldSource.Mrz, FieldId.All);
            if (OCRParamsCheckedListBox.GetItemCheckState(1) == CheckState.Checked)
                OcrTask.Add(FieldSource.Viz, FieldId.All);
            if (OCRParamsCheckedListBox.GetItemCheckState(2) == CheckState.Checked)
                OcrTask.Add(FieldSource.Barcode, FieldId.All);

            Pr22.Processing.Page page;
            try { page = pr.Scanner.GetPage(0); }
            catch (Pr22.Exceptions.General) { return; }
            try { AnalyzeResult = pr.Engine.Analyze(page, OcrTask); }
            catch (Pr22.Exceptions.General ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            FillOcrDataGrid();
            FillDataPage();
        }

        void FillOcrDataGrid()
        {
            string personalData1 = String.Empty;
            string personalData2 = String.Empty;
            string barCodeData = String.Empty;
            string expirationDate = String.Empty;
            int expired = 0;
            List<Pr22.Processing.FieldReference> Fields = AnalyzeResult.GetFields();
            for (int i = 0; i < Fields.Count; i++)
            {
                try
                {
                    Pr22.Processing.Field field = AnalyzeResult.GetField(Fields[i]);
                    string[] values = new string[4];
                    values[0] = i.ToString();
                    values[1] = Fields[i].ToString(" ") + new StrCon() + GetAmid(field);
                    try { values[2] = field.GetBestStringValue(); }
                    catch (Pr22.Exceptions.InvalidParameter)
                    {
                        values[2] = PrintBinary(field.GetBinaryValue(), 0, 16);
                    }
                    catch (Pr22.Exceptions.General) { }

                    values[3] = field.GetStatus().ToString();

                    FieldsDataGridView.Rows.Add(values);


                    // Get identification code from passport
                    try
                    {
                        if (Fields[i].Id == FieldId.PersonalData1)
                            if (values[3] == "Ok")
                                personalData1 = values[2];
                    }
                    catch
                    {
                        // ignored
                    }

                    // Get identification code from ca
                    try
                    {
                        if (Fields[i].Id == FieldId.PersonalData2) personalData2 = values[2];
                    }
                    catch
                    {
                        // ignored
                    }

                    // Get expiration date
                    try
                    {
                        if (Fields[i].Id == FieldId.ExpiryDate) expirationDate = values[2];
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        if (Fields[i].Id == FieldId.Composite1)
                        {
                            if (values[1] == "Barcode Composite1")
                            {
                                barCodeData = values[2];
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                catch (Pr22.Exceptions.General)
                {
                }
            }

            if (!string.IsNullOrEmpty(expirationDate))
            {
                try
                {
                    //TODO disabled for current version
                    //var dt = DateTime.ParseExact(expirationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    //if (dt < DateTime.Now) expired = 1;
                }
                catch
                {
                    expired = 0;
                }
            }

            if (!string.IsNullOrEmpty(personalData1))
            {
                SetScanToClipboard(personalData1, expired);
            }
            else
            {
                if (!string.IsNullOrEmpty(personalData2))
                {
                    SetScanToClipboard(personalData2, expired);
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(personalData1) || !string.IsNullOrEmpty(personalData2)) return;

                if (string.IsNullOrEmpty(barCodeData)) return;

                var lines = barCodeData.Split('\n');

                if (lines.Length == 1)
                {
                    var validIdentificationCode = GetDataFromOldCa(lines);
                    if(!string.IsNullOrEmpty(validIdentificationCode)) SetScanToClipboard(validIdentificationCode, 0);
                }
                else if (!string.IsNullOrEmpty(lines[8]))
                {
                    expired = GetIfExpiredDate(lines);
                    SetScanToClipboard(lines[0], expired);
                }
            }
            catch
            {
                // ignored
            }
        }

        #region ClipBoard Methods

        private static void SetScanToClipboard(string value, int expireDate)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Length == 13)
                {
                    SendKeys.Send(value);
                    SendKeys.Send("{Enter}");
                }
            }

            //TODO disabled for current version
            //Clipboard.SetText($"scan={value}&{expireDate}");
        }

        private static int GetIfExpiredDate(string[] lines)
        {
            //TODO disabled for current version
            //try
            //{
            //    for (var z = 0; z < lines.Length; z += 1)
            //        lines[z] = lines[z].Trim();

            //    var date   = lines[8];
            //    var result = date.Any(x => !char.IsLetter(x));
            //    if (!result) return 0;

            //    var dt = DateTime.ParseExact(date, "dd MM yyyy", CultureInfo.InvariantCulture);
            //    return dt < DateTime.Now ? 1 : 0;
            //}
            //catch
            //{
            //    return 0;
            //}
            return 0;
        }

        private static string GetDataFromOldCa(string[] lines)
        {
            try
            {
                var valuesArray = lines[0].Split(' ');
                var identificationCode = valuesArray.OrderByDescending(s => s.Length).First();
                var validIdentificationCode = identificationCode.Substring(0, 13);
                var isNumeric = long.TryParse(validIdentificationCode, out long n);
                return isNumeric ? validIdentificationCode : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        void FieldsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ClearOCRData();
            if (FieldsDataGridView.SelectedCells.Count == 0) return;
            int ix = FieldsDataGridView.SelectedCells[0].RowIndex;
            if (AnalyzeResult == null || ix < 0 || AnalyzeResult.GetFields().Count <= ix
                || ix == FieldsDataGridView.Rows.Count - 1) return;

            ix = int.Parse(FieldsDataGridView.Rows[ix].Cells[0].Value.ToString());
            Pr22.Processing.FieldReference SelectedField = AnalyzeResult.GetFields()[ix];
            Pr22.Processing.Field field = AnalyzeResult.GetField(SelectedField);
            try { RAWValueLabel.Text = field.GetRawStringValue(); }
            catch (Pr22.Exceptions.General) { }
            try { FormattedValueLabel.Text = field.GetFormattedStringValue(); }
            catch (Pr22.Exceptions.General) { }
            try { StandardizedValueLabel.Text = field.GetStandardizedStringValue(); }
            catch (Pr22.Exceptions.General) { }
            try { FieldImagePictureBox.Image = field.GetImage().ToBitmap(); }
            catch (Pr22.Exceptions.General) { }
        }

        void FillDataPage()
        {
            Name1.Text = GetFieldValue(FieldId.Surname);
            if (Name1.Text != "")
            {
                Name1.Text += " " + GetFieldValue(FieldId.Surname2);
                Name2.Text = GetFieldValue(FieldId.Givenname) + new StrCon()
                    + GetFieldValue(FieldId.MiddleName);
            }
            else Name1.Text = GetFieldValue(FieldId.Name);

            Birth.Text = new StrCon("on") + GetFieldValue(FieldId.BirthDate)
                + new StrCon("in") + GetFieldValue(FieldId.BirthPlace);

            Nationality.Text = GetFieldValue(FieldId.Nationality);

            Sex.Text = GetFieldValue(FieldId.Sex);

            Issuer.Text = GetFieldValue(FieldId.IssueCountry) + new StrCon()
                + GetFieldValue(FieldId.IssueState);

            Type.Text = GetFieldValue(FieldId.DocType) + new StrCon()
                + GetFieldValue(FieldId.DocTypeDisc);
            if (Type.Text == "") Type.Text = GetFieldValue(FieldId.Type);

            Page.Text = GetFieldValue(FieldId.DocPage);

            Number.Text = GetFieldValue(FieldId.DocumentNumber);

            Validity.Text = new StrCon("from") + GetFieldValue(FieldId.IssueDate)
                + new StrCon("to") + GetFieldValue(FieldId.ExpiryDate);

            try
            {
                PhotoPictureBox.Image = AnalyzeResult.GetField(FieldSource.Viz,
                    FieldId.Face).GetImage().ToBitmap();
            }
            catch (Pr22.Exceptions.General) { }

            try
            {
                SignaturePictureBox.Image = AnalyzeResult.GetField(FieldSource.Viz,
                    FieldId.Signature).GetImage().ToBitmap();
            }
            catch (Pr22.Exceptions.General) { }
        }

        #endregion

        #region General tools
        //----------------------------------------------------------------------

        string GetAmid(Field field)
        {
            try
            {
                return field.ToVariant().GetChild((int)Pr22.Util.VariantId.AMID, 0);
            }
            catch (Pr22.Exceptions.General) { return ""; }
        }

        string GetFieldValue(Pr22.Processing.FieldId Id)
        {
            FieldReference filter = new FieldReference(FieldSource.All, Id);
            List<FieldReference> Fields = AnalyzeResult.GetFields(filter);
            foreach (FieldReference FR in Fields)
            {
                string value = AnalyzeResult.GetField(FR).GetBestStringValue();
                if (value != "") return value;
            }
            return "";
        }

        static string PrintBinary(byte[] arr, int pos, int sz)
        {
            int p0;
            string str = "", str2 = "";
            for (p0 = pos; p0 < arr.Length && p0 < pos + sz; p0++)
            {
                str += arr[p0].ToString("X2") + " ";
                str2 += arr[p0] < 0x21 || arr[p0] > 0x7e ? '.' : (char)arr[p0];
            }
            for (; p0 < pos + sz; p0++) { str += "   "; str2 += " "; }
            return str + str2;
        }

        Control[] AddTabPage(string lightName)
        {
            TabPage ImageTabPage = new TabPage(lightName);
            ImageTabPage.Name = lightName;
            PictureBox PBox = new PictureBox();
            ImageTabPage.Controls.Add(PBox);
            FieldsTabControl.Controls.Add(ImageTabPage);
            PBox.Size = ImageTabPage.Size;
            return new Control[1] { ImageTabPage };
        }

        void ClearOCRData()
        {
            FieldImagePictureBox.Image = null;
            RAWValueLabel.Text = FormattedValueLabel.Text = StandardizedValueLabel.Text = "";
        }

        void ClearDataPage()
        {
            Name1.Text = Name2.Text = Birth.Text = Nationality.Text = Sex.Text =
                Issuer.Text = Type.Text = Page.Text = Number.Text = Validity.Text = "";
            PhotoPictureBox.Image = null;
            SignaturePictureBox.Image = null;
        }

        int GetCodePlatform()
        {
            System.Reflection.PortableExecutableKinds pek;
            System.Reflection.ImageFileMachine mac;
            System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.GetPEKind(out pek, out mac);

            if ((pek & System.Reflection.PortableExecutableKinds.PE32Plus) != 0) return 64;
            if ((pek & System.Reflection.PortableExecutableKinds.Required32Bit) != 0) return 32;
            return 0;
        }

        #endregion

        private void OCRParamsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void OCRGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void LightsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
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

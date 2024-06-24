using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Pr22.Processing;

namespace ReadCard
{
    public partial class Form1 : Form
    {
        readonly Pr22.DocumentReaderDevice pr;
        bool DeviceIsConnected;
        Pr22.Task.TaskControl ReadCtrl;
        Pr22.ECard Card;
        Pr22.Processing.Document VizResult;
        Pr22.Processing.Document FaceDoc;

        public Form1()
        {
            InitializeComponent();
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

        private void FormLoad(object sender, EventArgs e)
        {
            if (pr == null) { Close(); return; }

            pr.Connection += DeviceConnected;

            pr.AuthBegin += AuthBegin;
            pr.AuthFinished += AuthFinished;
            pr.AuthWaitForInput += AuthWaitForInput;
            pr.ReadBegin += ReadBegin;
            pr.ReadFinished += ReadFinished;
            pr.FileChecked += FileChecked;

            string[] values = Enum.GetNames(typeof(Pr22.ECardHandling.FileId));
            foreach (string file in values)
                FilesListView.Items.Add(file);

            foreach (Pr22.ECardHandling.AuthLevel level in Enum.GetValues(typeof(Pr22.ECardHandling.AuthLevel)))
                AuthSelector.Items.Add(level);

            AuthSelector.SelectedIndex = 1;

            LoadCertificates(Properties.Settings.Default.CertDir);
        }

        void FormClose(object sender, FormClosingEventArgs e)
        {
            if (ReadCtrl != null) ReadCtrl.Stop().Wait();
            if (DeviceIsConnected) pr.Close();
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
            if (DevicesListBox.SelectedIndex < 0) return;

            ConnectButton.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                pr.UseDevice(DevicesListBox.Text);
                DeviceIsConnected = true;
                DisconnectButton.Enabled = true;
                List<Pr22.ECardReader> Readers = pr.Readers;
                foreach (Pr22.ECardReader reader in Readers)
                    ReadersCheckedListBox.Items.Add(reader.Info.HwType.ToString());
                StartButton.Enabled = true;
            }
            catch (Pr22.Exceptions.General ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                DisconnectButton_Click(sender, e);
            }
            Cursor = Cursors.Default;
        }

        void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (DeviceIsConnected)
            {
                if (ReadCtrl != null)
                {
                    ReadCtrl.Stop().Wait();
                    ReadCtrl = null;
                    Application.DoEvents();
                }
                if (Card != null)
                {
                    Card.Disconnect();
                    Card = null;
                }
                pr.Close();
                DeviceIsConnected = false;
            }
            ConnectButton.Enabled = true;
            DisconnectButton.Enabled = false;
            StartButton.Enabled = false;
            ReadersCheckedListBox.Items.Clear();
            textBox1.Clear();
        }

        #endregion

        #region Reading
        //----------------------------------------------------------------------

        void StartButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            ClearControls();

            if (ReadCtrl != null)
            {
                ReadCtrl.Wait();
                ReadCtrl = null;
            }
            if (Card != null)
            {
                try { Card.Disconnect(); }
                catch (Pr22.Exceptions.General) { }
                Card = null;
            }

            Pr22.ECardReader Reader = null;
            foreach (Pr22.ECardReader reader in pr.Readers)
            {
                if (ReadersCheckedListBox.CheckedItems.Contains(reader.Info.HwType.ToString()))
                {
                    List<string> Cards = reader.GetCards();
                    if (Cards.Count > 0)
                    {
                        Card = reader.ConnectCard(0);
                        Reader = reader;
                        break;
                    }
                }
            }
            if (Reader != null && Card != null)
            {
                StartReading(Reader);
            }
        }

        void StartReading(Pr22.ECardReader Reader)
        {
            ClearControls();
            StartButton.Enabled = false;

            LogText("Scanning");
            Pr22.Task.DocScannerTask ScanTask = new Pr22.Task.DocScannerTask();
            ScanTask.Add(Pr22.Imaging.Light.Infra).Add(Pr22.Imaging.Light.White);
            Pr22.Processing.Page page = pr.Scanner.Scan(ScanTask, Pr22.Imaging.PagePosition.First);

            LogText("Analyzing");
            Pr22.Task.EngineTask EngineTask = new Pr22.Task.EngineTask();
            EngineTask.Add(FieldSource.Mrz, FieldId.All);
            EngineTask.Add(FieldSource.Viz, FieldId.CAN);

            Pr22.Processing.FieldReference FaceFieldId, SignatureFieldId;
            FaceFieldId = new FieldReference(FieldSource.Viz, FieldId.Face);
            EngineTask.Add(FaceFieldId);
            SignatureFieldId = new FieldReference(FieldSource.Viz, FieldId.Signature);
            EngineTask.Add(SignatureFieldId);
            VizResult = pr.Engine.Analyze(page, EngineTask);
            FaceDoc = null;

            try { DrawImage(FacePictureBox2, VizResult.GetField(FaceFieldId).GetImage().ToBitmap()); }
            catch (Pr22.Exceptions.General) { }
            try { DrawImage(SignaturePictureBox2, VizResult.GetField(SignatureFieldId).GetImage().ToBitmap()); }
            catch (Pr22.Exceptions.General) { }

            Pr22.Task.ECardTask task = new Pr22.Task.ECardTask();
            task.AuthLevel = (Pr22.ECardHandling.AuthLevel)AuthSelector.SelectedItem;

            foreach (ListViewItem item in FilesListView.CheckedItems)
                task.Add((Pr22.ECardHandling.FileId)Enum.Parse(typeof(Pr22.ECardHandling.FileId), item.Text));

            try { ReadCtrl = Reader.StartRead(Card, task); }
            catch (Pr22.Exceptions.General) { }
        }

        void AuthBegin(object sender, Pr22.Events.AuthEventArgs e)
        {
            LogText("Auth Begin: " + e.Authentication.ToString());
            ColorAuthLabel(e.Authentication, Color.Yellow);
        }

        void AuthFinished(object sender, Pr22.Events.AuthEventArgs e)
        {
            string errstr = e.Result.ToString();
            if (!Enum.IsDefined(typeof(Pr22.Exceptions.ErrorCodes), e.Result))
                errstr = ((int)e.Result).ToString("X4");

            LogText("Auth Done: " + e.Authentication.ToString() + " status: " + errstr);
            bool ok = e.Result == Pr22.Exceptions.ErrorCodes.ENOERR;
            ColorAuthLabel(e.Authentication, ok ? Color.Green : Color.Red);
        }

        void AuthWaitForInput(object sender, Pr22.Events.AuthEventArgs e)
        {
            LogText("Auth Wait For Input: " + e.Authentication.ToString());
            ColorAuthLabel(e.Authentication, Color.Yellow);

            Pr22.Processing.BinData AuthData = null;
            int selector = 0;

            switch (e.Authentication)
            {
                case Pr22.ECardHandling.AuthProcess.BAC:
                case Pr22.ECardHandling.AuthProcess.BAP:
                case Pr22.ECardHandling.AuthProcess.PACE:

                    List<Pr22.Processing.FieldReference> authFields;
                    Pr22.Processing.FieldReference fr;
                    fr = new FieldReference(FieldSource.Mrz, FieldId.All);
                    authFields = VizResult.GetFields(fr);
                    selector = 1;
                    if (authFields.Count == 0)
                    {
                        fr = new FieldReference(FieldSource.Viz, FieldId.CAN);
                        authFields = VizResult.GetFields(fr);
                        selector = 2;
                    }
                    if (authFields.Count == 0) break;

                    AuthData = new Pr22.Processing.BinData();
                    AuthData.SetString(VizResult.GetField(fr).GetBestStringValue());
                    break;
            }
            try
            {
                Card.Authenticate(e.Authentication, AuthData, selector);
            }
            catch (Pr22.Exceptions.General) { }
        }

        void ReadBegin(object sender, Pr22.Events.FileEventArgs e)
        {
            LogText("Read Begin: " + e.FileId.ToString());
        }

        void ReadFinished(object sender, Pr22.Events.FileEventArgs e)
        {
            string errstr = e.Result.ToString();
            if (!Enum.IsDefined(typeof(Pr22.Exceptions.ErrorCodes), e.Result))
                errstr = ((int)e.Result).ToString("X4");

            LogText("Read End: " + e.FileId.ToString() + " result: " + errstr);

            if (e.FileId.Id == (int)Pr22.ECardHandling.FileId.All)
            {
                ProcessAfterAllRead();
                BeginInvoke(new MethodInvoker(delegate { StartButton.Enabled = true; }));
            }
            else if (e.Result != Pr22.Exceptions.ErrorCodes.ENOERR)
            {
                ColorFileName(e.FileId, Color.Red);
            }
            else
            {
                ColorFileName(e.FileId, Color.Blue);
                ProcessAfterFileRead(e.FileId);
            }
        }

        void ProcessAfterAllRead()
        {
            try
            {
                string mrz = VizResult.GetField(FieldSource.Mrz, FieldId.All).GetRawStringValue();
                string dg1 = MRZTextBox.Text.Replace("\r", "");
                if (dg1.Length > 40)
                    ColorLabel(MrzLabel, (mrz == dg1 ? Color.Green : Color.Red));
            }
            catch (Pr22.Exceptions.General) { }
            try
            {
                Pr22.Processing.Document facecmp = VizResult + FaceDoc;
                List<Pr22.Processing.FieldCompare> fcl = facecmp.GetFieldCompareList();
                foreach (Pr22.Processing.FieldCompare fc in fcl)
                    if (fc.field1.Id == FieldId.Face && fc.field2.Id == FieldId.Face)
                    {
                        Color col = Color.Yellow;
                        if (fc.confidence < 300) col = Color.Red;
                        else if (fc.confidence > 600) col = Color.Green;
                        ColorLabel(FaceLabel, col);
                    }
            }
            catch (Pr22.Exceptions.General) { }
        }

        void ProcessAfterFileRead(Pr22.ECardHandling.File File)
        {
            try
            {
                Pr22.Processing.BinData RawFileContent = Card.GetFile(File);
                Pr22.Processing.Document FileDoc = pr.Engine.Analyze(RawFileContent);

                Pr22.Processing.FieldReference FaceFieldId = new FieldReference(FieldSource.ECard, FieldId.Face);
                Pr22.Processing.FieldReference MrzFieldId = new FieldReference(FieldSource.ECard, FieldId.CompositeMrz);
                Pr22.Processing.FieldReference SignatureFieldId = new FieldReference(FieldSource.ECard, FieldId.Signature);
                Pr22.Processing.FieldReference FingerFieldId = new FieldReference(FieldSource.ECard, FieldId.Fingerprint);

                if (FileDoc.GetFields().Contains(FaceFieldId))
                {
                    FaceDoc = FileDoc;
                    DrawImage(FacePictureBox1, FileDoc.GetField(FaceFieldId).GetImage().ToBitmap());
                }
                if (FileDoc.GetFields().Contains(MrzFieldId))
                {
                    string mrz = FileDoc.GetField(MrzFieldId).GetRawStringValue();
                    if (mrz.Length == 90) mrz = mrz.Insert(60, "\r\n").Insert(30, "\r\n");
                    else if (mrz.Length > 50) mrz = mrz.Insert(mrz.Length / 2, "\r\n");
                    PrintMrzLines(mrz);
                }
                if (FileDoc.GetFields().Contains(SignatureFieldId))
                {
                    DrawImage(SignaturePictureBox1, FileDoc.GetField(SignatureFieldId).GetImage().ToBitmap());
                }
                if (FileDoc.GetFields().Contains(FingerFieldId))
                {
                    try
                    {
                        DrawImage(FingerPictureBox1, FileDoc.GetField(FieldSource.ECard, FieldId.Fingerprint, 0).GetImage().ToBitmap());
                        DrawImage(FingerPictureBox2, FileDoc.GetField(FieldSource.ECard, FieldId.Fingerprint, 1).GetImage().ToBitmap());
                    }
                    catch (Exception) { }
                }
            }
            catch (Pr22.Exceptions.General) { }
        }

        void FileChecked(object sender, Pr22.Events.FileEventArgs e)
        {
            LogText("File Checked: " + e.FileId.ToString());
            bool ok = e.Result == Pr22.Exceptions.ErrorCodes.ENOERR;
            ColorFileName(e.FileId, (ok ? Color.Green : Color.Yellow));
        }

        #endregion

        #region General tools
        //----------------------------------------------------------------------

        void ColorFileName(Pr22.ECardHandling.File file, Color color)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate { ColorFileName(file, color); }));
                return;
            }
            ListViewItem fi = FilesListView.FindItemWithText(file.ToString());
            if (fi == null)
            {
                try { file = Card.ConvertFileId(file); }
                catch (Pr22.Exceptions.General) { }
                fi = FilesListView.FindItemWithText(file.ToString());
            }
            if (fi != null) fi.ForeColor = color;
        }

        void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (FilesListView.SelectedItems.Count == 0 || Card == null) return;

            Pr22.Processing.BinData filedata = null;

            foreach (ListViewItem item in FilesListView.SelectedItems)
            {
                if (item.ForeColor != Color.Black && item.ForeColor != Color.Red)
                {
                    Pr22.ECardHandling.FileId file;
                    file = (Pr22.ECardHandling.FileId)Enum.Parse(typeof(Pr22.ECardHandling.FileId), item.Text);

                    saveFileDialog1.Filter = "binary file (*.bin)|*.bin|document file (*.xml)|*.xml";
                    saveFileDialog1.FileName = file.ToString();
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        filedata = Card.GetFile(file);
                        if (saveFileDialog1.FilterIndex == 1) filedata.Save(saveFileDialog1.FileName);
                        else if (saveFileDialog1.FilterIndex == 2)
                            pr.Engine.Analyze(filedata).Save(Pr22.Processing.Document.FileFormat.Xml)
                                .Save(saveFileDialog1.FileName);

                    }
                    break;
                }
            }
        }

        System.Collections.ArrayList FileList(string dirname, string mask)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(dirname);
                foreach (System.IO.DirectoryInfo d in dir.GetDirectories())
                    list.AddRange(FileList(dir.FullName + "/" + d.Name, mask));
                foreach (System.IO.FileInfo f in dir.GetFiles(mask))
                    list.Add(f.FullName);
            }
            catch (System.Exception) { }
            return list;
        }

        void LoadCertificates(string dir)
        {
            string[] exts = { "*.cer", "*.crt", "*.der", "*.pem", "*.crl", "*.cvcert", "*.ldif", "*.ml" };

            foreach (string ext in exts)
            {
                System.Collections.ArrayList list = FileList(dir, ext);
                foreach (string file in list)
                {
                    try
                    {
                        Pr22.Processing.BinData fd = new BinData().Load(file);
                        string pk = null;
                        if (ext == "*.cvcert")
                        {
                            //Searching for private key
                            pk = file.Substring(0, file.LastIndexOf('.') + 1) + "pkcs8";
                            if (!System.IO.File.Exists(pk)) pk = null;
                        }
                        if (pk == null) pr.GlobalCertificateManager.Load(fd);
                        else pr.GlobalCertificateManager.Load(fd, new BinData().Load(pk));
                    }
                    catch (Pr22.Exceptions.General) { }
                }
            }
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

        #region Display
        //----------------------------------------------------------------------

        void LogText(string s)
        {
            if (InvokeRequired) BeginInvoke(new Action<string>(LogText), s);
            else textBox1.AppendText(s + "\r\n");
        }

        void PrintMrzLines(string mrz)
        {
            if (InvokeRequired) BeginInvoke(new Action<string>(PrintMrzLines), mrz);
            else MRZTextBox.Text = mrz;
        }

        void ColorAuthLabel(Pr22.ECardHandling.AuthProcess auth, Color color)
        {
            Label label;

            switch (auth)
            {
                case Pr22.ECardHandling.AuthProcess.BAC:
                case Pr22.ECardHandling.AuthProcess.BAP:
                    label = BACLabel;
                    break;
                case Pr22.ECardHandling.AuthProcess.Active:
                    label = AALabel;
                    break;
                case Pr22.ECardHandling.AuthProcess.Chip:
                    label = CALabel;
                    break;
                case Pr22.ECardHandling.AuthProcess.PACE:
                    label = PACELabel;
                    break;
                case Pr22.ECardHandling.AuthProcess.Passive:
                    label = PALabel;
                    break;
                case Pr22.ECardHandling.AuthProcess.Terminal:
                    label = TALabel;
                    break;
                default:
                    return;
            }

            ColorLabel(label, color);
        }

        void ColorLabel(Label lbl, Color col)
        {
            if (InvokeRequired)
                BeginInvoke(new MethodInvoker(delegate { ColorLabel(lbl, col); }));
            else
                lbl.ForeColor = col;
        }

        void DrawImage(PictureBox pbox, Image bmp)
        {
            if (InvokeRequired)
                BeginInvoke(new MethodInvoker(delegate { DrawImage(pbox, bmp); }));
            else
                pbox.Image = bmp;
        }

        void ClearControls()
        {
            MRZTextBox.Clear();
            FacePictureBox1.Image = null;
            FacePictureBox2.Image = null;
            SignaturePictureBox1.Image = null;
            SignaturePictureBox2.Image = null;
            FingerPictureBox1.Image = null;
            FingerPictureBox2.Image = null;

            AALabel.ForeColor = Color.Black;
            BACLabel.ForeColor = Color.Black;
            PACELabel.ForeColor = Color.Black;
            CALabel.ForeColor = Color.Black;
            TALabel.ForeColor = Color.Black;
            PALabel.ForeColor = Color.Black;
            MrzLabel.ForeColor = Color.Black;
            FaceLabel.ForeColor = Color.Black;

            foreach (ListViewItem item in FilesListView.Items)
                item.ForeColor = Color.Black;

            Update();
        }

        #endregion
    }
}

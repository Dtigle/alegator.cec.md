using System.Drawing;

namespace PassportScanner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.OptionsTabControl = new System.Windows.Forms.TabControl();
            this.OptionsTab = new System.Windows.Forms.TabPage();
            this.DevicesGroupBox = new System.Windows.Forms.GroupBox();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.DevicesListBox = new System.Windows.Forms.ListBox();
            this.OCRGroupBox = new System.Windows.Forms.GroupBox();
            this.OCRParamsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.LightsGroupBox = new System.Windows.Forms.GroupBox();
            this.LightsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.DocViewCheckBox = new System.Windows.Forms.CheckBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.FieldsTabControl = new System.Windows.Forms.TabControl();
            this.OcrTab = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.FieldsDataGridView = new System.Windows.Forms.DataGridView();
            this.IndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldImagePictureBox = new System.Windows.Forms.PictureBox();
            this.FieldImageGroup = new System.Windows.Forms.GroupBox();
            this.ValuesGroup = new System.Windows.Forms.GroupBox();
            this.RAWValueLabel = new System.Windows.Forms.Label();
            this.RLabel = new System.Windows.Forms.Label();
            this.FLabel = new System.Windows.Forms.Label();
            this.StandardizedValueLabel = new System.Windows.Forms.Label();
            this.SLabel = new System.Windows.Forms.Label();
            this.FormattedValueLabel = new System.Windows.Forms.Label();
            this.DataTab = new System.Windows.Forms.TabPage();
            this.SignatureGroupBox = new System.Windows.Forms.GroupBox();
            this.SignaturePictureBox = new System.Windows.Forms.PictureBox();
            this.PhotoGroupBox = new System.Windows.Forms.GroupBox();
            this.PhotoPictureBox = new System.Windows.Forms.PictureBox();
            this.DocumentGroupBox = new System.Windows.Forms.GroupBox();
            this.Validity = new System.Windows.Forms.Label();
            this.Number = new System.Windows.Forms.Label();
            this.Page = new System.Windows.Forms.Label();
            this.Type = new System.Windows.Forms.Label();
            this.Issuer = new System.Windows.Forms.Label();
            this.ValidityLabel = new System.Windows.Forms.Label();
            this.IssuerLabel = new System.Windows.Forms.Label();
            this.PageLabel = new System.Windows.Forms.Label();
            this.DocNumberLabel = new System.Windows.Forms.Label();
            this.TypeLabel = new System.Windows.Forms.Label();
            this.PersonalGroupBox = new System.Windows.Forms.GroupBox();
            this.Sex = new System.Windows.Forms.Label();
            this.Nationality = new System.Windows.Forms.Label();
            this.Birth = new System.Windows.Forms.Label();
            this.Name2 = new System.Windows.Forms.Label();
            this.Name1 = new System.Windows.Forms.Label();
            this.SexLabel = new System.Windows.Forms.Label();
            this.NationalityLabel = new System.Windows.Forms.Label();
            this.BirthLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.OptionsTabControl.SuspendLayout();
            this.OptionsTab.SuspendLayout();
            this.DevicesGroupBox.SuspendLayout();
            this.OCRGroupBox.SuspendLayout();
            this.LightsGroupBox.SuspendLayout();
            this.FieldsTabControl.SuspendLayout();
            this.OcrTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FieldsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FieldImagePictureBox)).BeginInit();
            this.ValuesGroup.SuspendLayout();
            this.DataTab.SuspendLayout();
            this.SignatureGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SignaturePictureBox)).BeginInit();
            this.PhotoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PhotoPictureBox)).BeginInit();
            this.DocumentGroupBox.SuspendLayout();
            this.PersonalGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.OptionsTabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.FieldsTabControl);
            this.splitContainer1.Size = new System.Drawing.Size(1016, 533);
            this.splitContainer1.SplitterDistance = 283;
            this.splitContainer1.TabIndex = 2;
            // 
            // OptionsTabControl
            // 
            this.OptionsTabControl.Controls.Add(this.OptionsTab);
            this.OptionsTabControl.Location = new System.Drawing.Point(0, 0);
            this.OptionsTabControl.Name = "OptionsTabControl";
            this.OptionsTabControl.SelectedIndex = 0;
            this.OptionsTabControl.Size = new System.Drawing.Size(280, 533);
            this.OptionsTabControl.TabIndex = 0;
            // 
            // OptionsTab
            // 
            this.OptionsTab.Controls.Add(this.DevicesGroupBox);
            this.OptionsTab.Controls.Add(this.OCRGroupBox);
            this.OptionsTab.Controls.Add(this.LightsGroupBox);
            this.OptionsTab.Controls.Add(this.StartButton);
            this.OptionsTab.Location = new System.Drawing.Point(4, 22);
            this.OptionsTab.Name = "OptionsTab";
            this.OptionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.OptionsTab.Size = new System.Drawing.Size(272, 507);
            this.OptionsTab.TabIndex = 2;
            this.OptionsTab.Text = "Options";
            this.OptionsTab.UseVisualStyleBackColor = true;
            // 
            // DevicesGroupBox
            // 
            this.DevicesGroupBox.Controls.Add(this.DisconnectButton);
            this.DevicesGroupBox.Controls.Add(this.ConnectButton);
            this.DevicesGroupBox.Controls.Add(this.DevicesListBox);
            this.DevicesGroupBox.Location = new System.Drawing.Point(7, 3);
            this.DevicesGroupBox.Name = "DevicesGroupBox";
            this.DevicesGroupBox.Size = new System.Drawing.Size(259, 160);
            this.DevicesGroupBox.TabIndex = 11;
            this.DevicesGroupBox.TabStop = false;
            this.DevicesGroupBox.Text = "Devices";
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Enabled = false;
            this.DisconnectButton.Location = new System.Drawing.Point(137, 112);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(83, 23);
            this.DisconnectButton.TabIndex = 8;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(38, 112);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(83, 23);
            this.ConnectButton.TabIndex = 6;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // DevicesListBox
            // 
            this.DevicesListBox.FormattingEnabled = true;
            this.DevicesListBox.Location = new System.Drawing.Point(12, 19);
            this.DevicesListBox.Name = "DevicesListBox";
            this.DevicesListBox.Size = new System.Drawing.Size(236, 69);
            this.DevicesListBox.TabIndex = 5;
            // 
            // OCRGroupBox
            // 
            this.OCRGroupBox.Controls.Add(this.OCRParamsCheckedListBox);
            this.OCRGroupBox.Location = new System.Drawing.Point(7, 334);
            this.OCRGroupBox.Name = "OCRGroupBox";
            this.OCRGroupBox.Size = new System.Drawing.Size(259, 107);
            this.OCRGroupBox.TabIndex = 10;
            this.OCRGroupBox.TabStop = false;
            this.OCRGroupBox.Text = "OCR";
            this.OCRGroupBox.Enter += new System.EventHandler(this.OCRGroupBox_Enter);
            // 
            // OCRParamsCheckedListBox
            // 
            this.OCRParamsCheckedListBox.CheckOnClick = true;
            this.OCRParamsCheckedListBox.FormattingEnabled = true;
            this.OCRParamsCheckedListBox.Items.AddRange(new object[] {
            "MRZ fields",
            "VIZ fields",
            "BCR fields"});
            this.OCRParamsCheckedListBox.Location = new System.Drawing.Point(12, 19);
            this.OCRParamsCheckedListBox.Name = "OCRParamsCheckedListBox";
            this.OCRParamsCheckedListBox.Size = new System.Drawing.Size(236, 64);
            this.OCRParamsCheckedListBox.TabIndex = 0;
            this.OCRParamsCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.OCRParamsCheckedListBox_SelectedIndexChanged);

            // 
            // LightsGroupBox
            // 
            this.LightsGroupBox.Controls.Add(this.LightsCheckedListBox);
            this.LightsGroupBox.Controls.Add(this.DocViewCheckBox);
            this.LightsGroupBox.Location = new System.Drawing.Point(7, 169);
            this.LightsGroupBox.Name = "LightsGroupBox";
            this.LightsGroupBox.Size = new System.Drawing.Size(259, 159);
            this.LightsGroupBox.TabIndex = 7;
            this.LightsGroupBox.TabStop = false;
            this.LightsGroupBox.Text = "Images";
            // 
            // LightsCheckedListBox
            // 
            this.LightsCheckedListBox.CheckOnClick = true;
            this.LightsCheckedListBox.FormattingEnabled = true;
            this.LightsCheckedListBox.Location = new System.Drawing.Point(12, 19);
            this.LightsCheckedListBox.Name = "LightsCheckedListBox";
            this.LightsCheckedListBox.Size = new System.Drawing.Size(236, 109);
            this.LightsCheckedListBox.TabIndex = 0;
            this.LightsCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.LightsCheckedListBox_SelectedIndexChanged);
            // 
            // DocViewCheckBox
            // 
            this.DocViewCheckBox.AutoSize = true;
            this.DocViewCheckBox.Location = new System.Drawing.Point(12, 134);
            this.DocViewCheckBox.Name = "DocViewCheckBox";
            this.DocViewCheckBox.Size = new System.Drawing.Size(149, 17);
            this.DocViewCheckBox.TabIndex = 11;
            this.DocViewCheckBox.Text = "Crop and rotate document";
            this.DocViewCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartButton
            // 
            this.StartButton.Enabled = false;
            this.StartButton.Location = new System.Drawing.Point(69, 458);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(133, 34);
            this.StartButton.TabIndex = 9;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // FieldsTabControl
            // 
            this.FieldsTabControl.Controls.Add(this.OcrTab);
            this.FieldsTabControl.Controls.Add(this.DataTab);
            this.FieldsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FieldsTabControl.Location = new System.Drawing.Point(0, 0);
            this.FieldsTabControl.Name = "FieldsTabControl";
            this.FieldsTabControl.SelectedIndex = 0;
            this.FieldsTabControl.Size = new System.Drawing.Size(729, 533);
            this.FieldsTabControl.TabIndex = 0;
            // 
            // OcrTab
            // 
            this.OcrTab.Controls.Add(this.splitContainer2);
            this.OcrTab.Location = new System.Drawing.Point(4, 22);
            this.OcrTab.Name = "OcrTab";
            this.OcrTab.Size = new System.Drawing.Size(721, 507);
            this.OcrTab.TabIndex = 0;
            this.OcrTab.Text = "OCR";
            this.OcrTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.FieldsDataGridView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer2.Panel2.Controls.Add(this.FieldImagePictureBox);
            this.splitContainer2.Panel2.Controls.Add(this.FieldImageGroup);
            this.splitContainer2.Panel2.Controls.Add(this.ValuesGroup);
            this.splitContainer2.Size = new System.Drawing.Size(721, 507);
            this.splitContainer2.SplitterDistance = 327;
            this.splitContainer2.TabIndex = 0;
            // 
            // FieldsDataGridView
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.FieldsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.FieldsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FieldsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexColumn,
            this.FieldIDColumn,
            this.ValueColumn,
            this.StatusColumn});
            this.FieldsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FieldsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.FieldsDataGridView.Name = "FieldsDataGridView";
            this.FieldsDataGridView.Size = new System.Drawing.Size(721, 327);
            this.FieldsDataGridView.TabIndex = 0;
            this.FieldsDataGridView.SelectionChanged += new System.EventHandler(this.FieldsDataGridView_SelectionChanged);
            // 
            // IndexColumn
            // 
            this.IndexColumn.HeaderText = "Index";
            this.IndexColumn.Name = "IndexColumn";
            this.IndexColumn.Visible = false;
            // 
            // FieldIDColumn
            // 
            this.FieldIDColumn.HeaderText = "Field ID";
            this.FieldIDColumn.MinimumWidth = 160;
            this.FieldIDColumn.Name = "FieldIDColumn";
            this.FieldIDColumn.ReadOnly = true;
            this.FieldIDColumn.Width = 160;
            // 
            // ValueColumn
            // 
            this.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ValueColumn.HeaderText = "Value";
            this.ValueColumn.Name = "ValueColumn";
            this.ValueColumn.ReadOnly = true;
            // 
            // StatusColumn
            // 
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            // 
            // FieldImagePictureBox
            // 
            this.FieldImagePictureBox.Location = new System.Drawing.Point(141, 112);
            this.FieldImagePictureBox.Name = "FieldImagePictureBox";
            this.FieldImagePictureBox.Size = new System.Drawing.Size(566, 53);
            this.FieldImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.FieldImagePictureBox.TabIndex = 3;
            this.FieldImagePictureBox.TabStop = false;
            // 
            // FieldImageGroup
            // 
            this.FieldImageGroup.Location = new System.Drawing.Point(3, 101);
            this.FieldImageGroup.Name = "FieldImageGroup";
            this.FieldImageGroup.Size = new System.Drawing.Size(710, 69);
            this.FieldImageGroup.TabIndex = 7;
            this.FieldImageGroup.TabStop = false;
            this.FieldImageGroup.Text = "Field image";
            // 
            // ValuesGroup
            // 
            this.ValuesGroup.Controls.Add(this.RAWValueLabel);
            this.ValuesGroup.Controls.Add(this.RLabel);
            this.ValuesGroup.Controls.Add(this.FLabel);
            this.ValuesGroup.Controls.Add(this.StandardizedValueLabel);
            this.ValuesGroup.Controls.Add(this.SLabel);
            this.ValuesGroup.Controls.Add(this.FormattedValueLabel);
            this.ValuesGroup.Location = new System.Drawing.Point(3, 3);
            this.ValuesGroup.Name = "ValuesGroup";
            this.ValuesGroup.Size = new System.Drawing.Size(710, 92);
            this.ValuesGroup.TabIndex = 8;
            this.ValuesGroup.TabStop = false;
            this.ValuesGroup.Text = "Values";
            // 
            // RAWValueLabel
            // 
            this.RAWValueLabel.AutoSize = true;
            this.RAWValueLabel.Location = new System.Drawing.Point(141, 16);
            this.RAWValueLabel.Name = "RAWValueLabel";
            this.RAWValueLabel.Size = new System.Drawing.Size(0, 13);
            this.RAWValueLabel.TabIndex = 6;
            // 
            // RLabel
            // 
            this.RLabel.AutoSize = true;
            this.RLabel.Location = new System.Drawing.Point(8, 19);
            this.RLabel.Name = "RLabel";
            this.RLabel.Size = new System.Drawing.Size(36, 13);
            this.RLabel.TabIndex = 2;
            this.RLabel.Text = "RAW:";
            // 
            // FLabel
            // 
            this.FLabel.AutoSize = true;
            this.FLabel.Location = new System.Drawing.Point(8, 41);
            this.FLabel.Name = "FLabel";
            this.FLabel.Size = new System.Drawing.Size(57, 13);
            this.FLabel.TabIndex = 1;
            this.FLabel.Text = "Formatted:";
            // 
            // StandardizedValueLabel
            // 
            this.StandardizedValueLabel.AutoSize = true;
            this.StandardizedValueLabel.Location = new System.Drawing.Point(141, 60);
            this.StandardizedValueLabel.Name = "StandardizedValueLabel";
            this.StandardizedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.StandardizedValueLabel.TabIndex = 6;
            // 
            // SLabel
            // 
            this.SLabel.AutoSize = true;
            this.SLabel.Location = new System.Drawing.Point(8, 63);
            this.SLabel.Name = "SLabel";
            this.SLabel.Size = new System.Drawing.Size(72, 13);
            this.SLabel.TabIndex = 2;
            this.SLabel.Text = "Standardized:";
            // 
            // FormattedValueLabel
            // 
            this.FormattedValueLabel.AutoSize = true;
            this.FormattedValueLabel.Location = new System.Drawing.Point(141, 38);
            this.FormattedValueLabel.Name = "FormattedValueLabel";
            this.FormattedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.FormattedValueLabel.TabIndex = 5;
            // 
            // DataTab
            // 
            this.DataTab.Controls.Add(this.SignatureGroupBox);
            this.DataTab.Controls.Add(this.PhotoGroupBox);
            this.DataTab.Controls.Add(this.DocumentGroupBox);
            this.DataTab.Controls.Add(this.PersonalGroupBox);
            this.DataTab.Location = new System.Drawing.Point(4, 22);
            this.DataTab.Name = "DataTab";
            this.DataTab.Size = new System.Drawing.Size(721, 507);
            this.DataTab.TabIndex = 1;
            this.DataTab.Text = "Data";
            this.DataTab.UseVisualStyleBackColor = true;
            // 
            // SignatureGroupBox
            // 
            this.SignatureGroupBox.Controls.Add(this.SignaturePictureBox);
            this.SignatureGroupBox.Location = new System.Drawing.Point(3, 334);
            this.SignatureGroupBox.Name = "SignatureGroupBox";
            this.SignatureGroupBox.Size = new System.Drawing.Size(574, 158);
            this.SignatureGroupBox.TabIndex = 4;
            this.SignatureGroupBox.TabStop = false;
            this.SignatureGroupBox.Text = "Signature";
            // 
            // SignaturePictureBox
            // 
            this.SignaturePictureBox.Location = new System.Drawing.Point(6, 19);
            this.SignaturePictureBox.Name = "SignaturePictureBox";
            this.SignaturePictureBox.Size = new System.Drawing.Size(562, 133);
            this.SignaturePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SignaturePictureBox.TabIndex = 0;
            this.SignaturePictureBox.TabStop = false;
            // 
            // PhotoGroupBox
            // 
            this.PhotoGroupBox.Controls.Add(this.PhotoPictureBox);
            this.PhotoGroupBox.Location = new System.Drawing.Point(583, 3);
            this.PhotoGroupBox.Name = "PhotoGroupBox";
            this.PhotoGroupBox.Size = new System.Drawing.Size(130, 160);
            this.PhotoGroupBox.TabIndex = 3;
            this.PhotoGroupBox.TabStop = false;
            this.PhotoGroupBox.Text = "Face Photo";
            // 
            // PhotoPictureBox
            // 
            this.PhotoPictureBox.Location = new System.Drawing.Point(6, 19);
            this.PhotoPictureBox.Name = "PhotoPictureBox";
            this.PhotoPictureBox.Size = new System.Drawing.Size(118, 135);
            this.PhotoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PhotoPictureBox.TabIndex = 0;
            this.PhotoPictureBox.TabStop = false;
            // 
            // DocumentGroupBox
            // 
            this.DocumentGroupBox.Controls.Add(this.Validity);
            this.DocumentGroupBox.Controls.Add(this.Number);
            this.DocumentGroupBox.Controls.Add(this.Page);
            this.DocumentGroupBox.Controls.Add(this.Type);
            this.DocumentGroupBox.Controls.Add(this.Issuer);
            this.DocumentGroupBox.Controls.Add(this.ValidityLabel);
            this.DocumentGroupBox.Controls.Add(this.IssuerLabel);
            this.DocumentGroupBox.Controls.Add(this.PageLabel);
            this.DocumentGroupBox.Controls.Add(this.DocNumberLabel);
            this.DocumentGroupBox.Controls.Add(this.TypeLabel);
            this.DocumentGroupBox.Location = new System.Drawing.Point(3, 169);
            this.DocumentGroupBox.Name = "DocumentGroupBox";
            this.DocumentGroupBox.Size = new System.Drawing.Size(574, 159);
            this.DocumentGroupBox.TabIndex = 2;
            this.DocumentGroupBox.TabStop = false;
            this.DocumentGroupBox.Text = "Document Data";
            // 
            // Validity
            // 
            this.Validity.AutoSize = true;
            this.Validity.Location = new System.Drawing.Point(88, 130);
            this.Validity.Name = "Validity";
            this.Validity.Size = new System.Drawing.Size(0, 13);
            this.Validity.TabIndex = 9;
            // 
            // Number
            // 
            this.Number.AutoSize = true;
            this.Number.Location = new System.Drawing.Point(88, 104);
            this.Number.Name = "Number";
            this.Number.Size = new System.Drawing.Size(0, 13);
            this.Number.TabIndex = 8;
            // 
            // Page
            // 
            this.Page.AutoSize = true;
            this.Page.Location = new System.Drawing.Point(88, 78);
            this.Page.Name = "Page";
            this.Page.Size = new System.Drawing.Size(0, 13);
            this.Page.TabIndex = 7;
            // 
            // Type
            // 
            this.Type.AutoSize = true;
            this.Type.Location = new System.Drawing.Point(88, 52);
            this.Type.Name = "Type";
            this.Type.Size = new System.Drawing.Size(0, 13);
            this.Type.TabIndex = 6;
            // 
            // Issuer
            // 
            this.Issuer.AutoSize = true;
            this.Issuer.Location = new System.Drawing.Point(88, 26);
            this.Issuer.Name = "Issuer";
            this.Issuer.Size = new System.Drawing.Size(0, 13);
            this.Issuer.TabIndex = 5;
            // 
            // ValidityLabel
            // 
            this.ValidityLabel.AutoSize = true;
            this.ValidityLabel.Location = new System.Drawing.Point(6, 130);
            this.ValidityLabel.Name = "ValidityLabel";
            this.ValidityLabel.Size = new System.Drawing.Size(33, 13);
            this.ValidityLabel.TabIndex = 4;
            this.ValidityLabel.Text = "Valid:";
            // 
            // IssuerLabel
            // 
            this.IssuerLabel.AutoSize = true;
            this.IssuerLabel.Location = new System.Drawing.Point(6, 26);
            this.IssuerLabel.Name = "IssuerLabel";
            this.IssuerLabel.Size = new System.Drawing.Size(38, 13);
            this.IssuerLabel.TabIndex = 0;
            this.IssuerLabel.Text = "Issuer:";
            // 
            // PageLabel
            // 
            this.PageLabel.AutoSize = true;
            this.PageLabel.Location = new System.Drawing.Point(6, 78);
            this.PageLabel.Name = "PageLabel";
            this.PageLabel.Size = new System.Drawing.Size(35, 13);
            this.PageLabel.TabIndex = 2;
            this.PageLabel.Text = "Page:";
            // 
            // DocNumberLabel
            // 
            this.DocNumberLabel.AutoSize = true;
            this.DocNumberLabel.Location = new System.Drawing.Point(6, 104);
            this.DocNumberLabel.Name = "DocNumberLabel";
            this.DocNumberLabel.Size = new System.Drawing.Size(47, 13);
            this.DocNumberLabel.TabIndex = 3;
            this.DocNumberLabel.Text = "Number:";
            // 
            // TypeLabel
            // 
            this.TypeLabel.AutoSize = true;
            this.TypeLabel.Location = new System.Drawing.Point(6, 52);
            this.TypeLabel.Name = "TypeLabel";
            this.TypeLabel.Size = new System.Drawing.Size(34, 13);
            this.TypeLabel.TabIndex = 1;
            this.TypeLabel.Text = "Type:";
            // 
            // PersonalGroupBox
            // 
            this.PersonalGroupBox.Controls.Add(this.Sex);
            this.PersonalGroupBox.Controls.Add(this.Nationality);
            this.PersonalGroupBox.Controls.Add(this.Birth);
            this.PersonalGroupBox.Controls.Add(this.Name2);
            this.PersonalGroupBox.Controls.Add(this.Name1);
            this.PersonalGroupBox.Controls.Add(this.SexLabel);
            this.PersonalGroupBox.Controls.Add(this.NationalityLabel);
            this.PersonalGroupBox.Controls.Add(this.BirthLabel);
            this.PersonalGroupBox.Controls.Add(this.NameLabel);
            this.PersonalGroupBox.Location = new System.Drawing.Point(3, 3);
            this.PersonalGroupBox.Name = "PersonalGroupBox";
            this.PersonalGroupBox.Size = new System.Drawing.Size(574, 160);
            this.PersonalGroupBox.TabIndex = 1;
            this.PersonalGroupBox.TabStop = false;
            this.PersonalGroupBox.Text = "Personal Data";
            // 
            // Sex
            // 
            this.Sex.AutoSize = true;
            this.Sex.Location = new System.Drawing.Point(88, 132);
            this.Sex.Name = "Sex";
            this.Sex.Size = new System.Drawing.Size(0, 13);
            this.Sex.TabIndex = 8;
            // 
            // Nationality
            // 
            this.Nationality.AutoSize = true;
            this.Nationality.Location = new System.Drawing.Point(88, 106);
            this.Nationality.Name = "Nationality";
            this.Nationality.Size = new System.Drawing.Size(0, 13);
            this.Nationality.TabIndex = 7;
            // 
            // Birth
            // 
            this.Birth.AutoSize = true;
            this.Birth.Location = new System.Drawing.Point(88, 80);
            this.Birth.Name = "Birth";
            this.Birth.Size = new System.Drawing.Size(0, 13);
            this.Birth.TabIndex = 6;
            // 
            // Name2
            // 
            this.Name2.AutoSize = true;
            this.Name2.Location = new System.Drawing.Point(88, 54);
            this.Name2.Name = "Name2";
            this.Name2.Size = new System.Drawing.Size(0, 13);
            this.Name2.TabIndex = 5;
            // 
            // Name1
            // 
            this.Name1.AutoSize = true;
            this.Name1.Location = new System.Drawing.Point(88, 28);
            this.Name1.Name = "Name1";
            this.Name1.Size = new System.Drawing.Size(0, 13);
            this.Name1.TabIndex = 4;
            // 
            // SexLabel
            // 
            this.SexLabel.AutoSize = true;
            this.SexLabel.Location = new System.Drawing.Point(6, 132);
            this.SexLabel.Name = "SexLabel";
            this.SexLabel.Size = new System.Drawing.Size(28, 13);
            this.SexLabel.TabIndex = 3;
            this.SexLabel.Text = "Sex:";
            // 
            // NationalityLabel
            // 
            this.NationalityLabel.AutoSize = true;
            this.NationalityLabel.Location = new System.Drawing.Point(6, 106);
            this.NationalityLabel.Name = "NationalityLabel";
            this.NationalityLabel.Size = new System.Drawing.Size(59, 13);
            this.NationalityLabel.TabIndex = 2;
            this.NationalityLabel.Text = "Nationality:";
            // 
            // BirthLabel
            // 
            this.BirthLabel.AutoSize = true;
            this.BirthLabel.Location = new System.Drawing.Point(6, 80);
            this.BirthLabel.Name = "BirthLabel";
            this.BirthLabel.Size = new System.Drawing.Size(31, 13);
            this.BirthLabel.TabIndex = 1;
            this.BirthLabel.Text = "Birth:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(6, 28);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(38, 13);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Name:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 533);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Passport Scanner";
            this.Icon = new Icon("icon.ico");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            this.Load += new System.EventHandler(this.FormLoad);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.OptionsTabControl.ResumeLayout(false);
            this.OptionsTab.ResumeLayout(false);
            this.DevicesGroupBox.ResumeLayout(false);
            this.OCRGroupBox.ResumeLayout(false);
            this.LightsGroupBox.ResumeLayout(false);
            this.LightsGroupBox.PerformLayout();
            this.FieldsTabControl.ResumeLayout(false);
            this.OcrTab.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FieldsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FieldImagePictureBox)).EndInit();
            this.ValuesGroup.ResumeLayout(false);
            this.ValuesGroup.PerformLayout();
            this.DataTab.ResumeLayout(false);
            this.SignatureGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SignaturePictureBox)).EndInit();
            this.PhotoGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PhotoPictureBox)).EndInit();
            this.DocumentGroupBox.ResumeLayout(false);
            this.DocumentGroupBox.PerformLayout();
            this.PersonalGroupBox.ResumeLayout(false);
            this.PersonalGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox DevicesListBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.GroupBox LightsGroupBox;
        private System.Windows.Forms.CheckedListBox LightsCheckedListBox;
        private System.Windows.Forms.CheckBox DocViewCheckBox;
        private System.Windows.Forms.GroupBox OCRGroupBox;
        private System.Windows.Forms.CheckedListBox OCRParamsCheckedListBox;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TabControl FieldsTabControl;
        private System.Windows.Forms.TabPage OcrTab;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView FieldsDataGridView;
        private System.Windows.Forms.Label RAWValueLabel;
        private System.Windows.Forms.Label FormattedValueLabel;
        private System.Windows.Forms.Label StandardizedValueLabel;
        private System.Windows.Forms.PictureBox FieldImagePictureBox;
        private System.Windows.Forms.Label RLabel;
        private System.Windows.Forms.Label FLabel;
        private System.Windows.Forms.Label SLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.TabPage DataTab;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.GroupBox PersonalGroupBox;
        private System.Windows.Forms.Label SexLabel;
        private System.Windows.Forms.Label NationalityLabel;
        private System.Windows.Forms.Label BirthLabel;
        private System.Windows.Forms.GroupBox DocumentGroupBox;
        private System.Windows.Forms.Label ValidityLabel;
        private System.Windows.Forms.Label DocNumberLabel;
        private System.Windows.Forms.Label PageLabel;
        private System.Windows.Forms.Label TypeLabel;
        private System.Windows.Forms.Label IssuerLabel;
        private System.Windows.Forms.Label Validity;
        private System.Windows.Forms.Label Number;
        private System.Windows.Forms.Label Page;
        private System.Windows.Forms.Label Type;
        private System.Windows.Forms.Label Issuer;
        private System.Windows.Forms.Label Sex;
        private System.Windows.Forms.Label Nationality;
        private System.Windows.Forms.Label Birth;
        private System.Windows.Forms.Label Name2;
        private System.Windows.Forms.Label Name1;
        private System.Windows.Forms.GroupBox PhotoGroupBox;
        private System.Windows.Forms.PictureBox PhotoPictureBox;
        private System.Windows.Forms.GroupBox SignatureGroupBox;
        private System.Windows.Forms.PictureBox SignaturePictureBox;
        private System.Windows.Forms.GroupBox DevicesGroupBox;
        private System.Windows.Forms.GroupBox FieldImageGroup;
        private System.Windows.Forms.GroupBox ValuesGroup;
        private System.Windows.Forms.TabPage OptionsTab;
        private System.Windows.Forms.TabControl OptionsTabControl;
    }
}

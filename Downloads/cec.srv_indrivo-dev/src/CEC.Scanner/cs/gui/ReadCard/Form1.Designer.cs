namespace ReadCard
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.AuthSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FilesGroupBox = new System.Windows.Forms.GroupBox();
            this.FilesListView = new System.Windows.Forms.ListView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.StartButton = new System.Windows.Forms.Button();
            this.ReadersGroupBox = new System.Windows.Forms.GroupBox();
            this.ReadersCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.DevicesGroupBox = new System.Windows.Forms.GroupBox();
            this.DevicesListBox = new System.Windows.Forms.ListBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.SignaturesGroupBox = new System.Windows.Forms.GroupBox();
            this.SignaturePictureBox2 = new System.Windows.Forms.PictureBox();
            this.SignaturePictureBox1 = new System.Windows.Forms.PictureBox();
            this.FingersGroupBox = new System.Windows.Forms.GroupBox();
            this.FingerPictureBox2 = new System.Windows.Forms.PictureBox();
            this.FingerPictureBox1 = new System.Windows.Forms.PictureBox();
            this.FacesGroupBox = new System.Windows.Forms.GroupBox();
            this.FacePictureBox2 = new System.Windows.Forms.PictureBox();
            this.FacePictureBox1 = new System.Windows.Forms.PictureBox();
            this.AuthGroupBox = new System.Windows.Forms.GroupBox();
            this.PACELabel = new System.Windows.Forms.Label();
            this.TALabel = new System.Windows.Forms.Label();
            this.CALabel = new System.Windows.Forms.Label();
            this.FaceLabel = new System.Windows.Forms.Label();
            this.PALabel = new System.Windows.Forms.Label();
            this.MrzLabel = new System.Windows.Forms.Label();
            this.AALabel = new System.Windows.Forms.Label();
            this.BACLabel = new System.Windows.Forms.Label();
            this.MRZTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.FilesGroupBox.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.ReadersGroupBox.SuspendLayout();
            this.DevicesGroupBox.SuspendLayout();
            this.SignaturesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SignaturePictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SignaturePictureBox1)).BeginInit();
            this.FingersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FingerPictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FingerPictureBox1)).BeginInit();
            this.FacesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FacePictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FacePictureBox1)).BeginInit();
            this.AuthGroupBox.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.AuthSelector);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.FilesGroupBox);
            this.splitContainer1.Panel1.Controls.Add(this.StartButton);
            this.splitContainer1.Panel1.Controls.Add(this.ReadersGroupBox);
            this.splitContainer1.Panel1.Controls.Add(this.DevicesGroupBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.SignaturesGroupBox);
            this.splitContainer1.Panel2.Controls.Add(this.FingersGroupBox);
            this.splitContainer1.Panel2.Controls.Add(this.FacesGroupBox);
            this.splitContainer1.Panel2.Controls.Add(this.AuthGroupBox);
            this.splitContainer1.Panel2.Controls.Add(this.MRZTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Size = new System.Drawing.Size(792, 543);
            this.splitContainer1.SplitterDistance = 283;
            this.splitContainer1.TabIndex = 2;
            // 
            // AuthSelector
            // 
            this.AuthSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AuthSelector.FormattingEnabled = true;
            this.AuthSelector.Location = new System.Drawing.Point(149, 465);
            this.AuthSelector.Name = "AuthSelector";
            this.AuthSelector.Size = new System.Drawing.Size(123, 21);
            this.AuthSelector.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 468);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Authentication level:";
            // 
            // FilesGroupBox
            // 
            this.FilesGroupBox.Controls.Add(this.FilesListView);
            this.FilesGroupBox.Location = new System.Drawing.Point(13, 242);
            this.FilesGroupBox.Name = "FilesGroupBox";
            this.FilesGroupBox.Size = new System.Drawing.Size(259, 209);
            this.FilesGroupBox.TabIndex = 11;
            this.FilesGroupBox.TabStop = false;
            this.FilesGroupBox.Text = "Files";
            // 
            // FilesListView
            // 
            this.FilesListView.CheckBoxes = true;
            this.FilesListView.ContextMenuStrip = this.contextMenuStrip1;
            this.FilesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.FilesListView.Location = new System.Drawing.Point(11, 19);
            this.FilesListView.MultiSelect = false;
            this.FilesListView.Name = "FilesListView";
            this.FilesListView.ShowGroups = false;
            this.FilesListView.Size = new System.Drawing.Size(237, 184);
            this.FilesListView.TabIndex = 0;
            this.FilesListView.UseCompatibleStateImageBehavior = false;
            this.FilesListView.View = System.Windows.Forms.View.List;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(110, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(109, 22);
            this.toolStripMenuItem1.Text = "Save";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // StartButton
            // 
            this.StartButton.Enabled = false;
            this.StartButton.Location = new System.Drawing.Point(166, 496);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(106, 35);
            this.StartButton.TabIndex = 9;
            this.StartButton.Text = "Read";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // ReadersGroupBox
            // 
            this.ReadersGroupBox.Controls.Add(this.ReadersCheckedListBox);
            this.ReadersGroupBox.Location = new System.Drawing.Point(12, 142);
            this.ReadersGroupBox.Name = "ReadersGroupBox";
            this.ReadersGroupBox.Size = new System.Drawing.Size(260, 94);
            this.ReadersGroupBox.TabIndex = 7;
            this.ReadersGroupBox.TabStop = false;
            this.ReadersGroupBox.Text = "Card Readers";
            // 
            // ReadersCheckedListBox
            // 
            this.ReadersCheckedListBox.CheckOnClick = true;
            this.ReadersCheckedListBox.FormattingEnabled = true;
            this.ReadersCheckedListBox.Location = new System.Drawing.Point(12, 19);
            this.ReadersCheckedListBox.Name = "ReadersCheckedListBox";
            this.ReadersCheckedListBox.Size = new System.Drawing.Size(237, 64);
            this.ReadersCheckedListBox.TabIndex = 0;
            // 
            // DevicesGroupBox
            // 
            this.DevicesGroupBox.Controls.Add(this.DevicesListBox);
            this.DevicesGroupBox.Controls.Add(this.ConnectButton);
            this.DevicesGroupBox.Controls.Add(this.DisconnectButton);
            this.DevicesGroupBox.Location = new System.Drawing.Point(12, 8);
            this.DevicesGroupBox.Name = "DevicesGroupBox";
            this.DevicesGroupBox.Size = new System.Drawing.Size(260, 128);
            this.DevicesGroupBox.TabIndex = 15;
            this.DevicesGroupBox.TabStop = false;
            this.DevicesGroupBox.Text = "Devices";
            // 
            // DevicesListBox
            // 
            this.DevicesListBox.FormattingEnabled = true;
            this.DevicesListBox.Location = new System.Drawing.Point(12, 19);
            this.DevicesListBox.Name = "DevicesListBox";
            this.DevicesListBox.Size = new System.Drawing.Size(237, 56);
            this.DevicesListBox.TabIndex = 5;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(46, 90);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 6;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Enabled = false;
            this.DisconnectButton.Location = new System.Drawing.Point(140, 90);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(75, 23);
            this.DisconnectButton.TabIndex = 8;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // SignaturesGroupBox
            // 
            this.SignaturesGroupBox.Controls.Add(this.SignaturePictureBox2);
            this.SignaturesGroupBox.Controls.Add(this.SignaturePictureBox1);
            this.SignaturesGroupBox.Location = new System.Drawing.Point(12, 312);
            this.SignaturesGroupBox.Name = "SignaturesGroupBox";
            this.SignaturesGroupBox.Size = new System.Drawing.Size(367, 139);
            this.SignaturesGroupBox.TabIndex = 17;
            this.SignaturesGroupBox.TabStop = false;
            this.SignaturesGroupBox.Text = "Signatures";
            // 
            // SignaturePictureBox2
            // 
            this.SignaturePictureBox2.Location = new System.Drawing.Point(6, 78);
            this.SignaturePictureBox2.Name = "SignaturePictureBox2";
            this.SignaturePictureBox2.Size = new System.Drawing.Size(355, 53);
            this.SignaturePictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SignaturePictureBox2.TabIndex = 14;
            this.SignaturePictureBox2.TabStop = false;
            // 
            // SignaturePictureBox1
            // 
            this.SignaturePictureBox1.Location = new System.Drawing.Point(6, 19);
            this.SignaturePictureBox1.Name = "SignaturePictureBox1";
            this.SignaturePictureBox1.Size = new System.Drawing.Size(355, 53);
            this.SignaturePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SignaturePictureBox1.TabIndex = 3;
            this.SignaturePictureBox1.TabStop = false;
            // 
            // FingersGroupBox
            // 
            this.FingersGroupBox.Controls.Add(this.FingerPictureBox2);
            this.FingersGroupBox.Controls.Add(this.FingerPictureBox1);
            this.FingersGroupBox.Location = new System.Drawing.Point(385, 8);
            this.FingersGroupBox.Name = "FingersGroupBox";
            this.FingersGroupBox.Size = new System.Drawing.Size(108, 228);
            this.FingersGroupBox.TabIndex = 16;
            this.FingersGroupBox.TabStop = false;
            this.FingersGroupBox.Text = "Fingerprints";
            // 
            // FingerPictureBox2
            // 
            this.FingerPictureBox2.Location = new System.Drawing.Point(10, 122);
            this.FingerPictureBox2.Name = "FingerPictureBox2";
            this.FingerPictureBox2.Size = new System.Drawing.Size(88, 100);
            this.FingerPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.FingerPictureBox2.TabIndex = 5;
            this.FingerPictureBox2.TabStop = false;
            // 
            // FingerPictureBox1
            // 
            this.FingerPictureBox1.Location = new System.Drawing.Point(10, 19);
            this.FingerPictureBox1.Name = "FingerPictureBox1";
            this.FingerPictureBox1.Size = new System.Drawing.Size(88, 100);
            this.FingerPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.FingerPictureBox1.TabIndex = 4;
            this.FingerPictureBox1.TabStop = false;
            // 
            // FacesGroupBox
            // 
            this.FacesGroupBox.Controls.Add(this.FacePictureBox2);
            this.FacesGroupBox.Controls.Add(this.FacePictureBox1);
            this.FacesGroupBox.Location = new System.Drawing.Point(12, 8);
            this.FacesGroupBox.Name = "FacesGroupBox";
            this.FacesGroupBox.Size = new System.Drawing.Size(367, 228);
            this.FacesGroupBox.TabIndex = 15;
            this.FacesGroupBox.TabStop = false;
            this.FacesGroupBox.Text = "Face images";
            // 
            // FacePictureBox2
            // 
            this.FacePictureBox2.Location = new System.Drawing.Point(187, 19);
            this.FacePictureBox2.Name = "FacePictureBox2";
            this.FacePictureBox2.Size = new System.Drawing.Size(174, 203);
            this.FacePictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.FacePictureBox2.TabIndex = 13;
            this.FacePictureBox2.TabStop = false;
            // 
            // FacePictureBox1
            // 
            this.FacePictureBox1.Location = new System.Drawing.Point(6, 19);
            this.FacePictureBox1.Name = "FacePictureBox1";
            this.FacePictureBox1.Size = new System.Drawing.Size(174, 203);
            this.FacePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.FacePictureBox1.TabIndex = 1;
            this.FacePictureBox1.TabStop = false;
            // 
            // AuthGroupBox
            // 
            this.AuthGroupBox.Controls.Add(this.PACELabel);
            this.AuthGroupBox.Controls.Add(this.TALabel);
            this.AuthGroupBox.Controls.Add(this.CALabel);
            this.AuthGroupBox.Controls.Add(this.FaceLabel);
            this.AuthGroupBox.Controls.Add(this.PALabel);
            this.AuthGroupBox.Controls.Add(this.MrzLabel);
            this.AuthGroupBox.Controls.Add(this.AALabel);
            this.AuthGroupBox.Controls.Add(this.BACLabel);
            this.AuthGroupBox.Location = new System.Drawing.Point(385, 312);
            this.AuthGroupBox.Name = "AuthGroupBox";
            this.AuthGroupBox.Size = new System.Drawing.Size(108, 139);
            this.AuthGroupBox.TabIndex = 12;
            this.AuthGroupBox.TabStop = false;
            this.AuthGroupBox.Text = "Authentications";
            // 
            // PACELabel
            // 
            this.PACELabel.AutoSize = true;
            this.PACELabel.Location = new System.Drawing.Point(59, 25);
            this.PACELabel.Name = "PACELabel";
            this.PACELabel.Size = new System.Drawing.Size(35, 13);
            this.PACELabel.TabIndex = 17;
            this.PACELabel.Text = "PACE";
            // 
            // TALabel
            // 
            this.TALabel.AutoSize = true;
            this.TALabel.Location = new System.Drawing.Point(59, 54);
            this.TALabel.Name = "TALabel";
            this.TALabel.Size = new System.Drawing.Size(21, 13);
            this.TALabel.TabIndex = 16;
            this.TALabel.Text = "TA";
            // 
            // CALabel
            // 
            this.CALabel.AutoSize = true;
            this.CALabel.Location = new System.Drawing.Point(14, 54);
            this.CALabel.Name = "CALabel";
            this.CALabel.Size = new System.Drawing.Size(21, 13);
            this.CALabel.TabIndex = 15;
            this.CALabel.Text = "CA";
            // 
            // FaceLabel
            // 
            this.FaceLabel.AutoSize = true;
            this.FaceLabel.Location = new System.Drawing.Point(59, 112);
            this.FaceLabel.Name = "FaceLabel";
            this.FaceLabel.Size = new System.Drawing.Size(31, 13);
            this.FaceLabel.TabIndex = 7;
            this.FaceLabel.Text = "Face";
            // 
            // PALabel
            // 
            this.PALabel.AutoSize = true;
            this.PALabel.Location = new System.Drawing.Point(14, 83);
            this.PALabel.Name = "PALabel";
            this.PALabel.Size = new System.Drawing.Size(21, 13);
            this.PALabel.TabIndex = 14;
            this.PALabel.Text = "PA";
            // 
            // MrzLabel
            // 
            this.MrzLabel.AutoSize = true;
            this.MrzLabel.Location = new System.Drawing.Point(14, 112);
            this.MrzLabel.Name = "MrzLabel";
            this.MrzLabel.Size = new System.Drawing.Size(31, 13);
            this.MrzLabel.TabIndex = 6;
            this.MrzLabel.Text = "MRZ";
            // 
            // AALabel
            // 
            this.AALabel.AutoSize = true;
            this.AALabel.Location = new System.Drawing.Point(59, 83);
            this.AALabel.Name = "AALabel";
            this.AALabel.Size = new System.Drawing.Size(21, 13);
            this.AALabel.TabIndex = 13;
            this.AALabel.Text = "AA";
            // 
            // BACLabel
            // 
            this.BACLabel.AutoSize = true;
            this.BACLabel.Location = new System.Drawing.Point(14, 25);
            this.BACLabel.Name = "BACLabel";
            this.BACLabel.Size = new System.Drawing.Size(28, 13);
            this.BACLabel.TabIndex = 11;
            this.BACLabel.Text = "BAC";
            // 
            // MRZTextBox
            // 
            this.MRZTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MRZTextBox.Location = new System.Drawing.Point(12, 248);
            this.MRZTextBox.Multiline = true;
            this.MRZTextBox.Name = "MRZTextBox";
            this.MRZTextBox.ReadOnly = true;
            this.MRZTextBox.Size = new System.Drawing.Size(481, 58);
            this.MRZTextBox.TabIndex = 2;
            this.MRZTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 459);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(481, 72);
            this.textBox1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(792, 543);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.RightToLeftLayout = true;
            this.Text = "Read Card";
            this.Load += new System.EventHandler(this.FormLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.FilesGroupBox.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ReadersGroupBox.ResumeLayout(false);
            this.DevicesGroupBox.ResumeLayout(false);
            this.SignaturesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SignaturePictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SignaturePictureBox1)).EndInit();
            this.FingersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FingerPictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FingerPictureBox1)).EndInit();
            this.FacesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FacePictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FacePictureBox1)).EndInit();
            this.AuthGroupBox.ResumeLayout(false);
            this.AuthGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.GroupBox ReadersGroupBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ListBox DevicesListBox;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.CheckedListBox ReadersCheckedListBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox FilesGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox AuthSelector;
        private System.Windows.Forms.ListView FilesListView;
        private System.Windows.Forms.TextBox MRZTextBox;
        private System.Windows.Forms.PictureBox FacePictureBox1;
        private System.Windows.Forms.PictureBox SignaturePictureBox1;
        private System.Windows.Forms.PictureBox FingerPictureBox2;
        private System.Windows.Forms.PictureBox FingerPictureBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label FaceLabel;
        private System.Windows.Forms.Label MrzLabel;
        private System.Windows.Forms.GroupBox AuthGroupBox;
        private System.Windows.Forms.Label PACELabel;
        private System.Windows.Forms.Label TALabel;
        private System.Windows.Forms.Label CALabel;
        private System.Windows.Forms.Label PALabel;
        private System.Windows.Forms.Label AALabel;
        private System.Windows.Forms.Label BACLabel;
        private System.Windows.Forms.PictureBox FacePictureBox2;
        private System.Windows.Forms.PictureBox SignaturePictureBox2;
        private System.Windows.Forms.GroupBox DevicesGroupBox;
        private System.Windows.Forms.GroupBox FacesGroupBox;
        private System.Windows.Forms.GroupBox FingersGroupBox;
        private System.Windows.Forms.GroupBox SignaturesGroupBox;

    }
}


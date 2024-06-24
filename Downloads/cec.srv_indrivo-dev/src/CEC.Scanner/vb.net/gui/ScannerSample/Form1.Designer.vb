Namespace ScannerSample
    Partial Class Form1
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
            Me.splitContainer1 = New System.Windows.Forms.SplitContainer
            Me.OptionsTabControl = New System.Windows.Forms.TabControl
            Me.OptionsTab = New System.Windows.Forms.TabPage
            Me.DevicesGroupBox = New System.Windows.Forms.GroupBox
            Me.DisconnectButton = New System.Windows.Forms.Button
            Me.ConnectButton = New System.Windows.Forms.Button
            Me.DevicesListBox = New System.Windows.Forms.ListBox
            Me.OCRGroupBox = New System.Windows.Forms.GroupBox
            Me.OCRParamsCheckedListBox = New System.Windows.Forms.CheckedListBox
            Me.LightsGroupBox = New System.Windows.Forms.GroupBox
            Me.LightsCheckedListBox = New System.Windows.Forms.CheckedListBox
            Me.DocViewCheckBox = New System.Windows.Forms.CheckBox
            Me.StartButton = New System.Windows.Forms.Button
            Me.FieldsTabControl = New System.Windows.Forms.TabControl
            Me.OcrTab = New System.Windows.Forms.TabPage
            Me.splitContainer2 = New System.Windows.Forms.SplitContainer
            Me.FieldsDataGridView = New System.Windows.Forms.DataGridView
            Me.IndexColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
            Me.FieldIDColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
            Me.ValueColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
            Me.StatusColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
            Me.FieldImagePictureBox = New System.Windows.Forms.PictureBox
            Me.FieldImageGroup = New System.Windows.Forms.GroupBox
            Me.ValuesGroup = New System.Windows.Forms.GroupBox
            Me.RAWValueLabel = New System.Windows.Forms.Label
            Me.RLabel = New System.Windows.Forms.Label
            Me.FLabel = New System.Windows.Forms.Label
            Me.StandardizedValueLabel = New System.Windows.Forms.Label
            Me.SLabel = New System.Windows.Forms.Label
            Me.FormattedValueLabel = New System.Windows.Forms.Label
            Me.DataTab = New System.Windows.Forms.TabPage
            Me.SignatureGroupBox = New System.Windows.Forms.GroupBox
            Me.SignaturePictureBox = New System.Windows.Forms.PictureBox
            Me.PhotoGroupBox = New System.Windows.Forms.GroupBox
            Me.PhotoPictureBox = New System.Windows.Forms.PictureBox
            Me.DocumentGroupBox = New System.Windows.Forms.GroupBox
            Me.Validity = New System.Windows.Forms.Label
            Me.Number = New System.Windows.Forms.Label
            Me.Page = New System.Windows.Forms.Label
            Me.Type = New System.Windows.Forms.Label
            Me.Issuer = New System.Windows.Forms.Label
            Me.ValidityLabel = New System.Windows.Forms.Label
            Me.IssuerLabel = New System.Windows.Forms.Label
            Me.PageLabel = New System.Windows.Forms.Label
            Me.DocNumberLabel = New System.Windows.Forms.Label
            Me.TypeLabel = New System.Windows.Forms.Label
            Me.PersonalGroupBox = New System.Windows.Forms.GroupBox
            Me.Sex = New System.Windows.Forms.Label
            Me.Nationality = New System.Windows.Forms.Label
            Me.Birth = New System.Windows.Forms.Label
            Me.Name2 = New System.Windows.Forms.Label
            Me.Name1 = New System.Windows.Forms.Label
            Me.SexLabel = New System.Windows.Forms.Label
            Me.NationalityLabel = New System.Windows.Forms.Label
            Me.BirthLabel = New System.Windows.Forms.Label
            Me.NameLabel = New System.Windows.Forms.Label
            Me.splitContainer1.Panel1.SuspendLayout()
            Me.splitContainer1.Panel2.SuspendLayout()
            Me.splitContainer1.SuspendLayout()
            Me.OptionsTabControl.SuspendLayout()
            Me.OptionsTab.SuspendLayout()
            Me.DevicesGroupBox.SuspendLayout()
            Me.OCRGroupBox.SuspendLayout()
            Me.LightsGroupBox.SuspendLayout()
            Me.FieldsTabControl.SuspendLayout()
            Me.OcrTab.SuspendLayout()
            Me.splitContainer2.Panel1.SuspendLayout()
            Me.splitContainer2.Panel2.SuspendLayout()
            Me.splitContainer2.SuspendLayout()
            CType(Me.FieldsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.FieldImagePictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ValuesGroup.SuspendLayout()
            Me.DataTab.SuspendLayout()
            Me.SignatureGroupBox.SuspendLayout()
            CType(Me.SignaturePictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.PhotoGroupBox.SuspendLayout()
            CType(Me.PhotoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.DocumentGroupBox.SuspendLayout()
            Me.PersonalGroupBox.SuspendLayout()
            Me.SuspendLayout()
            '
            'splitContainer1
            '
            Me.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
            Me.splitContainer1.IsSplitterFixed = True
            Me.splitContainer1.Location = New System.Drawing.Point(0, 0)
            Me.splitContainer1.Name = "splitContainer1"
            '
            'splitContainer1.Panel1
            '
            Me.splitContainer1.Panel1.Controls.Add(Me.OptionsTabControl)
            '
            'splitContainer1.Panel2
            '
            Me.splitContainer1.Panel2.Controls.Add(Me.FieldsTabControl)
            Me.splitContainer1.Size = New System.Drawing.Size(1016, 533)
            Me.splitContainer1.SplitterDistance = 283
            Me.splitContainer1.TabIndex = 2
            '
            'OptionsTabControl
            '
            Me.OptionsTabControl.Controls.Add(Me.OptionsTab)
            Me.OptionsTabControl.Location = New System.Drawing.Point(0, 0)
            Me.OptionsTabControl.Name = "OptionsTabControl"
            Me.OptionsTabControl.SelectedIndex = 0
            Me.OptionsTabControl.Size = New System.Drawing.Size(280, 533)
            Me.OptionsTabControl.TabIndex = 0
            '
            'OptionsTab
            '
            Me.OptionsTab.Controls.Add(Me.DevicesGroupBox)
            Me.OptionsTab.Controls.Add(Me.OCRGroupBox)
            Me.OptionsTab.Controls.Add(Me.LightsGroupBox)
            Me.OptionsTab.Controls.Add(Me.StartButton)
            Me.OptionsTab.Location = New System.Drawing.Point(4, 22)
            Me.OptionsTab.Name = "OptionsTab"
            Me.OptionsTab.Padding = New System.Windows.Forms.Padding(3)
            Me.OptionsTab.Size = New System.Drawing.Size(272, 507)
            Me.OptionsTab.TabIndex = 2
            Me.OptionsTab.Text = "Options"
            Me.OptionsTab.UseVisualStyleBackColor = True
            '
            'DevicesGroupBox
            '
            Me.DevicesGroupBox.Controls.Add(Me.DisconnectButton)
            Me.DevicesGroupBox.Controls.Add(Me.ConnectButton)
            Me.DevicesGroupBox.Controls.Add(Me.DevicesListBox)
            Me.DevicesGroupBox.Location = New System.Drawing.Point(7, 3)
            Me.DevicesGroupBox.Name = "DevicesGroupBox"
            Me.DevicesGroupBox.Size = New System.Drawing.Size(259, 160)
            Me.DevicesGroupBox.TabIndex = 11
            Me.DevicesGroupBox.TabStop = False
            Me.DevicesGroupBox.Text = "Devices"
            '
            'DisconnectButton
            '
            Me.DisconnectButton.Enabled = False
            Me.DisconnectButton.Location = New System.Drawing.Point(137, 112)
            Me.DisconnectButton.Name = "DisconnectButton"
            Me.DisconnectButton.Size = New System.Drawing.Size(83, 23)
            Me.DisconnectButton.TabIndex = 8
            Me.DisconnectButton.Text = "Disconnect"
            Me.DisconnectButton.UseVisualStyleBackColor = True
            '
            'ConnectButton
            '
            Me.ConnectButton.Location = New System.Drawing.Point(38, 112)
            Me.ConnectButton.Name = "ConnectButton"
            Me.ConnectButton.Size = New System.Drawing.Size(83, 23)
            Me.ConnectButton.TabIndex = 6
            Me.ConnectButton.Text = "Connect"
            Me.ConnectButton.UseVisualStyleBackColor = True
            '
            'DevicesListBox
            '
            Me.DevicesListBox.FormattingEnabled = True
            Me.DevicesListBox.Location = New System.Drawing.Point(12, 19)
            Me.DevicesListBox.Name = "DevicesListBox"
            Me.DevicesListBox.Size = New System.Drawing.Size(236, 69)
            Me.DevicesListBox.TabIndex = 5
            '
            'OCRGroupBox
            '
            Me.OCRGroupBox.Controls.Add(Me.OCRParamsCheckedListBox)
            Me.OCRGroupBox.Location = New System.Drawing.Point(7, 334)
            Me.OCRGroupBox.Name = "OCRGroupBox"
            Me.OCRGroupBox.Size = New System.Drawing.Size(259, 107)
            Me.OCRGroupBox.TabIndex = 10
            Me.OCRGroupBox.TabStop = False
            Me.OCRGroupBox.Text = "OCR"
            '
            'OCRParamsCheckedListBox
            '
            Me.OCRParamsCheckedListBox.CheckOnClick = True
            Me.OCRParamsCheckedListBox.FormattingEnabled = True
            Me.OCRParamsCheckedListBox.Items.AddRange(New Object() {"MRZ fields", "VIZ fields", "BCR fields"})
            Me.OCRParamsCheckedListBox.Location = New System.Drawing.Point(12, 19)
            Me.OCRParamsCheckedListBox.Name = "OCRParamsCheckedListBox"
            Me.OCRParamsCheckedListBox.Size = New System.Drawing.Size(236, 64)
            Me.OCRParamsCheckedListBox.TabIndex = 0
            '
            'LightsGroupBox
            '
            Me.LightsGroupBox.Controls.Add(Me.LightsCheckedListBox)
            Me.LightsGroupBox.Controls.Add(Me.DocViewCheckBox)
            Me.LightsGroupBox.Location = New System.Drawing.Point(7, 169)
            Me.LightsGroupBox.Name = "LightsGroupBox"
            Me.LightsGroupBox.Size = New System.Drawing.Size(259, 159)
            Me.LightsGroupBox.TabIndex = 7
            Me.LightsGroupBox.TabStop = False
            Me.LightsGroupBox.Text = "Images"
            '
            'LightsCheckedListBox
            '
            Me.LightsCheckedListBox.CheckOnClick = True
            Me.LightsCheckedListBox.FormattingEnabled = True
            Me.LightsCheckedListBox.Location = New System.Drawing.Point(12, 19)
            Me.LightsCheckedListBox.Name = "LightsCheckedListBox"
            Me.LightsCheckedListBox.Size = New System.Drawing.Size(236, 109)
            Me.LightsCheckedListBox.TabIndex = 0
            '
            'DocViewCheckBox
            '
            Me.DocViewCheckBox.AutoSize = True
            Me.DocViewCheckBox.Location = New System.Drawing.Point(12, 134)
            Me.DocViewCheckBox.Name = "DocViewCheckBox"
            Me.DocViewCheckBox.Size = New System.Drawing.Size(149, 17)
            Me.DocViewCheckBox.TabIndex = 11
            Me.DocViewCheckBox.Text = "Crop and rotate document"
            Me.DocViewCheckBox.UseVisualStyleBackColor = True
            '
            'StartButton
            '
            Me.StartButton.Enabled = False
            Me.StartButton.Location = New System.Drawing.Point(69, 458)
            Me.StartButton.Name = "StartButton"
            Me.StartButton.Size = New System.Drawing.Size(133, 34)
            Me.StartButton.TabIndex = 9
            Me.StartButton.Text = "Start"
            Me.StartButton.UseVisualStyleBackColor = True
            '
            'FieldsTabControl
            '
            Me.FieldsTabControl.Controls.Add(Me.OcrTab)
            Me.FieldsTabControl.Controls.Add(Me.DataTab)
            Me.FieldsTabControl.Dock = System.Windows.Forms.DockStyle.Fill
            Me.FieldsTabControl.Location = New System.Drawing.Point(0, 0)
            Me.FieldsTabControl.Name = "FieldsTabControl"
            Me.FieldsTabControl.SelectedIndex = 0
            Me.FieldsTabControl.Size = New System.Drawing.Size(729, 533)
            Me.FieldsTabControl.TabIndex = 0
            '
            'OcrTab
            '
            Me.OcrTab.Controls.Add(Me.splitContainer2)
            Me.OcrTab.Location = New System.Drawing.Point(4, 22)
            Me.OcrTab.Name = "OcrTab"
            Me.OcrTab.Size = New System.Drawing.Size(721, 507)
            Me.OcrTab.TabIndex = 0
            Me.OcrTab.Text = "OCR"
            Me.OcrTab.UseVisualStyleBackColor = True
            '
            'splitContainer2
            '
            Me.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.splitContainer2.Location = New System.Drawing.Point(0, 0)
            Me.splitContainer2.Name = "splitContainer2"
            Me.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'splitContainer2.Panel1
            '
            Me.splitContainer2.Panel1.Controls.Add(Me.FieldsDataGridView)
            '
            'splitContainer2.Panel2
            '
            Me.splitContainer2.Panel2.BackColor = System.Drawing.Color.Transparent
            Me.splitContainer2.Panel2.Controls.Add(Me.FieldImagePictureBox)
            Me.splitContainer2.Panel2.Controls.Add(Me.FieldImageGroup)
            Me.splitContainer2.Panel2.Controls.Add(Me.ValuesGroup)
            Me.splitContainer2.Size = New System.Drawing.Size(721, 507)
            Me.splitContainer2.SplitterDistance = 327
            Me.splitContainer2.TabIndex = 0
            '
            'FieldsDataGridView
            '
            DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
            DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
            DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
            DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
            DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
            Me.FieldsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
            Me.FieldsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.FieldsDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.IndexColumn, Me.FieldIDColumn, Me.ValueColumn, Me.StatusColumn})
            Me.FieldsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
            Me.FieldsDataGridView.Location = New System.Drawing.Point(0, 0)
            Me.FieldsDataGridView.Name = "FieldsDataGridView"
            Me.FieldsDataGridView.Size = New System.Drawing.Size(721, 327)
            Me.FieldsDataGridView.TabIndex = 0
            '
            'IndexColumn
            '
            Me.IndexColumn.HeaderText = "Index"
            Me.IndexColumn.Name = "IndexColumn"
            Me.IndexColumn.Visible = False
            '
            'FieldIDColumn
            '
            Me.FieldIDColumn.HeaderText = "Field ID"
            Me.FieldIDColumn.MinimumWidth = 160
            Me.FieldIDColumn.Name = "FieldIDColumn"
            Me.FieldIDColumn.ReadOnly = True
            '
            'ValueColumn
            '
            Me.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me.ValueColumn.HeaderText = "Value"
            Me.ValueColumn.Name = "ValueColumn"
            Me.ValueColumn.ReadOnly = True
            '
            'StatusColumn
            '
            Me.StatusColumn.HeaderText = "Status"
            Me.StatusColumn.Name = "StatusColumn"
            Me.StatusColumn.ReadOnly = True
            '
            'FieldImagePictureBox
            '
            Me.FieldImagePictureBox.Location = New System.Drawing.Point(141, 112)
            Me.FieldImagePictureBox.Name = "FieldImagePictureBox"
            Me.FieldImagePictureBox.Size = New System.Drawing.Size(566, 53)
            Me.FieldImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.FieldImagePictureBox.TabIndex = 3
            Me.FieldImagePictureBox.TabStop = False
            '
            'FieldImageGroup
            '
            Me.FieldImageGroup.Location = New System.Drawing.Point(3, 101)
            Me.FieldImageGroup.Name = "FieldImageGroup"
            Me.FieldImageGroup.Size = New System.Drawing.Size(710, 69)
            Me.FieldImageGroup.TabIndex = 7
            Me.FieldImageGroup.TabStop = False
            Me.FieldImageGroup.Text = "Field image"
            '
            'ValuesGroup
            '
            Me.ValuesGroup.Controls.Add(Me.RAWValueLabel)
            Me.ValuesGroup.Controls.Add(Me.RLabel)
            Me.ValuesGroup.Controls.Add(Me.FLabel)
            Me.ValuesGroup.Controls.Add(Me.StandardizedValueLabel)
            Me.ValuesGroup.Controls.Add(Me.SLabel)
            Me.ValuesGroup.Controls.Add(Me.FormattedValueLabel)
            Me.ValuesGroup.Location = New System.Drawing.Point(3, 3)
            Me.ValuesGroup.Name = "ValuesGroup"
            Me.ValuesGroup.Size = New System.Drawing.Size(710, 92)
            Me.ValuesGroup.TabIndex = 8
            Me.ValuesGroup.TabStop = False
            Me.ValuesGroup.Text = "Values"
            '
            'RAWValueLabel
            '
            Me.RAWValueLabel.AutoSize = True
            Me.RAWValueLabel.Location = New System.Drawing.Point(141, 16)
            Me.RAWValueLabel.Name = "RAWValueLabel"
            Me.RAWValueLabel.Size = New System.Drawing.Size(0, 13)
            Me.RAWValueLabel.TabIndex = 6
            '
            'RLabel
            '
            Me.RLabel.AutoSize = True
            Me.RLabel.Location = New System.Drawing.Point(8, 19)
            Me.RLabel.Name = "RLabel"
            Me.RLabel.Size = New System.Drawing.Size(36, 13)
            Me.RLabel.TabIndex = 2
            Me.RLabel.Text = "RAW:"
            '
            'FLabel
            '
            Me.FLabel.AutoSize = True
            Me.FLabel.Location = New System.Drawing.Point(8, 41)
            Me.FLabel.Name = "FLabel"
            Me.FLabel.Size = New System.Drawing.Size(57, 13)
            Me.FLabel.TabIndex = 1
            Me.FLabel.Text = "Formatted:"
            '
            'StandardizedValueLabel
            '
            Me.StandardizedValueLabel.AutoSize = True
            Me.StandardizedValueLabel.Location = New System.Drawing.Point(141, 60)
            Me.StandardizedValueLabel.Name = "StandardizedValueLabel"
            Me.StandardizedValueLabel.Size = New System.Drawing.Size(0, 13)
            Me.StandardizedValueLabel.TabIndex = 6
            '
            'SLabel
            '
            Me.SLabel.AutoSize = True
            Me.SLabel.Location = New System.Drawing.Point(8, 63)
            Me.SLabel.Name = "SLabel"
            Me.SLabel.Size = New System.Drawing.Size(72, 13)
            Me.SLabel.TabIndex = 2
            Me.SLabel.Text = "Standardized:"
            '
            'FormattedValueLabel
            '
            Me.FormattedValueLabel.AutoSize = True
            Me.FormattedValueLabel.Location = New System.Drawing.Point(141, 38)
            Me.FormattedValueLabel.Name = "FormattedValueLabel"
            Me.FormattedValueLabel.Size = New System.Drawing.Size(0, 13)
            Me.FormattedValueLabel.TabIndex = 5
            '
            'DataTab
            '
            Me.DataTab.Controls.Add(Me.SignatureGroupBox)
            Me.DataTab.Controls.Add(Me.PhotoGroupBox)
            Me.DataTab.Controls.Add(Me.DocumentGroupBox)
            Me.DataTab.Controls.Add(Me.PersonalGroupBox)
            Me.DataTab.Location = New System.Drawing.Point(4, 22)
            Me.DataTab.Name = "DataTab"
            Me.DataTab.Size = New System.Drawing.Size(721, 507)
            Me.DataTab.TabIndex = 1
            Me.DataTab.Text = "Data"
            Me.DataTab.UseVisualStyleBackColor = True
            '
            'SignatureGroupBox
            '
            Me.SignatureGroupBox.Controls.Add(Me.SignaturePictureBox)
            Me.SignatureGroupBox.Location = New System.Drawing.Point(3, 334)
            Me.SignatureGroupBox.Name = "SignatureGroupBox"
            Me.SignatureGroupBox.Size = New System.Drawing.Size(574, 158)
            Me.SignatureGroupBox.TabIndex = 4
            Me.SignatureGroupBox.TabStop = False
            Me.SignatureGroupBox.Text = "Signature"
            '
            'SignaturePictureBox
            '
            Me.SignaturePictureBox.Location = New System.Drawing.Point(6, 19)
            Me.SignaturePictureBox.Name = "SignaturePictureBox"
            Me.SignaturePictureBox.Size = New System.Drawing.Size(562, 133)
            Me.SignaturePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.SignaturePictureBox.TabIndex = 0
            Me.SignaturePictureBox.TabStop = False
            '
            'PhotoGroupBox
            '
            Me.PhotoGroupBox.Controls.Add(Me.PhotoPictureBox)
            Me.PhotoGroupBox.Location = New System.Drawing.Point(583, 3)
            Me.PhotoGroupBox.Name = "PhotoGroupBox"
            Me.PhotoGroupBox.Size = New System.Drawing.Size(130, 160)
            Me.PhotoGroupBox.TabIndex = 3
            Me.PhotoGroupBox.TabStop = False
            Me.PhotoGroupBox.Text = "Face Photo"
            '
            'PhotoPictureBox
            '
            Me.PhotoPictureBox.Location = New System.Drawing.Point(6, 19)
            Me.PhotoPictureBox.Name = "PhotoPictureBox"
            Me.PhotoPictureBox.Size = New System.Drawing.Size(118, 135)
            Me.PhotoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.PhotoPictureBox.TabIndex = 0
            Me.PhotoPictureBox.TabStop = False
            '
            'DocumentGroupBox
            '
            Me.DocumentGroupBox.Controls.Add(Me.Validity)
            Me.DocumentGroupBox.Controls.Add(Me.Number)
            Me.DocumentGroupBox.Controls.Add(Me.Page)
            Me.DocumentGroupBox.Controls.Add(Me.Type)
            Me.DocumentGroupBox.Controls.Add(Me.Issuer)
            Me.DocumentGroupBox.Controls.Add(Me.ValidityLabel)
            Me.DocumentGroupBox.Controls.Add(Me.IssuerLabel)
            Me.DocumentGroupBox.Controls.Add(Me.PageLabel)
            Me.DocumentGroupBox.Controls.Add(Me.DocNumberLabel)
            Me.DocumentGroupBox.Controls.Add(Me.TypeLabel)
            Me.DocumentGroupBox.Location = New System.Drawing.Point(3, 169)
            Me.DocumentGroupBox.Name = "DocumentGroupBox"
            Me.DocumentGroupBox.Size = New System.Drawing.Size(574, 159)
            Me.DocumentGroupBox.TabIndex = 2
            Me.DocumentGroupBox.TabStop = False
            Me.DocumentGroupBox.Text = "Document Data"
            '
            'Validity
            '
            Me.Validity.AutoSize = True
            Me.Validity.Location = New System.Drawing.Point(88, 130)
            Me.Validity.Name = "Validity"
            Me.Validity.Size = New System.Drawing.Size(0, 13)
            Me.Validity.TabIndex = 9
            '
            'Number
            '
            Me.Number.AutoSize = True
            Me.Number.Location = New System.Drawing.Point(88, 104)
            Me.Number.Name = "Number"
            Me.Number.Size = New System.Drawing.Size(0, 13)
            Me.Number.TabIndex = 8
            '
            'Page
            '
            Me.Page.AutoSize = True
            Me.Page.Location = New System.Drawing.Point(88, 78)
            Me.Page.Name = "Page"
            Me.Page.Size = New System.Drawing.Size(0, 13)
            Me.Page.TabIndex = 7
            '
            'Type
            '
            Me.Type.AutoSize = True
            Me.Type.Location = New System.Drawing.Point(88, 52)
            Me.Type.Name = "Type"
            Me.Type.Size = New System.Drawing.Size(0, 13)
            Me.Type.TabIndex = 6
            '
            'Issuer
            '
            Me.Issuer.AutoSize = True
            Me.Issuer.Location = New System.Drawing.Point(88, 26)
            Me.Issuer.Name = "Issuer"
            Me.Issuer.Size = New System.Drawing.Size(0, 13)
            Me.Issuer.TabIndex = 5
            '
            'ValidityLabel
            '
            Me.ValidityLabel.AutoSize = True
            Me.ValidityLabel.Location = New System.Drawing.Point(6, 130)
            Me.ValidityLabel.Name = "ValidityLabel"
            Me.ValidityLabel.Size = New System.Drawing.Size(33, 13)
            Me.ValidityLabel.TabIndex = 4
            Me.ValidityLabel.Text = "Valid:"
            '
            'IssuerLabel
            '
            Me.IssuerLabel.AutoSize = True
            Me.IssuerLabel.Location = New System.Drawing.Point(6, 26)
            Me.IssuerLabel.Name = "IssuerLabel"
            Me.IssuerLabel.Size = New System.Drawing.Size(38, 13)
            Me.IssuerLabel.TabIndex = 0
            Me.IssuerLabel.Text = "Issuer:"
            '
            'PageLabel
            '
            Me.PageLabel.AutoSize = True
            Me.PageLabel.Location = New System.Drawing.Point(6, 78)
            Me.PageLabel.Name = "PageLabel"
            Me.PageLabel.Size = New System.Drawing.Size(35, 13)
            Me.PageLabel.TabIndex = 2
            Me.PageLabel.Text = "Page:"
            '
            'DocNumberLabel
            '
            Me.DocNumberLabel.AutoSize = True
            Me.DocNumberLabel.Location = New System.Drawing.Point(6, 104)
            Me.DocNumberLabel.Name = "DocNumberLabel"
            Me.DocNumberLabel.Size = New System.Drawing.Size(47, 13)
            Me.DocNumberLabel.TabIndex = 3
            Me.DocNumberLabel.Text = "Number:"
            '
            'TypeLabel
            '
            Me.TypeLabel.AutoSize = True
            Me.TypeLabel.Location = New System.Drawing.Point(6, 52)
            Me.TypeLabel.Name = "TypeLabel"
            Me.TypeLabel.Size = New System.Drawing.Size(34, 13)
            Me.TypeLabel.TabIndex = 1
            Me.TypeLabel.Text = "Type:"
            '
            'PersonalGroupBox
            '
            Me.PersonalGroupBox.Controls.Add(Me.Sex)
            Me.PersonalGroupBox.Controls.Add(Me.Nationality)
            Me.PersonalGroupBox.Controls.Add(Me.Birth)
            Me.PersonalGroupBox.Controls.Add(Me.Name2)
            Me.PersonalGroupBox.Controls.Add(Me.Name1)
            Me.PersonalGroupBox.Controls.Add(Me.SexLabel)
            Me.PersonalGroupBox.Controls.Add(Me.NationalityLabel)
            Me.PersonalGroupBox.Controls.Add(Me.BirthLabel)
            Me.PersonalGroupBox.Controls.Add(Me.NameLabel)
            Me.PersonalGroupBox.Location = New System.Drawing.Point(3, 3)
            Me.PersonalGroupBox.Name = "PersonalGroupBox"
            Me.PersonalGroupBox.Size = New System.Drawing.Size(574, 160)
            Me.PersonalGroupBox.TabIndex = 1
            Me.PersonalGroupBox.TabStop = False
            Me.PersonalGroupBox.Text = "Personal Data"
            '
            'Sex
            '
            Me.Sex.AutoSize = True
            Me.Sex.Location = New System.Drawing.Point(88, 132)
            Me.Sex.Name = "Sex"
            Me.Sex.Size = New System.Drawing.Size(0, 13)
            Me.Sex.TabIndex = 8
            '
            'Nationality
            '
            Me.Nationality.AutoSize = True
            Me.Nationality.Location = New System.Drawing.Point(88, 106)
            Me.Nationality.Name = "Nationality"
            Me.Nationality.Size = New System.Drawing.Size(0, 13)
            Me.Nationality.TabIndex = 7
            '
            'Birth
            '
            Me.Birth.AutoSize = True
            Me.Birth.Location = New System.Drawing.Point(88, 80)
            Me.Birth.Name = "Birth"
            Me.Birth.Size = New System.Drawing.Size(0, 13)
            Me.Birth.TabIndex = 6
            '
            'Name2
            '
            Me.Name2.AutoSize = True
            Me.Name2.Location = New System.Drawing.Point(88, 54)
            Me.Name2.Name = "Name2"
            Me.Name2.Size = New System.Drawing.Size(0, 13)
            Me.Name2.TabIndex = 5
            '
            'Name1
            '
            Me.Name1.AutoSize = True
            Me.Name1.Location = New System.Drawing.Point(88, 28)
            Me.Name1.Name = "Name1"
            Me.Name1.Size = New System.Drawing.Size(0, 13)
            Me.Name1.TabIndex = 4
            '
            'SexLabel
            '
            Me.SexLabel.AutoSize = True
            Me.SexLabel.Location = New System.Drawing.Point(6, 132)
            Me.SexLabel.Name = "SexLabel"
            Me.SexLabel.Size = New System.Drawing.Size(28, 13)
            Me.SexLabel.TabIndex = 3
            Me.SexLabel.Text = "Sex:"
            '
            'NationalityLabel
            '
            Me.NationalityLabel.AutoSize = True
            Me.NationalityLabel.Location = New System.Drawing.Point(6, 106)
            Me.NationalityLabel.Name = "NationalityLabel"
            Me.NationalityLabel.Size = New System.Drawing.Size(59, 13)
            Me.NationalityLabel.TabIndex = 2
            Me.NationalityLabel.Text = "Nationality:"
            '
            'BirthLabel
            '
            Me.BirthLabel.AutoSize = True
            Me.BirthLabel.Location = New System.Drawing.Point(6, 80)
            Me.BirthLabel.Name = "BirthLabel"
            Me.BirthLabel.Size = New System.Drawing.Size(31, 13)
            Me.BirthLabel.TabIndex = 1
            Me.BirthLabel.Text = "Birth:"
            '
            'NameLabel
            '
            Me.NameLabel.AutoSize = True
            Me.NameLabel.Location = New System.Drawing.Point(6, 28)
            Me.NameLabel.Name = "NameLabel"
            Me.NameLabel.Size = New System.Drawing.Size(38, 13)
            Me.NameLabel.TabIndex = 0
            Me.NameLabel.Text = "Name:"
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(1016, 533)
            Me.Controls.Add(Me.splitContainer1)
            Me.Name = "Form1"
            Me.Text = "Scanner Sample"
            Me.splitContainer1.Panel1.ResumeLayout(False)
            Me.splitContainer1.Panel2.ResumeLayout(False)
            Me.splitContainer1.ResumeLayout(False)
            Me.OptionsTabControl.ResumeLayout(False)
            Me.OptionsTab.ResumeLayout(False)
            Me.DevicesGroupBox.ResumeLayout(False)
            Me.OCRGroupBox.ResumeLayout(False)
            Me.LightsGroupBox.ResumeLayout(False)
            Me.LightsGroupBox.PerformLayout()
            Me.FieldsTabControl.ResumeLayout(False)
            Me.OcrTab.ResumeLayout(False)
            Me.splitContainer2.Panel1.ResumeLayout(False)
            Me.splitContainer2.Panel2.ResumeLayout(False)
            Me.splitContainer2.ResumeLayout(False)
            CType(Me.FieldsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.FieldImagePictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ValuesGroup.ResumeLayout(False)
            Me.ValuesGroup.PerformLayout()
            Me.DataTab.ResumeLayout(False)
            Me.SignatureGroupBox.ResumeLayout(False)
            CType(Me.SignaturePictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.PhotoGroupBox.ResumeLayout(False)
            CType(Me.PhotoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.DocumentGroupBox.ResumeLayout(False)
            Me.DocumentGroupBox.PerformLayout()
            Me.PersonalGroupBox.ResumeLayout(False)
            Me.PersonalGroupBox.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

#End Region

        Private splitContainer1 As System.Windows.Forms.SplitContainer
        Private DevicesListBox As System.Windows.Forms.ListBox
        Private WithEvents ConnectButton As System.Windows.Forms.Button
        Private WithEvents DisconnectButton As System.Windows.Forms.Button
        Private LightsGroupBox As System.Windows.Forms.GroupBox
        Private LightsCheckedListBox As System.Windows.Forms.CheckedListBox
        Private DocViewCheckBox As System.Windows.Forms.CheckBox
        Private OCRGroupBox As System.Windows.Forms.GroupBox
        Private OCRParamsCheckedListBox As System.Windows.Forms.CheckedListBox
        Private WithEvents StartButton As System.Windows.Forms.Button
        Private FieldsTabControl As System.Windows.Forms.TabControl
        Private OcrTab As System.Windows.Forms.TabPage
        Private splitContainer2 As System.Windows.Forms.SplitContainer
        Private WithEvents FieldsDataGridView As System.Windows.Forms.DataGridView
        Private RAWValueLabel As System.Windows.Forms.Label
        Private FormattedValueLabel As System.Windows.Forms.Label
        Private StandardizedValueLabel As System.Windows.Forms.Label
        Private FieldImagePictureBox As System.Windows.Forms.PictureBox
        Private RLabel As System.Windows.Forms.Label
        Private FLabel As System.Windows.Forms.Label
        Private SLabel As System.Windows.Forms.Label
        Private IndexColumn As System.Windows.Forms.DataGridViewTextBoxColumn
        Private FieldIDColumn As System.Windows.Forms.DataGridViewTextBoxColumn
        Private ValueColumn As System.Windows.Forms.DataGridViewTextBoxColumn
        Private StatusColumn As System.Windows.Forms.DataGridViewTextBoxColumn
        Private DataTab As System.Windows.Forms.TabPage
        Private NameLabel As System.Windows.Forms.Label
        Private PersonalGroupBox As System.Windows.Forms.GroupBox
        Private SexLabel As System.Windows.Forms.Label
        Private NationalityLabel As System.Windows.Forms.Label
        Private BirthLabel As System.Windows.Forms.Label
        Private DocumentGroupBox As System.Windows.Forms.GroupBox
        Private ValidityLabel As System.Windows.Forms.Label
        Private DocNumberLabel As System.Windows.Forms.Label
        Private PageLabel As System.Windows.Forms.Label
        Private TypeLabel As System.Windows.Forms.Label
        Private IssuerLabel As System.Windows.Forms.Label
        Private Validity As System.Windows.Forms.Label
        Private Number As System.Windows.Forms.Label
        Private Page As System.Windows.Forms.Label
        Private Type As System.Windows.Forms.Label
        Private Issuer As System.Windows.Forms.Label
        Private Sex As System.Windows.Forms.Label
        Private Nationality As System.Windows.Forms.Label
        Private Birth As System.Windows.Forms.Label
        Private Name2 As System.Windows.Forms.Label
        Private Name1 As System.Windows.Forms.Label
        Private PhotoGroupBox As System.Windows.Forms.GroupBox
        Private PhotoPictureBox As System.Windows.Forms.PictureBox
        Private SignatureGroupBox As System.Windows.Forms.GroupBox
        Private SignaturePictureBox As System.Windows.Forms.PictureBox
        Private DevicesGroupBox As System.Windows.Forms.GroupBox
        Private FieldImageGroup As System.Windows.Forms.GroupBox
        Private ValuesGroup As System.Windows.Forms.GroupBox
        Private OptionsTab As System.Windows.Forms.TabPage
        Private OptionsTabControl As System.Windows.Forms.TabControl
    End Class
End Namespace

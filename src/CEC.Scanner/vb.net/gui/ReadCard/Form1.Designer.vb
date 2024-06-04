Namespace ReadCard
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
            Me.components = New System.ComponentModel.Container
            Me.splitContainer1 = New System.Windows.Forms.SplitContainer
            Me.AuthSelector = New System.Windows.Forms.ComboBox
            Me.label1 = New System.Windows.Forms.Label
            Me.FilesGroupBox = New System.Windows.Forms.GroupBox
            Me.FilesListView = New System.Windows.Forms.ListView
            Me.contextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.toolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
            Me.StartButton = New System.Windows.Forms.Button
            Me.ReadersGroupBox = New System.Windows.Forms.GroupBox
            Me.ReadersCheckedListBox = New System.Windows.Forms.CheckedListBox
            Me.DevicesGroupBox = New System.Windows.Forms.GroupBox
            Me.DevicesListBox = New System.Windows.Forms.ListBox
            Me.ConnectButton = New System.Windows.Forms.Button
            Me.DisconnectButton = New System.Windows.Forms.Button
            Me.SignaturesGroupBox = New System.Windows.Forms.GroupBox
            Me.SignaturePictureBox2 = New System.Windows.Forms.PictureBox
            Me.SignaturePictureBox1 = New System.Windows.Forms.PictureBox
            Me.FingersGroupBox = New System.Windows.Forms.GroupBox
            Me.FingerPictureBox2 = New System.Windows.Forms.PictureBox
            Me.FingerPictureBox1 = New System.Windows.Forms.PictureBox
            Me.FacesGroupBox = New System.Windows.Forms.GroupBox
            Me.FacePictureBox2 = New System.Windows.Forms.PictureBox
            Me.FacePictureBox1 = New System.Windows.Forms.PictureBox
            Me.AuthGroupBox = New System.Windows.Forms.GroupBox
            Me.PACELabel = New System.Windows.Forms.Label
            Me.TALabel = New System.Windows.Forms.Label
            Me.CALabel = New System.Windows.Forms.Label
            Me.FaceLabel = New System.Windows.Forms.Label
            Me.PALabel = New System.Windows.Forms.Label
            Me.MrzLabel = New System.Windows.Forms.Label
            Me.AALabel = New System.Windows.Forms.Label
            Me.BACLabel = New System.Windows.Forms.Label
            Me.MRZTextBox = New System.Windows.Forms.TextBox
            Me.textBox1 = New System.Windows.Forms.TextBox
            Me.saveFileDialog1 = New System.Windows.Forms.SaveFileDialog
            Me.splitContainer1.Panel1.SuspendLayout()
            Me.splitContainer1.Panel2.SuspendLayout()
            Me.splitContainer1.SuspendLayout()
            Me.FilesGroupBox.SuspendLayout()
            Me.contextMenuStrip1.SuspendLayout()
            Me.ReadersGroupBox.SuspendLayout()
            Me.DevicesGroupBox.SuspendLayout()
            Me.SignaturesGroupBox.SuspendLayout()
            CType(Me.SignaturePictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.SignaturePictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.FingersGroupBox.SuspendLayout()
            CType(Me.FingerPictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.FingerPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.FacesGroupBox.SuspendLayout()
            CType(Me.FacePictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.FacePictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.AuthGroupBox.SuspendLayout()
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
            Me.splitContainer1.Panel1.Controls.Add(Me.AuthSelector)
            Me.splitContainer1.Panel1.Controls.Add(Me.label1)
            Me.splitContainer1.Panel1.Controls.Add(Me.FilesGroupBox)
            Me.splitContainer1.Panel1.Controls.Add(Me.StartButton)
            Me.splitContainer1.Panel1.Controls.Add(Me.ReadersGroupBox)
            Me.splitContainer1.Panel1.Controls.Add(Me.DevicesGroupBox)
            '
            'splitContainer1.Panel2
            '
            Me.splitContainer1.Panel2.Controls.Add(Me.SignaturesGroupBox)
            Me.splitContainer1.Panel2.Controls.Add(Me.FingersGroupBox)
            Me.splitContainer1.Panel2.Controls.Add(Me.FacesGroupBox)
            Me.splitContainer1.Panel2.Controls.Add(Me.AuthGroupBox)
            Me.splitContainer1.Panel2.Controls.Add(Me.MRZTextBox)
            Me.splitContainer1.Panel2.Controls.Add(Me.textBox1)
            Me.splitContainer1.Size = New System.Drawing.Size(792, 543)
            Me.splitContainer1.SplitterDistance = 283
            Me.splitContainer1.TabIndex = 2
            '
            'AuthSelector
            '
            Me.AuthSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.AuthSelector.FormattingEnabled = True
            Me.AuthSelector.Location = New System.Drawing.Point(149, 465)
            Me.AuthSelector.Name = "AuthSelector"
            Me.AuthSelector.Size = New System.Drawing.Size(123, 21)
            Me.AuthSelector.TabIndex = 14
            '
            'label1
            '
            Me.label1.AutoSize = True
            Me.label1.Location = New System.Drawing.Point(21, 468)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(103, 13)
            Me.label1.TabIndex = 13
            Me.label1.Text = "Authentication level:"
            '
            'FilesGroupBox
            '
            Me.FilesGroupBox.Controls.Add(Me.FilesListView)
            Me.FilesGroupBox.Location = New System.Drawing.Point(13, 242)
            Me.FilesGroupBox.Name = "FilesGroupBox"
            Me.FilesGroupBox.Size = New System.Drawing.Size(259, 209)
            Me.FilesGroupBox.TabIndex = 11
            Me.FilesGroupBox.TabStop = False
            Me.FilesGroupBox.Text = "Files"
            '
            'FilesListView
            '
            Me.FilesListView.CheckBoxes = True
            Me.FilesListView.ContextMenuStrip = Me.contextMenuStrip1
            Me.FilesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.FilesListView.Location = New System.Drawing.Point(11, 19)
            Me.FilesListView.MultiSelect = False
            Me.FilesListView.Name = "FilesListView"
            Me.FilesListView.ShowGroups = False
            Me.FilesListView.Size = New System.Drawing.Size(237, 184)
            Me.FilesListView.TabIndex = 0
            Me.FilesListView.UseCompatibleStateImageBehavior = False
            Me.FilesListView.View = System.Windows.Forms.View.List
            '
            'contextMenuStrip1
            '
            Me.contextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripMenuItem1})
            Me.contextMenuStrip1.Name = "contextMenuStrip1"
            Me.contextMenuStrip1.Size = New System.Drawing.Size(110, 26)
            '
            'toolStripMenuItem1
            '
            Me.toolStripMenuItem1.Name = "toolStripMenuItem1"
            Me.toolStripMenuItem1.Size = New System.Drawing.Size(109, 22)
            Me.toolStripMenuItem1.Text = "Save"
            '
            'StartButton
            '
            Me.StartButton.Enabled = False
            Me.StartButton.Location = New System.Drawing.Point(166, 496)
            Me.StartButton.Name = "StartButton"
            Me.StartButton.Size = New System.Drawing.Size(106, 35)
            Me.StartButton.TabIndex = 9
            Me.StartButton.Text = "Read"
            Me.StartButton.UseVisualStyleBackColor = True
            '
            'ReadersGroupBox
            '
            Me.ReadersGroupBox.Controls.Add(Me.ReadersCheckedListBox)
            Me.ReadersGroupBox.Location = New System.Drawing.Point(12, 142)
            Me.ReadersGroupBox.Name = "ReadersGroupBox"
            Me.ReadersGroupBox.Size = New System.Drawing.Size(260, 94)
            Me.ReadersGroupBox.TabIndex = 7
            Me.ReadersGroupBox.TabStop = False
            Me.ReadersGroupBox.Text = "Card Readers"
            '
            'ReadersCheckedListBox
            '
            Me.ReadersCheckedListBox.CheckOnClick = True
            Me.ReadersCheckedListBox.FormattingEnabled = True
            Me.ReadersCheckedListBox.Location = New System.Drawing.Point(12, 19)
            Me.ReadersCheckedListBox.Name = "ReadersCheckedListBox"
            Me.ReadersCheckedListBox.Size = New System.Drawing.Size(237, 64)
            Me.ReadersCheckedListBox.TabIndex = 0
            '
            'DevicesGroupBox
            '
            Me.DevicesGroupBox.Controls.Add(Me.DevicesListBox)
            Me.DevicesGroupBox.Controls.Add(Me.ConnectButton)
            Me.DevicesGroupBox.Controls.Add(Me.DisconnectButton)
            Me.DevicesGroupBox.Location = New System.Drawing.Point(12, 8)
            Me.DevicesGroupBox.Name = "DevicesGroupBox"
            Me.DevicesGroupBox.Size = New System.Drawing.Size(260, 128)
            Me.DevicesGroupBox.TabIndex = 15
            Me.DevicesGroupBox.TabStop = False
            Me.DevicesGroupBox.Text = "Devices"
            '
            'DevicesListBox
            '
            Me.DevicesListBox.FormattingEnabled = True
            Me.DevicesListBox.Location = New System.Drawing.Point(12, 19)
            Me.DevicesListBox.Name = "DevicesListBox"
            Me.DevicesListBox.Size = New System.Drawing.Size(237, 56)
            Me.DevicesListBox.TabIndex = 5
            '
            'ConnectButton
            '
            Me.ConnectButton.Location = New System.Drawing.Point(46, 90)
            Me.ConnectButton.Name = "ConnectButton"
            Me.ConnectButton.Size = New System.Drawing.Size(75, 23)
            Me.ConnectButton.TabIndex = 6
            Me.ConnectButton.Text = "Connect"
            Me.ConnectButton.UseVisualStyleBackColor = True
            '
            'DisconnectButton
            '
            Me.DisconnectButton.Enabled = False
            Me.DisconnectButton.Location = New System.Drawing.Point(140, 90)
            Me.DisconnectButton.Name = "DisconnectButton"
            Me.DisconnectButton.Size = New System.Drawing.Size(75, 23)
            Me.DisconnectButton.TabIndex = 8
            Me.DisconnectButton.Text = "Disconnect"
            Me.DisconnectButton.UseVisualStyleBackColor = True
            '
            'SignaturesGroupBox
            '
            Me.SignaturesGroupBox.Controls.Add(Me.SignaturePictureBox2)
            Me.SignaturesGroupBox.Controls.Add(Me.SignaturePictureBox1)
            Me.SignaturesGroupBox.Location = New System.Drawing.Point(12, 312)
            Me.SignaturesGroupBox.Name = "SignaturesGroupBox"
            Me.SignaturesGroupBox.Size = New System.Drawing.Size(367, 139)
            Me.SignaturesGroupBox.TabIndex = 17
            Me.SignaturesGroupBox.TabStop = False
            Me.SignaturesGroupBox.Text = "Signatures"
            '
            'SignaturePictureBox2
            '
            Me.SignaturePictureBox2.Location = New System.Drawing.Point(6, 78)
            Me.SignaturePictureBox2.Name = "SignaturePictureBox2"
            Me.SignaturePictureBox2.Size = New System.Drawing.Size(355, 53)
            Me.SignaturePictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.SignaturePictureBox2.TabIndex = 14
            Me.SignaturePictureBox2.TabStop = False
            '
            'SignaturePictureBox1
            '
            Me.SignaturePictureBox1.Location = New System.Drawing.Point(6, 19)
            Me.SignaturePictureBox1.Name = "SignaturePictureBox1"
            Me.SignaturePictureBox1.Size = New System.Drawing.Size(355, 53)
            Me.SignaturePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.SignaturePictureBox1.TabIndex = 3
            Me.SignaturePictureBox1.TabStop = False
            '
            'FingersGroupBox
            '
            Me.FingersGroupBox.Controls.Add(Me.FingerPictureBox2)
            Me.FingersGroupBox.Controls.Add(Me.FingerPictureBox1)
            Me.FingersGroupBox.Location = New System.Drawing.Point(385, 8)
            Me.FingersGroupBox.Name = "FingersGroupBox"
            Me.FingersGroupBox.Size = New System.Drawing.Size(108, 228)
            Me.FingersGroupBox.TabIndex = 16
            Me.FingersGroupBox.TabStop = False
            Me.FingersGroupBox.Text = "Fingerprints"
            '
            'FingerPictureBox2
            '
            Me.FingerPictureBox2.Location = New System.Drawing.Point(10, 122)
            Me.FingerPictureBox2.Name = "FingerPictureBox2"
            Me.FingerPictureBox2.Size = New System.Drawing.Size(88, 100)
            Me.FingerPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.FingerPictureBox2.TabIndex = 5
            Me.FingerPictureBox2.TabStop = False
            '
            'FingerPictureBox1
            '
            Me.FingerPictureBox1.Location = New System.Drawing.Point(10, 19)
            Me.FingerPictureBox1.Name = "FingerPictureBox1"
            Me.FingerPictureBox1.Size = New System.Drawing.Size(88, 100)
            Me.FingerPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.FingerPictureBox1.TabIndex = 4
            Me.FingerPictureBox1.TabStop = False
            '
            'FacesGroupBox
            '
            Me.FacesGroupBox.Controls.Add(Me.FacePictureBox2)
            Me.FacesGroupBox.Controls.Add(Me.FacePictureBox1)
            Me.FacesGroupBox.Location = New System.Drawing.Point(12, 8)
            Me.FacesGroupBox.Name = "FacesGroupBox"
            Me.FacesGroupBox.Size = New System.Drawing.Size(367, 228)
            Me.FacesGroupBox.TabIndex = 15
            Me.FacesGroupBox.TabStop = False
            Me.FacesGroupBox.Text = "Face images"
            '
            'FacePictureBox2
            '
            Me.FacePictureBox2.Location = New System.Drawing.Point(187, 19)
            Me.FacePictureBox2.Name = "FacePictureBox2"
            Me.FacePictureBox2.Size = New System.Drawing.Size(174, 203)
            Me.FacePictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.FacePictureBox2.TabIndex = 13
            Me.FacePictureBox2.TabStop = False
            '
            'FacePictureBox1
            '
            Me.FacePictureBox1.Location = New System.Drawing.Point(6, 19)
            Me.FacePictureBox1.Name = "FacePictureBox1"
            Me.FacePictureBox1.Size = New System.Drawing.Size(174, 203)
            Me.FacePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.FacePictureBox1.TabIndex = 1
            Me.FacePictureBox1.TabStop = False
            '
            'AuthGroupBox
            '
            Me.AuthGroupBox.Controls.Add(Me.PACELabel)
            Me.AuthGroupBox.Controls.Add(Me.TALabel)
            Me.AuthGroupBox.Controls.Add(Me.CALabel)
            Me.AuthGroupBox.Controls.Add(Me.FaceLabel)
            Me.AuthGroupBox.Controls.Add(Me.PALabel)
            Me.AuthGroupBox.Controls.Add(Me.MrzLabel)
            Me.AuthGroupBox.Controls.Add(Me.AALabel)
            Me.AuthGroupBox.Controls.Add(Me.BACLabel)
            Me.AuthGroupBox.Location = New System.Drawing.Point(385, 312)
            Me.AuthGroupBox.Name = "AuthGroupBox"
            Me.AuthGroupBox.Size = New System.Drawing.Size(108, 139)
            Me.AuthGroupBox.TabIndex = 12
            Me.AuthGroupBox.TabStop = False
            Me.AuthGroupBox.Text = "Authentications"
            '
            'PACELabel
            '
            Me.PACELabel.AutoSize = True
            Me.PACELabel.Location = New System.Drawing.Point(59, 25)
            Me.PACELabel.Name = "PACELabel"
            Me.PACELabel.Size = New System.Drawing.Size(35, 13)
            Me.PACELabel.TabIndex = 17
            Me.PACELabel.Text = "PACE"
            '
            'TALabel
            '
            Me.TALabel.AutoSize = True
            Me.TALabel.Location = New System.Drawing.Point(59, 54)
            Me.TALabel.Name = "TALabel"
            Me.TALabel.Size = New System.Drawing.Size(21, 13)
            Me.TALabel.TabIndex = 16
            Me.TALabel.Text = "TA"
            '
            'CALabel
            '
            Me.CALabel.AutoSize = True
            Me.CALabel.Location = New System.Drawing.Point(14, 54)
            Me.CALabel.Name = "CALabel"
            Me.CALabel.Size = New System.Drawing.Size(21, 13)
            Me.CALabel.TabIndex = 15
            Me.CALabel.Text = "CA"
            '
            'FaceLabel
            '
            Me.FaceLabel.AutoSize = True
            Me.FaceLabel.Location = New System.Drawing.Point(59, 112)
            Me.FaceLabel.Name = "FaceLabel"
            Me.FaceLabel.Size = New System.Drawing.Size(31, 13)
            Me.FaceLabel.TabIndex = 7
            Me.FaceLabel.Text = "Face"
            '
            'PALabel
            '
            Me.PALabel.AutoSize = True
            Me.PALabel.Location = New System.Drawing.Point(14, 83)
            Me.PALabel.Name = "PALabel"
            Me.PALabel.Size = New System.Drawing.Size(21, 13)
            Me.PALabel.TabIndex = 14
            Me.PALabel.Text = "PA"
            '
            'MrzLabel
            '
            Me.MrzLabel.AutoSize = True
            Me.MrzLabel.Location = New System.Drawing.Point(14, 112)
            Me.MrzLabel.Name = "MrzLabel"
            Me.MrzLabel.Size = New System.Drawing.Size(31, 13)
            Me.MrzLabel.TabIndex = 6
            Me.MrzLabel.Text = "MRZ"
            '
            'AALabel
            '
            Me.AALabel.AutoSize = True
            Me.AALabel.Location = New System.Drawing.Point(59, 83)
            Me.AALabel.Name = "AALabel"
            Me.AALabel.Size = New System.Drawing.Size(21, 13)
            Me.AALabel.TabIndex = 13
            Me.AALabel.Text = "AA"
            '
            'BACLabel
            '
            Me.BACLabel.AutoSize = True
            Me.BACLabel.Location = New System.Drawing.Point(14, 25)
            Me.BACLabel.Name = "BACLabel"
            Me.BACLabel.Size = New System.Drawing.Size(28, 13)
            Me.BACLabel.TabIndex = 11
            Me.BACLabel.Text = "BAC"
            '
            'MRZTextBox
            '
            Me.MRZTextBox.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.MRZTextBox.Location = New System.Drawing.Point(12, 248)
            Me.MRZTextBox.Multiline = True
            Me.MRZTextBox.Name = "MRZTextBox"
            Me.MRZTextBox.ReadOnly = True
            Me.MRZTextBox.Size = New System.Drawing.Size(481, 58)
            Me.MRZTextBox.TabIndex = 2
            Me.MRZTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            '
            'textBox1
            '
            Me.textBox1.Location = New System.Drawing.Point(12, 459)
            Me.textBox1.Multiline = True
            Me.textBox1.Name = "textBox1"
            Me.textBox1.ReadOnly = True
            Me.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.textBox1.Size = New System.Drawing.Size(481, 72)
            Me.textBox1.TabIndex = 0
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoSize = True
            Me.ClientSize = New System.Drawing.Size(792, 543)
            Me.Controls.Add(Me.splitContainer1)
            Me.Name = "Form1"
            Me.RightToLeftLayout = True
            Me.Text = "Read Card"
            Me.splitContainer1.Panel1.ResumeLayout(False)
            Me.splitContainer1.Panel1.PerformLayout()
            Me.splitContainer1.Panel2.ResumeLayout(False)
            Me.splitContainer1.Panel2.PerformLayout()
            Me.splitContainer1.ResumeLayout(False)
            Me.FilesGroupBox.ResumeLayout(False)
            Me.contextMenuStrip1.ResumeLayout(False)
            Me.ReadersGroupBox.ResumeLayout(False)
            Me.DevicesGroupBox.ResumeLayout(False)
            Me.SignaturesGroupBox.ResumeLayout(False)
            CType(Me.SignaturePictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.SignaturePictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.FingersGroupBox.ResumeLayout(False)
            CType(Me.FingerPictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.FingerPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.FacesGroupBox.ResumeLayout(False)
            CType(Me.FacePictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.FacePictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.AuthGroupBox.ResumeLayout(False)
            Me.AuthGroupBox.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

#End Region

        Private splitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents DisconnectButton As System.Windows.Forms.Button
        Private ReadersGroupBox As System.Windows.Forms.GroupBox
        Private WithEvents ConnectButton As System.Windows.Forms.Button
        Private DevicesListBox As System.Windows.Forms.ListBox
        Private WithEvents StartButton As System.Windows.Forms.Button
        Private ReadersCheckedListBox As System.Windows.Forms.CheckedListBox
        Private textBox1 As System.Windows.Forms.TextBox
        Private FilesGroupBox As System.Windows.Forms.GroupBox
        Private label1 As System.Windows.Forms.Label
        Private AuthSelector As System.Windows.Forms.ComboBox
        Private FilesListView As System.Windows.Forms.ListView
        Private MRZTextBox As System.Windows.Forms.TextBox
        Private FacePictureBox1 As System.Windows.Forms.PictureBox
        Private SignaturePictureBox1 As System.Windows.Forms.PictureBox
        Private FingerPictureBox2 As System.Windows.Forms.PictureBox
        Private FingerPictureBox1 As System.Windows.Forms.PictureBox
        Private contextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
        Private WithEvents toolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
        Private saveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Private FaceLabel As System.Windows.Forms.Label
        Private MrzLabel As System.Windows.Forms.Label
        Private AuthGroupBox As System.Windows.Forms.GroupBox
        Private PACELabel As System.Windows.Forms.Label
        Private TALabel As System.Windows.Forms.Label
        Private CALabel As System.Windows.Forms.Label
        Private PALabel As System.Windows.Forms.Label
        Private AALabel As System.Windows.Forms.Label
        Private BACLabel As System.Windows.Forms.Label
        Private FacePictureBox2 As System.Windows.Forms.PictureBox
        Private SignaturePictureBox2 As System.Windows.Forms.PictureBox
        Private DevicesGroupBox As System.Windows.Forms.GroupBox
        Private FacesGroupBox As System.Windows.Forms.GroupBox
        Private FingersGroupBox As System.Windows.Forms.GroupBox
        Private SignaturesGroupBox As System.Windows.Forms.GroupBox

    End Class
End Namespace


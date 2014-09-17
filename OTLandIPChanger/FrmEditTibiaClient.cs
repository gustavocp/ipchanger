namespace OTLandIPChanger
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class FrmEditTibiaClient : Form
    {
        private readonly FrmMain _main;
        private readonly FrmSettings _settings;
        private Button browseButton;
        private IContainer components;
        private Button editButton;
        private Label label1;
        private Label label2;
        private TextBox pathText;
        private TextBox versionText;

        public FrmEditTibiaClient(FrmMain main, FrmSettings settings, string version)
        {
            this.InitializeComponent();
            this._main = main;
            this._settings = settings;
            this.versionText.Text = version;
            this.pathText.Text = this._main.GetClientPath(version);
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Title = "Select the executable for Tibia",
                Filter = "Tibia executable file|Tibia.exe"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.pathText.Text = dialog.FileName;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EditButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.pathText.Text))
            {
                MessageBox.Show(string.Format("Please select the path to Tibia {0}.", this.versionText.Text), "OtLand IP Changer");
            }
            else if (!this.pathText.Text.EndsWith(".exe") || !File.Exists(this.pathText.Text))
            {
                MessageBox.Show("The client path must point to an executable file.", "OtLand IP Changer");
            }
            else
            {
                this._main.SetClient(this.versionText.Text, this.pathText.Text);
                base.Close();
            }
        }

        private void InitializeComponent()
        {
            this.editButton = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.versionText = new TextBox();
            this.pathText = new TextBox();
            this.browseButton = new Button();
            base.SuspendLayout();
            this.editButton.Location = new Point(12, 0x24);
            this.editButton.Name = "editButton";
            this.editButton.Size = new Size(370, 0x17);
            this.editButton.TabIndex = 0;
            this.editButton.Text = "Save";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new EventHandler(this.EditButtonClick);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x2d, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Version:";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x71, 13);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x20, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path:";
            this.versionText.Location = new Point(0x40, 10);
            this.versionText.MaxLength = 5;
            this.versionText.Name = "versionText";
            this.versionText.ReadOnly = true;
            this.versionText.Size = new Size(0x2b, 20);
            this.versionText.TabIndex = 3;
            this.pathText.Location = new Point(0x97, 10);
            this.pathText.Name = "pathText";
            this.pathText.Size = new Size(0xa5, 20);
            this.pathText.TabIndex = 4;
            this.browseButton.Location = new Point(0x142, 9);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new Size(60, 0x16);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new EventHandler(this.BrowseButtonClick);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x185, 0x42);
            base.Controls.Add(this.browseButton);
            base.Controls.Add(this.pathText);
            base.Controls.Add(this.versionText);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.editButton);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FrmEditTibiaClient";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.Manual;
            this.Text = "Edit Tibia client";
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}


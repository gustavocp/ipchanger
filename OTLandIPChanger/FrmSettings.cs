namespace OTLandIPChanger
{
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FrmSettings : Form
    {
        private readonly FrmMain _main;
        private Button addNewTibiaClientButton;
        private CheckBox alwaysLaunchNewClientCheckbox;
        private ListBox clientPaths;
        private Button closeButton;
        private IContainer components;
        private Button deleteClientButton;
        private Button editClientButton;
        private CheckBox forceGraphicsEngineCheckbox;
        private ComboBox graphicsEnginesComboBox;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private CheckBox launchWithGamemasterFlagCheckbox;
        private CheckBox startWithWindowsCheckbox;
        private CheckBox storeClientConfigSeparateCheckbox;

        public FrmSettings(FrmMain parent)
        {
            this.InitializeComponent();
            this._main = parent;
            this.startWithWindowsCheckbox.Checked = Config.Instance.StartWithWindows;
            this.alwaysLaunchNewClientCheckbox.Checked = Config.Instance.AlwaysLaunchNewClient;
            this.launchWithGamemasterFlagCheckbox.Checked = Config.Instance.LaunchWithGamemasterFlag;
            this.storeClientConfigSeparateCheckbox.Checked = Config.Instance.StoreClientConfigurationSeparate;
            this.forceGraphicsEngineCheckbox.Checked = Config.Instance.ForceGraphicsEngine;
            this.graphicsEnginesComboBox.SelectedIndex = Config.Instance.SelectedGraphicsEngine;
            this.clientPaths.DataSource = this._main.Clients;
        }

        private void AddNewTibiaClientButtonClick(object sender, EventArgs e)
        {
            FrmAddTibiaClient client = new FrmAddTibiaClient(this._main, this);
            client.SetDesktopLocation((base.Location.X + (base.Width / 2)) - (client.Width / 2), (base.Location.Y + (base.Height / 2)) - (client.Height / 2));
            client.ShowDialog();
        }

        private void AlwaysLaunchNewClientCheckboxCheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.AlwaysLaunchNewClient = this.alwaysLaunchNewClientCheckbox.Checked;
        }

        private void ClientPathsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.editClientButton.Enabled = this.deleteClientButton.Enabled = this.clientPaths.SelectedIndex != -1;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            this._main.SaveConfiguration();
            base.Close();
        }

        private void DeleteClientButtonClick(object sender, EventArgs e)
        {
            string itemText = this.clientPaths.GetItemText(this.clientPaths.SelectedItem);
            if (MessageBox.Show(string.Format("Are you sure that you want to delete the version entry for {0}?", itemText), "OtLand IP Changer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this._main.RemoveClient(itemText);
                this.UpdateClientPaths();
                if (this.clientPaths.Items.Count == 0)
                {
                    this.editClientButton.Enabled = false;
                    this.deleteClientButton.Enabled = false;
                }
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

        private void EditClientButtonClick(object sender, EventArgs e)
        {
            FrmEditTibiaClient client = new FrmEditTibiaClient(this._main, this, this.clientPaths.GetItemText(this.clientPaths.SelectedItem));
            client.SetDesktopLocation((base.Location.X + (base.Width / 2)) - (client.Width / 2), (base.Location.Y + (base.Height / 2)) - (client.Height / 2));
            client.ShowDialog();
        }

        private void ForceGraphicsEngineCheckboxCheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.ForceGraphicsEngine = this.forceGraphicsEngineCheckbox.Checked;
        }

        private void GraphicsEnginesComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            Config.Instance.SelectedGraphicsEngine = this.graphicsEnginesComboBox.SelectedIndex;
        }

        private void InitializeComponent()
        {
            this.startWithWindowsCheckbox = new CheckBox();
            this.groupBox1 = new GroupBox();
            this.alwaysLaunchNewClientCheckbox = new CheckBox();
            this.storeClientConfigSeparateCheckbox = new CheckBox();
            this.launchWithGamemasterFlagCheckbox = new CheckBox();
            this.groupBox2 = new GroupBox();
            this.deleteClientButton = new Button();
            this.editClientButton = new Button();
            this.addNewTibiaClientButton = new Button();
            this.clientPaths = new ListBox();
            this.closeButton = new Button();
            this.groupBox3 = new GroupBox();
            this.graphicsEnginesComboBox = new ComboBox();
            this.forceGraphicsEngineCheckbox = new CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            base.SuspendLayout();
            this.startWithWindowsCheckbox.AutoSize = true;
            this.startWithWindowsCheckbox.Location = new Point(15, 0x13);
            this.startWithWindowsCheckbox.Name = "startWithWindowsCheckbox";
            this.startWithWindowsCheckbox.Size = new Size(0x80, 0x11);
            this.startWithWindowsCheckbox.TabIndex = 0;
            this.startWithWindowsCheckbox.Text = "Run at system startup";
            this.startWithWindowsCheckbox.UseVisualStyleBackColor = true;
            this.startWithWindowsCheckbox.CheckedChanged += new EventHandler(this.StartWithWindowsCheckboxCheckedChanged);
            this.groupBox1.Controls.Add(this.alwaysLaunchNewClientCheckbox);
            this.groupBox1.Controls.Add(this.startWithWindowsCheckbox);
            this.groupBox1.Location = new Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(270, 0x3b);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            this.alwaysLaunchNewClientCheckbox.AutoSize = true;
            this.alwaysLaunchNewClientCheckbox.Location = new Point(15, 0x26);
            this.alwaysLaunchNewClientCheckbox.Name = "alwaysLaunchNewClientCheckbox";
            this.alwaysLaunchNewClientCheckbox.Size = new Size(180, 0x11);
            this.alwaysLaunchNewClientCheckbox.TabIndex = 1;
            this.alwaysLaunchNewClientCheckbox.Text = "Always launch a new Tibia client";
            this.alwaysLaunchNewClientCheckbox.UseVisualStyleBackColor = true;
            this.alwaysLaunchNewClientCheckbox.CheckedChanged += new EventHandler(this.AlwaysLaunchNewClientCheckboxCheckedChanged);
            this.storeClientConfigSeparateCheckbox.AutoSize = true;
            this.storeClientConfigSeparateCheckbox.Location = new Point(15, 0x26);
            this.storeClientConfigSeparateCheckbox.Name = "storeClientConfigSeparateCheckbox";
            this.storeClientConfigSeparateCheckbox.Size = new Size(0xfd, 0x11);
            this.storeClientConfigSeparateCheckbox.TabIndex = 3;
            this.storeClientConfigSeparateCheckbox.Text = "Store client configuration files in separate folders";
            this.storeClientConfigSeparateCheckbox.UseVisualStyleBackColor = true;
            this.storeClientConfigSeparateCheckbox.CheckedChanged += new EventHandler(this.StoreClientConfigSeparateCheckboxCheckedChanged);
            this.launchWithGamemasterFlagCheckbox.AutoSize = true;
            this.launchWithGamemasterFlagCheckbox.Location = new Point(15, 0x13);
            this.launchWithGamemasterFlagCheckbox.Name = "launchWithGamemasterFlagCheckbox";
            this.launchWithGamemasterFlagCheckbox.Size = new Size(0xf7, 0x11);
            this.launchWithGamemasterFlagCheckbox.TabIndex = 2;
            this.launchWithGamemasterFlagCheckbox.Text = "Launch client with gamemaster flag (multiclient)";
            this.launchWithGamemasterFlagCheckbox.UseVisualStyleBackColor = true;
            this.launchWithGamemasterFlagCheckbox.CheckedChanged += new EventHandler(this.LaunchWithGamemasterFlagCheckboxCheckedChanged);
            this.groupBox2.Controls.Add(this.deleteClientButton);
            this.groupBox2.Controls.Add(this.editClientButton);
            this.groupBox2.Controls.Add(this.addNewTibiaClientButton);
            this.groupBox2.Controls.Add(this.clientPaths);
            this.groupBox2.Location = new Point(12, 0xa6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(270, 0x6c);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configure client versions";
            this.deleteClientButton.Enabled = false;
            this.deleteClientButton.Location = new Point(0x60, 0x4f);
            this.deleteClientButton.Name = "deleteClientButton";
            this.deleteClientButton.Size = new Size(0xa8, 0x17);
            this.deleteClientButton.TabIndex = 6;
            this.deleteClientButton.Text = "Delete selected Tibia client";
            this.deleteClientButton.UseVisualStyleBackColor = true;
            this.deleteClientButton.Click += new EventHandler(this.DeleteClientButtonClick);
            this.editClientButton.Enabled = false;
            this.editClientButton.Location = new Point(0x60, 0x31);
            this.editClientButton.Name = "editClientButton";
            this.editClientButton.Size = new Size(0xa8, 0x17);
            this.editClientButton.TabIndex = 5;
            this.editClientButton.Text = "Edit selected Tibia client";
            this.editClientButton.UseVisualStyleBackColor = true;
            this.editClientButton.Click += new EventHandler(this.EditClientButtonClick);
            this.addNewTibiaClientButton.Location = new Point(0x60, 0x12);
            this.addNewTibiaClientButton.Name = "addNewTibiaClientButton";
            this.addNewTibiaClientButton.Size = new Size(0xa8, 0x17);
            this.addNewTibiaClientButton.TabIndex = 4;
            this.addNewTibiaClientButton.Text = "Add new Tibia client";
            this.addNewTibiaClientButton.UseVisualStyleBackColor = true;
            this.addNewTibiaClientButton.Click += new EventHandler(this.AddNewTibiaClientButtonClick);
            this.clientPaths.FormattingEnabled = true;
            this.clientPaths.Location = new Point(15, 0x13);
            this.clientPaths.Name = "clientPaths";
            this.clientPaths.Size = new Size(0x48, 0x52);
            this.clientPaths.TabIndex = 1;
            this.clientPaths.SelectedIndexChanged += new EventHandler(this.ClientPathsSelectedIndexChanged);
            this.closeButton.Location = new Point(12, 280);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new Size(270, 0x17);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new EventHandler(this.CloseButtonClick);
            this.groupBox3.Controls.Add(this.graphicsEnginesComboBox);
            this.groupBox3.Controls.Add(this.forceGraphicsEngineCheckbox);
            this.groupBox3.Controls.Add(this.launchWithGamemasterFlagCheckbox);
            this.groupBox3.Controls.Add(this.storeClientConfigSeparateCheckbox);
            this.groupBox3.Location = new Point(12, 0x4d);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(270, 0x53);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Client flags";
            this.graphicsEnginesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.graphicsEnginesComboBox.FormattingEnabled = true;
            this.graphicsEnginesComboBox.ItemHeight = 13;
            this.graphicsEnginesComboBox.Items.AddRange(new object[] { "DirectX 5", "OpenGL", "DirectX 9" });
            this.graphicsEnginesComboBox.Location = new Point(0x95, 0x36);
            this.graphicsEnginesComboBox.Name = "graphicsEnginesComboBox";
            this.graphicsEnginesComboBox.Size = new Size(0x6d, 0x15);
            this.graphicsEnginesComboBox.TabIndex = 5;
            this.graphicsEnginesComboBox.SelectedIndexChanged += new EventHandler(this.GraphicsEnginesComboBoxSelectedIndexChanged);
            this.forceGraphicsEngineCheckbox.AutoSize = true;
            this.forceGraphicsEngineCheckbox.Location = new Point(15, 0x39);
            this.forceGraphicsEngineCheckbox.Name = "forceGraphicsEngineCheckbox";
            this.forceGraphicsEngineCheckbox.Size = new Size(0x86, 0x11);
            this.forceGraphicsEngineCheckbox.TabIndex = 4;
            this.forceGraphicsEngineCheckbox.Text = "Force graphics engine:";
            this.forceGraphicsEngineCheckbox.UseVisualStyleBackColor = true;
            this.forceGraphicsEngineCheckbox.CheckedChanged += new EventHandler(this.ForceGraphicsEngineCheckboxCheckedChanged);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x126, 0x137);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.closeButton);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.groupBox1);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FrmSettings";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.Manual;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            base.ResumeLayout(false);
        }

        private void LaunchWithGamemasterFlagCheckboxCheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.LaunchWithGamemasterFlag = this.launchWithGamemasterFlagCheckbox.Checked;
        }

        private void StartWithWindowsCheckboxCheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null)
            {
                if (this.startWithWindowsCheckbox.Checked)
                {
                    key.SetValue("OtLandIPChanger", '"' + Application.ExecutablePath + "\" /tray");
                    Config.Instance.StartWithWindows = true;
                }
                else
                {
                    key.DeleteValue("OtLandIPChanger", false);
                    Config.Instance.StartWithWindows = false;
                }
            }
        }

        private void StoreClientConfigSeparateCheckboxCheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.StoreClientConfigurationSeparate = this.storeClientConfigSeparateCheckbox.Checked;
        }

        public void UpdateClientPaths()
        {
            this.clientPaths.DataSource = this._main.Clients;
            this._main.UpdateVersions();
        }
    }
}


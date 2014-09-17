namespace OTLandIPChanger
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class FrmMain : Form
    {
        private SortedDictionary<string, string> _clientPaths = new SortedDictionary<string, string>();
        private bool _exitFromTray;
        private readonly bool _trayOnly;
        private Button applyButton;
        private ToolStripMenuItem applyToolStripMenuItem;
        private IContainer components;
        private ContextMenuStrip contextMenuStrip1;
        private Label label1;
        private Label label3;
        private NotifyIcon notifyIcon1;
        private Button settingsButton;
        public static SynchronizationContext SyncContext = SynchronizationContext.Current;
        private ToolStripMenuItem toolStripMenuItem1;
        private TextBox txtHostname;
        private ComboBox versionBox;

        public FrmMain(bool trayOnly)
        {
            ThreadPool.QueueUserWorkItem(_ => Updater.CheckForUpdate());
            this._trayOnly = trayOnly;
            this.InitializeComponent();
        }

        public void AddClient(string version, string path)
        {
            this._clientPaths.Add(version, path);
        }

        private void ApplyClick(object sender, EventArgs e)
        {
            Process processById = null;
            if (Config.Instance.AlwaysLaunchNewClient)
            {
                if (this.versionBox.SelectedValue != null)
                {
                    string version = this.versionBox.SelectedValue.ToString();
                    processById = Process.GetProcessById(TibiaClient.GetProcessId(TibiaClient.StartNewClient(this.GetClientPath(version), version)));
                }
                else
                {
                    MessageBox.Show("Please select a version.", "IP Changer");
                }
            }
            else
            {
                Func<Process, bool> predicate = null;
                IntPtr topWindow = TibiaClient.FindClient();
                if (topWindow == IntPtr.Zero)
                {
                    if (this.versionBox.SelectedValue == null)
                    {
                        MessageBox.Show("Tibia client not found.", "IP Changer");
                        return;
                    }
                    string str3 = this.versionBox.SelectedValue.ToString();
                    processById = Process.GetProcessById(TibiaClient.GetProcessId(TibiaClient.StartNewClient(this.GetClientPath(str3), str3)));
                }
                else
                {
                    if (predicate == null)
                    {
                        predicate = p => p.MainWindowHandle == topWindow;
                    }
                    processById = Process.GetProcesses().FirstOrDefault<Process>(predicate);
                    SetForegroundWindow(topWindow);
                }
            }
            if (processById != null)
            {
                ushort num;
                string[] strArray = this.txtHostname.Text.Split(new char[] { ':' }, 2);
                string hostname = strArray[0];
                if ((strArray.Length < 2) || !ushort.TryParse(strArray[1], out num))
                {
                    num = 0x1c03;
                }
                string str6 = hostname.ToLower();
                if ((!str6.Contains("aurera")))
                {
                    processById.WaitForInputIdle();
                    TibiaClient.TryPatchRSA(processById);
                    if (TibiaClient.TryReplaceHostname(processById, hostname, num) == 0)
                    {
                        TibiaClient.SetWindowText(processById.MainWindowHandle, string.Format("Tibia - {0}:{1}", hostname, num));
                    }
                }
            }
        }

        private void applyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ApplyClick(sender, e);
        }

        public bool AutoDetectClient()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + ((IntPtr.Size == 8) ? @"Wow6432Node\" : "") + @"Microsoft\Windows\CurrentVersion\Uninstall\Tibia_is1");
            if (key == null)
            {
                return false;
            }
            string str = (string) key.GetValue("DisplayVersion");
            string str2 = (string) key.GetValue("InstallLocation");
            if (string.IsNullOrEmpty(str2))
            {
                return false;
            }
            str2 = Path.Combine(str2, "Tibia.exe");
            if (this._clientPaths.ContainsKey(str))
            {
                return false;
            }
            this.AddClient(str, str2);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._exitFromTray && (e.CloseReason != CloseReason.ApplicationExitCall))
            {
                e.Cancel = true;
                base.Hide();
            }
            this.SaveConfiguration();
        }

        private void FrmMainLoad(object sender, EventArgs e)
        {
            if (this._trayOnly)
            {
                base.Visible = false;
                base.ShowInTaskbar = false;
            }
            this.txtHostname.Text = Config.Instance.TargetHostname;
            foreach (TibiaPathEntry entry in Config.Instance.ClientPaths)
            {
                this._clientPaths.Add(entry.Version, entry.Path);
            }
            Register("otserv", Application.ExecutablePath);
            bool flag = this.AutoDetectClient();
            if (this._clientPaths.Count != 0)
            {
                this.versionBox.DataSource = new BindingSource(this._clientPaths, null);
            }
            this.versionBox.DisplayMember = "Key";
            this.versionBox.ValueMember = "Key";
            this.versionBox.SelectedValue = Config.Instance.TargetVersion;
            if ((this.versionBox.SelectedValue == null) && (this._clientPaths.Count > 0))
            {
                this.versionBox.SelectedValue = this._clientPaths.First<KeyValuePair<string, string>>().Key;
            }
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null)
            {
                if ((key.GetValue("IPChanger") != null) && (key.GetValue("IPChanger").ToString() != ('"' + Application.ExecutablePath + "\" /tray")))
                {
                    key.SetValue("IPChanger", '"' + Application.ExecutablePath + "\" /tray");
                }
                Config.Instance.StartWithWindows = key.GetValue("IPChanger") != null;
            }
            if (flag)
            {
                this.SaveConfiguration();
            }
        }

        public string GetClientPath(string version)
        {
            string str;
            if (!this._clientPaths.TryGetValue(version, out str))
            {
                return "";
            }
            return str;
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(FrmMain));
            this.txtHostname = new TextBox();
            this.label1 = new Label();
            this.applyButton = new Button();
            this.settingsButton = new Button();
            this.label3 = new Label();
            this.versionBox = new ComboBox();
            this.notifyIcon1 = new NotifyIcon(this.components);
            this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            this.applyToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            base.SuspendLayout();
            this.txtHostname.Location = new Point(0x26, 7);
            this.txtHostname.Name = "txtHostname";
            this.txtHostname.Size = new Size(0xb9, 20);
            this.txtHostname.TabIndex = 0;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new Size(20, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP:";
            this.applyButton.Location = new Point(9, 0x21);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new Size(0xd6, 0x17);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new EventHandler(this.ApplyClick);
            this.settingsButton.Location = new Point(0xe8, 0x21);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new Size(0x69, 0x17);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.Text = "Settings";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new EventHandler(this.SettingsButtonClick);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0xe5, 10);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x2d, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Version:";
            this.versionBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.versionBox.FormattingEnabled = true;
            this.versionBox.Location = new Point(280, 6);
            this.versionBox.MaxDropDownItems = 100;
            this.versionBox.MaxLength = 4;
            this.versionBox.Name = "versionBox";
            this.versionBox.Size = new Size(0x38, 0x15);
            this.versionBox.TabIndex = 2;
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = (Icon) manager.GetObject("notifyIcon1.Icon");
            this.notifyIcon1.Text = "IP Changer";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new EventHandler(this.NotifyIcon1DoubleClick);
            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] { this.applyToolStripMenuItem, this.toolStripMenuItem1 });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new Size(0x6a, 0x30);
            this.applyToolStripMenuItem.Name = "applyToolStripMenuItem";
            this.applyToolStripMenuItem.Size = new Size(0x69, 0x16);
            this.applyToolStripMenuItem.Text = "Apply";
            this.applyToolStripMenuItem.Click += new EventHandler(this.applyToolStripMenuItem_Click);
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(0x69, 0x16);
            this.toolStripMenuItem1.Text = "Exit";
            this.toolStripMenuItem1.Click += new EventHandler(this.ToolStripMenuItem1Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x158, 0x3e);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.versionBox);
            base.Controls.Add(this.settingsButton);
            base.Controls.Add(this.applyButton);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.txtHostname);
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.Name = "FrmMain";
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "IP Changer";
            base.FormClosing += new FormClosingEventHandler(this.FrmMainFormClosing);
            base.Load += new EventHandler(this.FrmMainLoad);
            this.contextMenuStrip1.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void NotifyIcon1DoubleClick(object sender, EventArgs e)
        {
            if (!base.ShowInTaskbar)
            {
                base.ShowInTaskbar = true;
            }
            base.Show();
            base.WindowState = FormWindowState.Normal;
        }

        private static void Register(string protocol, string application)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Classes\\" + protocol, true) ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\" + protocol);
                if (key != null)
                {
                    key.SetValue("URL Protocol", "");
                }
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\" + protocol + @"\DefaultIcon", true) ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\" + protocol + @"\DefaultIcon");
                if (key != null)
                {
                    key.SetValue("", "\"" + application + "\",1");
                }
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\" + protocol + @"\shell\open\command", true) ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\" + protocol + @"\shell\open\command");
                if (key != null)
                {
                    key.SetValue("", "\"" + application + "\" \"/otserv=%1\"");
                }
                if (Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Wow6432Node\Classes") != null)
                {
                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Wow6432Node\Classes\" + protocol, true) ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Wow6432Node\Classes\" + protocol);
                    if (key != null)
                    {
                        key.SetValue("URL Protocol", "");
                    }
                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Wow6432Node\Classes\" + protocol + @"\DefaultIcon", true) ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Wow6432Node\Classes\" + protocol + @"\DefaultIcon");
                    if (key != null)
                    {
                        key.SetValue("", "\"" + application + "\",1");
                    }
                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Wow6432Node\Classes\" + protocol + @"\shell\open\command", true) ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Wow6432Node\Classes\" + protocol + @"\shell\open\command");
                    if (key != null)
                    {
                        key.SetValue("", "\"" + application + "\" \"/otserv=%1\"");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You do not have permission to make changes to the registry, make sure that you have administrative rights on this computer.", "IP Changer", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public void RemoveClient(string version)
        {
            this._clientPaths.Remove(version);
        }

        public void SaveConfiguration()
        {
            Config.Instance.TargetHostname = this.txtHostname.Text;
            if (this.versionBox.SelectedValue != null)
            {
                Config.Instance.TargetVersion = this.versionBox.SelectedValue.ToString();
            }
            Config.Instance.ClientPaths = (from entry in this._clientPaths select new TibiaPathEntry { Version = entry.Key, Path = entry.Value }).ToList<TibiaPathEntry>();
            Config.Instance.Save();
        }

        public void SetClient(string version, string path)
        {
            this._clientPaths[version] = path;
        }

        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private void SettingsButtonClick(object sender, EventArgs e)
        {
            FrmSettings settings = new FrmSettings(this);
            settings.SetDesktopLocation((base.Location.X + (base.Width / 2)) - (settings.Width / 2), (base.Location.Y + (base.Height / 2)) - (settings.Height / 2));
            settings.ShowDialog();
        }

        private void ToolStripMenuItem1Click(object sender, EventArgs e)
        {
            this._exitFromTray = true;
            base.Close();
        }

        public void UpdateVersions()
        {
            this.versionBox.DataSource = (this._clientPaths.Count != 0) ? new BindingSource(this._clientPaths, null) : null;
        }

        public List<string> Clients
        {
            get
            {
                return this._clientPaths.Keys.ToList<string>();
            }
        }

        private class TibiaClientEntry
        {
            public override string ToString()
            {
                return this.Process.MainWindowTitle;
            }

            public List<long> Addresses { get; set; }

            public System.Diagnostics.Process Process { private get; set; }
        }
    }
}


namespace OTLandIPChanger
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public static class Program
    {
        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        [STAThread]
        private static void Main(string[] args)
        {
            string str = args.FirstOrDefault<string>(x => x.StartsWith("/update="));
            if (str != null)
            {
                string[] strArray = str.Split(new char[] { '=' }, 2);
                if (strArray.Length >= 2)
                {
                    int num;
                    string[] strArray2 = strArray[1].Split(new char[] { ',' }, 2);
                    if ((strArray2.Length >= 2) && int.TryParse(strArray2[0], out num))
                    {
                        try
                        {
                            Process processById = Process.GetProcessById(num);
                            processById.WaitForExit(0x1388);
                            if (!processById.HasExited)
                            {
                                processById.Kill();
                            }
                        }
                        catch (ArgumentException)
                        {
                        }
                        catch (NotSupportedException)
                        {
                        }
                        Updater.FinalizeUpdate(strArray2[1]);
                    }
                }
            }
            else
            {
                str = args.FirstOrDefault<string>(x => x.StartsWith("/update2="));
                if (str != null)
                {
                    int num2;
                    string[] strArray3 = str.Split(new char[] { '=' }, 2);
                    if (strArray3.Length < 2)
                    {
                        return;
                    }
                    string[] strArray4 = strArray3[1].Split(new char[] { ',' }, 2);
                    if (strArray4.Length < 2)
                    {
                        return;
                    }
                    if (int.TryParse(strArray4[0], out num2))
                    {
                        try
                        {
                            Process process2 = Process.GetProcessById(num2);
                            process2.WaitForExit(0x1388);
                            if (!process2.HasExited)
                            {
                                process2.Kill();
                            }
                        }
                        catch (ArgumentException)
                        {
                        }
                        catch (NotSupportedException)
                        {
                        }
                        File.Delete(strArray4[1]);
                    }
                }
                try
                {
                    if (Config.Instance.UpdateRequired)
                    {
                        Config.Instance.Upgrade();
                        Config.Instance.UpdateRequired = false;
                    }
                }
                catch (ConfigurationErrorsException exception)
                {
                    File.Delete(((ConfigurationErrorsException) exception.InnerException).Filename);
                    MessageBox.Show("It appears that your configuration file was corrupted. It has been deleted and a new configuration file will be created the next time you launch OTLand IP Changer.", "OTLand IP Changer");
                    return;
                }
                string str3 = args.FirstOrDefault<string>(x => x.Contains("/otserv="));
                if (str3 == null)
                {
                    bool flag;
                    using (new Mutex(true, "OtLandIPChanger", out flag))
                    {
                        if (flag)
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new FrmMain((args.Length > 0) && (args[0] == "/tray")));
                        }
                        else
                        {
                            Process current = Process.GetCurrentProcess();
                            foreach (Process process4 in from process in Process.GetProcessesByName(current.ProcessName)
                                where process.Id != current.Id
                                select process)
                            {
                                SetForegroundWindow(process4.MainWindowHandle);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        string path;
                        Func<TibiaPathEntry, bool> predicate = null;
                        Func<TibiaPathEntry, bool> func2 = null;
                        Func<TibiaPathEntry, bool> func3 = null;
                        string[] strArray5 = str3.Replace("/otserv=otserv://", "").Split(new char[] { '/' });
                        string hostname = strArray5[0];
                        ushort port = ushort.Parse(strArray5[1]);
                        string version = strArray5[2];
                        try
                        {
                            if (predicate == null)
                            {
                                predicate = entry => entry.Version == version;
                            }
                            path = Config.Instance.ClientPaths.First<TibiaPathEntry>(predicate).Path;
                        }
                        catch (InvalidOperationException)
                        {
                            try
                            {
                                string tmpVersion = version.PadRight(3, '0');
                                path = Config.Instance.ClientPaths.First<TibiaPathEntry>(entry => (entry.Version == tmpVersion)).Path;
                            }
                            catch (InvalidOperationException)
                            {
                                try
                                {
                                    if (version.Contains("."))
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    version = version.Insert(1, ".");
                                    if (func2 == null)
                                    {
                                        func2 = entry => entry.Version == version;
                                    }
                                    path = Config.Instance.ClientPaths.First<TibiaPathEntry>(func2).Path;
                                }
                                catch (InvalidOperationException)
                                {
                                    try
                                    {
                                        version = version.PadRight(4, '0');
                                        if (func3 == null)
                                        {
                                            func3 = entry => entry.Version == version;
                                        }
                                        path = Config.Instance.ClientPaths.First<TibiaPathEntry>(func3).Path;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        path = null;
                                    }
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(path))
                        {
                            if (MessageBox.Show(string.Format("No path found for client {0}. Would you like to locate it?", version), "OtLand IP Changer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                            {
                                return;
                            }
                            OpenFileDialog dialog = new OpenFileDialog {
                                Title = "Select the executable for Tibia " + version,
                                Filter = "Tibia executable file|Tibia.exe"
                            };
                            if (dialog.ShowDialog() != DialogResult.OK)
                            {
                                return;
                            }
                            path = dialog.FileName;
                            if (!path.EndsWith(".exe"))
                            {
                                MessageBox.Show(string.Format("The client path must be an executable file.", new object[0]), "OtLand IP Changer");
                                return;
                            }
                            TibiaPathEntry item = new TibiaPathEntry {
                                Path = path,
                                Version = version
                            };
                            Config.Instance.ClientPaths.Add(item);
                            Config.Instance.Save();
                        }
                        Process process3 = Process.GetProcessById(TibiaClient.GetProcessId(TibiaClient.StartNewClient(path, version)));
                        process3.WaitForInputIdle();
                        TibiaClient.TryPatchRSA(process3);
                        if (TibiaClient.TryReplaceHostname(process3, hostname, port) == 0)
                        {
                            TibiaClient.SetWindowText(process3.MainWindowHandle, string.Format("Tibia - {0}:{1}", hostname, port));
                        }
                    }
                    catch (Exception exception2)
                    {
                        MessageBox.Show(exception2.Message, "Error");
                    }
                }
            }
        }

        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}


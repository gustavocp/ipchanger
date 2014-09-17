namespace OTLandIPChanger
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    internal static class Updater
    {
        private static WebClient _wc = new WebClient();

        public static void CheckForUpdate()
        {
            if (!Debugger.IsAttached)
            {
                string[] strArray;
                try
                {
                    strArray = _wc.DownloadString("http://reptera.net/ipchanger/update.php").Split(new char[] { ' ' }, 3);
                }
                catch (WebException)
                {
                    return;
                }
                string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
                byte[] publicKeyToken = Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken();
                string strA = strArray[0];
                string str3 = strArray[1];
                string address = strArray[2];
                if (string.Compare(strA, productVersion, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    string tempFileName = Path.GetTempFileName();
                    try
                    {
                        _wc.DownloadFile(address, tempFileName);
                    }
                    catch (WebException)
                    {
                        MessageBox.Show("Failed downloading update (1)!", "IP Changer");
                        return;
                    }
                    string strB = SHA1Hash(tempFileName);
                    if (string.Compare(str3, strB, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        MessageBox.Show("Failed downloading update (2)!", "IP Changer");
                    }
                    else
                    {
                        byte pfWasVerified = 0;
                        StrongNameSignatureVerificationEx(tempFileName, 1, ref pfWasVerified);
                        if (pfWasVerified == 0)
                        {
                            MessageBox.Show("Update signature verification failed! Please re-download the IP Changer.", "IP Changer", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else
                        {
                            byte[] second = Assembly.LoadFile(tempFileName).GetName().GetPublicKeyToken();
                            if (!publicKeyToken.SequenceEqual<byte>(second))
                            {
                                MessageBox.Show("Update signature verification failed (2)! Please re-download the IP Changer.", "IP Changer", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                            else
                            {
                                Process currentProcess = Process.GetCurrentProcess();
                                string str7 = string.Format("/update={0},\"{1}\"", currentProcess.Id, currentProcess.MainModule.FileName);
                                ProcessStartInfo startInfo = new ProcessStartInfo {
                                    FileName = tempFileName,
                                    Arguments = str7,
                                    UseShellExecute = false
                                };
                                Process.Start(startInfo);
                                FrmMain.SyncContext.Send(_ => Application.Exit(), null);
                            }
                        }
                    }
                }
            }
        }

        public static void FinalizeUpdate(string filename)
        {
            string location = Assembly.GetExecutingAssembly().Location;
            try
            {
                System.IO.File.Copy(location, filename, true);
                Process.Start(filename, string.Format("/update2={0},\"{1}\"", Process.GetCurrentProcess().Id, location));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Updating failed! " + exception.Message, "IP Changer", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private static string SHA1Hash(string path)
        {
            string str;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (SHA1Managed managed = new SHA1Managed())
                {
                    byte[] buffer = managed.ComputeHash(stream);
                    StringBuilder builder = new StringBuilder(2 * buffer.Length);
                    foreach (byte num in buffer)
                    {
                        builder.AppendFormat("{0:x2}", num);
                    }
                    str = builder.ToString();
                }
            }
            return str;
        }

        [DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
        private static extern bool StrongNameSignatureVerificationEx(string wszFilePath, byte fForceVerification, ref byte pfWasVerified);
    }
}


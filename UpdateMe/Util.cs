using NLog.Fluent;
using Sandbox.Game.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.API.WebAPI;
using Torch.Managers;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Globalization;

namespace UpdateMe
{
    public class Util
    {
        private static IChatManagerServer chatManager;
        

        public Util()
        {

        }

        public static void SendMessage(string msg)
        {
            IChatManagerServer manager = null;
            if (Util.chatManager == null)
            {
                ITorchSession currentSession = UpdateMePlugin.Instance.Torch.CurrentSession;
                if (currentSession != null)
                {
                    IDependencyManager managers = currentSession.Managers;
                    if (managers != null)
                    {
                        manager = DependencyProviderExtensions.GetManager<IChatManagerServer>(managers);
                    }
                }

                Util.chatManager = manager;
            }
            try
            {
                //Util.chatManager.DisplayMessageOnSelf("Server", msg);
                Util.chatManager.SendMessageAsSelf(msg);
            }
            catch(Exception ex)
            {
                Util.Log(string.Concat("UpdateMe catched an exception!", ex, ex.StackTrace));
            }
        }

        public static void Log(string msg)
        {
            if (UpdateMePlugin.Instance.Config.LogEnabled)
            {
                UpdateMePlugin.Log.Info(msg);
            }
        }

        public struct Message
        {
            public ulong steamId;

            public Util.MessageType type;
        }

        public enum MessageType
        {
            none,
            command,
            evento
        }

        public static async void checkTorchUpdatesAsync()
        {
            if (UpdateMePlugin.Instance.Torch.Config.NoUpdate || !UpdateMePlugin.Instance.Torch.Config.GetTorchUpdates)
                return;

            try
            {
                if (!UpdateMePlugin.Instance.myTimer.Enabled && UpdateMePlugin.Instance.Config.RestartForNewTorchVersion)
                {
                    var job = await JenkinsQuery.Instance.GetLatestVersion(UpdateMePlugin.Instance.Torch.TorchVersion.Branch);
                    if (job == null)
                    {
                        Util.Log("UpdateMe had failed to fetch latest Torch version.");
                        return;
                    }

                    if (job.Version > UpdateMePlugin.Instance.Torch.TorchVersion)
                    {
                        Util.SendMessage(UpdateMePlugin.Instance.Config.MessageForNewTorchVersion);
                        Util.Log(UpdateMePlugin.Instance.Config.MessageForNewTorchVersion);
                        UpdateMePlugin.Instance.setUpAndStartTimers();

                        
                    }
                }
            }
            catch (Exception ex)
            {
                Util.Log(string.Concat("UpdateMe catched an exception!", ex, ex.StackTrace));
            }

        }

        [Obsolete("checkDSUpdates is deprecated, use checkDSUpdatesByDate instead.")]
        public static bool checkDSUpdates()
        {
            bool exit = false;
            try
            {
                if (!UpdateMePlugin.Instance.myTimer.Enabled && UpdateMePlugin.Instance.Config.RestartForNewDSVersion)
                {
                    #region steamcmd

                    string STEAMCMD_DIR = "steamcmd";
                    string STEAMCMD_PATH = $"{STEAMCMD_DIR}\\steamcmd.exe";
                    string RUNSCRIPT_PATH = $"{STEAMCMD_DIR}\\updateme.txt";
                    string RUNSCRIPT = @"force_install_dir ../updatemeDS
login anonymous
app_update 298740
quit";

                    if (Directory.Exists(STEAMCMD_DIR))
                    {
                        if (!File.Exists(RUNSCRIPT_PATH))
                            File.WriteAllText(RUNSCRIPT_PATH, RUNSCRIPT);

                        if (File.Exists(STEAMCMD_PATH))
                        {
                            //UpdateMePlugin.Instance.GameVersion
                            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                            {
                                p.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), STEAMCMD_PATH);
                                p.StartInfo.Arguments = "+runscript updateme.txt";
                                //p.StartInfo.CreateNoWindow = true;
                                //p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                p.Start();
                                p.WaitForExit();
                            }

                        }
                    }

                    #endregion

                    //Warning: If you have custom/injected dlls in DedicatedServer64 the pluging is going to indicate that a new version of DS is available when it may really not! 
                    var dsPath = Directory.GetCurrentDirectory() + "DedicatedServer64";
                    //var updatmeDSPath = Directory.GetCurrentDirectory() + "updatemeDS";
                    var newVersionFile = "";

                    foreach (string inputFile in Directory.EnumerateFiles(dsPath, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        newVersionFile = inputFile.Replace("DedicatedServer64", @"updatemeDS\DedicatedServer64");
                        if (File.Exists(newVersionFile))
                        {
                            if (filesAreDifferent(inputFile, newVersionFile))
                            {
                                Util.SendMessage(UpdateMePlugin.Instance.Config.MessageForNewDSVersion);
                                UpdateMePlugin.Instance.setUpAndStartTimers();
                                exit = true;
                                break;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Util.Log(string.Concat("UpdateMe catched an exception!", ex, ex.StackTrace));
            }
            return exit;
        }

        public static void checkTorchStatus()
        {
            //Torch.API.ServerState.Error
            //Util.Log("GameVersion_BUILDID: " + UpdateMePlugin.Instance.GameVersion.Build);
            Util.SendMessage("GameVersion_BUILDID: " + UpdateMePlugin.Instance.GameVersion.Build);
        }

        private static bool filesAreDifferent(string file1, string file2)
        {
            return new FileInfo(file1).Length != new FileInfo(file2).Length ||
                        !File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2));
        }

        public static bool checkDSUpdatesByDate()
        {
            bool exit = false;
            try
            {
                if (!UpdateMePlugin.Instance.myTimer.Enabled && UpdateMePlugin.Instance.Config.RestartForNewDSVersion)
                {
                    string STEAMCMD_DIR = Path.Combine(Directory.GetCurrentDirectory(), "steamcmd");

                    //Util.Log("checkDSUpdates");
                    if (Directory.Exists(STEAMCMD_DIR))
                    {
                        //UpdateMePlugin.Instance.GameVersion
                        //DateTime updateDate;
                        string lastRemoteDate = "";

                        using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                        {
                            p.StartInfo.FileName = Path.Combine(STEAMCMD_DIR, "steamcmd.exe");
                            p.StartInfo.Arguments = "+login anonymous +app_info_update 1 +app_info_print 298740 +quit";
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardError = true;
                            p.Start();
                            string output = p.StandardOutput.ReadToEnd();
                            p.WaitForExit();

                            string dateStart = "last change :", dateEnd = "\r\n";
                            output = output.Substring(output.IndexOf(dateStart)).Substring(0, output.IndexOf(dateEnd));
                            lastRemoteDate = output.Replace(dateStart, "").Replace(dateEnd, "").Replace("\"", "").TrimStart().TrimEnd();
                            //updateDate = DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                        }

                        string ctrlFile = Path.Combine(STEAMCMD_DIR, "updatemeds.txt");
                        
                        if (!File.Exists(ctrlFile))
                            File.WriteAllText(ctrlFile, lastRemoteDate);
                        else
                        {
                            string lastLocalDate = "";
                            using (StreamReader sr = File.OpenText(ctrlFile))
                            {
                                lastLocalDate = sr.ReadLine().TrimStart().TrimEnd();
                            }

                            if (lastLocalDate != lastRemoteDate)
                            {
                                File.Delete(ctrlFile);
                                Util.SendMessage(UpdateMePlugin.Instance.Config.MessageForNewDSVersion);
                                Util.Log(UpdateMePlugin.Instance.Config.MessageForNewDSVersion);
                                UpdateMePlugin.Instance.setUpAndStartTimers();
                                exit = true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Util.Log(string.Concat("UpdateMe catched an exception!", ex, ex.StackTrace));
            }
            return exit; 
        }

        public static void setUpAndRunGuardian()
        {
            //Check here if UpdateMeGuardian.exe process is running already.
            if (true)
            {
                //Check if UpdateMeGuardian.exe exists in Torch work directory. If not, unzip it and extract it there.

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "UpdateMeGuardian.exe");
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.Start();
                    p.WaitForExit();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Test
{
    class Program
    {
        public static Timer myTimer, msgTimer;
        public static Stopwatch stopWatch;

        static void Main(string[] args)
        {
            checkDSUpdates();
            //autoUpdateDS();
        }

        static void autoUpdateDS()
        {
            try
            {
                string STEAMCMD_DIR = @"C:\Torch\"+"steamcmd";
                string STEAMCMD_PATH = $"{STEAMCMD_DIR}\\steamcmd.exe";
                string RUNSCRIPT_PATH = $"{STEAMCMD_DIR}\\updateme.txt";
                string RUNSCRIPT = @"force_install_dir ../
login anonymous
app_info_print 298740
quit";

                if (Directory.Exists(STEAMCMD_DIR))
                {
                    if (!File.Exists(RUNSCRIPT_PATH))
                        File.WriteAllText(RUNSCRIPT_PATH, RUNSCRIPT);

                    if (File.Exists(STEAMCMD_PATH))
                    {
                        //UpdateMePlugin.Instance.GameVersion
                        //UpdateMePlugin.Instance.Torch.CurrentSession.KeenSession.AppVersionFromSave;
                        //UpdateMePlugin.Instance.Torch.CurrentSession.KeenSession.GameDefinition.

                        var steamCmdProc = new ProcessStartInfo(STEAMCMD_PATH, "+runscript updateme.txt")
                        {
                            WorkingDirectory = STEAMCMD_DIR,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            StandardOutputEncoding = Encoding.ASCII
                        };
                        var cmd = Process.Start(steamCmdProc);

                        while (!cmd.HasExited)
                        {
                            //Util.Log(cmd.StandardOutput.ReadToEnd());
                            Thread.Sleep(100);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                //Util.Log(string.Concat("UpdateMe plugin catched an exception!", ex, ex.StackTrace));
            }
        }

        private static void msgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            double timeDiff = (2 * 60) - stopWatch.Elapsed.Seconds;

            if (timeDiff >= 60)
            {
                
                timeDiff = Math.Round(timeDiff / 60, 2);
            }
            else
            {
                timeDiff = Math.Round(timeDiff, 0);
            }
            
        }

        public static bool checkDSUpdates()
        {
            bool exit = false;
            try
            {
                if (true)
                {
                    /*
                    myTimer = new Timer();
                    myTimer.Enabled = false;

                    stopWatch = new Stopwatch();
                    stopWatch.Start();

                    msgTimer = new Timer(1000 * 10);
                    msgTimer.Elapsed += msgTimer_Elapsed;
                    msgTimer.Start();

                    while (true) ;
                    */
                    #region steamcmd

                    string STEAMCMD_DIR = @"C:\Torch\" + "steamcmd";
                    string STEAMCMD_PATH = $"{STEAMCMD_DIR}\\steamcmd.exe";
                    string RUNSCRIPT_PATH = $"{STEAMCMD_DIR}\\updateme.txt";
                    /*
                    string RUNSCRIPT = @"force_install_dir ../updatemeDS
                                    login anonymous
                                    app_update 298740
                                    quit";
                    */
                    string RUNSCRIPT = @"login anonymous
app_info_update 1 
app_info_print 298740";

                    if (Directory.Exists(STEAMCMD_DIR))
                    {

                        if (File.Exists(STEAMCMD_PATH))
                        {
                            string lastRemoteInfo = "";
                            //UpdateMePlugin.Instance.GameVersion
                            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                            {
                                p.StartInfo.FileName = Path.Combine(@"C:\Torch\", STEAMCMD_PATH);
                                p.StartInfo.Arguments = "+login anonymous +app_info_update 1 +app_info_print 298740 +quit";//"+runscript updateme.txt";
                                p.StartInfo.CreateNoWindow = true;
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.RedirectStandardError = true;
                                p.Start();
                                string output = p.StandardOutput.ReadToEnd();
                                p.WaitForExit();

                                string infoStart = "change number :", infoEnd = ",";
                                output = output.Substring(output.IndexOf(infoStart));
                                output = output.Substring(0, output.IndexOf(infoEnd));
                                lastRemoteInfo = output.Replace(infoStart, "").Replace(infoEnd, "").Replace("\"", "").TrimStart().TrimEnd();

                                //DateTime dateTime = DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                                //" Thu Apr 16 14:06:37 2020 "
                                //p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            }

                            string ctrlFile = Path.Combine(STEAMCMD_DIR, "updatemeds.txt");

                            if (!File.Exists(ctrlFile))
                                File.WriteAllText(ctrlFile, lastRemoteInfo);
                            else
                            {
                                string lastLocalInfo = "";
                                using (StreamReader sr = File.OpenText(ctrlFile))
                                {
                                    lastLocalInfo = sr.ReadLine().TrimStart().TrimEnd();
                                }

                                if (lastLocalInfo != lastRemoteInfo)
                                {
                                    File.Delete(ctrlFile);
                                    //Util.SendMessage(UpdateMePlugin.Instance.Config.MessageForNewDSVersion);
                                    //Util.Log(UpdateMePlugin.Instance.Config.MessageForNewDSVersion);
                                    //UpdateMePlugin.Instance.setUpAndStartTimers();
                                    exit = true;
                                }
                            }

                        }
                    }

                    #endregion

                    //Warning: If you have custom/injected dlls in DedicatedServer64 the pluging is going to indicate that a new version of DS is available when it may really not! 
                    var dsPath = @"C:\Torch\" + "DedicatedServer64";
                    //var updatmeDSPath = Directory.GetCurrentDirectory() + "updatemeDS";
                    var newVersionFile = "";

                    foreach (string inputFile in Directory.EnumerateFiles(dsPath, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        newVersionFile = inputFile.Replace("DedicatedServer64", @"updatemeDS\DedicatedServer64");
                        if (File.Exists(newVersionFile))
                        {
                            if (filesAreDifferent(inputFile, newVersionFile))
                            {
                                //Util.SendMessage(UpdateMePlugin.Instance.Config.MessageForNewDSVersion);
                                //Util.SendMessage("Server will restart in " + UpdateMePlugin.Instance.Config.RestartInForDSVersion + " seconds.");
                                //UpdateMePlugin.Instance.myTimer = new Timer(UpdateMePlugin.Instance.Config.RestartInForDSVersion * 1000);
                                //UpdateMePlugin.Instance.myTimer.Start();
                                exit = true;
                                break;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //Util.Log(string.Concat("UpdateMe catched an exception!", ex, ex.StackTrace));
            }
            return exit;
        }

        private static bool filesAreDifferent(string file1, string file2)
        {
            return new FileInfo(file1).Length != new FileInfo(file2).Length ||
                        !File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2));
        }


    }
}

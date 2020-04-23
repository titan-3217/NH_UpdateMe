using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateMeGuardian
{
    class Program
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        static string _guardianPath, _torchAssembly, _tmpDirectory;

        static void Main(string[] args)
        {       
            _guardianPath = Path.Combine(Directory.GetCurrentDirectory(), "UpdateMeGuardian.exe");
            _torchAssembly = Path.Combine(Directory.GetCurrentDirectory(), "Torch.dll");
            _tmpDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tmp");

            while (true)
            {
                try
                {
                    checkScheduledTask();

                    if (torchIsBroken())
                    {
                        Log.Warn("UpdateMeGuardian: Torch is broken and requires a forced manual instalation, working on it...");
                        ForceTorchManualUpdate();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "UpdateMeGuardian");
                }
                finally
                {
                    Thread.Sleep(10000);
                }
            }
        }

        /// <summary>
        /// There are a bunch of scenarios when a new DS version brakes Torch so a manual installation will be requeried.
        /// Don't rely on any Torch component here, casue it may be broken, it uses the same implementation that Torch uses.
        /// </summary>
        static async void ForceTorchManualUpdate()
        {
            InformationalVersion torchVersion;
            var versionString = Assembly.LoadFrom(_torchAssembly)
                                      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                      .InformationalVersion;

            if (!InformationalVersion.TryParse(versionString, out torchVersion))
                throw new TypeLoadException("UpdateMeGuardian: Unable to parse the Torch version from the assembly.");

            var torchLatestVersion = await JenkinsQuery.Instance.GetLatestVersion(torchVersion.Branch);
            if (torchLatestVersion == null)
            {
                Log.Warn("UpdateMeGuardian: Failed to fetch latest version.");
                return;
            }

            if (torchLatestVersion.Version > torchVersion)
            {
                //Util.Log($"Updating Torch from version {UpdateMePlugin.Instance.Torch.TorchVersion} to version {job.Version}");
                var updateName = Path.Combine(_tmpDirectory, "torchupdate.zip");
                if (!await JenkinsQuery.Instance.DownloadRelease(torchLatestVersion, updateName))
                {
                    Log.Warn("UpdateMeGuardian: Failed to download new release!");
                    return;
                }
                UpdateFromZip(updateName, Directory.GetCurrentDirectory());
                File.Delete(updateName);
                Log.Info($"UpdateMeGuardian: Torch version {torchLatestVersion.Version} has been installed.");
            }

            
        }

        static void UpdateFromZip(string zipFile, string extractPath)
        {
            using (var zip = ZipFile.OpenRead(zipFile))
            {
                foreach (var file in zip.Entries)
                {
                    if (file.Name == "NLog-user.config" && File.Exists(Path.Combine(extractPath, file.FullName)))
                        continue;

                    Log.Info($"UpdateMeGuardian: Unzipping {file.FullName}");
                    var targetFile = Path.Combine(extractPath, file.FullName);
                    SoftDelete(extractPath, file.FullName);
                    file.ExtractToFile(targetFile, true);
                }

                //zip.ExtractToDirectory(extractPath); //throws exceptions sometimes?
            }
        }

        /// <summary>
        /// Move the given file (if it exists) to a temporary directory that will be cleared the next time Torch starts.
        /// </summary>
        static void SoftDelete(string path, string file)
        {
            string source = Path.Combine(path, file);
            if (!File.Exists(source))
                return;
            var rand = Path.GetRandomFileName();
            var dest = Path.Combine(_tmpDirectory, rand);
            File.Move(source, rand);
            string rsource = Path.Combine(path, rand);
            File.Move(rsource, dest);
        }

        /// <summary>
        /// Check if the Task is scheduled in Windows Tasks. If not it creates it.
        /// </summary>
        static void checkScheduledTask()
        {
            //Extract the exe file from plugins zip in Torch working folder in the plugin code, not here.
            string command = "schtasks /query /TN UpdateMeGuardian > NUL 2>&1 || schtasks /create /TN UpdateMeGuardian /IT /SC ONLOGON /TR " + _guardianPath + " /RU SYSTEM";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = @"%systemroot%\system32\cmd.exe";
                p.StartInfo.Arguments = command;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                p.WaitForExit();
            }

            Thread.Sleep(3000);
            //command = "schtasks /run /tn UpdateMeGuardian";
        }

        static bool torchIsBroken()
        {
            bool exit = false;



            return exit;
        }
    }
}

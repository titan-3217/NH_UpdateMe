using NLog;
using Sandbox.Game;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Torch;
using Torch.API;
using Torch.API.Plugins;
using VRage.Utils;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UpdateMe
{
    public class UpdateMePlugin : TorchPluginBase, ITorchPlugin, IDisposable
    {
        public MainLogic _logic;

        private Persistent<UpdateMePluginConfig> _config;

        public readonly static Logger Log;

        private bool _init;

        public Version GameVersion { get; private set; }
        public Timer myTimer, msgTimer;
        public System.Diagnostics.Stopwatch stopWatch;

        public UpdateMePluginConfig Config
        {
            get
            {
                Persistent<UpdateMePluginConfig> persistent = this._config;
                if (persistent != null)
                {
                    return persistent.Data;
                }
                return null;
            }
        }

        public static UpdateMePlugin Instance
        {
            get;
            private set;
        }

        static UpdateMePlugin()
        {
            UpdateMePlugin.Log = LogManager.GetLogger("UpdateMe");
        }

        public UpdateMePlugin()
        {
        }

        public override void Dispose()
        {
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            UpdateMePlugin.Log.Warn("Loading UpdateMe config.");

            bool initializeConfig = !File.Exists(Path.Combine(base.StoragePath, "UpdateMe.cfg"));

            this._config = Persistent<UpdateMePluginConfig>.Load(Path.Combine(base.StoragePath, "UpdateMe.cfg"), true);

            if (initializeConfig)
            {
                this._config.Data.Enabled = true;
                this._config.Data.LogEnabled = true;
                this._config.Data.RestartForNewTorchVersion = true;
                this._config.Data.MessageForNewTorchVersion = "A new Torch version has been found!";
                this._config.Data.RestartForNewDSVersion = true;
                this._config.Data.MessageForNewDSVersion = "A new Space Engineers DS version has been found!";
                this._config.Data.RestartInMinutes = 2;
                this._config.Data.CheckFrequencyInMinutes = 5;
            }

            this._config.Save();

            this._config.Data.SetLoaded(this);
            UpdateMePlugin.Log.Debug("UpdateMe config loaded.");
            /*
            try
            {
                GameVersion = new MyVersion(MyPerGameSettings.BasicGameInfo.GameVersion.Value);
            }
            catch (Exception ex)
            {
                Util.Log(string.Concat("UpdateMe catched an exception! - MyVersion", ex, ex.StackTrace));
            }
            */
            myTimer = new Timer();
            myTimer.Enabled = false;

            UpdateMePlugin.Instance = this;
        }

        private void Initialize()
        {
            UpdateMePlugin.Instance = this;
            this._init = true;

            if (!this._config.Data.Enabled)
            {
                UpdateMePlugin.Log.Warn("UpdateMe is disabled!");
                return;
            }

            UpdateMePlugin.Log.Warn("UpdateMe initialized.");
            this._logic = new MainLogic();
        }

        public void Save()
        {
            try
            {
                this._config.Save();
                UpdateMePlugin.Log.Info("Configuration Saved.");
            }
            catch (Exception ex)
            {
                UpdateMePlugin.Log.Warn("Configuration failed to save!");
            }
        }

        public override void Update()
        {
            if (MyAPIGateway.Session == null)
            {
                return;
            }

            if (!this._init)
            {
                this.Initialize();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Util.SendMessage("Restaring server now!");
            Util.Log("Restaring server now!");
            //Torch should save the game before restart. Adding a few seconds warning before restart...
            //Util.SendMessage("!restart 15");
            Thread.Sleep(3000);
            UpdateMePlugin.Instance.Torch.Restart(true);
        }

        private void msgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan timeDiff = new TimeSpan(0, Config.RestartInMinutes, 0) - stopWatch.Elapsed;
            //Util.Log("timeDiff seconds: " + timeDiff.TotalSeconds);

            if (timeDiff.TotalSeconds >= 60)
            {
                double timeDiff2 = Math.Round(timeDiff.TotalSeconds / 60, 2);
                Util.SendMessage("Server will restart in " + timeDiff2 + " minutes.");
                Util.Log("Server will restart in " + timeDiff2 + " minutes.");
            }
            else if (timeDiff.TotalSeconds > 0)
            {
                double timeDiff2 = Math.Round(timeDiff.TotalSeconds, 0);
                Util.SendMessage("Server will restart in " + timeDiff2 + " seconds.");
                Util.Log("Server will restart in " + timeDiff2 + " seconds.");
            }
            else
            {
                //Trying to restart just one more time.
                Util.SendMessage("Warning: Server is waiting for an inminent restart!");
                Util.Log("Warning: Server is waiting for an inminent restart!");
                stopWatch.Stop();
                msgTimer.Stop();
                Timer_Elapsed(null, null);
            }
        }

        public void setUpAndStartTimers()
        {
            Util.SendMessage("Server will restart in " + Config.RestartInMinutes + " minutes.");
            Util.Log("Server will restart in " + Config.RestartInMinutes + " minutes.");
            stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            msgTimer = new Timer(1000 * 50);
            msgTimer.Elapsed += msgTimer_Elapsed;
            msgTimer.Start();

            myTimer = new Timer(Config.RestartInMinutes * 60 * 1000);
            myTimer.Elapsed += Timer_Elapsed;
            myTimer.Start();
        }

    }
}

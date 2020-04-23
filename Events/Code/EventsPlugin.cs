using Events.Forms;
using NLog;
using Sandbox.ModAPI;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Plugins;

namespace Events.Code
{
    public class EventsPlugin : TorchPluginBase, IWpfPlugin, ITorchPlugin, IDisposable
    {
        public MainLogic _logic;

        private UserControl _control;

        private Persistent<EventsPluginConfig> _config;

        public readonly static Logger Log;

        private bool _init;

        public EventsPluginConfig Config
        {
            get
            {
                Persistent<EventsPluginConfig> persistent = this._config;
                if (persistent != null)
                {
                    return persistent.Data;
                }
                return null;
            }
        }

        public static EventsPlugin Instance
        {
            get;
            private set;
        }

        static EventsPlugin()
        {
            EventsPlugin.Log = LogManager.GetLogger("Events");
        }

        public EventsPlugin()
        {
        }

        public override void Dispose()
        {
        }

        public UserControl GetControl()
        {
            UserControl userControl = this._control;
            if (userControl == null)
            {
                Settings eventsSettings = new Settings(this)
                {
                    DataContext = this.Config,
                    /*IsEnabled = false*/
                };
                UserControl userControl1 = eventsSettings;
                this._control = eventsSettings;
                userControl = userControl1;
            }
            return userControl; 
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            EventsPlugin.Log.Warn("Loading EventsPlugin config.");

            

            //if (File.Exists(Path.Combine(base.StoragePath, "Events.cfg")))
            
            this._config = Persistent<EventsPluginConfig>.Load(Path.Combine(base.StoragePath, "Events.cfg"), true);
            this._config.Save();
           
            this._config.Data.SetLoaded(this);
            EventsPlugin.Log.Debug("EventsPlugin config loaded.");

            EventsPlugin.Instance = this;
        }

        private void Initialize()
        {
            EventsPlugin.Instance = this;
            this._init = true;
            UserControl userControl = this._control;

            if (userControl != null)
            {
                userControl.Dispatcher.Invoke(() =>
                {
                    this._control.IsEnabled = true;
                    this._control.DataContext = this.Config;
                });
            }

            if (!this._config.Data.Enabled)
            {
                EventsPlugin.Log.Warn("Events plugin is disabled!");
                return;
            }

            EventsPlugin.Log.Warn("Events plugin initialized.");
            this._logic = new MainLogic();
        }

        public void Save()
        {
            try
            {
                this._config.Save();
                EventsPlugin.Log.Info("Configuration Saved.");
            }
            catch (Exception ex)
            {
                EventsPlugin.Log.Warn("Configuration failed to save!");
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

    }
}

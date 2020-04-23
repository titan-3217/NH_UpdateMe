using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Torch;

namespace Events.Code
{
    public class EventConfig : ViewModel
    {
        private string _event;
        private string _description;
        private string _scriptFile;
        private bool _enable;
        private Dictionary<string, string> _runAs;

        private EventsPlugin _core;

        public string Event
        {
            get
            {
                return this._event;
            }
            /*
            set
            {
                this._command = value;
                this.OnPropertyChanged("Command");
            }*/
        }

        public bool Enable
        {
            get
            {
                return this._enable;
            }
            set
            {
                this._enable = value;
                //this.OnPropertyChanged("Enabled");
                this.SetValue<bool>(ref this._enable, value, "Enable");
            }
        }

        public Dictionary<string, string> RunAs
        {
            get
            {
                return this._runAs;
            }
            /*
            set
            {
                this._inGameCmd = value;
                //this.OnPropertyChanged("Enabled");
                this.SetValue<bool>(ref this._inGameCmd, value, "InGameCommand");
            }*/
        }

        public string ScriptFile
        {
            get
            {
                return this._scriptFile;
            }

            set
            {
                this._scriptFile = value;
                this.OnPropertyChanged("ScriptFile");
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }

            set
            {
                this._description = value;
                this.OnPropertyChanged("Description");
            }
        }

        private Dictionary<string, string> getNewRunAsList()
        {
            Dictionary<string, string> runAs = new Dictionary<string, string>();
            runAs.Add("process", "New process");
            runAs.Add("command", "Torch/Mod cmd");
            return runAs;
        }

        public EventConfig()
        {
            this._runAs = getNewRunAsList();
        }

        public EventConfig(string _event, string desc, string scriptFile, bool enable)
        {
            this._event = _event;
            this._description = desc;
            this._scriptFile = scriptFile;
            this._enable = enable;
            this._runAs = getNewRunAsList();
        }

        public void Initialize(EventsPlugin core)
        {
            this._core = core;

        }
    }
}

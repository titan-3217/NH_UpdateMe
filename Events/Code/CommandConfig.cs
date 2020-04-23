using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;


namespace Events.Code
{
    public class CommandConfig : ViewModel
    {
        private string _command;
        private string _description;
        private int _cooldown = 5; //In seconds.
        private bool _playerCanUseIt;

        private EventsPlugin _core;

        public string Command
        {
            get
            {
                return this._command;
            }
            /*
            set
            {
                this._command = value;
                this.OnPropertyChanged("Command");
            }*/
        }

        public int Cooldown
        {
            get
            {
                return this._cooldown;
            }
            set
            {
                this._cooldown = value;
                //this.OnPropertyChanged("Enabled");
                this.SetValue<int>(ref this._cooldown, value, "Cooldown");
            }
        }

        public bool PlayerCanUseIt
        {
            get
            {
                return this._playerCanUseIt;
            }
            set
            {
                this._playerCanUseIt = value;
                //this.OnPropertyChanged("Enabled");
                this.SetValue<bool>(ref this._playerCanUseIt, value, "PlayerCanUseIt");
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

        public CommandConfig()
        {
        }

        public CommandConfig(string cmd, string desc, int cooldown, bool playerCanIseIt)
        {
            this._command = cmd;
            this._description = desc;
            this._cooldown = cooldown;
            this._playerCanUseIt = playerCanIseIt;
        }

        public void Initialize(EventsPlugin core)
        {
            this._core = core;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Torch;

namespace UpdateMe
{
    public class UpdateMePluginConfig : ViewModel
    {
        [XmlIgnore]
        public int cmdCooldown = 60; //In seconds.

        private int restartInMinutes = 2; // In minutes.
        private int checkFrequencyInMinutes = 5; // In minutes.

        private bool _enable;
        private bool _logenabled;
        private bool _restartForNewTorchVersion;
        private bool _restartForNewDSVersion;

        private string _messageForNewTorchVersion;
        private string _messageForNewDSVersion;

        public bool Enabled
        {
            get
            {
                return this._enable;
            }
            set
            {
                this.SetValue<bool>(ref this._enable, value, "Enabled");
            }
        }

        public bool LogEnabled
        {
            get
            {
                return this._logenabled;
            }
            set
            {
                this.SetValue<bool>(ref this._logenabled, value, "LogEnabled");
            }
        }

        public bool RestartForNewTorchVersion
        {
            get
            {
                return this._restartForNewTorchVersion;
            }
            set
            {
                this.SetValue<bool>(ref this._restartForNewTorchVersion, value, "RestartForNewTorchVersion");
            }
        }

        

        public string MessageForNewTorchVersion
        {
            get
            {
                return this._messageForNewTorchVersion;
            }
            set
            {
                this.SetValue<string>(ref this._messageForNewTorchVersion, value, "MessageForNewTorchVersion");
            }
        }

        public bool RestartForNewDSVersion
        {
            get
            {
                return this._restartForNewDSVersion;
            }
            set
            {
                this.SetValue<bool>(ref this._restartForNewDSVersion, value, "RestartForNewDSVersion");
            }
        }

        public string MessageForNewDSVersion
        {
            get
            {
                return this._messageForNewDSVersion;
            }
            set
            {
                this.SetValue<string>(ref this._messageForNewDSVersion, value, "MessageForNewDSVersion");
            }
        }

        public int RestartInMinutes
        {
            get
            {
                return this.restartInMinutes;
            }
            set
            {
                this.SetValue<int>(ref this.restartInMinutes, value, "RestartInMinutes");
            }
        }

        public int CheckFrequencyInMinutes
        {
            get
            {
                return this.checkFrequencyInMinutes;
            }
            set
            {
                this.SetValue<int>(ref this.checkFrequencyInMinutes, value, "CheckFrequencyInMinutes");
            }
        }

        public UpdateMePluginConfig()
        {
        }

        public void SetLoaded(UpdateMePlugin core)
        {

        }
    }
}

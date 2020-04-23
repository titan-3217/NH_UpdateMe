using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Torch;

namespace Events.Code
{
    public class EventsPluginConfig : ViewModel
    {
        [XmlIgnore]
        public int cmdCooldown = 5; //In seconds.

        private bool _enable;
        private bool _logenabled;
        

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

        public ObservableCollection<CommandConfig> Commands { get; } = new ObservableCollection<CommandConfig>();
        public ObservableCollection<EventConfig> Events { get; } = new ObservableCollection<EventConfig>();

        public EventsPluginConfig()
        {
        }

        public void SetLoaded(EventsPlugin core)
        {
            foreach (CommandConfig cmd in this.Commands)
            {
                cmd.Initialize(core);
            }

            foreach (EventConfig evnt in this.Events)
            {
                evnt.Initialize(core);
            }
        }

    }
}

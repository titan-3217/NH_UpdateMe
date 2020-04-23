using Events.Code;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Events.Forms
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public EventsPluginConfig Config
        {
            get
            {
                return (EventsPluginConfig)base.DataContext;
            }
        }

        public Settings()
        {
            InitializeComponent();
        }

        private EventsPlugin Plugin
        {
            get;
        }

        public Settings(EventsPlugin plugin) : this()
        {
            this.Plugin = plugin;
            base.DataContext = plugin.Config;
        }

        private void AddCommand_OnClick(object sender, RoutedEventArgs e)
        {
            this.Plugin.Config.Commands.Add(new CommandConfig(string.Empty, string.Empty, 5, false));
        }

        private void AddEvent_OnClick(object sender, RoutedEventArgs e)
        {
            this.Plugin.Config.Events.Add(new EventConfig(string.Empty, string.Empty, string.Empty, false));
        }

        private void SaveConfig_OnClick(object sender, RoutedEventArgs e)
        {
            this.Plugin.Save();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
            {
                return;
            }
            foreach (CommandConfig list in ((DataGrid)sender).SelectedItems.Cast<CommandConfig>().ToList<CommandConfig>())
            {
                list.PlayerCanUseIt = false;
                this.Plugin.Config.Commands.Remove(list);
            }
        }

        private void UIElementEvent_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
            {
                return;
            }
            foreach (EventConfig list in ((DataGrid)sender).SelectedItems.Cast<EventConfig>().ToList<EventConfig>())
            {
                this.Plugin.Config.Events.Remove(list);
            }
        }

    }
}

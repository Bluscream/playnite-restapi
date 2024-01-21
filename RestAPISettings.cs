using Playnite.SDK;
using Playnite.SDK.Data;
using System.Collections.Generic;
using System.Net;

namespace RestAPI {
    public class RestAPISettings : ObservableObject {
        private string host_address = ApiServer.DefaultIp;
        private short host_port = 9000;

        public string HostAddress { get => host_address; set => SetValue(ref host_address, value); }
        public short HostPort { get => host_port; set => SetValue(ref host_port, value); }
    }

    public class RestAPISettingsViewModel : ObservableObject, ISettings {
        private readonly RestAPI plugin;
        private RestAPISettings editingClone { get; set; }

        private RestAPISettings settings;
        public RestAPISettings Settings {
            get => settings;
            set {
                settings = value;
                OnPropertyChanged();
            }
        }

        public RestAPISettingsViewModel(RestAPI plugin) {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<RestAPISettings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null) {
                Settings = savedSettings;
            } else {
                Settings = new RestAPISettings();
            }
        }

        public void BeginEdit() {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit() {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit() {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors) {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            if (Settings.HostPort < 0 || Settings.HostPort > 65535) {
                errors.Add("Port must be between 0 and 65535");
            }
            if (!IPAddress.TryParse(Settings.HostAddress, out _)) {
                errors.Add("Host address is not a valid IP address");
            }
            if (errors.Count > 0) return false;
            return true;
        }
    }
}
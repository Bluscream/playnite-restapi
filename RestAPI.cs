using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RestAPI {
    public class RestAPI : GenericPlugin {
        private static readonly ILogger logger = LogManager.GetLogger();
        public override Guid Id { get; } = Guid.Parse("2fae73fa-6abd-4bbd-b8de-e0f2d7787bcd");
        private RestAPISettingsViewModel settings { get; set; }
        internal IPlayniteAPI playniteApi { get; set; }
        //private ApiServer apiServer { get; set; }

        public RestAPI(IPlayniteAPI _playniteApi) : base(_playniteApi) {
            this.playniteApi = _playniteApi;
            settings = new RestAPISettingsViewModel(this);
            Properties = new GenericPluginProperties {
                HasSettings = true
            };
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args) {
            Task.Run(() => { ApiServer.Start(playniteApi, settings.Settings.HostAddress, settings.Settings.HostPort); });
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args) {
            ApiServer.Stop();
        }

        public override ISettings GetSettings(bool firstRunSettings) {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings) {
            return new RestAPISettingsView();
        }
    }
}
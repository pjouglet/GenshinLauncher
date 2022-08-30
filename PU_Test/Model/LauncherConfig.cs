using CommunityToolkit.Mvvm.ComponentModel;

namespace PU_Test.Model
{
    partial class LauncherConfig : ObservableObject
    {
        [ObservableProperty]
        private GameInfo gameInfo;

        [ObservableProperty]
        private ProxyConfig proxyConfig;

        [ObservableProperty]
        private bool proxyOnly;
    }
}

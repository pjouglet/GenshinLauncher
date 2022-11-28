using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.Common;
using Launcher.Common.Game;
using Launcher.Common.Patch;
using Launcher.Common.Proxy;
using Launcher.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Launcher.ViewModel
{
    partial class MainWindow : ObservableObject
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 10) };

        public ProxyHelper.ProxyController proxyController;

        public MainWindow()
        {
            try
            {
                launcherConfig = JsonConvert.DeserializeObject<LauncherConfig>(File.ReadAllText("config.json"));

                //Task.Run(async () =>
                //{
                //    ServerInfo = await ServerInfoGetter.GetAsync(launcherConfig.ProxyConfig.ProxyServer);
                //    AnnounceMents = await ServerInfoGetter.GetAnnounceAsync(launcherConfig.ProxyConfig.ProxyServer);

                UpdateSI();

                //});
            }
            catch (Exception ex)
            {
                launcherConfig = new LauncherConfig();
                launcherConfig.ProxyConfig = new ProxyConfig(true, "127.0.0.1");
                launcherConfig.ProxyConfig.ProxyPort = "25565";

                launcherConfig.GameInfo = new GameInfo(GameHelper.GameRegReader.GetGameExePath());
                launcherConfig.ProxyOnly = false;

                SaveConfig();
            }
            try
            {


            }
            catch (Exception ex)
            {

            }

            ShowPatchStatue();

            dispatcherTimer.Tick += (a, b) =>
            {
                UpdateSI();
            };

            dispatcherTimer.Start();


        }

        [ObservableProperty]
        private List<AnnounceMentItem> announceMents;

        public void UpdateSI()
        {
            Task.Run(async () =>
            {
                ServerInfoGetter.scheme = launcherConfig.ProxyConfig.UseHttp ? "http" : "https";
                ServerInfo = await ServerInfoGetter.GetAsync(launcherConfig.ProxyConfig.ProxyServer);
                AnnounceMents = await ServerInfoGetter.GetAnnounceAsync(launcherConfig.ProxyConfig.ProxyServer);

            });
        }

        public void ShowPatchStatue()
        {
            switch (new PatchHelper(launcherConfig.GameInfo).GetPatchStatue())
            {
                case PatchHelper.PatchType.None: PatchStatueStr = Launcher.Resources.Strings.PATCH_OFFICIAL; break;
                case PatchHelper.PatchType.All: PatchStatueStr = Launcher.Resources.Strings.PATCH_PATCHED_ALL; break;
                case PatchHelper.PatchType.MetaData: PatchStatueStr = Launcher.Resources.Strings.PATCH_PATCHED_META; break;
                case PatchHelper.PatchType.UserAssemby: PatchStatueStr = Launcher.Resources.Strings.PATCH_PATCHED_UA; break;
                case PatchHelper.PatchType.Unknown: PatchStatueStr = Launcher.Resources.Strings.UNKNOWN; break;
            }
        }
        public void SaveConfig()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(launcherConfig));

        }

        [ObservableProperty]
        private LauncherConfig launcherConfig;

        [ObservableProperty]
        private string startGameBtnText = Launcher.Resources.Strings.LAUNCH;

        [ObservableProperty]
        private string patchStatueStr = Launcher.Resources.Strings.UNKNOWN;

        [ObservableProperty]
        private ServerInfo serverInfo = new ServerInfo();

        private bool IsGameRunning = false;

        [RelayCommand]
        private void StartGame()
        {


            if (launcherConfig.ProxyOnly)
            {
                if (proxyController == null)
                {
                    proxyController = new ProxyHelper.ProxyController(host: launcherConfig.ProxyConfig.ProxyServer, port: launcherConfig.ProxyConfig.ProxyPort, usehttp: launcherConfig.ProxyConfig.UseHttp);
                    proxyController.Start();
                    StartGameBtnText = Launcher.Resources.Strings.STOP_PROXY;
                    return;

                }
                if (proxyController._IsRun)
                {
                    proxyController.Stop();
                    proxyController = null;
                    StartGameBtnText = Launcher.Resources.Strings.LAUNCH;

                }
                else
                {
                    proxyController.Start();
                    StartGameBtnText = Launcher.Resources.Strings.STOP_PROXY;

                }
                return;
            }



            if (new PatchHelper(launcherConfig.GameInfo).GetPatchStatue() == PatchHelper.PatchType.None)
            {
                GameHelper.StartGame(launcherConfig.GameInfo.GameExePath);
                return;
            }
            if (!IsGameRunning)
            {

                if (!CheckGameCfg())
                {
                    MessageBox.Show(Launcher.Resources.Strings.CONFIGURATION_ERROR);
                    return;
                }
                IsGameRunning = true;

                proxyController = new ProxyHelper.ProxyController(
                    host: launcherConfig.ProxyConfig.ProxyServer,
                    port: launcherConfig.ProxyConfig.ProxyPort,
                    usehttp: launcherConfig.ProxyConfig.UseHttp
                    );
                proxyController.Start();
                StartGameBtnText = Launcher.Resources.Strings.STOP_PROXY;

                GameHelper.StartGame(launcherConfig.GameInfo.GameExePath);


            }
            else
            {
                if (proxyController != null)
                {

                    proxyController.Stop();
                }
                proxyController = null;
                StartGameBtnText = Launcher.Resources.Strings.LAUNCH;
                IsGameRunning = false;
            }

        }
        private bool CheckGameCfg()
        {
            if (launcherConfig.GameInfo != null)
            {
                return true;
            }
            MessageBox.Show(Launcher.Resources.Strings.GAME_PATH_NOT_SET);
            return false;
        }

        public void Official_Set()
        {
            new PatchHelper(launcherConfig.GameInfo).UnPatchUserAssembly();
            //MessageBox.Show("暂不支持！");
            ShowPatchStatue();
        }

        public void Private_Set()
        {
            new PatchHelper(launcherConfig.GameInfo).PatchUserAssembly();
            //MessageBox.Show("暂不支持！");
            ShowPatchStatue();
        }

        //public void ShowPatchStatue()
        //{
        //    string PatchStatueStr = "";
        //    switch (new PatchHelper(launcherConfig.GameInfo).GetPatchStatue())
        //    {
        //        case PatchHelper.PatchType.None: PatchStatueStr = "官方"; break;
        //        case PatchHelper.PatchType.All: PatchStatueStr = "已打补丁-ALL"; break;
        //        case PatchHelper.PatchType.MetaData: PatchStatueStr = "已打补丁-Meta"; break;
        //        case PatchHelper.PatchType.UserAssemby: PatchStatueStr = "已打补丁-UA"; break;
        //    }
        //    GlobalValues.mainWindow.vm.PatchStatueStr = PatchStatueStr;

        //}
    }
}

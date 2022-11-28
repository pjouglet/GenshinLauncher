using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using Launcher.Common.Game;
using Launcher.Common.Patch;
using Launcher.Model;
using System;
using System.IO;
using System.Windows;

namespace Launcher.ViewModel
{
    partial class SettingPage : ObservableObject
    {
        [ObservableProperty]
        private LauncherConfig launcherConfig;

        [ObservableProperty]
        private string patchStatueStr = Launcher.Resources.Strings.UNKNOWN;


        public SettingPage()
        {
            try
            {
                launcherConfig = JsonConvert.DeserializeObject<LauncherConfig>(File.ReadAllText("config.json"));
            }
            catch (Exception ex)
            {
                launcherConfig = new LauncherConfig();
                launcherConfig.ProxyConfig = new ProxyConfig(true, "127.0.0.1");
                launcherConfig.ProxyConfig.ProxyPort = "25565";

                launcherConfig.GameInfo = new GameInfo(GameHelper.GameRegReader.GetGameExePath());
            }
            ShowPatchStatue();
        }

        public void ShowPatchStatue()
        {

            switch (new PatchHelper(launcherConfig.GameInfo).GetPatchStatue())
            {
                case PatchHelper.PatchType.None: PatchStatueStr = Launcher.Resources.Strings.PATCH_OFFICIAL; break;
                case PatchHelper.PatchType.All: PatchStatueStr = Launcher.Resources.Strings.PATCH_PATCHED_ALL; break;
                case PatchHelper.PatchType.MetaData: PatchStatueStr = Launcher.Resources.Strings.PATCH_PATCHED_META; break;
                case PatchHelper.PatchType.UserAssemby: PatchStatueStr = Launcher.Resources.Strings.PATCH_PATCHED_UA; break;
            }
            GlobalValues.mainWindow.vm.PatchStatueStr = PatchStatueStr;

        }
        [RelayCommand]
        private void SaveConfig()
        {
            GlobalValues.mainWindow.vm.LauncherConfig = launcherConfig;
            GlobalValues.mainWindow.vm.SaveConfig();
            GlobalValues.frame.Visibility = Visibility.Collapsed;
        }
        [RelayCommand]
        private void RestorePatch()
        {
            var s = new PatchHelper(launcherConfig.GameInfo).GetPatchStatue();

            if (s == PatchHelper.PatchType.UserAssemby)
            {
                new PatchHelper(launcherConfig.GameInfo).UnPatchUserAssembly();

            }
            ShowPatchStatue();
        }
        [RelayCommand]
        private void PatchUA()
        {
            new PatchHelper(launcherConfig.GameInfo).PatchUserAssembly();
            //MessageBox.Show("暂不支持！");
            ShowPatchStatue();
        }
        [RelayCommand]
        private void SearchGameFile()
        {
            launcherConfig.GameInfo = new GameInfo(GameHelper.GameRegReader.GetGameExePath());
            if (File.Exists(launcherConfig.GameInfo.GameExePath))
            {
                MessageBox.Show($"已找到位于{launcherConfig.GameInfo.GameExePath}的游戏文件!");
            }
            else
            {
                MessageBox.Show($"搜索失败，注册表中没有相关信息!");

            }

        }
        [RelayCommand]
        private void SetGameExePath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "原神游戏程序（YuanShen.exe)|YuanShen.exe|原神游戏程序(GenshinImpact.exe)|GenshinImpact.exe";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {

                launcherConfig.GameInfo = new GameInfo(openFileDialog.FileName);

            }

        }
    }
}

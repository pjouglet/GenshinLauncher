using Launcher.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using static Launcher.Model.GameInfo;

namespace Launcher.Common.Patch
{
    internal class PatchHelper
    {
        GameInfo gameInfo;
        const string METADATA_FILE_NAME = "global-metadata.dat";
        const string UA_FILE_NAME = "UserAssembly.dll";
        const string PKG_VERSION_FILE = "pkg_version";
        public PatchHelper(GameInfo info)
        {
            gameInfo = info;
        }


        private string GetUAPatchDir()
        {
            var ret = "";
            if (gameInfo == null)
            {
                //MessageBox.Show("游戏路径配置不正确");
                return "";
            }
            var gamedir = Path.GetDirectoryName(gameInfo.GameExePath);

            string file_path = Path.Combine(gamedir, "YuanShen_Data", "Native");
            string file_path_osrel = Path.Combine(gamedir, "GenshinImpact_Data", "Native");

            if (gameInfo.GetGameType() == GameType.OS)
            {
                file_path = file_path_osrel;
            }
            return file_path;
        }

        public string GetHashFromPkgVer(string filepath)
        {


            var gamedir = Path.GetDirectoryName(gameInfo.GameExePath);

            var lines = File.ReadAllLines(Path.Combine(gamedir, PKG_VERSION_FILE));

            string target = null;
            foreach (var item in lines)
            {
                if (item.Contains(filepath))
                {
                    target = item;
                    break;

                }
            }
            return JsonConvert.DeserializeObject<PkgVersionItem>(target).md5;
        }

        public string GetHashFromFile(string filepath)
        {
            try
            {
                FileStream file = new FileStream(filepath, System.IO.FileMode.Open);
                MD5 md5 = MD5.Create();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"{Launcher.Resources.Strings.MD5_HASH_ERROR}: {ex.Message}");
            }

        }

        public void BackUpFile(string FILE_NAME)
        {
            var gamedir = Path.GetDirectoryName(gameInfo.GameExePath);

            string official = string.Empty;
            string backup = string.Empty;

            if (!File.Exists(FILE_NAME))
                throw new Exception(Launcher.Resources.Strings.PKG_VERSION_NOT_FOUND);

            if (FILE_NAME.Contains(METADATA_FILE_NAME))
            {

                official = GetHashFromPkgVer("Managed/Metadata/global-metadata.dat");
            }
            else
            {
                official = GetHashFromPkgVer($"Native/{UA_FILE_NAME}");

            }

            string currentMd5 = GetHashFromFile(Path.Combine(FILE_NAME));

            if (File.Exists(Path.Combine(FILE_NAME + ".bak")))
            {
                backup = GetHashFromFile(Path.Combine(FILE_NAME + ".bak"));

            }

            //官方与备份相同，不用备份
            if (official == backup)
                return;

            if (official != currentMd5)
                throw new Exception(Launcher.Resources.Strings.INCORRECT_PATCH_TARGET);

            File.Copy(FILE_NAME, FILE_NAME + ".bak");
        }

        public void RestoreFile(string FILE_NAME)
        {
            string official = string.Empty;
            string backup = string.Empty;

            if (!File.Exists(FILE_NAME + ".bak"))
            {
                MessageBox.Show(Launcher.Resources.Strings.NO_BACKUP_FOUND);
                return;
            }


            backup = GetHashFromFile(Path.Combine(FILE_NAME + ".bak"));

            if (FILE_NAME.Contains(METADATA_FILE_NAME))
            {

                official = GetHashFromPkgVer("Managed/Metadata/global-metadata.dat");
            }
            else
            {
                official = GetHashFromPkgVer($"Native/{UA_FILE_NAME}");

            }

            if (official != backup)
            {
                MessageBox.Show(Launcher.Resources.Strings.RESTORE_FAILED_NOT_OFFICIAL);
                return;
            }

            File.Copy(FILE_NAME + ".bak", FILE_NAME, true);
            MessageBox.Show(Launcher.Resources.Strings.RESTORE_SUCCESS);
        }


        public enum PatchType
        {
            None,
            MetaData,
            UserAssemby,
            All,
            Unknown,
        }

        public PatchType GetPatchStatue()
        {
            PatchType result = PatchType.None;

            try
            {
                var official = GetHashFromPkgVer("UserAssembly.dll");
                var current = GetHashFromFile(Path.Combine(GetUAPatchDir(), UA_FILE_NAME));
                if (current != official)
                {
                    if (result == PatchType.None)
                    {
                        result = PatchType.UserAssemby;

                    }
                    else
                    {
                        result = PatchType.All;
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return result;
        }

        internal void PatchUserAssembly()
        {
            var file_path = GetUAPatchDir();
            try
            {
                BackUpFile(Path.Combine(file_path, UA_FILE_NAME));
                DoPatchUserAssembly(Path.Combine(file_path, UA_FILE_NAME));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                throw;
            }
        }

        private void DoPatchUserAssembly(string p)
        {

            try
            {
                new UA_Util(gameInfo).Patch(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Launcher.Resources.Strings.FAILED}: {ex}");
                return;
            }

            MessageBox.Show(Launcher.Resources.Strings.USERASSEMBLY_PATCH_SUCCESS);
        }

        internal void UnPatchUserAssembly()
        {
            var file_path = GetUAPatchDir();
            RestoreFile(Path.Combine(file_path, UA_FILE_NAME));
        }
    }
}

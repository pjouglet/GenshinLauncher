using System.IO;

namespace Launcher.Model
{
    internal class GameInfo
    {
        string GameLauncherFolder { get; set; }
        public GameInfo(string gameExePath)
        {
            GameExePath = gameExePath;

            GameLauncherFolder = Path.GetDirectoryName(gameExePath);
        }

        public enum GameType
        {
            CN,
            OS,
            UnKnown,
        }
        public GameType GetGameType()
        {
            GameType gameType = GameType.UnKnown;

            if (Directory.Exists(Path.Combine(GameLauncherFolder, "YuanShen_Data")))
            {
                gameType = GameType.CN;
            }
            else if (Directory.Exists(Path.Combine(GameLauncherFolder, "GenshinImpact_Data")))
            {
                gameType = GameType.OS;
            }

            return gameType;
        }

        public string GameExePath { get; set; }

    }
}

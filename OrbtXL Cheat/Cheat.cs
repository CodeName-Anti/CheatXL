using JNNJMods.OrbtXLCheat.Patches;
using Steamworks;
using System.Reflection;
using UnityEngine;

namespace JNNJMods.OrbtXLCheat
{
    public class Cheat : MonoBehaviour
    {
        public bool draw = true;
        public string prevScoreField = "100";

        public void OnGUI()
        {
            if (!draw)
                return;

            GUI.Window(0, new Rect(100, 100, 400, 500), DrawWindow, "OrbtXL Cheat by JNNJ");
        }

        public void DrawWindow(int id)
        {
            GUILayout.Label("Steam Cheats");

            if (GUILayout.Button("Get all Achievements"))
                UnlockAllAchievements();

            if (GUILayout.Button("Reset everything"))
                SteamUserStats.ResetAllStats(true);

            GUILayout.Label("Game Cheats");

            if (GUILayout.Button("GodMode - " + GetState(GodmodePatch.Godmode)))
                GodmodePatch.Godmode = !GodmodePatch.Godmode;

            string str = GUILayout.TextField(prevScoreField);

            if (IsNumeric(str))
                prevScoreField = str;

            if (GUILayout.Button("Set Score"))
                Gameplay.playerScore = int.Parse(prevScoreField);

            if (GUILayout.Button("Set Score and End Round"))
            {
                Gameplay.playerScore = int.Parse(prevScoreField);
                Gameplay objectOfType = FindObjectOfType<Gameplay>();
                objectOfType.RoundEnd();
            }

            if(GUILayout.Button("Reset Score"))
            {
                SaveManager.bestScore = 0;
                SaveManager.SaveData();

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                var asm = Assembly.GetAssembly(typeof(Gameplay));
                var steamManager = asm.GetType("SteamManager");
                var field = steamManager.GetProperty("Leaderboards", flags);

                SteamLeaderboards leaderboards = (SteamLeaderboards)field.GetValue(null, null);
                leaderboards.UploadScore(Leaderboard.orbt_xl_High_Scores, 0);
            }

        }

        public bool IsNumeric(string value)
        {
            return double.TryParse(value, out _);
        }

        public void OnDestroy()
        {
            draw = false;
        }

        private void UnlockAllAchievements()
        {
            for(uint i = 0; i < SteamUserStats.GetNumAchievements(); i++)
            {
                string achievementName = SteamUserStats.GetAchievementName(i);

                SteamUserStats.GetAchievement(achievementName, out bool pbAchieved);
                if (pbAchieved)
                    continue;
                SteamUserStats.SetAchievement(achievementName);
            }
        }

        public string GetState(bool state) => state ? "ON" : "OFF";
    }
}
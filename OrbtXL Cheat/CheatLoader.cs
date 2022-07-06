using UnityEngine;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;

namespace JNNJMods.OrbtXLCheat
{
    [BepInPlugin("de.jnnj.orbtxlcheat", "Orbt XL Cheat", "1.0")]
    public class CheatLoader : BaseUnityPlugin
    {
        public static ManualLogSource Log => Instance.Logger;

        public static CheatLoader Instance;
        public static Cheat Cheat { get; private set; }

        public static Harmony Harmony { get; private set; }

        public void Awake()
        {
            Instance = this;
            Harmony = new Harmony("de.jnnj.orbtxlcheat");
            Harmony.PatchAll();

            GameObject cheatObj = new GameObject();
            cheatObj.hideFlags |= HideFlags.HideAndDontSave;
            Cheat = cheatObj.AddComponent<Cheat>();

            DontDestroyOnLoad(cheatObj);
        }
    }
}

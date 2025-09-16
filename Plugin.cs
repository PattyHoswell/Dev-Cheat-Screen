using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Patty_DevCheatScreen_MOD
{
    [BepInDependency("Patty_DevConsole_MOD", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Patty_DevConsole_MOD", "1.2.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        internal static string BasePath { get; } = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        internal static ManualLogSource LogSource { get; private set; }
        internal static Harmony PluginHarmony { get; private set; }
        internal static ConfigEntry<KeyCode> CheatHotkey { get; private set; }

        void Awake()
        {
            LogSource = Logger;
            try
            {
                PluginHarmony = Harmony.CreateAndPatchAll(typeof(PatchList), PluginInfo.GUID);
            }
            catch (HarmonyException ex)
            {
                LogSource.LogError((ex.InnerException ?? ex).Message);
            }

            CheatHotkey = Config.Bind(new ConfigDefinition("Hotkey", "Open/Close"), KeyCode.F2);
        }

        void Update()
        {
            if (Input.GetKeyDown(CheatHotkey.Value))
            {
                if (AllGameManagers.Instance == null)
                {
                    LogSource.LogError("AllGameManagers isn't available. Try waiting a bit more before opening the menu");
                    return;
                }
                var screenManager = AllGameManagers.Instance.GetScreenManager();
                if (screenManager == null)
                {
                    LogSource.LogError("ScreenManager isn't available. Try waiting a bit more before opening the menu");
                    return;
                }
                var isOpened = screenManager.GetScreenActive(ScreenName.Cheat);
                screenManager.SetScreenActive(ScreenName.Cheat, !isOpened);
            }
        }
    }
}

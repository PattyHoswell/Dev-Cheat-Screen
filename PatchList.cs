using HarmonyLib;
using ShinyShoe;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Patty_DevCheatScreen_MOD
{
    internal class PatchList
    {
        [HarmonyPostfix, HarmonyPatch(typeof(ShinyShoe.AppManager), "DoesThisBuildReportErrors")]
        public static void DisableErrorReportingPatch(ref bool __result)
        {
            __result = false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(CheatScreen), "AddCheat")]
        public static void AddCheat(CheatScreen __instance,
                                    string label,
                                    string category,
                                    CheatScreen.CheatTriggerDelegate cheatTriggerDelegate,
                                    bool closeCheatScreenOnTrigger,
                                    CheatScreen.LayoutGroup layoutGroup)
        {
            var cheatScreenTraverse = Traverse.Create(__instance);
            Transform targetTr = cheatScreenTraverse.Field<ScreenDialog>("_cheatScreenDialog").Value.transform.Find($"{layoutGroup.ToString()} content/CheatsArea");
            if (targetTr == null)
            {
                Plugin.LogSource.LogError($"Cannot find layout: {layoutGroup} content/CheatsArea");
                return;
            }
            var setButton = GameObject.Instantiate(cheatScreenTraverse.Field<GameUISelectableButton>("cheatButton").Value, targetTr);
            setButton.gameObject.SetActive(true);
            // GameUISelectableButton setButton, string setLabel, string setCategory, CheatScreen.CheatTriggerDelegate setTrigger, bool setCloseCheatScreenOnTrigger
            var cheatType = cheatScreenTraverse.Type("Cheat").GetValue<Type>();
            var cheat = Activator.CreateInstance(cheatType, new object[]
            {
                setButton,
                label,
                category,
                cheatTriggerDelegate,
                closeCheatScreenOnTrigger
            });
            var list = typeof(List<>).MakeGenericType(cheatType);
            list.GetMethod("Add", AccessTools.allDeclared).Invoke(cheatScreenTraverse.Field("cheats").GetValue(), new object[]
            {
                cheat
            });
            // Update the text
            __instance.Relabel(label, label);
        }
    }
}

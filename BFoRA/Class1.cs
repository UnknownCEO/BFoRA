using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using System;
using System.Reflection;

namespace BFoRA {
    static class Main {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry) {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            mod = modEntry;
            modEntry.OnToggle = OnToggle;

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            enabled = value;
            return true;
        }
    }

    [HarmonyPatch(typeof(BridgeControls))]
    [HarmonyPatch("RedAlert")]
    static class BFoRA_Patch {
        static void Postfix(TraderControls __instance) {
            if (!Main.enabled)
                return;

            try {
                __instance.GetComponent<BuildingInfo>().GetComponentInChildren<BridgeForcefields>().ActivateForcefield(true);
            } catch (Exception e) {
                Main.mod.Logger.Error(e.ToString());
            }
        }
    }
}
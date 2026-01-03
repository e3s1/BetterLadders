using System.Diagnostics.CodeAnalysis;
using BetterLadders.Config;
using GameNetcodeStuff;
using HarmonyLib;

namespace BetterLadders.Patches
{
    internal static class AllowTwoHanded
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [HarmonyPrefix, HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Interact_performed))]
        private static void LadderTwoHandedAccessPrefix(PlayerControllerB __instance)
        {
            var trigger = __instance.hoveringOverTrigger;
            if (trigger == null || !trigger.isLadder) return;
            
            // Allow player to exit ladder when carrying two-handed item if config is changed while climbing
            var allowTwoHanded = LocalConfig.Instance.AllowTwoHanded.Value || __instance.isClimbingLadder;
            
            trigger.twoHandedItemAllowed = allowTwoHanded;
            trigger.specialCharacterAnimation = !allowTwoHanded;
        }
    }
}

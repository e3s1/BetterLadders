using System.Diagnostics.CodeAnalysis;
using BetterLadders.Config;
using HarmonyLib;

namespace BetterLadders.Patches;

internal static class ExtensionLadderTime
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPrefix, HarmonyPatch(typeof(ExtensionLadderItem), nameof(ExtensionLadderItem.StartLadderAnimation))]
    private static void ExtensionTimePatch(ref float ___ladderTimer)
    {
        ___ladderTimer = LocalConfig.Instance.ExtensionTime.Value switch
        {
            <= 0 => float.MinValue,
            _ => LocalConfig.Instance.ExtensionTime.VanillaValue - LocalConfig.Instance.ExtensionTime.Value
        };
    }
}
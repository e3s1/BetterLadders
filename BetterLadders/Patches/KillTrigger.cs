using System.Diagnostics.CodeAnalysis;
using BetterLadders.Config;
using HarmonyLib;
using UnityEngine;

namespace BetterLadders.Patches;

internal static class KillTrigger
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPrefix, HarmonyPatch(typeof(ExtensionLadderItem), nameof(ExtensionLadderItem.StartLadderAnimation))]
    private static void KillTriggerPatch(ref Collider ___killTrigger)
    {
        ___killTrigger.gameObject.SetActive(LocalConfig.Instance.EnableKillTrigger.Value);
    }
}


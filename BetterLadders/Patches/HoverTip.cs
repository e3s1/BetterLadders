using System.Diagnostics.CodeAnalysis;
using BetterLadders.Config;
using GameNetcodeStuff;
using HarmonyLib;

namespace BetterLadders.Patches;

internal static class HoverTip
{
    private static bool? _wasTwoHanded;
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPrefix, HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetHoverTipAndCurrentInteractTrigger))]
    private static void LadderHoverTipPrefix(PlayerControllerB __instance)
    {
        if (__instance.hoveringOverTrigger == null || !__instance.hoveringOverTrigger.isLadder) return;

        if (!__instance.isHoldingInteract) return;
        
        if (LocalConfig.Instance.AllowTwoHanded.Value && __instance.twoHanded)
        {
            _wasTwoHanded = true;
            __instance.twoHanded = false;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPostfix, HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetHoverTipAndCurrentInteractTrigger))]
    private static void LadderHoverTipPostfix(PlayerControllerB __instance)
    {
        if (_wasTwoHanded == null) return;

        __instance.twoHanded = (bool)_wasTwoHanded;
        _wasTwoHanded = null;
    }
}

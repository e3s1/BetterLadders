using System.Diagnostics.CodeAnalysis;
using BetterLadders.Config;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.SocialPlatforms;

namespace BetterLadders.Patches;

internal static class HideItems
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPostfix, HarmonyPatch(typeof(InteractTrigger), nameof(InteractTrigger.SetUsingLadderOnLocalClient))]
    private static void SetVisibilityOnStartClimb(InteractTrigger __instance, bool isUsing)
    {
        var player = GameNetworkManager.Instance.localPlayerController;
        if (player.currentlyHeldObjectServer == null) return;
        
        var visible = !isUsing;
        
        BetterLaddersPlugin.Logger.LogInfo("Toggling visibility for held item");
        if ((LocalConfig.Instance.HideOneHanded.Value && !player.twoHanded) || (LocalConfig.Instance.HideTwoHanded.Value && player.twoHanded))
        {
            player.currentlyHeldObjectServer.EnableItemMeshes(visible);
        }
        if (SyncedConfig.Instance.IsSpawned)
            SyncedConfig.Instance.RequestChangeItemVisibilityRpc(player.currentlyHeldObjectServer, visible);

    }
}

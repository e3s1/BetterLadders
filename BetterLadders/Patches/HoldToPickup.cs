using System.Diagnostics.CodeAnalysis;
using BetterLadders.Config;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace BetterLadders.Patches;

internal static class HoldToPickup
{
    private static bool _canPickupLadder = false;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPrefix, HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.BeginGrabObject))]
    private static bool ControlExtLadderPickup(PlayerControllerB __instance)
    {
        if (!LocalConfig.Instance.HoldToPickup.Value) return true;
        
        if (!LookingAtGrabbableExtLadder(__instance, out var extLadderObj))
            return true;

        if (extLadderObj == null || !extLadderObj.ladderActivated) return true;
        if (!_canPickupLadder) return false;
        
        _canPickupLadder = false;
        
        return true;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPostfix, HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ClickHoldInteraction))]
    private static void ShowHoldInteractHUD(PlayerControllerB __instance)
    {
        if (!LocalConfig.Instance.HoldToPickup.Value) return;
        
        if (!__instance.hoveringOverTrigger)
        {
            bool isHoldingInteract = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").IsPressed();
            if (!isHoldingInteract)
            {
                HUDManager.Instance.holdFillAmount = 0f;
                return;
            }
            if (LookingAtGrabbableExtLadder(__instance, out var extLadderObj))
            {
                if (extLadderObj != null && extLadderObj.ladderActivated)
                {
                    if (!HUDManager.Instance.HoldInteractionFill(LocalConfig.Instance.HoldTime.Value))
                    {
                        return;
                    }
                    _canPickupLadder = true;
                    __instance.BeginGrabObject();
                }
            }
        }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPrefix, HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.StopHoldInteractionOnTrigger))]
    private static bool StopHoldInteractionOnTrigger(PlayerControllerB __instance)
    {
        if (!LocalConfig.Instance.HoldToPickup.Value) return true;
        
        if (LookingAtGrabbableExtLadder(__instance, out var extLadderObj))
        {
            if (extLadderObj != null && extLadderObj.ladderActivated)
            {
                var isHoldingInteract = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").IsPressed();
                if (isHoldingInteract)
                {
                    return false;
                }
                
                HUDManager.Instance.holdFillAmount = 0f;
            }

        }
        else
        {
            HUDManager.Instance.holdFillAmount = 0f;
        }
        
        if (__instance.previousHoveringOverTrigger != null)
        {
            __instance.previousHoveringOverTrigger.StopInteraction();
        }
        
        if (__instance.hoveringOverTrigger != null)
        {
            __instance.hoveringOverTrigger.StopInteraction();
        }
        
        return false;
    }
    private static bool LookingAtGrabbableExtLadder(PlayerControllerB player, out ExtensionLadderItem extLadderObj)
    {
        // not sure what layer 8 is, this is taken from PlayerControllerB::BeginGrabObject()
        var success = Physics.Raycast(player.interactRay, out var hit, player.grabDistance, player.interactableObjectsMask) && hit.collider.gameObject.layer != 8;
        if (success)
        {
            extLadderObj = hit.collider.gameObject.GetComponent<ExtensionLadderItem>();
            if (player.twoHanded || (extLadderObj != null && !extLadderObj.grabbable)) return false;
        }
        else
        {
            extLadderObj = null;
        }
        return success;
    }
}
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using BetterLadders.Config;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace BetterLadders.Patches;

internal static class ClimbSpeed
{
    private static float _vanillaClimbSpeed = float.NaN;
    private static readonly int AnimationSpeed = Animator.StringToHash("animationSpeed");

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPostfix, HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Awake))]
    private static void GetVanillaClimbSpeedPatch(PlayerControllerB __instance)
    {
        if (!float.IsNaN(_vanillaClimbSpeed)) return;
        
        _vanillaClimbSpeed = __instance.climbSpeed;
        BetterLaddersPlugin.Logger.LogInfo($"Vanilla climb speed: {_vanillaClimbSpeed}");
    }
    
    [HarmonyPostfix, HarmonyPatch(typeof(InteractTrigger), nameof(InteractTrigger.SetUsingLadderOnLocalClient))]
    private static void LadderAnimationSpeedPatch(bool isUsing)
    {
        if (!isUsing) return;
        
        var player = GameNetworkManager.Instance.localPlayerController;
        
        player.StartCoroutine(SetAnimationSpeed(player));
        if (SyncedConfig.Instance.IsSpawned)
            SyncedConfig.Instance.RequestStartChangeAnimationSpeedRpc(player);
    }
    
    [HarmonyPostfix, HarmonyPatch(typeof(InteractTrigger), nameof(InteractTrigger.SetUsingLadderOnLocalClient))]
    private static void LadderClimbSpeedPatch(bool isUsing)
    {
        if (!isUsing) return;
        
        var player = GameNetworkManager.Instance.localPlayerController;
        
        player.StartCoroutine(SetClimbSpeed(player));
    }

    private class TimeoutResult
    {
        public bool TimedOut;
    }

    private static IEnumerator Timeout(PlayerControllerB player, TimeoutResult timeoutResult)
    {
        var timeWaiting = 0.0f;
        var timeout = 1.0f / LocalConfig.Instance.TransitionSpeedMultiplier.Value;
        while (!player.isClimbingLadder)
        {
            timeWaiting += Time.deltaTime;
            if (timeWaiting > timeout)
            {
                BetterLaddersPlugin.Logger.LogInfo($"Coroutine timed out for {player.playerUsername} after {timeWaiting}s (max {timeout}s)");
                timeoutResult.TimedOut = true;
                break;
            }
            
            yield return null;
        }
        
        BetterLaddersPlugin.Logger.LogInfo($"Waited {timeWaiting}s for {player.playerUsername} to start climbing ladder");
    }

    private static IEnumerator SetClimbSpeed(PlayerControllerB player)
    {
        BetterLaddersPlugin.Logger.LogInfo($"Starting SetClimbSpeed coroutine for {player.playerUsername}");
        var timeoutResult = new TimeoutResult();
        yield return Timeout(player, timeoutResult);
        if (timeoutResult.TimedOut)
            yield break;
        yield return new WaitWhile(() =>
        {
            player.climbSpeed = _vanillaClimbSpeed * LocalConfig.Instance.ClimbSpeedMultiplier.Value;
            if (player.isSprinting) player.climbSpeed *= LocalConfig.Instance.SprintingClimbSpeedMultiplier.Value;

            return player.isClimbingLadder;
        });
        BetterLaddersPlugin.Logger.LogInfo($"Finished SetClimbSpeed coroutine for {player.playerUsername}");
    }

    internal static IEnumerator SetAnimationSpeed(PlayerControllerB player)
    {
        BetterLaddersPlugin.Logger.LogInfo($"Starting SetAnimationSpeed coroutine for {player.playerUsername}");
        var timeoutResult = new TimeoutResult();
        yield return Timeout(player, timeoutResult);
        if (timeoutResult.TimedOut)
            yield break;
        yield return new WaitWhile(() =>
        {
            var animationSpeed = player.playerBodyAnimator.GetFloat(AnimationSpeed);

            if (animationSpeed == 0f)
                return player.isClimbingLadder;

            // player.isLocalPlayer always returns false :(
            var isLocalPlayer = player == GameNetworkManager.Instance.localPlayerController;

            // Prevents climbing animation from playing for the local player when only holding left/right (vanilla bug)
            // TODO: fix this when transmitting animation speed too???
            if (isLocalPlayer && player.moveInputVector.y == 0f)
            {
                player.playerBodyAnimator.SetFloat(AnimationSpeed, 0f);
                return player.isClimbingLadder;
            }

            var isSprinting = isLocalPlayer
                ? player.isSprinting
                : player.playerBodyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Sprinting");
            var newAnimationSpeed = LocalConfig.Instance.ClimbSpeedMultiplier.Value;
            if (isSprinting) newAnimationSpeed *= LocalConfig.Instance.SprintingClimbSpeedMultiplier.Value;
            var direction = animationSpeed switch
            {
                > 0 => 1,
                < 0 => -1,
                _ => 0
            };
            player.playerBodyAnimator.SetFloat(AnimationSpeed, newAnimationSpeed * direction);

            return player.isClimbingLadder;
        });
        BetterLaddersPlugin.Logger.LogInfo($"Finished SetAnimationSpeed coroutine for {player.playerUsername}");
    }
}

using BetterLadders.Patches;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace BetterLadders.Config;

/// <summary>
/// NetworkVariables are automatically generated at compile-time for each <see cref="SyncedConfigEntry&lt;T&gt;" /> in <see cref="LocalConfig" />
/// <seealso cref="SyncedConfig.NetworkVariables" />
/// </summary>
public partial class SyncedConfig : NetworkBehaviour
{
    public static SyncedConfig Instance { get; private set; }

    internal static event Action OnInitialized;

    internal static void Initialize(Scene sceneName, LoadSceneMode sceneEnum)
    {
        if (sceneName.name != "SampleSceneRelay") return;

        var gameObject = new GameObject("BetterLaddersSyncedConfig");
        gameObject.AddComponent<NetworkObject>();
        Instance = gameObject.AddComponent<SyncedConfig>();
        BetterLaddersPlugin.Logger.LogInfo("Initialized " + gameObject.name);
    }
    
    // This runs on all clients if the host has the mod
    public override void OnNetworkSpawn()
    {
        BetterLaddersPlugin.Logger.LogInfo("SyncedConfig network spawned");
        
        OnInitialized?.Invoke();
        OnInitialized?.GetInvocationList().Clear();
        
        BetterLaddersPlugin.Logger.LogInfo("Finished initializing NetworkVariables");
    }
    
    [Rpc(SendTo.NotMe)]
    public void RequestChangeItemVisibilityRpc(NetworkBehaviourReference itemReference, bool visible)
    {
        if (!itemReference.TryGet(out GrabbableObject item))
        {
            BetterLaddersPlugin.Logger.LogWarning("Failed to get item from reference while hiding item");
            return;
        }

        var player = item.playerHeldBy;
        
        if (player == null) return;

        if ((LocalConfig.Instance.HideOneHanded.Value && !player.twoHanded) || (LocalConfig.Instance.HideTwoHanded.Value && player.twoHanded))
        {
            BetterLaddersPlugin.Logger.LogInfo("Hiding item visibility for " + player.playerUsername);
            item.EnableItemMeshes(visible);
        }
    }

    [Rpc(SendTo.NotMe)]
    public void RequestStartChangeAnimationSpeedRpc(NetworkBehaviourReference playerReference)
    {
        if (!playerReference.TryGet(out PlayerControllerB player))
        {
            BetterLaddersPlugin.Logger.LogWarning("Failed to get player from reference while starting animation speed coroutine");
            return;
        }

        player.StartCoroutine(ClimbSpeed.SetAnimationSpeed(player));
    }
}
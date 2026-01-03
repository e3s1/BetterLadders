using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using BetterLadders.Config;
using BetterLadders.Patches;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterLadders;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalConfigGuid, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(LobbyCompatibilityGuid, BepInDependency.DependencyFlags.SoftDependency)]
public class BetterLaddersPlugin : BaseUnityPlugin
{
    private const string LethalConfigGuid = "ainavt.lc.lethalconfig";
    private const string LobbyCompatibilityGuid = "BMX.LobbyCompatibility";
    
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);
    internal new static ManualLogSource Logger { get; private set; }
    private void Awake()
    {
        Logger = base.Logger;
        
        NetcodePatcher();
        
        if (Chainloader.PluginInfos.ContainsKey(LethalConfigGuid))
        {
            Logger.LogInfo("LethalConfig detected");
            Compatibility.LethalConfig.Init();
        }
        
        if (Chainloader.PluginInfos.ContainsKey(LobbyCompatibilityGuid))
        {
            Logger.LogInfo("LobbyCompatibility detected, registering plugin...");
            Compatibility.LobbyCompatibility.Init(Info.Metadata);
        }
        
        _ = new LocalConfig(Config);

        SceneManager.sceneLoaded += SyncedConfig.Initialize;
        
        _harmony.PatchAll(typeof(AllowTwoHanded));
        _harmony.PatchAll(typeof(ClimbSpeed));
        _harmony.PatchAll(typeof(ExtensionLadderTime));
        _harmony.PatchAll(typeof(HideItems));
        _harmony.PatchAll(typeof(HoldToPickup));
        _harmony.PatchAll(typeof(HoverTip));
        _harmony.PatchAll(typeof(KillTrigger));
        
        Logger.LogInfo("Running transpilers...");
        if (!LocalConfig.Instance.DebugMode.Value) 
            Logger.LogInfo("Not logging IL code, DebugMode is disabled");
        
        _harmony.PatchAll(typeof(ExtensionLadderDelay));
        _harmony.PatchAll(typeof(ExtensionLadderSpeed));
        _harmony.PatchAll(typeof(TransitionSpeed));
        
        Logger.LogInfo("Finished loading");
    }
    
    private static void NetcodePatcher()
    {
        // Trying to load a type in the compatibility namespace will try to load the corresponding mod's assembly,
        // which isn't guaranteed to be present at this point, causing an error if the mod isn't installed.
        var types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace?.StartsWith("BetterLadders.Compatibility") != true); 
        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0)
                {
                    method.Invoke(null, null);
                }
            }
        }
    }
}
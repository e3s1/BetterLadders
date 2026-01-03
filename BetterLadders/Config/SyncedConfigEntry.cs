using BepInEx.Configuration;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace BetterLadders.Config;

// Separate class because static variables are tracked per generic
internal static class Counter
{
    public static int Count;
}

public class SyncedConfigEntry<T> : CustomConfigEntry<T>
{
    private static bool IsHost => NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer;
    private static bool InMainMenu => SceneManager.GetActiveScene().name == "MainMenu";
    
    private string ConfigName => ConfigEntry.Definition.Key;
    private NetworkVariable<T> NetworkVariable => (NetworkVariable<T>)SyncedConfig.Instance.NetworkVariables[_id];
    public override T Value
    {
        get {
            if (!IsHost && !SyncedConfig.Instance.IsSpawned)
                return VanillaValue;

            return NetworkVariable.Value;
        }
    }
    
    public readonly T VanillaValue;
    private readonly int _id;
    
    public SyncedConfigEntry(ConfigFile configFile, string section, string key, T defaultValue, ConfigDescription configDescription, T vanillaValue)
         : base(configFile, $"{section} (Synced)", key, defaultValue, configDescription)
    {
        Init();
        _id = Counter.Count++;
        VanillaValue = vanillaValue;
        ConfigEntry.SettingChanged += OnSettingChanged;
        SyncedConfig.OnInitialized += () =>
        {
            if (IsHost)
            {
                BetterLaddersPlugin.Logger.LogInfo($"[HOST] Setting initial value for {ConfigName} NetworkVariable to {ConfigEntry.Value}");
                NetworkVariable.Value = ConfigEntry.Value;
            }
            else
            {
                NetworkVariable.OnValueChanged += (oldValue, newValue) =>
                {
                    BetterLaddersPlugin.Logger.LogInfo($"[CLIENT] Host changed NetworkVariable for {ConfigName} to {newValue}");
                };
            }
            
        };
    }

    public SyncedConfigEntry(ConfigFile configFile, string section, string key, T defaultValue, string configDescription, T vanillaValue)
         : this(configFile, section, key, defaultValue, new ConfigDescription(configDescription), vanillaValue)
    {
    }

    private void OnSettingChanged(object sender, EventArgs args)
    {
        var newValue = ConfigEntry.Value;

        if (NetworkManager.Singleton.IsHost && SyncedConfig.Instance.IsSpawned)
        {
            BetterLaddersPlugin.Logger.LogInfo($"[HOST] Changed {ConfigName} to {newValue}, updating NetworkVariable...");
            NetworkVariable.Value = newValue;
        }
    }
    
    private (bool, string) CanModifyCallback()
    {
        if (IsHost || InMainMenu)
        {
            return (true, "");
        }

        if (!IsHost && !SyncedConfig.Instance.IsSpawned && !InMainMenu)
        {
            return (false, $"Host is missing BetterLadders; using vanilla value of {VanillaValue}");
        }

        var valueString = typeof(T) == typeof(float) ? ((float)(object)Value).ToString("0.00") : Value.ToString();
        return (false, $"Using host's value of {valueString}");
    }

    protected override void LethalConfigCompat()
    {
        Compatibility.LethalConfig.AddConfigEntry(ConfigEntry, CanModifyCallback);
    }
}
using BepInEx.Configuration;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;

namespace BetterLadders.Compatibility;

internal static class LethalConfig
{
    internal static bool IsInstalled { get; private set; }

    internal static void Init()
    {
        IsInstalled = true;
    }
    
    internal static void AddConfigEntry<T>(ConfigEntry<T> configEntry, Func<(bool allowed, string reason)> canModifyCallback = null)
    {
        BetterLaddersPlugin.Logger.LogInfo($"Setting up LethalConfig entry for {configEntry.Definition.Key}");
        LethalConfigManager.SkipAutoGenFor(configEntry);
        
        BaseConfigItem configItem = typeof(T) switch
        {
            var x when x == typeof(float) => new FloatSliderConfigItem(configEntry as ConfigEntry<float>, Options<FloatSliderOptions>()),
            var x when x == typeof(bool) => new BoolCheckBoxConfigItem(configEntry as ConfigEntry<bool>, Options<BoolCheckBoxOptions>()),
            _ => throw new NotImplementedException($"{typeof(T)} not implemented for Compatibility.LethalConfig.SetupConfigEntry<T>")
        };
        
        LethalConfigManager.AddConfigItem(configItem);
        
        return;
        
        TOptions Options<TOptions>() where TOptions : BaseOptions, new()
        {
            return new TOptions
            {
                RequiresRestart = false,
                CanModifyCallback = canModifyCallback != null ? () => canModifyCallback() : null
            };
        }
    }
}
using BepInEx.Configuration;

namespace BetterLadders.Config;

public class LocalConfigEntry<T> : CustomConfigEntry<T>
{
    public override T Value => ConfigEntry.Value;

    public LocalConfigEntry(ConfigFile configFile, string section, string key, T defaultValue, ConfigDescription configDescription)
        : base(configFile, section, key, defaultValue, configDescription)
    {
        Init();
    }
    
    public LocalConfigEntry(ConfigFile configFile, string section, string key, T defaultValue, string configDescription)
        : this(configFile, section, key, defaultValue, new ConfigDescription(configDescription))
    {
    }

    protected override void LethalConfigCompat()
    {
        Compatibility.LethalConfig.AddConfigEntry(ConfigEntry);
    }
}
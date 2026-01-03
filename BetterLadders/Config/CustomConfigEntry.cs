using BepInEx.Configuration;

namespace BetterLadders.Config;

public abstract class CustomConfigEntry<T>
{
    public ConfigEntry<T> ConfigEntry;

    public virtual T Value { get; }

    protected CustomConfigEntry(ConfigFile configFile, string section, string key, T defaultValue, ConfigDescription configDescription)
    {
        ConfigEntry = configFile.Bind(section, key, defaultValue, configDescription);
    }

    protected abstract void LethalConfigCompat();
    protected void Init()
    {
        if (Compatibility.LethalConfig.IsInstalled)
            LethalConfigCompat();
    }

}
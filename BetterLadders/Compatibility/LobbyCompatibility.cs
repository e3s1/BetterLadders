using BepInEx;
using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;

namespace BetterLadders.Compatibility;

internal static class LobbyCompatibility
{
    internal static void Init(BepInPlugin info)
    {
        PluginHelper.RegisterPlugin(info.GUID, info.Version, CompatibilityLevel.ClientOnly, VersionStrictness.Minor);
    }
}
using BepInEx.Configuration;
using HarmonyLib;

namespace BetterLadders.Config
{
    [SourceGen(typeof(SyncedConfigEntry<>), typeof(SyncedConfig))]
    public class LocalConfig
    {
        public static LocalConfig Instance { get; private set; }
        
        // General 
        public readonly SyncedConfigEntry<float> ClimbSpeedMultiplier;
        public readonly SyncedConfigEntry<float> SprintingClimbSpeedMultiplier;
        public readonly SyncedConfigEntry<float> TransitionSpeedMultiplier;
        public readonly SyncedConfigEntry<bool> AllowTwoHanded;
        // General (not synced)
        public readonly LocalConfigEntry<bool> ScaleAnimationSpeed;
        public readonly LocalConfigEntry<bool> HideOneHanded;
        public readonly LocalConfigEntry<bool> HideTwoHanded;
        // Extension Ladders 
        public readonly SyncedConfigEntry<float> ExtensionTime;
        public readonly SyncedConfigEntry<float> ExtensionSpeedMultiplier;
        public readonly SyncedConfigEntry<float> ExtensionDelay;
        public readonly SyncedConfigEntry<float> FallSpeedMultiplier;
        public readonly SyncedConfigEntry<float> FallDelay;
        public readonly SyncedConfigEntry<bool> EnableKillTrigger;
        // Extension Ladders (not synced)
        public readonly LocalConfigEntry<bool> HoldToPickup;
        public readonly LocalConfigEntry<float> HoldTime;
        // Debug (not synced)
        public readonly LocalConfigEntry<bool> DebugMode;

        internal LocalConfig(ConfigFile configFile)
        {
            Instance = this;
            
            configFile.SaveOnConfigSet = false;
            
            // General 
            ClimbSpeedMultiplier          = new SyncedConfigEntry<float>(configFile, "General", nameof(ClimbSpeedMultiplier), 1.0f, new ConfigDescription("Ladder climb speed multiplier (1 in vanilla)", new AcceptableValueRange<float>(0.1f, 10.0f)), 1.0f);
            SprintingClimbSpeedMultiplier = new SyncedConfigEntry<float>(configFile, "General", nameof(SprintingClimbSpeedMultiplier), 1.5f, new ConfigDescription("Additional climb speed multiplier while sprinting (1 in vanilla)", new AcceptableValueRange<float>(0.1f, 10.0f)), 1.0f);
            TransitionSpeedMultiplier     = new SyncedConfigEntry<float>(configFile, "General", nameof(TransitionSpeedMultiplier), 1.0f, new ConfigDescription("Ladder enter/exit animation speed multiplier (1 in vanilla)", new AcceptableValueRange<float>(0.1f, 10.0f)), 1.0f);
            AllowTwoHanded                = new SyncedConfigEntry<bool>(configFile, "General", nameof(AllowTwoHanded), true, "Whether ladders can be climbed while carrying a two-handed object (false in vanilla)", false);
            // General (not synced)
            ScaleAnimationSpeed           = new LocalConfigEntry<bool>(configFile, "General", nameof(ScaleAnimationSpeed), true, "Whether the climbing animation is scaled to the climbing speed (false in vanilla)");
            HideOneHanded                 = new LocalConfigEntry<bool>(configFile, "General", nameof(HideOneHanded), true, "Whether one-handed items should be hidden while climbing a ladder (false in vanilla)");
            HideTwoHanded                 = new LocalConfigEntry<bool>(configFile, "General", nameof(HideTwoHanded), true, "Whether two-handed items should be hidden while climbing a ladder (false in vanilla)");
            // Extension Ladders 
            ExtensionTime                 = new SyncedConfigEntry<float>(configFile, "Extension Ladders", nameof(ExtensionTime), 0.0f, new ConfigDescription("How long extension ladders should stay extended, in seconds (0 for permanent) (20 in vanilla)", new AcceptableValueRange<float>(0.0f, 3600.0f)), 20.0f);
            ExtensionSpeedMultiplier      = new SyncedConfigEntry<float>(configFile, "Extension Ladders", nameof(ExtensionSpeedMultiplier), 1.0f,  new ConfigDescription("Multiplies how fast the ladder extends once activated (1 in vanilla)", new AcceptableValueRange<float>(0.1f, 100.0f)), 1.0f);
            ExtensionDelay                = new SyncedConfigEntry<float>(configFile, "Extension Ladders", nameof(ExtensionDelay), 1.0f, new ConfigDescription("How long before the ladder begins to extend once activated, in seconds (1 in vanilla)", new AcceptableValueRange<float>(0.0f, 15.0f)), 1.0f);
            FallSpeedMultiplier           = new SyncedConfigEntry<float>(configFile, "Extension Ladders", nameof(FallSpeedMultiplier), 1.0f, new ConfigDescription("Multiplies how fast the ladder falls once fully extended (1 in vanilla)", new AcceptableValueRange<float>(0.1f, 10.0f)), 1.0f);
            FallDelay                     = new SyncedConfigEntry<float>(configFile, "Extension Ladders", nameof(FallDelay), 0.4f, new ConfigDescription("How long before the ladder begins to fall once fully extended, in seconds (0.4 in vanilla)", new AcceptableValueRange<float>(0.0f, 15.0f)), 0.4f);
            EnableKillTrigger             = new SyncedConfigEntry<bool>(configFile, "Extension Ladders", nameof(EnableKillTrigger), true, "Whether extension ladders should kill players they land on (true in vanilla)", true);
            // Extension Ladders (not synced)
            HoldToPickup                  = new LocalConfigEntry<bool>(configFile, "Extension Ladders", nameof(HoldToPickup), true, "Whether the interact key needs to be held to pick up an activated extension ladder");
            HoldTime                      = new LocalConfigEntry<float>(configFile, "Extension Ladders", nameof(HoldTime), 0.5f, new ConfigDescription("How long the interact key must be held if holdToPickup is true, in seconds", new AcceptableValueRange<float>(0.0f, 15.0f)));
            // Debug (not synced)
            DebugMode                     = new LocalConfigEntry<bool>(configFile, "Debug", nameof(DebugMode), false, "Displays debug messages in the BepInEx console if true");
            
            ClearOrphanedEntries(configFile);
            configFile.Save();
            configFile.SaveOnConfigSet = true;
        }

        private static void ClearOrphanedEntries(ConfigFile configFile)
        {
            var orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries"); 
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(configFile); 
            orphanedEntries.Clear(); 
        }
    }
}

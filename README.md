# BetterLadders
## Features
- Change climbing speed and sprint-climbing speed
	- The climbing animation speed can be scaled accordingly
- Change the speed of the animation to enter/exit a ladder
- Use ladders while holding a two-handed item
- Hide the held item while climbing a ladder
	- Visibility is synced with other clients, as long as the host has this mod
- Change how long extension ladders stay extended (or make them permanent)
- Require holding the interact button to pick up an extended extension ladder
	- Hold time can be adjusted or disabled

## Config
### General
| entry                         | default | description                                                       | synced with host |
|-------------------------------|:-------:|-------------------------------------------------------------------|:----------------:|
| ClimbSpeedMultiplier          |   1.0   | Ladder climb speed multiplier                                     |       `✓`        |
| SprintingClimbSpeedMultiplier |   1.5   | Additional climb speed multiplier while sprinting                 |       `✓`        |
| TransitionSpeedMultiplier     |   1.0   | Ladder enter/exit animation speed multiplier                      |       `✓`        |
| AllowTwoHanded                |  true   | Whether ladders can be climbed while carrying a two-handed object |       `✓`        |
| ScaleAnimationSpeed           |  true   | Whether the climbing animation is scaled to the climbing speed    |       `✓`        |
| HideOneHanded                 |  true   | Whether one-handed items should be hidden while climbing a ladder |       `✗`        |
| HideTwoHanded                 |  true   | Whether two-handed items should be hidden while climbing a ladder |       `✗`        |

### Extension Ladders
| entry                    | default | description                                                                                   | synced with host |
|--------------------------|:-------:|-----------------------------------------------------------------------------------------------|:----------------:|
| ExtensionTime            |   0.0   | How long extension ladders should stay extended, in seconds (0 for permanent) (20 in vanilla) |       `✓`        |
| ExtensionSpeedMultiplier |   1.0   | Multiplies how fast the ladder extends once activated                                         |       `✓`        |
| ExtensionDelay           |   1.0   | How long before the ladder begins to extend once activated, in seconds (1 in vanilla)         |       `✓`        |
| FallSpeedMultiplier      |   1.0   | Multiplies how fast the ladder falls once fully extended                                      |       `✓`        |
| FallDelay                |   0.4   | How long before the ladder begins to fall once fully extended, in seconds (0.4 in vanilla)    |       `✓`        |
| EnableKillTrigger        |  true   | Whether extension ladders should kill players they land on                                    |       `✓`        |
| HoldToPickup             |  true   | Whether the interact key needs to be held to pick up an activated extension ladder            |       `✗`        |
| HoldTime                 |   0.5   | How long the interact key must be held if holdToPickup is true, in seconds                    |       `✗`        |

### Debug
| entry     | default | description                                              |
|-----------|:-------:|----------------------------------------------------------|
| DebugMode |  false  | Whether to display debug messages in the BepInEx console |

## Building
[NetcodePatcher](https://github.com/EvaisaDev/UnityNetcodePatcher) must be installed with `dotnet tool install -g Evaisa.NetcodePatcher.Cli`  
To create a release zip in `BetterLadders/artifacts`, run `dotnet publish -c Release`  
This also generates a thunderstore.toml file to allow publishing from [tcli](https://github.com/thunderstore-io/thunderstore-cli)  

Publishing to Thunderstore can be done automatically via GitHub Actions:
1. Set Namespace, Product, Version, and RepositoryUrl in BetterLadders.csproj
1. Set TS_API_KEY in the repository's secrets
1. Run the `Build and Publish` workflow from the Actions page in GitHub
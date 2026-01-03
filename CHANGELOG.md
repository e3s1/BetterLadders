# Changelog
## 1.5.0
- Updated to Lethal Company v73
- Added ExtensionSpeedMultiplier, FallSpeedMultiplier, ExtensionDelay, and FallDelay config options
- Animation speed is now synced to other players
- Added support for LethalConfig and LobbyCompatibility
- Config values now take effect if changed without restarting the game
- Remade config syncing
- Better transpiler logging when DebugMode = true
## 1.4.3
- Transpilers are now unpatched correctly
- IL code is now only logged if debugMode is true in the config
## 1.4.2
- Fixed broken directory structure in zip file
## 1.4.1
- Fixed enableKillTrigger not resetting when leaving a lobby
## 1.4.0
- Added new config options
	- transitionSpeedMultiplier - Multiplier for the time between pressing interact and entering/exiting a ladder
	- enableKillTrigger - Whether or not extension ladders can kill players while falling
- Held item visibility is now synced between clients as long as the host has the mod
	- Clients who have hideOneHanded disabled will still see other players items, same with hideTwoHanded
- Bug fixes:
    - Fixed a vanilla bug where ladder animation would play while holding left/right
    - Progress bar no longer appears when trying to pick up an extension ladder while holding a two handed object or while it is not grabbable
	- Fixed climb animation not playing for other clients
    - Fixed transpilers duplicating code when a client leaves and joins another game
    - Transpilers that rely on synced config values should now actually use the synced config values
	    - Previously, they were running on game launch, so they were using the client's config, not the host's config
- Reorganized all patches
- Moved changelog to CHANGELOG.​md
## 1.3.0
- Added new config options for extension ladders
	- timeMultiplier - Extension ladder time multiplier (0 for permanent)
	- holdToPickup - Whether the interact key needs to be held to pick up an activated extension ladder
	- holdTime - How long, in seconds, the interact key must be held if holdToPickup is true
## 1.2.3
- Fixed NullReferenceException when climbing ladder with a ReservedItemSlot mod installed
	- Currently, the reserved item won't be hidden if climbing a ladder while holding it
- Added config syncing with host
	- If joining a host who doesn't have this mod, config options that can be synced will be set to their vanilla defaults
## 1.2.2
- When using a mod that adds hotkeys to switch item slots, items are properly hidden
	- Tested with [HotbarPlus](https://thunderstore.io/c/lethal-company/p/FlipMods/HotbarPlus/)
## 1.2.1
- Fixed README.​md
## 1.2.0
- Added new config options
    - hideOneHanded
    - hideTwoHanded
- Fixed ladder animation playing while not moving
## 1.1.0
- Added new config options
    - sprintingClimbSpeedMultiplier
    - scaleAnimationSpeed
## 1.0.1
- Fixed NullReferenceException when climbing ladder without two-handed object
## 1.0.0
- Release

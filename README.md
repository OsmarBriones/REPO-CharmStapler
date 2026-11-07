# Charm Stapler

Fastener: hits and shoves cannot make you drop what you hold.

> **Everyone in the lobby must install this mod, together with Pockets and CharmCore.** The power changes what your own character can do and its state shows on your inventory slot, so friends without the mods would see an empty slot and be unable to use the pocket.

## How to Use

1. Find the **Stapler** valuable and store it in a free inventory slot with [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/) (grab it, press the slot key).
2. While it stays in your pocket, hits on the thing you are holding and shoves from other players can no longer knock it out of your hands. Each time the Stapler saves your grip, it loses a little of its value.
3. Take the valuable out of the pocket and the power stops.

## Features

- **Fastener:** hits on the object you hold, hard knocks and shoves cannot make you let go of it. Letting go on purpose, running out of stamina, dying and monsters that grab things still work as usual. Climbing is not affected.
- **It costs the valuable:** each time it saves your grip it wears away a share of its value, and a worn-out valuable breaks. A hit that lasts a moment is charged once.
- **Everything is configurable:** wear, the pause between charges and an on/off switch. The host's choices apply to everyone.

## Requirements
- [BepInEx Pack for R.E.P.O.](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)
- [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/)
- [CharmCore](https://thunderstore.io/c/repo/p/OsmarBriones/CharmCore/)

## Installation
1. Install through r2modman / Thunderstore Mod Manager (Pockets and CharmCore come with it), or place `CharmStapler.dll` next to `Pockets.dll` and `CharmCore.dll` in `BepInEx/plugins`. **All players need it.**
2. Launch the game once; the configuration file is created inside `BepInEx/config`.

## Configuration
Settings are in `com.osmar.CharmStapler.cfg` inside `BepInEx/config`, section `Charm.Stapler`. The host's choices apply to everyone.

- `Enabled` (default `true`): turn this charm on or off.
- `WearPercent` (default `10`): share of the valuable's original value lost each time your grip is saved.
- `WearCooldownSeconds` (0 to 5, default `0.5`): saves within this time of the last charge cost no extra wear.
- `EnableBlocking` (0 or 1, default `1`): 0 switches the power off (safety switch).

Section `Debug`: `DebugMode` (default `false`) places a Stapler on the floor in front of you when a level starts (host or singleplayer) and writes extra log lines. For testing. `SpawnNow` (default `false`, needs `DebugMode`): set it to `true` while a level is running to place one right away; it switches itself back off.

## Issues & Bug Reports
The official way to report issues, suggest improvements, or submit feedback is by opening an issue on the official GitHub repository:
👉 [GitHub Issues](https://github.com/OsmarBriones/REPO-CharmStapler/issues)

## Credits
Developed by **Osmar Briones**

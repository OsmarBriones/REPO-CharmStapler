# CharmStapler Architecture

A Charm mod (layer 3 of the PocketCharms family): one valuable, one power. It depends on **Pockets** (storage) and **CharmCore** (registry, wear, cooldown display, host settings, debug mode).

## The power: Fastener

Hits and shoves cannot make the owner drop what they hold. Only the forced releases caused by a hit, a knock-out or a shove are stopped.

### How the game releases a grab
`PhysGrabber.OverrideGrabRelease` (multiplayer: the host sends `ReleaseObjectRPC` to the owner), `ReleaseObject` and `ReleaseObjectRPC` are shared by dozens of unrelated features (monsters, traps, shop, vehicles), so they cannot be blocked blindly. Origins we block:

| Origin | Singleplayer | Multiplayer (host) |
|---|---|---|
| Held object struck (`physReleaseGrab`), `HurtCollider.ColliderCheck` ~:509-519 | `ReleaseObject` | `OverrideGrabRelease` |
| Knock-out, `HurtCollider.PhysObjectHurt` ~:886-896 (hitForce >= 5) | `ReleaseObjectRPC` | `OverrideGrabRelease` |
| Shove, `PlayerTumble.TumbleImpactRPC` ~:595-599 | `ReleaseObjectRPC` | `OverrideGrabRelease` |

### Hooks (smallest set covering both modes)
- Windows (a marker set while that host code runs; `Domain/ReleaseWindow`, valid only in the frame it was entered): `PlayerTumble.TumbleImpactRPC`, `HurtCollider.PhysObjectHurt` (prefix + finalizer), and the compiler-generated `MoveNext` of the `HurtCollider.ColliderCheck` coroutine (patched by hand in `Patches/ReleaseWindowPatches.cs`; the loop that releases has no `yield`, so one `MoveNext` call contains the whole release).
- Blockers (prefix returning false): `PhysGrabber.OverrideGrabRelease`, `ReleaseObject`, `ReleaseObjectRPC`. Each asks `Fastener.ShouldBlock` and does nothing outside a window.
- Decision (`Fastener.ShouldBlock`): window open, `EnableBlocking` on, the grabber really holds the target object (`grabbed` plus the held object's view id, like `ReleaseObject` itself checks), and the grabber's avatar carries the Stapler (`CharmsApi.ForEachCarrier`, which uses Pockets' tracking of every player's pockets and honours Enabled / CoreEnabled). It runs where the release is requested: the host, or singleplayer. The receiving `ReleaseObjectRPC` is shared by every cause, so the owner's machine cannot tell a hit from a monster.
- Wear: `Charm.WearValuable(valuable, WearPercent)` on the host, once per `WearCooldownSeconds` per player (`Domain/WearThrottle`).

### What exactly is blocked, and what is left alone
Skipping the release call skips everything it does: no grab detach, and no `grabDisableTimer` is set (in multiplayer the RPC is never sent, so the owner's timers stay untouched). The shove's tumbling, force freeze and the hit's physics push on the held object are separate code and still happen. If the owner holds nothing, the call is not blocked, so vanilla timers apply and no wear is charged. A climb holds no object, so climbing is not protected. Not touched: release on death (`PlayerAvatar` ~:1594-1640), stamina release (`PhysGrabber` ~:1053), voluntary release, distance release, monsters, traps, shop, vehicles, and everything else that calls the three methods outside the windows.

## Game members used

Checked by `CharmStapler.Tests/GameContractTests.cs`: `PhysGrabber.OverrideGrabRelease/2`, `ReleaseObject/2`, `ReleaseObjectRPC/5`, fields `playerAvatar`, `grabbed`, `grabbedPhysGrabObject`, `grabbedStaticGrabObject`; `PlayerTumble.TumbleImpactRPC/2`; `HurtCollider.PhysObjectHurt/7`, `ColliderCheck/0`, nested `<ColliderCheck>*.MoveNext`, field `physReleaseGrab`. Also used but inherited from Photon/Unity (not testable there): `photonView.ViewID` of the grab objects.

## Configuration (host-synchronised)
- `WearPercent` 0-100, default 10 (CharmCore): per prevented release.
- `WearCooldownSeconds` float 0-5, default 0.5: 0 charges every prevented release.
- `EnableBlocking` 0/1, default 1: safety switch; below 0.5 means off.

## Decisions
- Window plus blockers instead of patching the three methods alone, because they have many unrelated callers.
- The decision is on the host through `ForEachCarrier`, not on the owner, for the reason above. Everybody installs the mod (Category 2), so the host always has the patches.
- `Fastener` is a static class, not a MonoBehaviour: nothing runs per frame.
- `EnableBlocking` defaults to 1 (the hook set is small and was read from the code) but it is a kill switch because nothing could be run in game.
- Exceptions inside the decision are caught and let the release happen (never break the game's flow).

## Debug mode

Provided by CharmCore: the `Debug.DebugMode` setting (off by default) places the valuable on the floor at the start of each level and enables `Charm.LogDebug` output.

## Known limits

- Looked up by exact prefab name; a game update that renames the prefab needs a mod update.
- Not verified in game: the whole effect (see the QA battery). In particular that `PhysGrabber.grabbed` is true on the host for remote players (read from `PhysGrabObject.GrabLinkRPC`), that Harmony can patch the coroutine `MoveNext`, and that nothing else inside those host windows asks to release a carrier (UnityEvents such as `onImpactAny`).
- Other releases can still drop the object after a shove: the distance release and a destroyed object.

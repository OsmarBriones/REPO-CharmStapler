# QA battery: Fastener (CharmStapler)

Deploy: Pockets, CharmCore and CharmStapler are in the r2modman `Debug` profile (compare hashes with the builds). Restart the game first.

## Quick start with debug mode
Set `DebugMode = true` in the `Debug` section of `BepInEx/config/com.osmar.CharmStapler.cfg` (off by default). When a level starts, a Stapler is placed on the floor about 2.5 m in front of you; the log then also shows the detailed lines. Any level works.

## Single player
- [ ] Log shows `Charm registered: Stapler - Fastener` and `CharmStapler 1.0.0 loaded!`.
- [ ] Store the valuable in a slot: the slot shows its picture and value.
- [ ] With the Stapler pocketed, hold a valuable and let an enemy hit it (or have it struck by a swinging object): you keep holding it; the Stapler loses about 10% of its original value; the log (DebugMode) shows `Prevented a forced release`.
- [ ] Repeat with a heavy hit on the held object (knock-out): you keep the grip, one wear.
- [ ] Get hit repeatedly within half a second: only one wear.
- [ ] Shove test (second player tumbles into the carrier): the carrier keeps holding. Without the Stapler in the pocket, the same hit or shove makes you drop the object.
- [ ] Hit while holding nothing: no wear, no change.
- [ ] Check what is NOT blocked: letting go by hand, running out of stamina, dying, being grabbed by a monster or trap, being knocked off a climb.
- [ ] Take the valuable out: the power stops.
- [ ] Use it until the valuable breaks: the slot empties, the power stops, no errors in the log.

## Settings
- [ ] `WearCooldownSeconds = 0`: every prevented release wears. `WearPercent = 0`: no wear but still blocks. `EnableBlocking = 0`: hits make you drop things as in vanilla.
- [ ] Charm `Enabled = false`: no effect and no wear. CharmCore `Enabled = false`: nothing works.

## Multiplayer (host and client)
- [ ] The effect works for a client carrying the Stapler (decided on the host), the value drops on both machines.
- [ ] If the log shows `the hit-on-held-object hook was not applied`, report it: hits on held objects will still release.
- [ ] Host settings override the client's.
- [ ] Client leaves and rejoins: no stuck veil.

Log patterns: `Charm registered`, `threw`, `Exception`.

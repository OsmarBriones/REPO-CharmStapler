## User Scenarios

### User Story 1 - Fastener (Priority: P1)
A player stores the Stapler in a pocket and gets this power while it stays there: hits and shoves cannot make you drop what you hold.
**Acceptance**: with the valuable pocketed the power works; with it out of the pocket it does not; each prevented release wears the valuable (once per WearCooldownSeconds).

## Requirements
- FR-001: Registers a charm with CharmCore for the prefab Valuable Arctic Stapler.
- FR-002: Settings through CharmCore: Enabled, WearPercent, WearCooldownSeconds, EnableBlocking, DebugMode; host-authoritative except DebugMode.
- FR-003: All players install it (Category 2); depends on Pockets and CharmCore.
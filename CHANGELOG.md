# Changelog

## [Unreleased]

### Fixed
- Runtime hygiene: the wear records are cleared when a level ends, and debug log lines in the release patches are only built in debug mode.

### Added
- Fastener: hits, knock-outs and shoves cannot make you drop what you hold (one wear per prevented release, `WearCooldownSeconds` 0-5 default 0.5, `EnableBlocking` 0/1 default 1).
- `Debug.DebugMode` (off by default): puts the Stapler on the floor at the start of each level and writes detailed logs.

# Changelog

## [0.7.0] - 2026-10-26

### New

- SmallOwl enemy animations, animator controller, and flying/attack clips.
- Fox and FoxShielded enemy animations, spritesheets, and animator controllers.
- EnemyAnimations_Test scene for isolated animation and behavior validation.
- Player stone-grabbing idle and movement animation clips.
- Player grapple movement toward target animations.
- Player sneeze attack animation and spritesheet asset.
- Interactive Wwise audio integration managed by AudioManager and MusicManager.
- Interactive music system supporting Overworld, Shop, Menu, Dungeon, and BossFight states.
- Wwise 3D spatialized sound effect event mappings for player, enemies, environment, and UI.
- Manual Main SoundBank preloading and unloading memory lifecycle handling.
- Drop shadow system for Player, Fox, FoxShielded, and SmallOwl prefabs.
- Exclamation mark alert indicator visual system for enemy detection states.
- Scratch attack visual FX spritesheets and prefabs for Fox and SmallOwl enemies.
- Sneeze stun animations for Fox and SmallOwl entities.

### Changes

- Dynamic horizontal sprite flipping for Fox and SmallOwl based on movement direction.
- Refactored prefab structures and organized scripts into dedicated folders for Fox, FoxShielded, and SmallOwl.
- Renamed primary player movement state from Locomotion to Move in FSM logic.
- Reorganized player animation clips and controllers into dedicated directory structure.
- Refactored MusicManager initialization and added playing ID guards to eliminate duplicated audio playback.
- Updated MusicManager codebase for compatibility with Wwise 2025 SDK.
- Synchronized player sneeze particle emission with animation frame timing.
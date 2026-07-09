# Context: Kids 2D Tracing Game

## General & Tech Stack
- **Engine**: Unity `6000.0.74f1`
- **Platform**: iOS, Landscape lock, 60 FPS, adapt 19.5:9 to 4:3 (iPhone 15 Pro, iPad Pro 12.9")
- **Input**: Tap, Swipe
- **Core Packages**: Zenject (Extenject), UniTask, Addressables, Newtonsoft Json
- **Architecture**: MVx (Menu), Data-Driven (Game)
- **Constraints**: Console must be absolutely clean (no errors/exceptions).
- **Assets**: `Assets/Content`. UI must perfectly match `References/` screenshots.
- **Data**: Level configs use JSON. Addressables used for Menu icons & Game assets.

## EditMode Tests
- Validate JSON level configs.
- Fail on: Invalid JSON schema, unresolved Addressables key, empty route (no segments, or segment <2 points), duplicate level IDs.

## Menu Scene
- **Layout**: Categories (vertical scroll) -> "Trace letters", "Trace numbers", "Trace shapes".
- **Levels**: 7 levels per category (horizontal scroll). Represent 7 rainbow colors (red->violet).
- **Action**: Tap level icon -> load Game scene. Match reference UI precisely (iPhone vs iPad).

## Game Scene
- **Flow**: 
  1. Show semi-transparent target silhouette.
  2. Voice instruction ("Follow the stars and trace the...").
  3. Route UI fades in (1s).
  4. Tracing.
  5. On all strokes complete: Random praise voice -> auto-transition to next level (loops category if finished).
- **Exit**: Top-left Home button returns to Menu.
- **Tracing Engine (Strictly Data-Driven)**: 
  - Universal logic, zero hardcoded glyph-specific branches. 
  - Glyph = ordered strokes. Stroke = directed path.
  - Adding a new glyph requires ONLY a config entry and asset.
- **Tracing Mechanics**:
  - Route UI: Chain of circles ending in a star.
  - Cursor: Mascot spaceship. Trail color set by level config.
  - Corridor: Configurable width. Finger must stay within.
  - Progress: Forward movement only. Out-of-bounds, reverse, or finger-lift pauses painting (no reset).
  - Reach star -> stroke completes -> next stroke appears.

## Hints (Inactivity)
- Timer resets on any tap/swipe.
- **7s**: Repeat voice instruction.
- **14s**: Show cyclic animated finger hint on route. Hides on draw start.

## Additional notes
- DO NOT build the solution.

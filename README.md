# Letter Tracing Game

2D Unity mobile tracing game for children to learn letters, numbers, and shapes. The player follows ordered strokes, stays inside a configurable corridor, and completes each level by reaching the final star.

## Architecture

- **Menu:** MVP style category and level presenters.
- **Game:** data-driven tracing pipeline. Levels are JSON configs, glyphs are ordered strokes, and adding new content should only require a config entry plus assets.
- **Presentation:** Zenject factories create scene presenters/views. `GameLevelPresenter` owns tracing progress, while path UI, pointer, and helper animations live in `LinePathView`.
- **Assets:** Addressables are used for level graphics and audio.
- **Validation:** EditMode tests validate level JSON, route shape, duplicate IDs, and Addressables references.

## Guidelines

Project guidelines live in `.junie/AGENTS.md`.

## Used Packages

- Zenject / Extenject
- DOTween
- UniTask
- Addressables
- Newtonsoft Json
- Unity Splines
- Unity Input System
- Unity Test Framework

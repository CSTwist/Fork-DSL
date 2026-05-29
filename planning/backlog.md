# MobaDSL Backlog and Milestones Registry

This registry tracks the backlog, issue labels, and stage milestones for the development of the MobaDSL runtime and compiler.

## Issue Labels Schema

To categorize issues and tasks:

| Label | Description |
|---|---|
| `compiler` | Lexer, parser, static type checkers, and IR generation. |
| `runtime` | Simulation tick loops, combat rules, stats, and effects. |
| `unity-editor` | Scripted importers, custom inspector GUIs, and compile panels. |
| `mobile` | Virtual joysticks, touch aim indicators, and target-lock systems. |
| `multiplayer` | Rollback synchronizations, netcode entities, and serialization. |
| `docs` | Language references, onboarding guides, and tutorials. |
| `core` | Free/Open-core package boundaries. |
| `pro-candidate` | Proprietary dashboards, simulators, and advanced templates. |
| `blocked` | Dependency blocking or decision gate outstanding. |

---

## Roadmap Milestones

### Milestone 0: Foundation and Assembly Verification
- **Goal:** Set up package layout, assemblies, baseline configuration, NUnit smoke tests, and coding standards.
- **Status:** **In Progress** (Increment 0)

### Milestone 1: Core Combat Simulation Engine
- **Goal:** Enable discrete simulation ticks, stats modifiers, damage calculations, and manual ability configuration.
- **Status:** Planned (Increment 1 & 2)

### Milestone 2: DSL Compiler Alpha
- **Goal:** Core parser syntax, symbol validation, IR serialization, and automatic Unity asset importer.
- **Status:** Planned (Increment 3 - 6)

### Milestone 3: Playable Mobile Arena Slice
- **Goal:** 4-Hero kit coverage, virtual touch controls, item shop, waves, towers, and local 1-lane win conditions.
- **Status:** Planned (Increment 7 - 10)

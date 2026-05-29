# MobaDSL for Unity — Comprehensive Incremental Development Plan

**Project:** Mobile MOBA Domain-Specific Language and Unity Runtime  
**Working package name:** `Fork`  
**Unity package ID:** `com.fork.dsl`  
**Document type:** Execution roadmap / engineering playbook  
**Version:** 1.0  
**Prepared:** 2026-05-29  
**Target developer:** Solo founder initially, expandable to contributors or contractors  
**Primary product thesis:** Let Unity developers author MOBA-style heroes, abilities, items and match rules in readable `.frk` files, compile them into validated gameplay definitions, and execute them through a reusable mobile-first Unity runtime.

---

## 1. Purpose of This Plan

This plan turns the MobaDSL concept into an incremental implementation sequence. It is intentionally ordered so that every major investment is justified by a playable or testable result.

The project is **not** treated as “build a full mobile MOBA.” It is treated as a developer tool with a progressively stronger demonstration game:

1. Prove that a reusable MOBA combat runtime works manually in Unity.
2. Prove that `.frk` files can describe real gameplay more cleanly than manual setup.
3. Prove that designers can iterate through importer diagnostics and tooling.
4. Prove that the runtime handles a mobile one-lane MOBA vertical slice.
5. Release a credible free core and validate demand.
6. Only then select and implement a commercial multiplayer path.
7. Turn the proven toolchain into a paid Unity product.

### Guiding rule

> No layer is expanded until the previous layer produces a concrete, tested artifact.

---

## 2. Fixed Decisions and Deferred Decisions

### 2.1 Decisions to Lock Now

| Decision | Choice | Reason |
|---|---|---|
| Host engine | Unity | Target audience and distribution strategy are Unity-centered. |
| Authoring format | Human-readable `.frk` source files | Enables diffable, testable and shareable content. |
| Compiler structure | Lexer/Parser → AST → Semantic Analysis → MOBA IR → Unity backend | Keeps language independent of runtime/backend changes. |
| First output | Unity serialized definition assets executed by generic C# runtime systems | Avoids generating bespoke C# per ability. |
| Initial simulation | Local/offline runtime | Validates product value before networking complexity. |
| Simulation/view boundary | Gameplay state separated from animations/VFX/audio/UI | Required for a future multiplayer-safe architecture. |
| Initial public positioning | MOBA gameplay authoring toolkit, not “generate a full MOBA” | More credible and achievable. |
| Product model | Open-core with paid Unity Pro toolkit later | Encourages adoption while preserving commercial value. |

### 2.2 Decisions to Defer Until Evidence Exists

| Deferred choice | Decision gate |
|---|---|
| Photon Quantum versus Unity Netcode for Entities backend | After the mobile one-lane local vertical slice and a dedicated networking spike. |
| Final free/pro boundary | After external beta feedback identifies what users value most. |
| Subscription or cloud services | Only after a local product has paid users and a hosted feature has proven demand. |
| Full 5v5 support | Only after one-lane small-match simulation and performance goals pass. |
| Visual node editor | Only after text DSL authoring is validated; do not replace syntax work prematurely. |

---

## 3. Product Scope

### 3.1 Product Owns

- `.frk` language, grammar, type rules and diagnostics.
- Compiler and backend-neutral intermediate representation.
- Unity importer, generated assets and authoring/editor tools.
- Generic MOBA runtime systems:
  - entities and stats;
  - targeting;
  - abilities and costs;
  - damage/healing;
  - modifiers and crowd control;
  - projectiles and areas;
  - items and progression;
  - minions, lanes, towers and objectives.
- Local mobile demo application.
- Automated compiler/runtime tests.
- Eventually, multiplayer adapters and balance/debug tooling.

### 3.2 Product Does Not Initially Own

- Matchmaking, accounts, rankings or live services.
- Anti-cheat beyond simulation architecture guidance.
- Complete 5v5 production game.
- High-end artwork, animation packs or monetization systems.
- Custom renderer or full Unity replacement.
- Arbitrary user scripting inside DSL v1.

### 3.3 Initial Value Proposition

> Define, validate and run MOBA-style abilities and combat rules in Unity through readable data-first source files, with a mobile-ready demo and extensible runtime.

---

## 4. Definition of Success by Stage

| Stage | Proof required before proceeding |
|---|---|
| Runtime proof | Two heroes can fight locally with manually-authored abilities using reusable systems and automated combat tests. |
| DSL proof | The same hero kits can be authored in `.frk`, imported into Unity, diagnosed clearly when invalid, and run without per-hero scripts. |
| Vertical-slice proof | One mobile-controlled lane with minions, towers, purchases and win conditions can be played locally. |
| Demand proof | External Unity developers download/test the core or join a waitlist and provide actionable feedback. |
| Paid-product proof | A polished package has documentation, samples, onboarding, support path and features exceeding the free core. |
| Multiplayer proof | A backend spike demonstrates responsive combat, authoritative/deterministic behavior and acceptable device performance. |

---

## 5. Architecture Overview

```text
.frk source files
       |
       v
Compiler Frontend
  Lexer -> Parser -> AST
       |
       v
Semantic Analyzer
  names, types, units, references, formula safety, diagnostic spans
       |
       v
Backend-Neutral MOBA IR
  stable IDs, effect instructions, triggers, content metadata
       |
       +----------------------------+
       |                            |
       v                            v
Unity Local Backend             Future Backend Adapter
ScriptableObject/binary data    Photon Quantum or Netcode for Entities
       |                            |
       v                            v
Reusable Simulation Runtime     Networked Simulation Runtime
       |
       v
Unity Presentation / Mobile UI / Demo Scene
```

### 5.1 Core Architectural Principles

1. **The DSL declares gameplay.** It does not directly control GameObjects, animations or network messages.
2. **The runtime implements primitives.** Abilities are combinations of tested primitives such as damage, status, projectile, dash and area.
3. **The IR is the stable contract.** Unity asset output, simulation and future tooling consume the same validated semantic model.
4. **Presentation never changes outcomes.** VFX, audio and animation can observe gameplay events but not calculate combat results.
5. **Deterministic-safe habits start early.** Ticks, seeded randomness, stable IDs and isolated state make future multiplayer feasible.

---

## 6. Repository and Package Structure

Start with a Git repository that contains both a Unity development project and the reusable package.

```text
mobadsl/
  README.md
  LICENSE.md                         # choose only when publishing code
  CHANGELOG.md
  CONTRIBUTING.md                    # add before public release
  SECURITY.md                        # add before public release

  docs/
    product-vision.md
    language-spec/
      overview.md
      grammar.md
      types-and-units.md
      abilities.md
      effects.md
      diagnostics.md
    architecture/
      runtime.md
      compiler.md
      multiplayer-decision.md
    tutorials/
      create-first-hero.md
      create-first-ability.md

  packages/
    com.fork.dsl/
      package.json
      README.md
      CHANGELOG.md
      Runtime/
        MobaDSL.Runtime.asmdef
        Content/
        Simulation/
        Presentation/
        Utilities/
      Editor/
        MobaDSL.Editor.asmdef
        Compiler/
        Importers/
        Inspectors/
        Diagnostics/
      Tests/
        Runtime/
          MobaDSL.Runtime.Tests.asmdef
        Editor/
          MobaDSL.Editor.Tests.asmdef
      Samples~/
        BasicArena/
        HeroAbilities/
        MobileControls/
      Documentation~/

  unity-project/
    Assets/
      MobaDslDev/
        Content/
          Heroes/
          Items/
          Rules/
        Scenes/
          RuntimeSandbox.unity
          OneLaneDemo.unity
        ArtPlaceholder/
    Packages/
      manifest.json

  tools/
    schema/
    generated-docs/
    validation-cli/                  # later

  planning/
    adr/
      ADR-0001-data-first-runtime.md
      ADR-0002-ir-boundary.md
      ADR-0003-simulation-presentation-separation.md
    backlog.md
    release-checklists/
```

### Repository rule

The `unity-project/` validates and demonstrates the package. Production code intended for reuse belongs inside `packages/com.fork.dsl/`, not directly under the demo project's `Assets/` folder.

---

## 7. Incremental Product Ladder

| Increment | Deliverable | What the developer can do after it |
|---:|---|---|
| 0 | Architecture/repository foundation | Build, test and track design decisions consistently. |
| 1 | Manual runtime kernel | Create a hero with stats, damage, health and events in C#. |
| 2 | Manual ability engine | Configure several reusable ability primitives without a DSL. |
| 3 | Compiler frontend alpha | Parse `.frk` syntax and produce precise errors. |
| 4 | Unity importer/backend | Save a `.frk` file and run its compiled ability in Unity. |
| 5 | Ability breadth | Author varied hero kits using composition, not custom scripts. |
| 6 | Mobile arena demo | Play touchscreen combat with multiple abilities and bots. |
| 7 | One-lane MOBA vertical slice | Play minions/towers/items/win condition locally. |
| 8 | Public core beta | External users can install, learn and test the free tool. |
| 9 | Multiplayer evaluation | Choose a backend using measured evidence. |
| 10 | Pro candidate | Sell a polished authoring toolkit with high-value features. |

---

# Part I — Foundation and Runtime Proof

## 8. Increment 0 — Project Foundation and Architectural Guardrails

### Goal

Establish a development environment that can support a long-lived Unity package rather than an improvised game prototype.

### Deliverables

- Git repository initialized.
- Unity development project created using a currently supported Unity version selected for package work.
- Embedded/local package `com.fork.dsl` referenced from the Unity project.
- `Runtime`, `Editor` and `Tests` assembly definition separation.
- Continuous integration workflow for Unity tests, or at minimum documented repeatable local test commands.
- Three initial architecture decision records:
  - data-first definitions instead of generated per-hero C#;
  - IR boundary;
  - simulation/presentation separation.
- Issue board with milestone labels.

### Engineering Tasks

- [ ] Create repository and `.gitignore` suitable for Unity.
- [ ] Create Unity package manifest and package folder layout.
- [ ] Add namespaces:
  - `MobaDSL.Runtime`
  - `MobaDSL.Editor`
  - `MobaDSL.Compiler`
  - `MobaDSL.Tests`
- [ ] Add assembly definitions with editor-only code excluded from builds.
- [ ] Add NUnit/Unity Test Framework smoke test.
- [ ] Create placeholder `RuntimeSandbox` scene.
- [ ] Write coding conventions:
  - deterministic-safe data structures;
  - no VFX reference inside core combat calculation;
  - no magic string IDs without identifier wrapper or registry;
  - all public systems covered by tests.
- [ ] Define issue labels: `compiler`, `runtime`, `unity-editor`, `mobile`, `multiplayer`, `docs`, `core`, `pro-candidate`, `blocked`.

### Exit Criteria

- Unity project compiles with package installed.
- One edit-mode test and one play-mode test run successfully.
- Runtime assembly does not reference Editor assembly.
- Architectural decisions are documented before combat code begins.

### Do Not Build Yet

- Parser.
- Multiplayer.
- Ability editor.
- Marketplace assets.
- Public announcement promising production readiness.

---

## 9. Increment 1 — Manual Simulation Runtime Kernel

### Goal

Build the smallest reliable combat simulation before any language syntax exists.

### Why This Comes First

A DSL that targets an unstable or weak runtime only creates elegant ways to describe broken behavior. The runtime is the product foundation.

### Runtime Data Model

Implement the following conceptual model:

```csharp
EntityId
CombatEntityState
  - EntityId Id
  - TeamId Team
  - TransformState Position/Direction
  - StatBlock BaseStats
  - StatModifierCollection ActiveModifiers
  - ResourcePool Health
  - ResourcePool Mana
  - StatusCollection Statuses
  - AbilityLoadout Abilities

SimulationTick
GameplayEventStream
```

### Minimum Systems

| System | Minimum responsibility |
|---|---|
| Entity registry | Create/read/remove stable combat entities. |
| Stats system | Resolve base value plus active modifiers. |
| Health/resource system | Spend mana, apply damage/healing and clamp values. |
| Team/faction system | Identify allies and enemies. |
| Tick runner | Progress simulation in discrete ticks, even if local only. |
| Event stream | Emit gameplay outcomes without coupling to visuals. |

### Tasks

- [ ] Define stable identifier value types: `HeroId`, `AbilityId`, `StatusId`, `EntityId`, `StatId`.
- [ ] Define `SimulationConfig` with tick rate.
- [ ] Implement entity creation and lookup.
- [ ] Implement health, mana and common stats.
- [ ] Implement damage packet/result types.
- [ ] Implement death state and respawn as a stub or explicitly excluded behavior.
- [ ] Implement gameplay events:
  - damage applied;
  - healed;
  - resource spent;
  - entity defeated.
- [ ] Implement placeholder visual observer that logs events in Play Mode.
- [ ] Create a sandbox scene with two placeholder capsules/sprites.

### Automated Tests

- [ ] Damage cannot reduce health below zero.
- [ ] Healing cannot exceed max health unless an explicit over-heal rule exists later.
- [ ] Mana cost cannot be paid when insufficient.
- [ ] Ally/enemy checks resolve correctly.
- [ ] Simulating the same fixed inputs for the same tick sequence gives the same result in the local runtime test harness.
- [ ] Visual observer can be removed without changing simulation test outputs.

### Demo Artifact

A sandbox with an Attack button or automated duel where one combat entity damages another, and the scene displays health changes by listening to gameplay events.

### Exit Criteria

- Combat results exist entirely in runtime state.
- Scene objects display state but do not calculate state.
- Core unit tests pass.
- The runtime supports a later ability layer without per-character logic.

---

## 10. Increment 2 — Manual Ability and Status-Effect Engine

### Goal

Prove the reusable primitive system manually before adding DSL compilation.

### Required Ability Primitives

Implement the smallest but expressive ability vocabulary:

| Primitive | First behavior |
|---|---|
| `SpendResource` | Spend mana before execution. |
| `CheckCooldown` / `StartCooldown` | Prevent recast until tick expiration. |
| `SelectTarget` | Self or single enemy in range. |
| `DealDamage` | Physical or magical classification, initially simple mitigation. |
| `Heal` | Restore health. |
| `ApplyStatus` | Apply slow or stun with duration. |
| `SpawnProjectile` | Projectile with travel and hit event. |
| `CreateArea` | Area hit on cast or expiration. |

### Ability Execution Shape

Use a data-driven definition even before the DSL exists:

```text
AbilityDefinition
  id
  targeting
  cast parameters
  costs[]
  cooldown
  triggers:
    OnCast -> effects[]
    OnHit -> effects[]
    OnExpire -> effects[]
```

### Tasks

- [ ] Build `AbilityDefinition` and effect definition types manually.
- [ ] Implement `AbilitySystem.TryCast`.
- [ ] Implement cooldown tracking by ticks.
- [ ] Implement single-target range validation.
- [ ] Implement projectile simulation without relying on presentation GameObjects.
- [ ] Implement `Slow` as a movement-speed stat modifier.
- [ ] Implement `Stun` as a control restriction.
- [ ] Implement simple damage-type mitigation.
- [ ] Create four manual abilities:
  - `IceBolt`: projectile damage plus slow;
  - `FireNova`: circular AoE damage;
  - `ShieldHeal`: self/ally healing or shield if shield is included;
  - `DashStrike`: postpone dash until a later primitive if movement complexity grows.

### Tests

- [ ] Ability fails without sufficient mana and does not begin cooldown.
- [ ] Cooldown expires after exact expected tick count.
- [ ] Projectile hit applies effects only once.
- [ ] Slow expires and restores original resolved movement speed.
- [ ] Stun prevents cast/action while active.
- [ ] Multiple effects execute in stable, documented order.
- [ ] Area effect filters allies/enemies correctly.

### Demo Artifact

A two-hero arena where abilities are bound manually and can be cast using keyboard controls. Placeholder VFX may observe cast/hit events.

### Exit Criteria

- At least three distinct ability behaviors are composed solely from reusable runtime primitives.
- No hero-specific combat script is necessary.
- It is now clear what the first DSL must express.

---

# Part II — Compiler and Unity Authoring Workflow

## 11. Increment 3 — DSL Language Minimum Viable Specification

### Goal

Write the first intentionally small language contract based on the runtime already proven.

### Syntax Scope for v0.1

Support only:

- hero declarations;
- stats;
- ability declarations;
- cooldown, cost, range and target rules;
- `cast` and `hit` trigger blocks;
- effects: `damage`, `heal`, `apply`;
- status declarations for `slow` and `stun`;
- number, duration, percentage and simple stat-scaled formulas.

### Illustrative v0.1 Source

```moba
status Slow {
    kind: movement_modifier
    stacking: refresh
}

hero FrostMage {
    stats {
        max_health: 580
        max_mana: 420
        ability_power: 0
        move_speed: 3.5
    }

    ability IceBolt {
        target: enemy_unit
        range: 8m
        cooldown: 6s
        cost: mana 40

        cast {
            projectile speed: 14mps
        }

        hit {
            damage magic: 120 + ability_power * 0.60
            apply Slow {
                amount: 30%
                duration: 2s
            }
        }
    }
}
```

### Language Design Rules

- Every numeric concept carries a domain unit when ambiguity is dangerous.
- Simulation times compile to integer ticks.
- Unknown identifiers are compilation errors, not runtime warnings.
- Formula operations are explicitly restricted.
- Versioned language header is optional in prototype and required before public beta.
- No arbitrary C# execution, reflection or Unity GameObject access from DSL.

### Tasks

- [ ] Create `docs/language-spec/overview.md`.
- [ ] Define tokens and reserved keywords.
- [ ] Define unit conversion rules (`s` to ticks, `%`, `m`, optionally `mps`).
- [ ] Define standard stats and built-in effect names.
- [ ] Define error code format such as `MDSL1001 UnknownIdentifier`.
- [ ] Produce 5 valid examples and 10 intentionally invalid examples.
- [ ] Review syntax against manually-configured abilities from Increment 2.

### Exit Criteria

- Every feature in syntax maps to a runtime behavior already implemented.
- There are no speculative language keywords without a runtime implementation plan.
- The language examples are concise enough to be a selling point.

---

## 12. Increment 4 — Compiler Frontend: Lexer, Parser and Diagnostics

### Goal

Parse `.frk` text into an AST and provide useful source-level diagnostics.

### Initial Technology Choice

Use a handwritten lexer and recursive-descent parser for v0.x. This keeps syntax easy to change while product discovery is ongoing. Reconsider a parser generator only after syntax stabilizes or parsing complexity becomes costly.

### Compiler Modules

```text
Editor/Compiler/
  Text/
    SourceText.cs
    TextSpan.cs
    SourceLocation.cs
  Lexing/
    TokenKind.cs
    Token.cs
    Lexer.cs
  Parsing/
    Parser.cs
    SyntaxNodes.cs
    ParseResult.cs
  Diagnostics/
    Diagnostic.cs
    DiagnosticBag.cs
    DiagnosticSeverity.cs
    ErrorCodes.cs
```

### Tasks

- [ ] Read `.frk` source text and preserve line/column spans.
- [ ] Tokenize identifiers, numbers, punctuation, strings if needed, comments and units.
- [ ] Parse document, hero, stats, ability, trigger and effect nodes.
- [ ] Add error recovery so one typo does not suppress all later diagnostics.
- [ ] Print diagnostics with file, line, column, message and suggestion where safe.
- [ ] Create compiler tests for valid and invalid files.
- [ ] Create golden test fixtures storing expected errors.

### Minimum Diagnostic Examples

```text
FrostMage.frk:14:19 MDSL1102
Unknown status 'Slwo'. Did you mean 'Slow'?

FrostMage.frk:7:19 MDSL1201
Expected duration such as '6s', but received 'six'.

FrostMage.frk:21:27 MDSL1304
Duration cannot be negative.
```

### Exit Criteria

- Compiler successfully parses all approved v0.1 sample files.
- Invalid examples produce exact, tested diagnostic codes and useful locations.
- No Unity scene is required to run parser tests.

---

## 13. Increment 5 — Semantic Analyzer and MOBA Intermediate Representation

### Goal

Turn parsed syntax into validated, backend-neutral gameplay meaning.

### Semantic Analyzer Passes

| Pass | Validates or resolves |
|---|---|
| Symbol collection | Unique hero, ability, status and stat identifiers. |
| Reference binding | Effect and status references exist. |
| Type/unit checking | Durations, distances, percentages and numeric formulas are valid. |
| Formula validation | Only approved stats and safe operators are allowed. |
| Rule validation | Costs non-negative, cooldown valid, effect ordering valid. |
| ID generation | Stable, deterministic IDs for runtime content. |
| Lowering | AST constructs become compact IR instructions. |

### IR Shape

```text
MobaModuleIR
  version
  heroes[]
  statuses[]
  abilities[]
  diagnosticsMetadata

AbilityIR
  stableId
  ownerHeroId
  targetRule
  rangeFixed
  cooldownTicks
  costs[]
  triggerPrograms[]

TriggerProgramIR
  trigger: OnCast | OnHit | OnExpire
  instructions[]

EffectInstructionIR
  opcode
  operands
  referencedContentIds
```

### Tasks

- [ ] Define semantic value types and stable ID strategy.
- [ ] Implement symbol table and duplicate detection.
- [ ] Implement formula AST-to-IR lowering.
- [ ] Convert seconds to ticks using compilation configuration.
- [ ] Define effect instruction opcodes for implemented runtime primitives only.
- [ ] Build `CompileResult` containing IR plus diagnostics.
- [ ] Ensure ordering is stable across repeated compilation.
- [ ] Serialize IR in a debug-readable JSON form only for tests/tools, not as the only production format.

### Tests

- [ ] Duplicate ability identifier rejected.
- [ ] Unknown stat in formula rejected.
- [ ] Negative mana cost rejected.
- [ ] Invalid target/effect pairing rejected where appropriate.
- [ ] Same source creates identical stable IDs and identical IR snapshot.
- [ ] Error-containing modules do not emit runnable definitions.

### Exit Criteria

- `.frk` compilation is backend-neutral.
- Runtime behavior is not hardcoded into parser classes.
- Test snapshots clearly show what authored source means.

---

## 14. Increment 6 — Unity Backend and `.frk` Import Workflow

### Goal

Make Unity developers experience the actual product workflow: edit a source file, save, see compiled Unity content, play.

### Unity Integration

Use a Unity custom importer for `.frk` assets. The importer should:

1. Read source text.
2. Invoke compiler.
3. Display import errors/warnings through Unity.
4. Create compiled definition assets or a container asset.
5. Store enough debug metadata to navigate authored content.
6. Trigger dependent demo/runtime refresh when content changes.

### Asset Output Strategy

For the alpha, prefer a single generated `MobaCompiledModule` asset per source file containing serialized hero/ability/status data. Split sub-assets only when it materially improves editor navigation.

### Tasks

- [ ] Implement `MobaDslImporter`.
- [ ] Create runtime-safe serialized definition classes.
- [ ] Create IR-to-Unity emitter.
- [ ] Add inspector that shows compilation summary:
  - module ID/version;
  - heroes/abilities/statuses found;
  - effect counts;
  - warnings.
- [ ] Add source-to-generated-content link metadata.
- [ ] Use imported assets to replace manually-created ability definitions in the arena demo.
- [ ] Provide a “Create > MobaDSL > Hero File” editor menu item with a valid starter template.
- [ ] Add editor tests that import valid and invalid sample assets.

### User Workflow at End of Increment

```text
Create FrostMage.frk
       ↓
Save in Assets/MobaContent/
       ↓
Unity imports it automatically
       ↓
Compilation errors appear in Console/Inspector if invalid
       ↓
Demo scene reads generated definition
       ↓
FrostMage casts IceBolt using authored data
```

### Exit Criteria

- Existing manually-demonstrated abilities now execute from `.frk` files.
- Editing damage or cooldown in source visibly changes the demo after import.
- Compilation failure blocks broken gameplay data from running silently.

---

# Part III — From Ability Tool to Mobile MOBA Toolkit

## 15. Increment 7 — Ability Breadth and Composition

### Goal

Ensure the language/runtime can describe meaningfully different hero kits without custom scripts.

### New Runtime/DSL Capabilities

Implement in controlled batches:

#### Batch A: Combat versatility

- shield;
- periodic damage/healing;
- stat modifier buff/debuff;
- cleanse/remove status;
- execute conditional based on health threshold;
- channel with interrupt rule.

#### Batch B: Movement and spatial effects

- dash;
- blink;
- knockback;
- persistent zone;
- line/cone/circle targeting;
- multi-target selection.

#### Batch C: Passives and chained effects

- passive trigger such as `on_basic_attack` or `on_damage_taken`;
- charges/stacks;
- ability level scaling;
- conditional branching limited to approved predicates.

### Sample Hero Kit Target

Build at least four authored heroes:

| Hero archetype | Demonstrated requirements |
|---|---|
| Mage | Projectile, AoE, slow, burst ultimate. |
| Tank | Shield, stun, taunt/forced behavior only if implemented safely, durability buff. |
| Assassin | Dash/blink, target execution condition, cooldown reset only if controlled. |
| Support | Heal, ally buff, protective zone or cleanse. |

### Tasks

- [ ] Implement new opcodes one at a time with tests.
- [ ] Add DSL syntax only after each primitive works manually.
- [ ] Add authoring samples for every primitive.
- [ ] Implement loop/complexity safeguards:
  - maximum chained trigger depth;
  - maximum effect instructions per cast;
  - status reapplication rules;
  - no infinite self-trigger loops.
- [ ] Add generated Markdown ability-documentation output from compiled IR.
- [ ] Build `AbilityDebugPanel` for current cooldowns, active statuses and event log.

### Exit Criteria

- Four hero kits execute without hero-specific gameplay scripts.
- At least one difficult edge case per primitive is covered by tests.
- Generated ability documentation matches runtime behavior.
- DSL is now expressive enough to market as a MOBA combat authoring tool.

---

## 16. Increment 8 — Mobile Arena Prototype

### Goal

Validate that authored combat feels usable on a mobile form factor, not only in editor tests.

### Scope

- Small arena.
- One locally controlled hero.
- Enemy training bot or simple scripted opponent.
- Four ability slots.
- Movement joystick.
- Ability aim/cast interactions.
- Health/mana/cooldown HUD.
- Target indicators and skill range visualization.

### Mobile Interaction Requirements

| Interaction | Initial behavior |
|---|---|
| Movement | Left virtual joystick. |
| Targeted ability | Drag aim indicator, release to cast, cancel zone to cancel. |
| Directional skill shot | Direction line/cone preview. |
| Ground AoE | Reticle clamped to ability range. |
| Self-cast | Tap or dedicated quick-cast behavior. |
| Basic attack | Target nearest valid enemy or explicit target mode. |

### Engineering Tasks

- [ ] Implement view/controller layer consuming simulation commands.
- [ ] Add mobile command input model independent of gameplay results.
- [ ] Add target previews sourced from ability definition metadata.
- [ ] Add cooldown and resource UI.
- [ ] Implement minimal bot logic for combat testing.
- [ ] Profile on an Android test device.
- [ ] Track frame rate, allocations and entity/effect counts during combat.

### User Testing Questions

- Does editing `.frk` content materially speed up iteration?
- Are the targeting declarations clear enough to produce mobile UI behavior?
- Which ability behaviors need manual code escape hatches?
- Do users expect editor GUI authoring in addition to text?
- Which debug feedback is missing during combat design?

### Exit Criteria

- Android build runs the local arena prototype.
- All four example heroes can be exercised through mobile controls.
- No major authoring blocker requires rewriting the core IR.
- Initial performance baseline is recorded, even if optimization remains later.

---

## 17. Increment 9 — Items, Progression and Match Content

### Goal

Expand from “ability authoring system” into a credible MOBA gameplay toolkit.

### Features

#### Item system

- purchasable item definitions;
- gold cost;
- flat stat modifiers;
- unique passive effect support through existing primitives;
- inventory slots;
- purchase/sell rules;
- simple recipes deferred unless required for demo.

#### Progression system

- XP and hero levels;
- ability leveling or fixed unlock rules;
- gold reward events;
- death/respawn timers;
- configurable match start stats.

#### Rules content

- team definitions;
- spawn locations;
- score/event rules;
- victory condition abstraction.

### Example Item Syntax Target

```moba
item ArcaneStaff {
    cost: 1200
    grants {
        ability_power: 55
        max_mana: 200
    }

    passive ManaSurge {
        on_cast {
            restore mana: 10
        }
    }
}
```

### Tasks

- [ ] Design item definitions using existing effect primitives.
- [ ] Add compiler validation for item references and stacking rules.
- [ ] Create shop UI in the demo application.
- [ ] Implement gold and XP as runtime resources/events.
- [ ] Add death and respawn flow.
- [ ] Add tests for stat recalculation after purchases/removal.
- [ ] Add generated reference docs for item definitions.

### Exit Criteria

- A player can buy items and see deterministic stat/effect changes.
- Level/gold/death loops are functional in the local prototype.
- Item syntax does not require arbitrary scripting.

---

## 18. Increment 10 — One-Lane Mobile MOBA Vertical Slice

### Goal

Create the main proof-of-value demo: not a full game, but a believable MOBA lane.

### Demo Scope

```text
Map:
  One lane
  One base structure per team
  At least one defensive tower per team
  Optional single neutral objective only after core passes

Teams:
  Player + simple ally bot/minions versus enemy bot/minions

Match:
  Minion waves
  Tower targeting
  Gold/XP
  Items
  Hero abilities
  Death/respawn
  Win condition: destroy enemy core
```

### Required Runtime Systems

| System | Minimum capability |
|---|---|
| Minion wave spawner | Timed wave configuration from content data. |
| Lane movement | Deterministic path or waypoint following. |
| Aggro/target selection | Prioritized, testable targeting rules. |
| Tower system | Range, attack cadence, target priority and destruction state. |
| Base/core objective | Defeat condition. |
| Reward system | Gold/XP for deaths/objectives. |
| Match rule system | Start, active, victory and end state. |
| Basic bot | Cast and purchase enough to demonstrate toolkit. |

### Content Authoring Target

Introduce `.frk` rules for:

- unit archetypes;
- towers;
- waves;
- item shop catalog;
- objective/core;
- match victory.

Keep map geometry and artwork in Unity initially; the DSL describes gameplay rules, not full scene layout.

### Tasks

- [ ] Create one-lane scene and placeholder assets.
- [ ] Implement minion archetypes and spawn wave configuration.
- [ ] Implement tower threat and attack logic.
- [ ] Implement objective health and victory state.
- [ ] Integrate hero abilities/items/progression.
- [ ] Build mobile match HUD and end screen.
- [ ] Run repeatable automated simulation for minion/tower balance.
- [ ] Record a polished demonstration video showing source edit → changed match behavior.

### Exit Criteria

- One complete match can be played on Android locally.
- Majority of gameplay rules demonstrated are authored via `.frk` definitions.
- A new example hero or item can be added without modifying core runtime code.
- The demo is convincing enough for public beta marketing.

---

# Part IV — Product Validation and Public Core

## 19. Increment 11 — Core Beta Packaging

### Goal

Release a useful but intentionally bounded free edition to validate developer demand.

### Proposed Free/Core Contents

| Include in Core | Keep for Pro candidate later |
|---|---|
| Language overview/specification | Advanced editor dashboards |
| Basic compiler/importer | Full MOBA vertical-slice template package |
| Parser/diagnostics source if open-sourced | Expanded hero/item/objective libraries |
| Damage, heal, cooldown, mana, slow, stun | Advanced effects and high-end debugging |
| Simple projectile and area abilities | Balance simulator suite |
| Local arena sample | Multiplayer backend adapters |
| Several authored sample abilities | Priority support and polished mobile starter kit |

The exact split must be adjusted after user feedback; do not promise a final edition boundary prematurely.

### Release Preparation Tasks

- [ ] Remove private keys, service configs, test credentials and paid-only experiments.
- [ ] Choose and document the license for released Core code.
- [ ] Write installation instructions for Unity Package Manager or Git URL.
- [ ] Write first-hero tutorial.
- [ ] Provide sample scene and troubleshooting guide.
- [ ] Create roadmap with clear non-promises about multiplayer.
- [ ] Add issue templates and discussion/contact channel.
- [ ] Build release archive/tag.
- [ ] Publish trailer/GIF and request focused feedback.

### Feedback Metrics

Track:

- GitHub stars and package downloads as weak signals only.
- Number of developers who successfully install and run sample.
- Number of developers who author a custom ability.
- Feature requests repeatedly mentioned.
- Willingness to pay for editor tooling, full lane systems, mobile template, multiplayer or simulator.
- Time spent supporting installation/problems.

### Exit Criteria

- At least several external developers can install and run the sample without one-to-one handholding.
- Feedback identifies paid-value candidates or exposes a fatal adoption problem.
- Compiler/runtime issues from external testing are triaged before multiplayer investment.

---

# Part V — Multiplayer and Commercial Product Gate

## 20. Increment 12 — Multiplayer Backend Evaluation Spike

### Goal

Choose the production multiplayer direction using a controlled experiment rather than preference.

### Important Constraint

Do not begin by networking the entire one-lane demo. The spike should use a tiny combat slice:

- two players;
- movement;
- one projectile ability;
- one area ability;
- cooldown/resource state;
- damage/status results;
- latency test conditions.

### Candidate A — Photon Quantum Adapter

Evaluate:

- how MOBA IR definitions map into Quantum deterministic data/assets;
- how generic ability execution maps into Quantum simulation systems;
- predict/rollback behavior for abilities and projectiles;
- Unity presentation binding;
- package/licensing implications for a commercial adapter.

### Candidate B — Unity Netcode for Entities Adapter

Evaluate:

- ECS conversion cost from current runtime model;
- server-authoritative execution of abilities;
- client prediction for movement/ability responsiveness;
- ghost/state synchronization and lag compensation needs;
- package/tooling complexity for customers.

### Decision Matrix

Score each candidate from 1–5:

| Criterion | Weight | Photon Quantum | Netcode for Entities |
|---|---:|---:|---:|
| Responsive mobile action combat under latency | 5 | TBD | TBD |
| Ease of mapping MobaDSL IR/runtime | 5 | TBD | TBD |
| Deterministic or authoritative correctness | 5 | TBD | TBD |
| Unity developer adoption barrier | 4 | TBD | TBD |
| Documentation/sample quality for your needs | 3 | TBD | TBD |
| Commercial distribution/licensing feasibility | 4 | TBD | TBD |
| Device/server performance | 5 | TBD | TBD |
| Maintenance burden for a solo developer | 5 | TBD | TBD |

### Spike Tasks

- [ ] Freeze a small network combat feature set.
- [ ] Define command/event data shared across candidates.
- [ ] Implement candidate proof or sufficiently detailed implementation prototype.
- [ ] Test controlled simulated latency/jitter/packet conditions.
- [ ] Measure responsiveness, correctness, integration effort and runtime cost.
- [ ] Document blockers and required DSL/runtime changes.
- [ ] Write `ADR-Multiplayer-Backend-Selection.md`.

### Exit Criteria

- Backend selected or multiplayer deferred explicitly with evidence.
- The core DSL/IR remains usable independent of chosen backend.
- No paid claim of “multiplayer ready” is made until a working demonstration exists.

---

## 21. Increment 13 — Pro Product Candidate

### Goal

Turn validated technology into a paid Unity package with professional onboarding.

### Candidate Paid Features

- Full effect primitive catalog suitable for production prototyping.
- Item, status, hero and lane authoring workflow.
- Polished mobile arena or one-lane template.
- Rich diagnostics and reference navigation.
- Ability debugger and event timeline.
- Generated documentation/export.
- Balance simulator or automated matchup runner.
- Selected multiplayer adapter once validated.
- Priority bug support and maintained package compatibility.

### Productization Tasks

- [ ] Separate Core and Pro package contents cleanly.
- [ ] Create package upgrade/migration path.
- [ ] Add API documentation and authored-content compatibility notes.
- [ ] Create onboarding project and tutorial videos.
- [ ] Test fresh installation in a blank Unity project.
- [ ] Test sample builds on Android target hardware.
- [ ] Prepare Asset Store descriptions, screenshots, video and support policy.
- [ ] Define semantic versioning and deprecation policy for DSL source compatibility.
- [ ] Maintain a public feature matrix that does not overpromise.

### Release Gate

Do not sell Pro until:

- clean installation works from a new Unity project;
- documentation enables a developer to author a new hero ability;
- demo scenes run on device;
- major compiler errors are clear;
- known limitations are documented;
- the offered features are complete without requiring hidden payments to work as marketed.

---

# Part VI — Technical Backlogs

## 22. Language Feature Backlog by Version

| Language version | Required content |
|---|---|
| `v0.1` | Heroes, stats, abilities, cost/cooldown/range, damage/heal, slow/stun, projectile, basic formulas. |
| `v0.2` | Shields, periodic effects, modifiers, AoE targeting shapes, status stacking policies. |
| `v0.3` | Movement effects, channels, passives, ability levels, controlled conditions. |
| `v0.4` | Items, item passives, gold/XP/progression, death/respawn configuration. |
| `v0.5` | Minions, towers, waves, objectives and match rules for one-lane demo. |
| `v0.6` | Simulator metadata, debug export and content-document generation. |
| `v1.0` | Stable syntax/API boundary, migration rules, polished Core/Pro product split. |

### Language Change Rule

Before `v1.0`, breaking syntax changes are allowed only when migration examples and changelog entries are added. After `v1.0`, introduce versioned migrations or compatibility handling.

---

## 23. Runtime Primitive Backlog

### Tier 1 — Must Build Before DSL Alpha

- entity/stat/resource state;
- damage;
- heal;
- cooldown;
- mana cost;
- team filtering;
- single target selection;
- slow;
- stun;
- projectile;
- circular AoE.

### Tier 2 — Needed for Credible Hero Kits

- shield;
- damage over time;
- heal over time;
- stat modifier buff/debuff;
- cleanse;
- dash/blink;
- knockback;
- targeting shapes;
- channels/interrupts;
- safe conditional execution;
- passive event triggers.

### Tier 3 — Needed for MOBA Vertical Slice

- basic attack rule;
- inventory/item effects;
- gold/XP/level;
- death/respawn;
- minion AI and waves;
- tower combat;
- structures/objectives;
- match state and victory.

### Tier 4 — Commercial Differentiators

- debugger/timeline;
- replay hooks;
- formula inspector;
- headless simulations;
- balance reports;
- backend adapters;
- advanced examples/template content.

---

## 24. Compiler Validation Backlog

### Syntax Diagnostics

- unexpected token;
- missing closing brace;
- malformed duration/percentage/distance;
- unsupported keyword.

### Semantic Diagnostics

- duplicate ID;
- unknown hero/ability/status/item/stat;
- invalid unit type;
- invalid formula operand;
- invalid targeting mode for effect;
- negative or zero-invalid values;
- unavailable effect primitive for current language version.

### Safety Diagnostics

- trigger recursion/infinite feedback loop risk;
- effect chain exceeds complexity budget;
- nondeterministic source requested in multiplayer-safe profile;
- presentation resource used as simulation condition;
- random behavior without declared deterministic RNG policy.

### Quality Warnings

- ability lacks mana/cost and cooldown;
- unreachable conditional effect;
- item grants no stats/effects;
- ability references missing presentation asset in a Unity binding profile;
- oversized instruction program likely to affect simulation budget.

---

## 25. Testing Strategy

### 25.1 Compiler Test Pyramid

| Test type | Examples |
|---|---|
| Lexer tests | Tokens and source spans for valid/invalid literals. |
| Parser tests | Valid syntax tree structures and recovery from errors. |
| Semantic tests | References, unit typing, formula rules, duplicate IDs. |
| IR snapshot tests | Approved sample files compile into stable IR. |
| Importer tests | Unity import produces asset or reports expected errors. |
| Compatibility tests | Previously published sample `.frk` files remain valid or receive documented migration. |

### 25.2 Runtime Test Pyramid

| Test type | Examples |
|---|---|
| Pure unit tests | Damage, mana, cooldown, stat resolution, status expiry. |
| Ability program tests | Cast programs apply exact expected state transitions. |
| Simulation scenario tests | Mage vs tank scripted duel over fixed ticks. |
| Lane system tests | Minion targeting, tower behavior, victory. |
| Presentation integration tests | View correctly reflects gameplay events without changing state. |
| Device tests | Android input, UI layout and baseline performance. |

### 25.3 Determinism/Multiplayer Readiness Tests

Begin before networking exists:

- same inputs + same content + same seed = same state digest at every checkpoint;
- fixed-tick timing only for simulation;
- random calls occur only through simulation RNG service;
- IR effect execution ordering is stable;
- visual frame rate changes do not change combat output;
- content stable IDs do not change unexpectedly across imports.

### 25.4 Release Test Gate Checklist

- [ ] All automated tests pass.
- [ ] Example `.frk` files import cleanly.
- [ ] Fresh Unity project installation tested.
- [ ] Android demo build tested.
- [ ] Documentation tutorial followed from scratch.
- [ ] Known limitations updated.
- [ ] No secrets or proprietary unintended files in public/package artifact.
- [ ] Changelog and version updated.

---

## 26. Performance and Mobile Constraints

Set baselines early rather than promising final production budgets immediately.

### Measurements to Collect

- simulation ticks per second;
- frame time on Android test device;
- garbage allocations during active combat;
- number of active combat entities;
- number of active effects/projectiles/areas;
- compiler/import time for representative modules;
- serialized content size;
- headless simulation speed once available.

### Architectural Constraints

- Core simulation should avoid unnecessary per-frame allocations.
- VFX pooling is presentation-side and must not affect game outcomes.
- Avoid using Unity physics as the unexamined truth source for future deterministic paths.
- Ability formulas should compile to compact evaluation programs.
- Projectiles/zones need strict lifetime and entity-count controls.

---

## 27. Documentation Deliverables by Milestone

| Milestone | Required documentation |
|---|---|
| Foundation | Architecture overview and contribution/code conventions. |
| Runtime proof | Combat state model and extension notes. |
| DSL alpha | Syntax overview, units, ability example, diagnostics. |
| Importer | Installation and “create first ability” guide. |
| Ability breadth | Primitive catalog and four hero examples. |
| Mobile arena | Controls integration tutorial and Android build steps. |
| Lane demo | Match rules and sample project walkthrough. |
| Public beta | README, FAQ, roadmap, known limitations and license. |
| Pro | Full manual, upgrade guide, support policy and feature matrix. |
| Multiplayer | Adapter documentation, prerequisites and supported scenario limits. |

---

# Part VII — Working Plan for the First Development Cycle

## 28. Suggested Sprint Rhythm

Use short increments. Each sprint should finish with:

- working code or a concrete design artifact;
- updated tests;
- updated demo or fixture;
- updated notes/ADR when architecture changed;
- a short recorded progress clip when a visible behavior exists.

The durations below are planning units, not deadlines.

---

## 29. Sprint-by-Sprint Execution Backlog

### Sprint 0 — Setup and Guardrails

**Objective:** Create a package-based Unity project with testing and architectural boundaries.

**Build**
- Repository structure.
- Unity project and embedded package.
- Runtime/Editor/Test assemblies.
- Basic CI/local test instructions.
- Architecture decision records.

**Done when**
- Package compiles.
- Tests execute.
- Scene can reference a runtime package component.
- No gameplay code is implemented in `Assets/` by accident.

---

### Sprint 1 — Combat State Kernel

**Objective:** Define entities, stats, health, mana, teams and event reporting.

**Build**
- Entity registry.
- Stat/resource value types.
- Damage/heal service.
- Gameplay event stream.
- Two dummy entities in sandbox.

**Tests**
- Damage/heal/resource boundaries.
- Team relations.
- Same tick scenario state equality.

**Done when**
- A deterministic scripted duel updates health in runtime state and a UI observer displays it.

---

### Sprint 2 — Abilities, Costs and Cooldowns

**Objective:** Cast reusable single-target effects from manual definitions.

**Build**
- Ability definitions.
- Cast request/result.
- Target validity.
- Mana spending and cooldown.
- Direct damage and heal effects.

**Tests**
- Invalid range/cost/cooldown cases.
- Effect execution order.

**Done when**
- Two manually defined spells cast in the sandbox without hero-specific behavior code.

---

### Sprint 3 — Projectile and Status Effects

**Objective:** Add MOBA-recognizable combat behavior.

**Build**
- Projectile simulation.
- Slow and stun.
- Area hit.
- Event-driven placeholder visual hooks.

**Tests**
- Hit only once.
- Status expiry/refresh policy.
- AoE team filtering.

**Done when**
- Manually authored IceBolt and FireNova work visibly in the arena.

---

### Sprint 4 — Language Specification and Compiler Skeleton

**Objective:** Make the language deliberately represent only proven mechanics.

**Build**
- v0.1 language spec.
- Lexer/parser framework.
- Text spans and diagnostic infrastructure.
- Valid/invalid sample fixture collection.

**Tests**
- Tokenization and parse trees.
- Diagnostic snapshots.

**Done when**
- `.frk` examples parse independently of Unity gameplay scenes.

---

### Sprint 5 — Semantic Compilation and IR

**Objective:** Validate meaningful game content and produce stable IR.

**Build**
- Symbol binding.
- Units and formulas.
- Stable IDs.
- Ability/status IR.
- Debug IR serialization for snapshots.

**Tests**
- Errors for invalid references/units.
- Stable IR snapshot tests.

**Done when**
- IceBolt source compiles to a validated ability program corresponding to the manual implementation.

---

### Sprint 6 — Unity Importer and Runtime Binding

**Objective:** Execute imported `.frk` gameplay in Unity.

**Build**
- `ScriptedImporter` for `.frk`.
- Serialized module/definition output.
- Inspector summary and import diagnostics.
- Arena uses imported FrostMage source.

**Tests**
- Asset import success/failure.
- Data changes reflected in runtime.

**Done when**
- Changing ability damage in a text file and saving changes gameplay in the demo without new C#.

---

### Sprint 7 — Four-Hero Ability Coverage

**Objective:** Discover whether primitives are broad enough for convincing content.

**Build**
- Add required Tier 2 primitives gradually.
- Mage, tank, assassin and support authored content.
- Ability debug HUD.
- Documentation export prototype.

**Tests**
- Each new primitive and edge cases.
- Content import regression fixtures.

**Done when**
- Four differentiated hero kits work from DSL and missing capabilities are clearly recorded.

---

### Sprint 8 — Touch Controls and Device Prototype

**Objective:** Validate mobile interaction.

**Build**
- Virtual joystick and ability controls.
- Aim indicators by targeting rule.
- HUD and simple enemy bot.
- Android build.

**Tests/Measures**
- Touch cast behavior.
- Initial device performance snapshot.

**Done when**
- A user can play authored combat on an Android device.

---

### Sprint 9 — Items, Gold, XP and Respawn

**Objective:** Add the gameplay economy needed for a lane match.

**Build**
- Items and purchases.
- Modifiers/passives.
- Gold/XP/level.
- Death/respawn.

**Tests**
- Item effect stacking.
- Reward and respawn rules.

**Done when**
- A local arena match includes advancement and purchases based on authored rules.

---

### Sprint 10 — Minions, Towers and Match Completion

**Objective:** Build a one-lane vertical slice.

**Build**
- Waves.
- Minion movement/aggro.
- Towers.
- Base/core.
- Victory condition.
- Bot behaviors sufficient for demo.

**Tests/Measures**
- Repeatable lane outcomes under fixed configurations.
- Android performance snapshot with minions/towers active.

**Done when**
- A full local one-lane match is playable and demonstrable.

---

### Sprint 11 — Tool Polish and Public Beta Candidate

**Objective:** Make the tool installable and understandable by strangers.

**Build**
- Documentation.
- Package samples.
- Tutorial.
- Better diagnostics.
- README visuals/video.
- Issue/feedback workflow.

**Done when**
- External beta testers can install, modify an ability and run a sample.

---

### Sprint 12 — Market Validation and Feature Prioritization

**Objective:** Decide what to build for revenue based on actual developer behavior.

**Run**
- Core beta feedback cycle.
- Landing/waitlist and demo content.
- Feature surveys/interviews.
- Bug fixes.

**Decide**
- Free/Pro split.
- Most valuable Pro features.
- Whether users need lane tooling, simulation, editor UX or networking first.

**Done when**
- The next commercial investment is justified by evidence rather than assumptions.

---

### Sprint 13 — Multiplayer Spike

**Objective:** Select a multiplayer backend using a small test slice.

**Build/Test**
- Minimal network combat test.
- Candidate mapping and performance/correctness evaluation.
- Decision ADR.

**Done when**
- Backend selected, rejected or deferred honestly.

---

### Sprint 14+ — Pro Packaging and Adapter Work

**Objective:** Build the paid package only around validated high-value capabilities.

**Build**
- Polished tools.
- Selected premium systems.
- Stable documentation.
- Asset Store preparation.
- Multiplayer adapter only if spike succeeds.

**Done when**
- A commercial user can obtain measurable value without relying on undocumented hacks.

---

## 30. Immediate First Backlog: What to Implement Next

This is the recommended order for starting the project immediately.

### Step A — Create the Unity package repository

- [ ] Create `mobadsl` Git repository privately first.
- [ ] Create Unity project for package development.
- [ ] Add `packages/com.fork.dsl`.
- [ ] Add runtime/editor/test assemblies.
- [ ] Add foundational README and ADRs.

### Step B — Build runtime types before parser work

- [ ] `EntityId`, `TeamId`, `StatId`, `AbilityId`, `StatusId`.
- [ ] `FixedTick`, `SimulationConfig`.
- [ ] `CombatEntityState`, `StatBlock`, `ResourcePool`.
- [ ] `DamageRequest`, `DamageResult`.
- [ ] `GameplayEvent` variants.
- [ ] Pure unit tests.

### Step C — Add the minimum playable mechanics

- [ ] Cast request and ability definition.
- [ ] Cost and cooldown.
- [ ] Target validation.
- [ ] Damage and heal.
- [ ] Slow/stun.
- [ ] Projectile/AoE.
- [ ] Arena scene for observation.

### Step D — Only then begin the DSL

- [ ] Freeze a minimal ability example set.
- [ ] Write syntax that expresses exactly those examples.
- [ ] Implement parser/semantic compiler.
- [ ] Implement importer.
- [ ] Replace manual definitions with `.frk` authored content.

### First visible milestone

Your first shareable progress clip should show:

```text
A tiny .frk file defining IceBolt
        ↓
Unity compiles/imports it
        ↓
A hero fires the projectile
        ↓
Changing damage/cooldown in text visibly changes gameplay
```

This is the core proof that the product has value.

---

# Part VIII — Development Practices, AI Use and Risk Control

## 31. AI-Assisted Coding Rules for This Project

Because this project may eventually contain proprietary paid runtime code, the development workflow should distinguish between public-safe and confidential code.

### Safe to use with models/routes whose data handling is not suitable for confidential production code

- public language examples;
- README and documentation drafts;
- disposable parser experiments;
- tests for already-open-source code;
- brainstorming of runtime interfaces without proprietary implementation;
- public Core issues after disclosure is acceptable.

### Keep out of non-confidential/free routes unless their policies change and you verify them

- proprietary Pro source;
- undisclosed vulnerabilities;
- API keys, credentials, signing material or store/payment details;
- private customer code or support data;
- unreleased multiplayer security/anti-cheat implementation;
- production logs containing user information.

### Mandatory Engineering Guardrails Regardless of AI Provider

- Do not accept generated code without reviewing it.
- Require tests for compiler, import and runtime changes.
- Never paste secrets into prompts.
- Keep commits small and revertible.
- Run static analysis/build/tests before merging.
- Use AI for acceleration, not for declaring security or multiplayer correctness.

---

## 32. Risk Register and Mitigations

| Risk | Impact | Early mitigation | Stop/go signal |
|---|---|---|---|
| DSL adds little value over ScriptableObjects | Product not worth adopting | Test authoring speed and readability with external users | Stop expanding syntax if users prefer inspectors only. |
| Runtime cannot express real hero kits cleanly | Constant custom C# destroys proposition | Build four differentiated kits before vertical slice | Rework primitive/escape-hatch design before lane systems. |
| Multiplayer architecture forces rewrite | Large delay or abandonment | Use simulation/view separation and fixed-tick habits early; perform spike before claims | Select backend only after measured prototype. |
| Tool scope becomes “build a whole game engine” | Project never ships | Strict incremental gates and non-goals | Reject new systems not needed for current proof. |
| Mobile performance fails | Demo not credible | Profile Android during arena and lane stages | Optimize architecture before expanding content. |
| Compiler diagnostics poor | Authoring is frustrating | Make diagnostics a first-class deliverable with fixtures | Do not release beta without readable errors. |
| Open-core split gives away paid value or is too limited | Weak revenue/adoption | Validate feature demand before final split | Adjust based on usage feedback. |
| AI/free provider exposes proprietary work | Loss of confidentiality | Keep sensitive code out of unsuitable routes | Use trusted paid/local path for Pro work. |

---

## 33. Decision Gates Summary

| Gate | Required evidence | Decision |
|---|---|---|
| G1: Runtime before language | Manual abilities are composable and tested | Begin DSL compiler or redesign runtime. |
| G2: DSL before expansion | Imported `.frk` modifies working gameplay reliably | Expand primitives/tooling or revisit authoring model. |
| G3: Tool before MOBA scope | Four hero kits require no bespoke scripts for ordinary behavior | Build mobile lane systems or repair abstraction. |
| G4: Mobile viability | Device demo is playable and baseline performance acceptable | Build vertical slice or optimize. |
| G5: Public release | Documentation/install/test quality works for external testers | Publish Core beta or continue polish. |
| G6: Commercial investment | Users signal which paid feature saves them time/money | Prioritize Pro features. |
| G7: Multiplayer investment | Backend spike proves feasible architecture and responsiveness | Implement adapter or defer honestly. |

---

## 34. Sources and Technical Anchors

The plan relies on the following current technical anchors:

1. **Unity Scripted Importers** provide the Unity integration point for supporting custom asset file extensions such as `.frk`, invoking a custom importer when a matching file is new or changed.
2. **Unity package layout and assembly separation** support structuring reusable runtime, editor and test code as a distributable package rather than mixing it into a demo game's assets.
3. **Unity Netcode for Entities** is positioned by Unity for server-authoritative, predicted multiplayer in competitive/high-complexity action games, making it an evaluation candidate rather than an assumed default.
4. **Photon Quantum 3** is a Unity-oriented deterministic ECS framework with predict/rollback networking and simulation/view separation; its Qtn DSL handles deterministic game-data generation, making Quantum a relevant adapter candidate for a higher-level MOBA authoring DSL.
5. Photon has previously published a MOBA sample for Quantum, which demonstrates domain relevance but does not replace the need for a current technical spike.

### Official Reference Links

- Unity Manual, **Scripted Importers**: <https://docs.unity.cn/Manual/ScriptedImporters.html>
- Unity Manual, **Package layout**: <https://docs.unity3d.com/Manual/cus-layout.html>
- Unity Multiplayer Docs, **Unity's netcode packages**: <https://docs.unity.com/en-us/multiplayer/netcode/netcode>
- Photon Engine, **Quantum 3 Intro**: <https://doc.photonengine.com/quantum/current/getting-started/quantum-intro>
- Photon Engine, **MOBA — Tiki Frutti sample**: <https://doc.photonengine.com/quantum/v1/demos-and-tutorials/moba>

Reference URLs, product capabilities and licensing should be rechecked during implementation and before any marketing claim, since packages and commercial terms can change.

---

## 35. Closing Execution Recommendation

Begin with **Sprint 0 through Sprint 3 only**:

1. Package structure and tests.
2. A pure local combat simulation kernel.
3. Manual ability primitives and visible arena behaviors.
4. A minimal language specification based only on those proven mechanics.

Do not begin with multiplayer, a full map editor, monetization systems or a large language grammar. The winning proof is much smaller:

> A developer writes a readable `.frk` ability, Unity validates and imports it, and a mobile-ready combat demo executes the ability correctly with no new hero-specific gameplay script.

Once that works, every later investment—items, lanes, editor tooling, public beta, Pro packaging and multiplayer—can be justified by a working foundation rather than a promise.

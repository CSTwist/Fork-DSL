# Mobile MOBA DSL for Unity — Comprehensive Development Plan

**Working name:** `Fork`  
**Proposed Unity package ID:** `com.fork.dsl`  
**Document version:** 0.1  
**Prepared:** 2026-05-29  
**Primary objective:** Build a domain-specific language and Unity runtime that lets developers author mobile-MOBA gameplay—heroes, abilities, status effects, items, lanes, minions, towers and objectives—at a higher level than hand-written gameplay scripts.

---

## 1. Executive Summary

`MobaDSL` should be developed as a **Unity-focused open-core developer tool**:

- A human-readable `.frk` language expresses MOBA gameplay content.
- A compiler parses and validates DSL source into a backend-neutral **MOBA Intermediate Representation (IR)**.
- A Unity backend emits serialized definitions and runs them through a reusable simulation runtime.
- Presentation—animations, VFX, sounds and UI—is explicitly separated from gameplay simulation.
- Multiplayer is not attempted on day one; the first release validates the authoring workflow in a local vertical slice.
- A later deterministic/multiplayer adapter targets either **Photon Quantum** or **Unity Netcode for Entities** after a measured technical spike.
- The public/free edition should prove the DSL and basic gameplay runtime; the paid Unity product should contain the production-scale MOBA toolkit, editor tooling, mobile templates, simulation/debug tools and multiplayer adapters.

The largest technical risk is not parsing the language. It is creating an extensible, deterministic-capable combat/runtime model that can express real hero abilities without devolving into ad-hoc C# per hero. The plan therefore prioritizes runtime primitives, validation and testing before broad language syntax.

---

## 2. Product Vision

### 2.1 Problem

Building a MOBA in Unity repeatedly requires developers to implement:

- Hero stats and progression.
- Abilities, targeting, projectiles, areas, crowd control and passives.
- Items, recipes and stat modifiers.
- Minion waves, lanes, towers and neutral objectives.
- Deterministic or server-authoritative multiplayer behavior.
- Debugging and balance testing tools.

Most teams build these systems in custom C# architectures or adapt generic ability frameworks. That can make gameplay definitions scattered, difficult to review, hard to validate and expensive to rebalance.

### 2.2 Product Thesis

A MOBA-focused language can make gameplay definitions:

- Readable by game designers and programmers.
- Validatable before runtime.
- Reusable across local simulation, multiplayer and balance-analysis backends.
- Easier to diff, test, document and version-control than Inspector-only configurations.
- Faster to create than implementing each ability through bespoke C#.

### 2.3 Product Promise

> Define MOBA gameplay in concise `.frk` files, compile it into validated Unity-ready content, and execute it through a reusable mobile-first runtime.

### 2.4 Target Users

| User | Need | Initial Value |
|---|---|---|
| Solo Unity developer | Build a MOBA prototype without building all combat infrastructure | Ready ability/runtime primitives and examples |
| Small indie studio | Iterate quickly on hero kits and balance | DSL, compiler diagnostics, editors, test harness |
| Technical designer | Edit gameplay without modifying core C# | Readable content language and previews |
| Commercial studio evaluating tooling | Confirm runtime extensibility and multiplayer feasibility | Sample vertical slice, documentation and adapter roadmap |

---

## 3. Scope Boundaries

### 3.1 Product Scope

`MobaDSL` is a **gameplay authoring tool and runtime**, not a complete game engine.

It should own:

- DSL syntax and semantics.
- Compilation, validation and diagnostics.
- Gameplay content definitions.
- Generic simulation primitives.
- Unity integration, importer and inspectors.
- Local battle/lane demo.
- Deterministic-safety rules.
- Later: multiplayer backend adapters and headless balance tools.

It should not initially own:

- Matchmaking or accounts.
- Voice/chat/social systems.
- Monetization or live operations.
- Full anti-cheat platform.
- Art asset production.
- A production backend hosting service.
- Complete generated Android/iOS game projects from one command.

### 3.2 First Product Boundary

The first meaningful proof is not “make a full 5v5 MOBA.” It is:

> A mobile-controlled, local one-lane vertical slice where multiple heroes, abilities, effects, items, minions and towers are authored primarily in `.frk` files and validated by the compiler.

---

## 4. Success Criteria

### 4.1 Technical Validation Criteria

The concept is technically validated when all of the following are true:

- A `.frk` file imports automatically in Unity and produces runnable gameplay definitions.
- A new simple hero kit can be added without writing new gameplay C#.
- Compiler diagnostics catch invalid references, illegal formulas and invalid effect combinations.
- The runtime executes a meaningful set of MOBA primitives consistently.
- Automated tests cover compiler, combat calculations and deterministic replay checks.
- A local one-lane match with mobile controls is playable.

### 4.2 Product Validation Criteria

The product is commercially worth continuing when:

- Outside Unity developers can set up the free demo from documentation alone.
- At least several developers independently author new abilities through the DSL.
- Feedback consistently values the authoring workflow or testing tools, not only the demo game.
- Developers express willingness to pay for editor tools, multiplayer integration, simulator/debugger or a polished mobile MOBA starter template.

### 4.3 MVP Definition of Done

The first releasable free core should include:

- Language reference and examples.
- Lexer/parser, semantic validation and diagnostics.
- Unity `.frk` importer.
- Hero, ability and status definition output.
- Runtime primitives: target selection, damage, heal, resource cost, cooldown, projectile, area, slow, stun, shield and stat modifier.
- Two or more playable example heroes.
- A local arena demo and automated tests.
- No credentials, online service or paid dependency required.

---

## 5. Core Architectural Decisions

### 5.1 Compile Gameplay Data, Not Per-Hero Scripts

The default compilation strategy should be:

```text
.frk source
  → tokens / syntax tree
  → semantic validation
  → backend-neutral MOBA IR
  → Unity serialized definition assets
  → reusable Unity simulation systems
```

Do **not** generate one C# behavior class per hero or ability in the initial architecture. That approach increases script recompiles, creates generated-code debugging problems and weakens the purpose of a data-driven DSL.

New DSL content should generally be executable without modifying core C#. C# is added when the product gains a genuinely new primitive such as chain bounce, terrain creation or clone control.

### 5.2 Separate Simulation from Presentation

All rules that affect winners, health, targeting or movement must live in simulation definitions/systems. VFX, sound, animation and screen feedback must be presentation-only.

```text
Simulation:
  health, mana, damage, cooldowns, status effects, target resolution, hit timing

Presentation:
  particles, animation clips, sound events, UI indicators, camera shake
```

This separation improves testability and is required if a deterministic rollback backend is later adopted.

### 5.3 Use an Intermediate Representation

The language must not be tightly coupled to a single Unity multiplayer stack.

```text
MobaDSL Frontend
  → MOBA IR
      ├── Unity local runtime emitter
      ├── Headless simulation emitter
      ├── Photon Quantum adapter candidate
      └── Unity Netcode for Entities adapter candidate
```

The IR is one of the product’s highest-value assets: it permits multiple runtimes without redesigning the language.

### 5.4 Start Local, Design for Determinism

Build a local Unity implementation first, while enforcing deterministic-friendly constraints from the outset:

- Use simulation ticks as the canonical time concept.
- Avoid frame-dependent gameplay calculations.
- Use an injected seeded random source, not uncontrolled randomness.
- Keep VFX and animation outside gameplay resolution.
- Represent formulas in controlled expressions.
- Provide stable IDs and explicit ordering where simultaneous events matter.

---

## 6. Proposed User Experience

### 6.1 Example Hero Definition

```moba
hero FrostMage {
    stats {
        health: 580
        mana: 420
        move_speed: 350
        attack_damage: 48
        ability_power: 0
        attack_range: 520
    }

    ability IceBolt {
        slot: q
        target: enemy_unit
        range: 800
        cooldown: 120 ticks
        cost: mana 40

        cast {
            projectile {
                speed: 280 units_per_tick
                hit: first_enemy
            }
        }

        hit(target) {
            damage magic: 120 + caster.ability_power * 0.60

            apply Slow {
                magnitude: 30%
                duration: 40 ticks
            }
        }

        presentation {
            icon: "Icons/FrostMage/IceBolt"
            projectile_prefab: "VFX/FrostMage/IceBolt"
            hit_vfx: "VFX/FrostMage/IceImpact"
            cast_sfx: "Audio/FrostMage/IceBoltCast"
        }
    }
}
```

### 6.2 Example Item Definition

```moba
item ArcaneStaff {
    cost: 1800

    grants {
        ability_power: 70
        mana: 250
    }

    passive ManaFlare unique {
        on ability_hit(target) {
            if target.has_status(Slow) {
                damage magic: 25 + owner.ability_power * 0.10
            }
        }

        internal_cooldown: 40 ticks
    }
}
```

### 6.3 Example Lane Rule Definition

```moba
match StandardOneLane {
    tick_rate: 20
    team_size: 1

    lane middle {
        wave every: 600 ticks
        composition: [melee * 3, ranged * 1]
        siege_wave: every 3 waves
    }

    structure NexusTower {
        health: 2500
        armor: 40
        attack_range: 750
    }

    win_condition {
        destroy enemy.core
    }
}
```

---

## 7. Language Design Plan

### 7.1 Design Principles

The DSL should be:

- **Declarative first:** describe gameplay outcomes rather than imperative engine operations.
- **MOBA-semantic:** use `hero`, `ability`, `status`, `item`, `wave`, `tower`, `objective`.
- **Safe by default:** invalid or multiplayer-unsafe content should fail compilation.
- **Composable:** designers combine primitives into varied abilities.
- **Reviewable:** diffs should clearly reveal balance changes.
- **Extensible:** advanced primitives may be implemented as runtime extensions.

### 7.2 Language Feature Tiers

#### Tier 1: Content and Basic Combat

- Declarations: `hero`, `ability`, `status`.
- Scalars: integers, fixed-point decimals, percentages, ticks.
- Properties: stats, range, cooldown, resource costs.
- Events: `cast`, `hit`, `expire`.
- Effects: damage, heal, slow, stun, shield, stat modification.
- References: hero/status/ability IDs.
- Simple formulas based on caster/target stats.

#### Tier 2: MOBA Ability Breadth

- Projectiles and area effects.
- Channels and interrupt behavior.
- Dash, blink, knockback and pull.
- Target filters and multi-target selection.
- Damage-over-time and healing-over-time.
- Buff stacking rules and dispels.
- Passive triggers and internal cooldowns.
- Summoned units.

#### Tier 3: Match Systems

- Items, recipes and shops.
- Units, minions, towers and objectives.
- Waves and lane rules.
- XP, levels and ability ranks.
- Vision/fog rules, if feasible.
- Map/objective scripting.

#### Tier 4: Advanced Tooling Semantics

- Balance constraints and assertions.
- Simulation scenarios.
- Documentation annotations.
- Compatibility profiles for multiplayer backends.
- Performance budgets or lint rules.

### 7.3 Type System and Units

Avoid ambiguous raw floats. Give concepts explicit units:

| Concept | DSL form | Internal form |
|---|---|---|
| Duration | `40 ticks` | integer tick count |
| Distance | `800` or `800 units` | configured integer/fixed unit |
| Percentage | `30%` | fixed-point fraction |
| Damage | `120` | numeric expression |
| Cooldown | `120 ticks` | integer tick count |
| Speed | `280 units_per_tick` | deterministic-friendly value |

Compiler errors should reject:

- Negative duration/cooldown/cost unless specifically permitted.
- Percentage where an absolute value is required.
- Unknown stat, ability, item or status references.
- Invalid event/effect relationships.
- Presentation references inside simulation expressions.

### 7.4 Formula Expression System

Initial formula syntax should support:

```moba
120 + caster.ability_power * 0.60
target.max_health * 0.08
60 + ability.rank * 20
```

Do not initially allow arbitrary loops, reflection, C# invocation or runtime object access.

Recommended expression AST:

```text
Literal
StatRead(entity, statId)
AbilityRankRead
UnaryOperation
BinaryOperation
Clamp
Min
Max
```

Later, add controlled conditionals and target aggregation only after formula execution and serialization are stable.

### 7.5 Extensibility Strategy

Provide a runtime registry:

```csharp
EffectRegistry.Register("deal_damage", new DealDamageExecutor());
EffectRegistry.Register("apply_status", new ApplyStatusExecutor());
```

However, extensions must declare:

- Input schema.
- Validation rules.
- Whether the effect is deterministic-compatible.
- Whether it is supported by each backend.
- Whether it emits presentation events.

This prevents third-party extension scripts from silently breaking multiplayer guarantees.

---

## 8. Compiler Architecture

### 8.1 Pipeline

```text
Source Files
  → Source Loader / Include Resolver
  → Lexer
  → Parser
  → AST
  → Symbol Table and Reference Resolution
  → Type/Unit Checking
  → Rule Validation and Diagnostics
  → MOBA IR
  → Backend Emitters
  → Unity Assets / Simulation Data / Documentation
```

### 8.2 Initial Parser Choice

Use a **handwritten recursive-descent parser** for the first language version:

- Syntax will change rapidly.
- Error messages can be customized early.
- The grammar is initially small.
- No external generation step is required in the Unity package.

Reassess a parser generator after language grammar stabilizes or when tooling such as syntax trees, formatting and IDE support becomes expensive to maintain.

### 8.3 AST Layer

Representative AST nodes:

```text
DocumentNode
DeclarationNode
  HeroDeclarationNode
  AbilityDeclarationNode
  StatusDeclarationNode
  ItemDeclarationNode
BlockNode
PropertyNode
EventBlockNode
EffectStatementNode
ExpressionNode
SourceSpan
```

Every node must retain `SourceSpan` information—file, starting line/column and ending line/column—to produce precise compiler diagnostics.

### 8.4 Semantic Validation Passes

Run multiple explicit validation passes:

| Pass | Examples of failures |
|---|---|
| Symbol declaration | Duplicate `IceBolt` declaration |
| Reference resolution | Unknown status `Slwo` |
| Unit/type checking | `cooldown: 20%` |
| Formula validation | Unknown `caster.apility_power` |
| Gameplay rule validation | Ground-only effect uses unit-only target |
| Backend capability validation | Unsupported effect for Quantum adapter |
| Asset validation | Missing icon/prefab references in Unity output |
| Determinism linting | Nondeterministic random or floating-time usage |
| Complexity validation | Recursive trigger chain or effect budget exceeded |

### 8.5 Diagnostics Quality Bar

Compiler diagnostics are part of the sellable product.

```text
Assets/MobaContent/Heroes/frost_mage.frk:14:19 error MOB0121
Unknown status 'Slwo'. Did you mean 'Slow'?

Assets/MobaContent/Heroes/frost_mage.frk:15:23 error MOB0204
Duration cannot be negative. Received: -40 ticks.

Assets/MobaContent/Items/arcane_staff.frk:10:9 warning MOB1003
This passive can trigger from its own damage. Add 'cannot_trigger_itself: true'
or an internal cooldown to prevent recursive execution.
```

### 8.6 Backend-Neutral IR

The IR should be plain, serializable and testable outside Unity editor code.

Illustrative IR schema:

```csharp
public sealed record AbilityIr(
    StableId Id,
    TargetingIr Targeting,
    int RangeUnits,
    int CooldownTicks,
    ResourceCostIr[] Costs,
    TriggerIr[] Triggers,
    PresentationIr? Presentation);

public sealed record TriggerIr(
    TriggerKind Trigger,
    EffectIr[] Effects);

public abstract record EffectIr;

public sealed record DealDamageIr(
    DamageType Type,
    FormulaIr Amount) : EffectIr;

public sealed record ApplyStatusIr(
    StableId StatusId,
    FormulaIr Magnitude,
    int DurationTicks) : EffectIr;
```

### 8.7 Stable IDs and Content Versioning

Use stable, explicit content IDs from the beginning:

```moba
hero FrostMage id: "hero.frost_mage"
ability IceBolt id: "hero.frost_mage.ice_bolt"
```

Support:

- Renaming display labels without breaking saved data.
- Content hash generation.
- Migration warnings for removed IDs.
- IR schema version and compiler version stored in output.
- Later: multiplayer content compatibility checking before match start.

---

## 9. Unity Integration Architecture

### 9.1 Package Structure

```text
Packages/com.fork.dsl/
  package.json
  README.md
  CHANGELOG.md
  LICENSE.md
  Third-Party Notices.txt

  Runtime/
    Core/
      StableId.cs
      FixedValue.cs
      SimulationTick.cs
      DiagnosticsRuntime.cs

    Definitions/
      HeroDefinition.cs
      AbilityDefinition.cs
      StatusDefinition.cs
      ItemDefinition.cs
      MatchDefinition.cs

    Simulation/
      World/
      Entity/
      Stats/
      Resources/
      Targeting/
      Effects/
      Abilities/
      Projectiles/
      StatusEffects/
      Combat/
      Minions/
      Structures/
      Objectives/

    Presentation/
      PresentationEvent.cs
      VfxBridge.cs
      AnimationBridge.cs
      AudioBridge.cs
      MobileFeedbackBridge.cs

    Input/
      MobileJoystickInput.cs
      AbilityAimController.cs
      TargetLockController.cs

    Backends/
      UnityLocal/

  Editor/
    Importers/
      MobaScriptedImporter.cs
    Compiler/
      Lexer/
      Parser/
      Ast/
      Validation/
      Ir/
      Emitters/
    Inspectors/
      HeroDefinitionInspector.cs
      AbilityDefinitionInspector.cs
    Windows/
      CompilerProblemsWindow.cs
      AbilityPreviewWindow.cs

  Tests/
    Editor/
      Compiler/
      Diagnostics/
      Importer/
    Runtime/
      Combat/
      Abilities/
      Determinism/
      Scenario/

  Samples~/
    LocalArena/
    OneLaneVerticalSlice/
    LanguageExamples/

  Documentation~/
    getting-started.md
    language-reference.md
    runtime-extension-guide.md
    packaging-guide.md
```

Use separate Unity assembly definitions for Runtime, Editor and Tests so the compiler/editor tooling does not enter player builds and changes compile in an appropriately bounded assembly.

### 9.2 `.frk` File Import

Unity’s `ScriptedImporter` is suitable for registering a custom `.frk` file extension and importing content whenever matching source files change.

Recommended workflow:

```text
Assets/MobaContent/Heroes/frost_mage.frk
  → MobaScriptedImporter.OnImportAsset(...)
  → compile + validate
  → emit HeroDefinition / AbilityDefinition sub-assets
  → Inspector shows compiled result and diagnostics
```

Importer requirements:

- Import must be deterministic for identical source content.
- Compilation errors should surface in Unity Console with file/line locations.
- Generated assets must not require manual editing.
- Imported definitions should expose a read-only inspector or clearly label generated fields.
- Cache IR hashes to avoid unnecessary downstream rebuilds.

### 9.3 Output Asset Strategy

For the local Unity runtime:

- Emit `ScriptableObject` assets or serializable sub-assets from `.frk` sources.
- Store source hashes and compiler version in output.
- Keep all runtime resolution ID-based rather than object-name-based.
- Emit presentation references separately from simulation definitions.

For a later deterministic backend:

- Reuse the IR.
- Add an adapter that emits or maps compatible deterministic data.
- Do not promise that every local-runtime extension works in every multiplayer backend.

### 9.4 Authoring Tools

#### Free/Core Tooling

- Unity import diagnostics.
- Generated-definition Inspector.
- Basic DSL documentation.
- Syntax highlighting extension.
- Sample scenes.

#### Paid/Pro Tooling Candidates

- Hero kit preview and dependency graph.
- Formula tester and damage breakpoint calculator.
- Status stacking visualizer.
- Ability timeline debugger.
- Match event/replay inspector.
- Lane/wave configuration editor.
- Mobile aiming/targeting presets.
- Balance scenario runner and result dashboard.
- Multiplayer capability linter.

---

## 10. Runtime Simulation Architecture

### 10.1 Gameplay Entity Model

Use a small gameplay entity abstraction independent from `MonoBehaviour` visual objects:

```text
Entity
  StableId
  Team
  Transform / Position
  StatCollection
  ResourceCollection
  StatusCollection
  AbilityLoadout
  Targetable / Damageable flags
```

A presentation GameObject can observe the entity and render it, but the entity owns simulation state.

### 10.2 Simulation Systems

| System | Responsibility |
|---|---|
| TickSystem | Advance fixed gameplay ticks |
| EntityRegistry | Track simulated entities and stable runtime handles |
| StatSystem | Resolve base, additive and multiplicative stats |
| ResourceSystem | Mana/energy/health spending and restoration |
| CooldownSystem | Ability cooldown and charges |
| TargetingSystem | Target rules, validity, selection and range |
| AbilitySystem | Cast requests, validation, trigger dispatch |
| EffectSystem | Execute effect opcodes |
| DamageSystem | Damage typing, mitigation, shields and death |
| StatusSystem | Buff/debuff application, stacking and expiration |
| ProjectileSystem | Deterministic movement/hit queries or adapter routing |
| AreaSystem | Zones and periodic effects |
| MovementSystem | Movement, dash, knockback, immobilization |
| SpawnSystem | Minions, summons and structures |
| ObjectiveSystem | Tower/core/neutral objective states |
| PresentationEventSystem | Publish non-authoritative feedback events |

### 10.3 Effect Primitive Backlog

#### MVP Primitives

- `deal_damage`
- `heal`
- `spend_resource`
- `restore_resource`
- `apply_status`
- `remove_status`
- `modify_stat`
- `shield`
- `spawn_projectile`
- `create_area`
- `select_targets`

#### Vertical Slice Primitives

- `dash`
- `blink`
- `knockback`
- `pull`
- `channel`
- `interrupt`
- `damage_over_time`
- `heal_over_time`
- `cleanse`
- `execute_if`
- `repeat_limited`
- `spawn_unit`
- `grant_vision` or a deliberately postponed equivalent

#### Advanced/Commercial Primitives

- Bouncing projectiles.
- Tethers.
- Terrain/wall effects.
- Stealth/invisibility/true sight.
- Transformations.
- Clones and controllable summons.
- Item active effects.
- Rune/talent systems.

### 10.4 Trigger Model

Provide restricted event triggers:

```text
on_cast
on_hit
on_damage_dealt
on_damage_taken
on_kill
on_death
on_status_applied
on_status_expired
on_basic_attack
on_ability_hit
on_tick_interval
```

Protect against recursive triggers through:

- Trigger-depth limit.
- Per-tick effect budget.
- Compiler cycle analysis for obvious loops.
- Required internal cooldown for dangerous passive combinations.
- Runtime trace diagnostics in development builds.

---

## 11. Mobile MOBA Requirements

### 11.1 Controls

The vertical slice should include mobile-oriented interaction early, because targeting UX changes ability semantics.

Required control modes:

- Virtual movement joystick.
- Tap-to-cast ability.
- Drag-to-aim direction skillshot.
- Drag-to-place ground area.
- Target-lock or priority targeting.
- Cancel zone for ability aiming.
- Auto-attack target selection.

DSL presentation/targeting metadata may describe recommended input behavior:

```moba
ability IceBolt {
    target: direction_skillshot
    controls {
        mobile_aim: drag_release
        show_range_indicator: true
        cancel_radius: 80
    }
}
```

Gameplay validity remains simulation-owned; the control metadata only selects input affordances.

### 11.2 Mobile Performance Budgets

Define target budgets before content expands:

- Fixed simulation tick rate chosen and measured on target Android hardware.
- Controlled allocations during match simulation.
- VFX pooling and projectile pooling.
- Maximum active units, projectiles, areas and status instances in the one-lane sample.
- Profiling on at least one lower/mid-range Android device before online multiplayer.

Do not optimize blindly. Maintain representative stress scenes and measure CPU time, memory allocation and visual frame rate.

---

## 12. Multiplayer Strategy

### 12.1 Do Not Couple the DSL to Multiplayer Yet

The DSL should describe MOBA rules regardless of transport/model. The local runtime proves semantics. Multiplayer support enters through an adapter and capability profile.

Example capability diagnostic:

```text
error MOBQ004: effect 'physics_rigidbody_knockback' is supported by UnityLocal
but is not deterministic-compatible with QuantumProfile. Use
'deterministic_displacement' instead.
```

### 12.2 Candidate A: Photon Quantum Adapter

Photon Quantum is a strong candidate for a competitive mobile MOBA because its current documentation describes:

- A deterministic ECS framework for Unity.
- Predict/rollback networking.
- A separation between simulation logic and Unity presentation.
- A Qtn DSL that generates game-state C# structures.
- Existing history of a MOBA sample.

Potential adapter architecture:

```text
MobaDSL
  → MOBA IR
  → Quantum-compatible content definitions
  → generic Quantum combat systems
  → Quantum deterministic simulation
  → Unity presentation bindings
```

Important design rule: do not attempt to replace Qtn. Your DSL should sit above it as a MOBA authoring layer; Quantum/Qtn remains responsible for the deterministic game-state structure required by its runtime.

### 12.3 Candidate B: Unity Netcode for Entities Adapter

Unity’s current multiplayer documentation positions Netcode for Entities as a server-authoritative solution with client prediction for complex/high-performance multiplayer games. It is worth evaluating if you want a Unity-native stack and greater direct control over hosting/runtime architecture.

Potential adapter architecture:

```text
MobaDSL
  → MOBA IR
  → ECS-compatible configuration data
  → Netcode for Entities authoritative systems and ghosts
  → client prediction/presentation
```

### 12.4 Decision Spike

Do not decide solely by reading product descriptions. Build the same tiny scenario in both candidates or formally reject one early.

**Scenario:** two heroes, one projectile skill, one area skill, one stun, one dash and one moving minion target.

Measure:

| Decision Factor | Evidence to collect |
|---|---|
| Ability integration complexity | Lines/systems/adapters required |
| Deterministic or authoritative behavior | Reconciliation/rollback correctness |
| Mobile performance | Stress test with projectiles/statuses |
| Debug workflow | Ability trace and state inspection |
| Content import path | How IR definitions are loaded |
| Cost/licensing exposure | Terms and expected operating cost |
| Packaging feasibility | Whether an Asset Store tool can disclose/handle dependency cleanly |

### 12.5 Anti-Cheat Boundary

A DSL does not itself stop cheating. Competitive online matches require an authority model in which clients cannot arbitrarily declare successful damage, rewards or progression. Keep authentication, commerce, player progression and authoritative reward granting out of untrusted client-only code.

---

## 13. Headless Simulation and Balance Tooling

### 13.1 Why It Matters

A MOBA DSL becomes more valuable if the same source definitions can run without rendered scenes. This makes the language useful for balance review, regression testing and automated scenario exploration.

### 13.2 Simulator Scope

Initial headless scenarios:

- Single cast damage verification.
- Full hero combo damage/mana/cooldown timeline.
- Hero versus target dummy.
- Level/item breakpoint comparisons.
- Minion versus tower timings.
- Wave collision outcomes.

Later scenarios:

- Automated duel simulations under controlled bot rules.
- Lane farming/pressure simulations.
- Objective time-to-kill analysis.
- Item build comparisons.
- Match telemetry replay validation.

### 13.3 Simulator CLI Example

```text
mobadsl simulate duel \
  --hero frost_mage \
  --opponent iron_guardian \
  --level 6 \
  --iterations 1000 \
  --seed 12345 \
  --output results/duel_frost_vs_guardian.json
```

### 13.4 Simulator Guardrails

Simulation output is only useful if clearly scoped. Avoid claiming that simulated win rates equal real-player balance. Label scenarios, assumptions, bot logic and seeds in all outputs.

---

## 14. Testing and Quality Plan

### 14.1 Compiler Tests

| Test Group | Examples |
|---|---|
| Lexer/parser golden tests | Valid source produces expected AST |
| Invalid syntax tests | Missing brace; invalid units; malformed formula |
| Diagnostic snapshot tests | Exact error codes and locations |
| Symbol/reference tests | Missing status or duplicate hero ID |
| IR serialization tests | Stable deterministic output |
| Importer tests | Changed `.frk` produces correct Unity assets |

### 14.2 Runtime Tests

| Test Group | Examples |
|---|---|
| Damage math | Physical/magic/true damage; shield handling |
| Status effects | Stack/refresh/replace/expire rules |
| Targeting | Enemy/ally/ground/range/line validation |
| Ability lifecycle | Cast, cost, cooldown, hit, interrupt |
| Projectile behavior | Hit selection and travel timing |
| Items | Passive triggers and internal cooldowns |
| Structures/waves | Tower targeting; minion spawn timing |

### 14.3 Deterministic-Safety Tests

Even for the local backend, add a replay harness:

1. Start from an identical source-derived match state.
2. Feed identical ordered inputs and random seed.
3. Run simulation more than once.
4. Compare state hashes at selected ticks.
5. Fail if state diverges.

This does not prove network determinism in every future backend, but it prevents many architecture choices that would block deterministic integration later.

### 14.4 Mobile/Performance Tests

Build stress scenarios:

- Large number of projectiles.
- Many simultaneous area effects.
- Heavy status stacking.
- Full minion wave plus structures.
- Rapid repeated casting.

Collect:

- Simulation duration per tick.
- Allocations per tick.
- Frame rate in presentation scene.
- Device thermal/performance degradation during sustained play.
- Memory footprint.

### 14.5 Release Gates

No public release should ship with:

- Compiler errors in included examples.
- Runtime exceptions in demo scenes.
- Missing setup documentation.
- Undisclosed dependencies.
- Unlicensed third-party assets.
- AI-generated functional package content that requires disclosure under current marketplace rules but is not disclosed.

---

## 15. Development Phases and Milestones

This roadmap uses outcome gates rather than fixed dates. Continue only when the previous phase has demonstrated its core hypothesis.

### Phase 0 — Discovery and Architecture Lock

**Goal:** prevent building a large language before confirming the runtime model.

Deliverables:

- Product requirements document.
- Ten representative ability definitions on paper, ranging from simple to difficult.
- Effect primitive inventory.
- Runtime architecture decision record.
- DSL syntax sketch.
- Local Unity prototype plan.
- Licensing/open-core boundary draft.

Representative ability challenge set:

1. Targeted projectile with slow.
2. Ground AoE damage.
3. Dash that shields on arrival.
4. Channeled beam interrupted by stun.
5. Damage-over-time passive.
6. Execute based on missing health.
7. Knockback cone.
8. Shield that explodes on expiry.
9. Summoned controllable unit.
10. Bounce/chain ability.

Exit criteria:

- At least the first eight abilities can be expressed by planned primitives without bespoke hero-specific systems.
- Clear decisions exist for triggers, formulas, tick timing and presentation events.

### Phase 1 — Manual Local Combat Runtime

**Goal:** prove core gameplay systems in C# before introducing compiler complexity.

Deliverables:

- Unity package skeleton and assemblies.
- Fixed-tick local simulation loop.
- Gameplay entity/stat/resource model.
- Ability, cooldown, damage, status and targeting systems.
- Manually constructed definitions for two example heroes.
- Local arena with keyboard controls for rapid testing.
- Core unit tests.

Exit criteria:

- Two heroes can fight using manually created definitions.
- No hero-specific behavior code is required for the chosen initial abilities.
- Basic replay/hash test passes for repeated local simulations.

### Phase 2 — DSL Compiler Alpha

**Goal:** author the existing combat content in `.frk` rather than manual Unity definitions.

Deliverables:

- Lexer, parser and source spans.
- AST and semantic passes.
- MOBA IR v0.
- Error codes and readable diagnostics.
- `.frk` ScriptedImporter.
- ScriptableObject/sub-asset emitter.
- Example hero sources replacing manually authored definitions.
- Language reference v0.

Supported syntax:

- `hero`, `stats`, `ability`, `status`.
- Cost, cooldown, range and targeting.
- Damage, heal, slow, stun, shield and basic modifiers.
- Simple formulas.

Exit criteria:

- All Phase 1 gameplay runs from compiled DSL sources.
- Common mistakes produce actionable errors.
- New basic ability creation requires only `.frk` edits and presentation assets.

### Phase 3 — Ability Breadth and Editor Preview

**Goal:** handle enough mechanics for real hero-kit experimentation.

Deliverables:

- Projectiles, areas, dash, knockback, damage-over-time, passive triggers and controlled conditionals.
- Ability dependency viewer or Inspector summary.
- Formula tester.
- Editor compilation problems panel.
- Four to six example heroes with distinct kits.
- Stress tests for combat systems.

Exit criteria:

- Most challenge-set abilities are representable.
- A developer unfamiliar with core runtime code can define a simple hero by following documentation.
- Performance is acceptable in representative local combat stress tests.

### Phase 4 — Mobile One-Lane Vertical Slice

**Goal:** prove that the product supports its intended game category rather than only isolated combat.

Deliverables:

- Mobile input and aiming modes.
- Minions, towers/core, lane wave spawner and match end condition.
- Items/shop subset and progression subset.
- One-lane mobile demo.
- Two teams playable locally or with bots.
- Setup/tutorial video and public-facing screenshots.

Exit criteria:

- The demo feels recognizably like a mobile-MOBA foundation.
- Hero, item and lane behavior primarily comes from DSL content.
- Public developer feedback can begin meaningfully.

### Phase 5 — Public Core Release and Demand Validation

**Goal:** determine whether a community wants this authoring approach.

Free/public deliverables:

- Public GitHub repository for `MobaDSL Core`.
- License, contribution rules and roadmap.
- Basic compiler/runtime and local demo.
- Documentation, examples and issue templates.
- Public feedback form or discussions.

Do not publish as a paid product yet unless installation/documentation and support expectations are manageable.

Exit criteria:

- Meaningful external setup attempts and issue feedback.
- At least some independent authored content or explicit demand for Pro features.
- Prioritized commercial feature list based on evidence.

### Phase 6 — Multiplayer Backend Spike

**Goal:** select a credible production multiplayer direction.

Deliverables:

- Backend capability abstraction in IR.
- Small identical gameplay test in Photon Quantum and/or Unity Netcode for Entities.
- Technical decision record with measured evidence.
- Packaging/licensing/dependency review.
- Multiplayer-specific deterministic lints.

Exit criteria:

- Selected backend can execute target primitives without a fundamental redesign.
- Its dependency and commercial implications can be disclosed cleanly.
- There is a roadmap for a paid adapter or integrated Pro version.

### Phase 7 — Pro Product Candidate

**Goal:** produce a sellable toolkit rather than a research prototype.

Potential Pro deliverables:

- Expanded effect/runtime suite.
- High-quality editors, debugging and documentation.
- Mobile controls/targeting templates.
- Playable polished one-lane demo.
- Balance scenario runner.
- One production-ready multiplayer adapter or a clearly labeled beta adapter.
- Upgrade/support policy and sample licensing clarity.

Exit criteria:

- Complete marketed features work without external unlocking.
- Package meets current Unity Asset Store submission requirements.
- Documentation supports purchasers without direct onboarding.
- Pricing is justified by working productivity gains.

---

## 16. Initial Backlog

### Epic A — Repository and Package Foundation

- Establish repository structure and branching policy.
- Create Unity package with Runtime, Editor and Tests assemblies.
- Add CI for C# tests and code formatting.
- Define stable ID, tick and fixed-value abstractions.
- Add sample project integration workflow.

### Epic B — Runtime Kernel

- Build fixed-tick driver.
- Implement entity registry.
- Implement stats/resources/cooldowns.
- Implement target selection.
- Implement damage and death.
- Implement status application and expiration.
- Implement presentation event queue.

### Epic C — Ability Content Model

- Define in-memory ability definitions.
- Implement cast validation.
- Implement effect executor registry.
- Implement first primitive effects.
- Add tracing for ability execution.
- Create manually configured FrostMage and IronGuardian.

### Epic D — Compiler Frontend

- Token model and lexer.
- Parser and AST.
- Source span preservation.
- Diagnostic system.
- Symbol table.
- Unit/type checker.
- Formula parser/evaluator.
- Initial IR.

### Epic E — Unity Emitter and Importer

- `.frk` `ScriptedImporter`.
- Generated asset model.
- Import diagnostics into Console.
- Reimport tests.
- Read-only compiled-definition Inspector.
- Example source conversion.

### Epic F — MOBA Vertical Slice

- Projectile/area/dash primitives.
- Mobile control mapping.
- Basic attack and target selection.
- Minion waves.
- Structures and match condition.
- Item/stat upgrades.
- Demo scene and tutorial.

### Epic G — Productization

- Docs and sample content.
- Extension API.
- Versioning/changelog.
- Licensing and third-party notices.
- Public repository setup.
- Asset Store-ready Pro packaging later.

---

## 17. Open-Core and Monetization Implementation Plan

### 17.1 Free `MobaDSL Core`

Proposed public GitHub contents:

- DSL specification.
- Parser/compiler and validator for supported core features.
- Basic Unity importer.
- Core runtime primitives.
- Two-hero local arena sample.
- Syntax highlighting support.
- Public documentation and contribution process.

Purpose:

- Validate developer interest.
- Build trust.
- Encourage examples and issue reports.
- Provide a real proof rather than marketing screenshots only.

### 17.2 Paid `MobaDSL Pro for Unity`

Potential paid value:

- Expanded MOBA primitive library.
- Hero/item/lane/objective editor tooling.
- Mobile MOBA input/targeting templates.
- Debug/replay/trace tools.
- Balance simulation tooling.
- Polished one-lane starter project.
- Production support and update cadence.
- Multiplayer backend adapter or premium add-on.

### 17.3 Asset Store Preparation

Before publishing a paid Unity package:

- Ensure it is professionally usable after setup and does not produce package-originated errors/warnings.
- Include comprehensive documentation for code/configuration.
- Declare package dependencies and provide third-party notices.
- Ensure the marketed product is complete; do not require external payment to unlock promised Asset Store functionality.
- Review current rules on AI-assisted/generated functional content and disclose as required.
- Confirm compatibility and packaging rules current at submission time.

---

## 18. AI-Assisted Coding Policy for This Project

### 18.1 Direct Answer: Can You Code This with OpenCode Zen’s Free AI Models?

**Yes for public, disposable or non-confidential portions of the project; no for confidential or commercially sensitive portions you do not want retained or used for model improvement.**

As verified on **2026-05-29**, OpenCode Zen documentation lists free options including **DeepSeek V4 Flash Free**, **MiMo-V2.5 Free**, **Nemotron 3 Super Free** and **Big Pickle**. The same documentation states:

- Data collected during the free period for Big Pickle, DeepSeek V4 Flash Free and MiMo-V2.5 Free may be used to improve the model.
- Nemotron 3 Super Free under NVIDIA free endpoints is trial-use-only, is not for production or sensitive data, and prompts/outputs are logged to improve models and services.
- Zen’s non-exception providers are described as zero-retention/no-training, while listed OpenAI and Anthropic API routes have 30-day retention policies.

This means free-model use is a **source-code confidentiality decision**, not merely a code-quality decision.

### 18.2 Safe Uses of Free Models

Reasonable free-model tasks when the associated information is public or non-secret:

- Brainstorming DSL syntax.
- Generating example public `.frk` hero files.
- Explaining compiler theory, parsing patterns or Unity APIs.
- Writing tests for code already intended to be open source.
- Refactoring documentation and README content.
- Implementing the planned free/core repository after you have decided it will be publicly released.
- Producing disposable prototype code with no credentials or business-sensitive content.

### 18.3 Do Not Send to Free Models

Do not supply:

- API keys, signing keys, tokens, service credentials or `.env` contents.
- Private repository code intended to remain proprietary.
- Paid `MobaDSL Pro` implementation before you accept disclosure risk.
- Security vulnerabilities that have not been fixed/disclosed.
- Customer data, telemetry, user identifiers or production logs.
- Store credentials, tax/payout records or business/private account details.
- Unreleased proprietary backend adapter algorithms or licensing material.

### 18.4 Recommended AI Workflow

Use a tiered workflow:

| Work Category | Recommended AI Route |
|---|---|
| Public language spec, examples, docs and free-core prototype | Free Zen models are acceptable after stripping secrets |
| Boilerplate/tests for code that will be MIT/public | Free Zen models are acceptable with human review |
| Pro runtime, unique commercial features and unreleased adapter code | Use a paid provider configuration whose terms meet your confidentiality requirements, or run a capable local model |
| Credentials, security-sensitive configuration, store keys, customer data | Do not paste into an AI coding model; handle manually/secrets tooling |
| Architectural or security review before release | Use a trusted model route plus manual verification and automated tests |

### 18.5 Coding-Agent Guardrails

Whether using free or paid AI:

- Never allow the agent to read secret files; deny/ignore `.env`, keystores, signing files and local credential paths.
- Keep each task small and require tests.
- Use branches and inspect every diff before merge.
- Never accept generated dependency additions without checking license and maintenance risk.
- Do not trust generated multiplayer/security logic without explicit tests and review.
- Run formatting, compilation, tests and static checks after each agent task.
- Require the agent to state assumptions and changed files.
- For Unity Asset Store submissions, document any AI-assisted/generated functional content if current rules require disclosure.

### 18.6 Suggested Model Task Split

A practical division for this project:

```text
Free Zen model:
  public docs, DSL examples, small parser unit tests, public core refactors

Trusted paid or local route:
  architecture decisions, Pro source code, multiplayer adapters,
  security-sensitive code, licensing-sensitive packaging

Human-controlled only:
  secrets, signing, payments, account configuration, final release approval
```

---

## 19. Risk Register

| Risk | Probability | Impact | Mitigation |
|---|---:|---:|---|
| DSL becomes a general programming language | High | High | Keep primitives constrained; require runtime extensions for novel behavior |
| Runtime cannot express diverse heroes cleanly | Medium | High | Challenge-set abilities before syntax lock; prioritize primitives |
| Multiplayer requires redesign | Medium | High | Tick-based simulation and presentation split from Phase 1; adapter spike before Pro promises |
| Editor/tooling takes more work than compiler | High | Medium | Deliver diagnostics/importer first; postpone advanced visual editors |
| Low developer demand | Medium | High | Public demo/free core before investing deeply in Pro features |
| Competitor/framework changes | Medium | Medium | Keep backend-neutral IR and verify market before launch |
| Asset Store policy/licensing issue | Medium | High | Third-party notices, self-contained product and policy check before submission |
| AI leaks proprietary code | Medium | High | Enforce task/data classification and do not use free-retained endpoints for private code |
| Mobile performance fails under combat load | Medium | High | Stress tests, pooling and device profiling before multiplayer scope |
| Balance simulator overpromises results | Medium | Medium | Label assumptions; use it for regression/scenarios rather than absolute fairness claims |

---

## 20. Key Decision Records to Create

Create short architecture decision records (ADRs) as the project evolves:

1. **ADR-001:** Data-driven runtime versus generated per-ability C#.
2. **ADR-002:** Simulation tick/fixed-number policy.
3. **ADR-003:** DSL grammar and parser choice.
4. **ADR-004:** IR schema/versioning and stable ID strategy.
5. **ADR-005:** Unity asset emission and import workflow.
6. **ADR-006:** Presentation event boundary.
7. **ADR-007:** Free-core versus Pro feature boundary.
8. **ADR-008:** Multiplayer backend decision after spike.
9. **ADR-009:** AI-assisted coding data classification policy.
10. **ADR-010:** Asset Store packaging/licensing policy.

---

## 21. First Implementation Sprint Checklist

The first implementation effort should not begin with grammar work. Build the executable core:

### Repository and Unity Setup

- [ ] Create repository and package folder `com.fork.dsl`.
- [ ] Add Runtime, Editor and Test assembly definitions.
- [ ] Add CI/build/test pipeline.
- [ ] Define formatting and naming rules.
- [ ] Add an ADR folder and write ADR-001 draft.

### Runtime Foundation

- [ ] Implement simulation tick loop.
- [ ] Implement `StableId`, entity handle and team identity.
- [ ] Implement stats and resources.
- [ ] Implement health/damage/death.
- [ ] Implement cooldown and basic ability activation.
- [ ] Implement status effect application/expiration.
- [ ] Implement presentation event collection.

### Manual Gameplay Proof

- [ ] Build `IceBolt`: projectile damage + slow.
- [ ] Build `ShieldDash`: dash + shield.
- [ ] Build target dummy and combat test scene.
- [ ] Add automated tests for damage, cooldown and status expiration.
- [ ] Add deterministic replay/state-hash smoke test.

### Only After Runtime Proof

- [ ] Write initial `.frk` grammar.
- [ ] Implement parser.
- [ ] Convert the manually configured abilities into DSL sources.
- [ ] Implement importer/output assets.

---

## 22. Questions to Resolve Before Large Investment

- Will the first commercial value be **ability authoring**, **complete one-lane template**, or **balance/debug tooling**?
- Which Unity version and render pipeline will be the baseline for the first public sample?
- Will Core permit commercial use under a permissive open-source license, and exactly which runtime capabilities remain Pro?
- Does the intended multiplayer adapter become a separate paid package or ship in a higher Pro tier?
- What performance budget must the runtime meet on an actual mid-range Android target?
- Does the language support items/lanes before multiplayer, or is combat breadth a better validation target?
- What kind of IDE/editor tooling is necessary for early adoption: syntax highlighting only, or Unity editor previews?
- Which source files are permitted to be processed by free/retained AI endpoints?

---

## 23. Recommended Immediate Next Actions

1. Write the ten challenge-set ability definitions as design examples, without implementing them.
2. Implement a local fixed-tick Unity combat runtime for two abilities using manually defined data.
3. Confirm that the runtime can execute those abilities without hero-specific scripts.
4. Only then write compiler/parser code and import `.frk` definitions.
5. Treat all code intended to remain private or paid as confidential: do not upload it to OpenCode Zen free-model endpoints unless you knowingly accept the stated data-use risk.

---

## 24. Verified Technical and Policy Sources

The following sources were checked for this plan on **2026-05-29**:

1. **Unity Manual — Scripted Importers**  
   Unity documents `ScriptedImporter` as the way to add support for custom file formats in C#, with `OnImportAsset` invoked for registered matching file extensions.  
   <https://docs.unity.cn/Manual/ScriptedImporters.html>

2. **Unity Multiplayer Documentation — Unity Netcode Packages**  
   Unity documents Netcode for Entities as server-authoritative with client prediction and suitable for high-performance multiplayer games with complex gameplay.  
   <https://docs.unity.com/en-us/multiplayer/netcode/netcode>

3. **Photon Quantum 3 Introduction**  
   Photon documents Quantum as a deterministic ECS for Unity using predict/rollback networking and separating simulation from Unity presentation; it also documents its Qtn DSL for game-state data/code generation.  
   <https://doc.photonengine.com/en-us/Quantum>

4. **Photon Quantum MOBA Sample History — Tiki Frutti**  
   Photon documents a prior real-time MOBA sample built using Quantum, useful as evidence that the technical stack has been applied to the game category.  
   <https://doc.photonengine.com/quantum/v1/demos-and-tutorials/moba>

5. **OpenCode Zen Documentation — Models, Pricing and Privacy**  
   Current Zen documentation lists its available/free coding models and states exceptions to its zero-retention/no-training policy, including free models whose collected data may be used to improve models; it states NVIDIA free endpoints are not for production or sensitive data.  
   <https://dev.opencode.ai/docs/zen>

6. **Unity Asset Store Submission Guidelines**  
   Current guidelines state requirements relevant to a future paid package, including professional usability, documentation for code/configuration, dependency disclosure, third-party notices, restrictions on unlocking marketed functionality through additional payment, and AI-assisted/generated-content disclosure rules.  
   <https://marketplace.unity.com/publishing/submission-guidelines>

---

## 25. Closing Recommendation

Build `MobaDSL` in this order:

```text
Reusable local simulation runtime
  → tiny DSL and compiler
  → Unity importer and diagnostics
  → mobile one-lane vertical slice
  → public free-core validation
  → measured multiplayer backend spike
  → paid production tooling
```

Use OpenCode Zen free models only for material you are comfortable treating as non-confidential or potentially useful for model improvement. For the paid/proprietary runtime, sensitive integrations, security work and unreleased commercial differentiators, use a route whose retention/training terms match your confidentiality requirement—or keep the work local and human-reviewed.

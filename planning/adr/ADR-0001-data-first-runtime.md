# ADR-0001: Data-First Definition Execution vs. Generated Per-Ability C# Scripts

## Context

In Unity, game abilities are traditionally coded via custom `MonoBehaviour` scripts per hero/ability, or generated C# script assemblies compiled dynamically. This creates:
1. Compilation wait-times when designers adjust combat balances.
2. Debugging complexities (navigating generated source code).
3. Risks of platform compilation issues (e.g., AOT/IL2CPP targets on iOS/Android).
4. Safety/determinism challenges for rollback-based networking.

## Decision

We will compile `.frk` DSL source files into structured, serializable data definitions rather than emitting separate C# scripts. A unified, generic gameplay engine runtime will read these serialized data definitions and execute them using pre-compiled, tested primitive actions (e.g., `DealDamage`, `ApplyStatus`, `SpawnProjectile`).

```text
.frk source 
  → Compiler 
  → MOBA Intermediate Representation (IR) 
  → Serialized definition asset (ScriptableObject)
  → Reusable C# Simulation Runtime Systems
```

## Consequences

### Positive
- **Fast Iteration:** Changes to stats, cooldowns, and effect lists in `.frk` files compile instantly into assets without triggering Unity C# assembly compilation.
- **Safety & Verification:** High-level DSL rules can be statically validated before execution, preventing arbitrary logic errors, runtime null-pointers, or illegal memory operations.
- **AOT Compatibility:** Safe for IL2CPP and console deployment since no runtime code generation or dynamic assembly loading is required.

### Negative
- **Extensibility Limit:** Adding new core combat mechanics (e.g., "chain bouncing" or "clones") requires writing a C# effect primitive and registering it with the compiler/runtime, rather than just scripting it inside the DSL itself.

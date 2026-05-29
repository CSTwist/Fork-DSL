# ADR-0002: Backend-Neutral Intermediate Representation (IR) Boundary

## Context

A key requirement of MobaDSL is to support multiple backends. Initially, we need a local, offline simulation for single-player play and designer validation. Eventually, we want to integrate with deterministic multiplayer frameworks (such as Photon Quantum 3) or server-authoritative netcode (such as Unity Netcode for Entities). 
If the compiler is tightly coupled to Unity's local game engine logic, adapting it to these other network backends would require a complete rewrite of the compiler.

## Decision

We will design a strict compilation boundary: the compiler frontend parses `.frk` text and emits a backend-neutral **MOBA Intermediate Representation (IR)**. The IR is a pure C# data structure (expressed as records/classes) that does not reference any Unity engine symbols (such as `ScriptableObject`, `Vector3`, or `MonoBehaviour`).

```text
MobaDSL Frontend 
  → MOBA IR (Strict Boundary)
      ├── Local Unity Emitter (ScriptableObjects)
      ├── Photon Quantum Adapter (Qtn files / data models)
      └── Netcode for Entities Adapter (ECS Components)
```

## Consequences

### Positive
- **Backend Portability:** The core compiler, diagnostics, and semantic validation can run headless in a CLI, or easily emit configuration models for Quantum or ECS.
- **Testability:** Compiler parsing and semantic checking can be fully tested in pure C# NUnit environments without opening Unity or spinning up a game scene.

### Negative
- **Translation Layer:** Requires separate backend emitters to map IR nodes to local Unity assets or specific network state models, adding a translation layer.

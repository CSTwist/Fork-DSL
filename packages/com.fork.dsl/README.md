# Fork DSL (`com.fork.dsl`)

Domain-specific language and runtime for authoring mobile-MOBA gameplay in Unity.

## Structure
- `Runtime/`: Core combat engine, tick system, stats, resources, status effects, and abilities.
- `Editor/`: Compiler front-end, lexer, parser, static validation, and `.frk` scripted importers.
- `Tests/`: NUnit tests for verifying compilation correctness, gameplay balance logic, and determinism.
- `Samples~/`: Playable local arena demo and mobile inputs.

## Installation
Add the package to your Unity project's `Packages/manifest.json`:
```json
"dependencies": {
  "com.fork.dsl": "file:../packages/com.fork.dsl"
}
```

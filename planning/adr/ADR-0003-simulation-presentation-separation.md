# ADR-0003: Simulation and Presentation Separation

## Context

MOBA gameplay relies on absolute consistency in gameplay resolution (e.g., did a skillshot hit a hero, did a stun disrupt a channel, what is the exact health left). If gameplay is tied to Unity visual frame rate, animation state, or visual GameObject lifecycle:
1. Physics/movement calculations are susceptible to frame drops.
2. Rollback-based netcode becomes impossible to implement (since visuals cannot easily be rolled back and resimulated deterministically).
3. Automated headless testing of gameplay scenarios cannot happen without rendering scenes.

## Decision

We enforce a strict boundary between gameplay simulation and visual presentation:
- **Simulation:** Owns the true game state. Runs on fixed ticks independent of frame rate. Resolves stats, cooldowns, damage, and projectile coordinates.
- **Presentation:** Listens to state events emitted by the Simulation. Controls rendering, particles, skeletal animations, and audio playback.

```text
[Simulation Ticks] 
  → calculates state transitions 
  → emits Gameplay Events (e.g. projectile_spawned, entity_damaged)
  → [Presentation Observers] update visuals (VFX, play audio, animate)
```

## Consequences

### Positive
- **Determinism:** The gameplay simulation is pure and predictable. Identical tick inputs will yield identical game outcomes.
- **Multiplayer-Ready:** Easily compatible with predict/rollback network stacks (Photon Quantum).
- **Headless Testing:** Ability scenarios can be tested in console CLI environments.

### Negative
- **Visual Synchronization:** Requires extra effort to align presentation objects with their corresponding simulation coordinates (e.g., visual projectile following simulation tick movement).
- **Redundancy:** Separate state definitions (e.g., entity simulation position vs visual GameObject transform).

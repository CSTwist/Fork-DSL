using System.Collections;
using UnityEngine;
using MobaDSL.Runtime;

namespace MobaDslDev
{
    public class MobaEntityObserver : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [SerializeField] private int ticksPerSecond = 20;

        [Header("Visual Settings")]
        [SerializeField] private GameObject orderPrefab;
        [SerializeField] private GameObject chaosPrefab;
        [SerializeField] private Material orderMaterial;
        [SerializeField] private Material chaosMaterial;

        private WorldSimulation _simulation;
        private EntityId _orderHeroId;
        private EntityId _chaosDummyId;

        private GameObject _orderVisual;
        private GameObject _chaosVisual;

        private void Start()
        {
            // 1. Initialize Simulation config and state
            var config = new SimulationConfig { TicksPerSecond = ticksPerSecond };
            _simulation = new WorldSimulation(config);

            // Subscribe to gameplay events to drive presentation
            _simulation.EventStream.OnEventEmitted += HandleGameplayEvent;

            // 2. Setup Base Stats for the combatants
            var heroStats = new StatBlock();
            heroStats.SetBase(StatType.MaxHealth, FixedValue.FromInt(300));
            heroStats.SetBase(StatType.MaxMana, FixedValue.FromInt(100));
            heroStats.SetBase(StatType.AttackDamage, FixedValue.FromInt(45));

            var dummyStats = new StatBlock();
            dummyStats.SetBase(StatType.MaxHealth, FixedValue.FromInt(500));
            dummyStats.SetBase(StatType.MaxMana, FixedValue.FromInt(50));
            dummyStats.SetBase(StatType.Armor, FixedValue.FromInt(50)); // 33% physical mitigation

            // 3. Spawn simulated entities
            _orderHeroId = _simulation.SpawnEntity(
                new HeroId("OrderMage"), 
                TeamId.Order, 
                heroStats, 
                new Vector2Fixed(FixedValue.FromInt(-3), FixedValue.Zero)
            );

            _chaosDummyId = _simulation.SpawnEntity(
                new HeroId("ChaosDummy"), 
                TeamId.Chaos, 
                dummyStats, 
                new Vector2Fixed(FixedValue.FromInt(3), FixedValue.Zero)
            );

            // 4. Create visual game objects to represent them
            _orderVisual = CreateVisual(orderPrefab, "Order Hero (Capsule)", new Vector3(-3, 1, 0), orderMaterial);
            _chaosVisual = CreateVisual(chaosPrefab, "Chaos Dummy (Capsule)", new Vector3(3, 1, 0), chaosMaterial);

            // 5. Start simulation update loop
            StartCoroutine(SimulationLoop());
        }

        private GameObject CreateVisual(GameObject prefab, string name, Vector3 pos, Material mat)
        {
            GameObject obj;
            if (prefab != null)
            {
                obj = Instantiate(prefab, pos, Quaternion.identity);
            }
            else
            {
                // Fallback to standard Unity capsule primitive
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obj.transform.position = pos;
            }
            obj.name = name;

            if (mat != null)
            {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = mat;
                }
            }

            return obj;
        }

        private IEnumerator SimulationLoop()
        {
            float interval = 1f / ticksPerSecond;
            while (true)
            {
                yield return new WaitForSeconds(interval);
                _simulation.Tick();

                // Periodically make Order Hero auto-attack the Chaos Dummy (every 1.5 seconds / 30 ticks)
                if (_simulation.CurrentTick % 30 == 0 && !_simulation.Registry.GetEntity(_chaosDummyId).IsDead)
                {
                    FixedValue damage = _simulation.Registry.GetEntity(_orderHeroId).Stats.GetVal(StatType.AttackDamage);
                    _simulation.ApplyDamage(_orderHeroId, _chaosDummyId, damage, DamageType.Physical);
                }
            }
        }

        private void HandleGameplayEvent(GameplayEvent ev)
        {
            switch (ev.Kind)
            {
                case GameplayEventKind.DamageApplied:
                    var dmgEvent = (DamageAppliedEvent)ev;
                    Debug.Log($"<color=orange>[Sim Event]</color> {dmgEvent}");
                    
                    // Visual feedback: Flash target red
                    if (dmgEvent.Target == _chaosDummyId)
                    {
                        StartCoroutine(FlashColor(_chaosVisual, Color.red, 0.15f));
                    }
                    else if (dmgEvent.Target == _orderHeroId)
                    {
                        StartCoroutine(FlashColor(_orderVisual, Color.red, 0.15f));
                    }
                    break;

                case GameplayEventKind.Healed:
                    var healEvent = (HealedEvent)ev;
                    Debug.Log($"<color=green>[Sim Event]</color> {healEvent}");
                    
                    if (healEvent.Target == _chaosDummyId)
                    {
                        StartCoroutine(FlashColor(_chaosVisual, Color.green, 0.15f));
                    }
                    break;

                case GameplayEventKind.EntityDefeated:
                    var deathEvent = (EntityDefeatedEvent)ev;
                    Debug.Log($"<color=red>[Sim Event]</color> {deathEvent}");
                    
                    // Visual feedback: Shrink and hide visual
                    if (deathEvent.Entity == _chaosDummyId)
                    {
                        StartCoroutine(PlayDeathVisual(_chaosVisual));
                    }
                    break;
            }
        }

        private IEnumerator FlashColor(GameObject visual, Color flashColor, float duration)
        {
            if (visual == null) yield break;
            var renderer = visual.GetComponent<Renderer>();
            if (renderer == null) yield break;

            Color originalColor = renderer.material.color;
            renderer.material.color = flashColor;
            yield return new WaitForSeconds(duration);
            if (visual != null)
            {
                renderer.material.color = originalColor;
            }
        }

        private IEnumerator PlayDeathVisual(GameObject visual)
        {
            if (visual == null) yield break;
            Vector3 startScale = visual.transform.localScale;
            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (visual == null) yield break;
                visual.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);
                yield return null;
            }

            if (visual != null)
            {
                visual.SetActive(false);
            }
        }

        private void OnGUI()
        {
            if (_simulation == null) return;

            // Draw a basic overlay for monitoring state
            GUILayout.BeginArea(new Rect(20, 20, 300, 300));
            GUILayout.Label("<b>MobaDSL Simulation Sandbox</b>", GetStyle(16, Color.white));
            GUILayout.Space(10);
            GUILayout.Label($"Current Tick: {_simulation.CurrentTick}", GetStyle(12, Color.white));

            DrawEntityGUI(_orderHeroId, "Order Hero");
            GUILayout.Space(10);
            DrawEntityGUI(_chaosDummyId, "Chaos Dummy");

            GUILayout.Space(15);
            if (GUILayout.Button("Manual Attack (Order -> Chaos)", GUILayout.Height(30)))
            {
                CombatEntityState order = _simulation.Registry.GetEntity(_orderHeroId);
                FixedValue dmg = order.Stats.GetVal(StatType.AttackDamage);
                _simulation.ApplyDamage(_orderHeroId, _chaosDummyId, dmg, DamageType.Physical);
            }

            if (GUILayout.Button("Respawn Chaos Dummy", GUILayout.Height(30)))
            {
                _simulation.RespawnEntity(_chaosDummyId, new Vector2Fixed(FixedValue.FromInt(3), FixedValue.Zero));
                if (_chaosVisual != null)
                {
                    _chaosVisual.SetActive(true);
                    _chaosVisual.transform.localScale = Vector3.one;
                }
            }
            GUILayout.EndArea();
        }

        private void DrawEntityGUI(EntityId id, string displayName)
        {
            CombatEntityState entity = _simulation.Registry.GetEntity(id);
            if (entity == null) return;

            FixedValue hp = entity.Health.GetCurrentClamped(entity.Stats);
            FixedValue maxHp = entity.Stats.GetVal(StatType.MaxHealth);
            FixedValue armor = entity.Stats.GetVal(StatType.Armor);

            GUILayout.Label($"<b>{displayName}</b> (ID: {id.Value})", GetStyle(12, Color.cyan));
            GUILayout.Label($"Health: {hp} / {maxHp} (Armor: {armor})", GetStyle(11, Color.white));
            
            // Mana representation
            FixedValue mp = entity.Mana.GetCurrentClamped(entity.Stats);
            FixedValue maxMp = entity.Stats.GetVal(StatType.MaxMana);
            GUILayout.Label($"Mana: {mp} / {maxMp}", GetStyle(11, Color.white));
        }

        private GUIStyle GetStyle(int fontSize, Color color)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
            style.normal.textColor = color;
            style.richText = true;
            return style;
        }

        private void OnDestroy()
        {
            if (_simulation != null)
            {
                _simulation.EventStream.OnEventEmitted -= HandleGameplayEvent;
            }
        }
    }
}

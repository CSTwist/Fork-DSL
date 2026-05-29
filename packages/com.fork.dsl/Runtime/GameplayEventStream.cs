using System;
using System.Collections.Generic;

namespace MobaDSL.Runtime
{
    [Serializable]
    public class GameplayEventStream
    {
        private readonly List<GameplayEvent> _events = new List<GameplayEvent>();
        
        [field: NonSerialized]
        public event Action<GameplayEvent> OnEventEmitted;

        public IReadOnlyList<GameplayEvent> Events => _events;

        public void Emit(GameplayEvent ev)
        {
            if (ev == null) return;
            _events.Add(ev);
            OnEventEmitted?.Invoke(ev);
        }

        public void Clear()
        {
            _events.Clear();
        }
    }
}

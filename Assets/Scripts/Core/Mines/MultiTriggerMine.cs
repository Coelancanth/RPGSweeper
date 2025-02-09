using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class MultiTriggerMine : BaseMine
    {
        private int m_TriggerCount;
        private readonly int m_RequiredTriggers;

        public MultiTriggerMine(MineData _data, Vector2Int _position) : base(_data, _position)
        {
            m_TriggerCount = 0;
            m_RequiredTriggers = Mathf.Max(2, _data.Value); // Minimum 2 triggers required
        }

        public override void OnTrigger(Player _player)
        {
            if (m_IsDestroyed) return;

            m_TriggerCount++;
            
            if (m_TriggerCount >= m_RequiredTriggers)
            {
                _player.TakeDamage(m_Data.Value);
                GameEvents.RaiseMineTriggered(Type);

                foreach (var effect in m_Data.Effects)
                {
                    ApplyEffect(effect);
                }

                OnDestroy();
            }
            else
            {
                // Visual feedback that mine was triggered but not destroyed
                GameEvents.RaiseEffectApplied(m_Position);
            }
        }
    }
} 
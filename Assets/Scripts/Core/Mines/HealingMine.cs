using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class HealingMine : BaseMine
    {
        public override bool CanDisguise => true; // Can look like a health pack

        public HealingMine(MineData _data, Vector2Int _position) : base(_data, _position)
        {
        }

        public override void OnTrigger(Player _player)
        {
            if (m_IsDestroyed) return;

            _player.TakeDamage(-m_Data.Value); // Negative value for healing
            GameEvents.RaiseMineTriggered(Type);

            foreach (var effect in m_Data.Effects)
            {
                ApplyEffect(effect);
            }

            OnDestroy();
        }

        protected override void ApplyEffect(EffectData _effect)
        {
            if (_effect.Type == EffectType.Heal)
            {
                GameEvents.RaiseEffectApplied(m_Position);
            }
        }
    }
} 
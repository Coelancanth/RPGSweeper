using UnityEngine;

namespace RPGMinesweeper.Effects
{
    public class HealEffect : IEffect
    {
        private readonly float m_Duration;
        private readonly float m_Magnitude;

        public EffectType Type => EffectType.Heal;
        public EffectTargetType TargetType => EffectTargetType.Player;
        public float Duration => m_Duration;

        public HealEffect(float duration, float magnitude)
        {
            m_Duration = duration;
            m_Magnitude = magnitude;
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var player = GameObject.FindFirstObjectByType<PlayerComponent>();
            if (player != null)
            {
                player.TakeDamage(-Mathf.RoundToInt(m_Magnitude));
            }
        }

        public void Remove(GameObject source, Vector2Int sourcePosition)
        {
            // Healing is instant, no need for removal logic
        }
    }
} 
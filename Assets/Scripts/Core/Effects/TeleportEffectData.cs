using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper;  // For GridPositionType
using Sirenix.OdinInspector;  // For better inspector attributes

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "TeleportEffectData", menuName = "RPGMinesweeper/Effects/TeleportEffect")]
    public class TeleportEffectData : EffectData
    {
        [Header("Teleport Properties")]
        [Tooltip("Type of position to teleport to")]
        [SerializeField]
        private GridPositionType m_TargetPositionType = GridPositionType.Random;

        [Tooltip("Interval between teleports in persistent mode (in seconds)")]
        [SerializeField, Min(0.1f)]
        private float m_TeleportInterval = 5f;

        [OverridableProperty("Target Position Type")]
        public GridPositionType TargetPositionType
        {
            get => m_TargetPositionType;
            set => m_TargetPositionType = value;
        }

        [OverridableProperty("Teleport Interval")]
        public float TeleportInterval
        {
            get => m_TeleportInterval;
            set => m_TeleportInterval = Mathf.Max(0.1f, value);
        }

        public override IEffect CreateEffect()
        {
            var effect = new TeleportEffect(Radius, Shape, m_TargetPositionType, m_TeleportInterval);
            return effect;
        }
    }
} 
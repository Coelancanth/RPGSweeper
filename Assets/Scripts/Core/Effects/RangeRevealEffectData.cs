using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper;  // For GridPositionType

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "RangeRevealEffectData", menuName = "RPGMinesweeper/Effects/RangeRevealEffect")]
    public class RangeRevealEffectData : EffectData
    {
        [Header("Trigger Position")]
        [Tooltip("Type of position to trigger the effect from")]
        [SerializeField]
        private GridPositionType m_TriggerPositionType = GridPositionType.Source;

        [Tooltip("Custom trigger position (optional, overrides position type)")]
        [SerializeField]
        private Vector2Int? m_TriggerPosition = null;

        public GridPositionType TriggerPositionType
        {
            get => m_TriggerPositionType;
            set => m_TriggerPositionType = value;
        }

        public Vector2Int? TriggerPosition
        {
            get => m_TriggerPosition;
            set => m_TriggerPosition = value;
        }

        public override IEffect CreateEffect()
        {
            return new RangeRevealEffect(Radius, Shape, m_TriggerPosition, m_TriggerPositionType);
        }
    }
} 
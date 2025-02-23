using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper;  // For GridPositionType
using Sirenix.OdinInspector;  // For better inspector attributes

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "SummonEffectData", menuName = "RPGMinesweeper/Effects/SummonEffect")]
    public class SummonEffectData : EffectData
    {
        [Header("Summon Properties")]
        [Tooltip("Type of mine to summon")]
        [SerializeField]
        private MineType m_MineType = MineType.Standard;

        [Tooltip("Type of monster to summon (if mine type is Monster)")]
        [SerializeField, ShowIf("m_MineType", MineType.Monster)]
        private MonsterType m_MonsterType = MonsterType.None;

        [Tooltip("Number of mines to summon")]
        [SerializeField]
        [PropertyRange(1, 10)]
        [OnValueChanged("ValidateCount")]
        private int m_Count = 1;

        [Header("Trigger Position")]
        [Tooltip("Type of position to trigger the effect from")]
        [SerializeField]
        private GridPositionType m_TriggerPositionType = GridPositionType.Source;

        [Tooltip("Custom trigger position (optional, overrides position type)")]
        [SerializeField]
        private Vector2Int? m_TriggerPosition = null;

        private void ValidateCount()
        {
            m_Count = Mathf.Max(1, m_Count);
        }

        public MineType MineType
        {
            get => m_MineType;
            set => m_MineType = value;
        }

        public MonsterType MonsterType
        {
            get => m_MonsterType;
            set => m_MonsterType = value;
        }

        public int Count
        {
            get => m_Count;
            set
            {
                m_Count = Mathf.Max(1, value);
                ValidateCount();
            }
        }

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
            return new SummonEffect(Radius, Shape, m_MineType, m_MonsterType, m_Count, m_TriggerPosition, m_TriggerPositionType);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateCount();
        }
#endif
    }
} 
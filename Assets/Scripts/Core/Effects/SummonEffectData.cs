using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

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
        [SerializeField]
        private MonsterType m_MonsterType = MonsterType.None;

        [Tooltip("Number of mines to summon")]
        [SerializeField]
        [Min(1)]
        private int m_Count = 1;

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
            set => m_Count = Mathf.Max(1, value);
        }

        public override IEffect CreateEffect()
        {
            return new SummonEffect(Radius, Shape, m_MineType, m_MonsterType, m_Count);
        }
    }
} 
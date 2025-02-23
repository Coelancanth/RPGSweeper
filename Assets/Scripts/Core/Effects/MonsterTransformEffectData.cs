using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "MonsterTransformEffectData", menuName = "RPGMinesweeper/Effects/MonsterTransformEffect")]
    public class MonsterTransformEffectData : EffectData
    {
        [Header("Monster Transform Properties")]
        [Tooltip("Types of monsters that can be transformed")]
        [SerializeField]
        private List<MonsterType> m_SourceMonsterTypes = new();

        [Tooltip("Type of monster to transform into")]
        [SerializeField]
        private MonsterType m_TargetMonsterType = MonsterType.None;

        [Tooltip("Maximum number of monsters to transform (0 for unlimited)")]
        [SerializeField, Min(0)]
        private int m_MaxTransformCount = 0;

        [OverridableProperty("Source Monster Types")]
        public List<MonsterType> SourceMonsterTypes
        {
            get => m_SourceMonsterTypes;
            set => m_SourceMonsterTypes = value;
        }

        [OverridableProperty("Target Monster Type")]
        public MonsterType TargetMonsterType
        {
            get => m_TargetMonsterType;
            set => m_TargetMonsterType = value;
        }

        [OverridableProperty("Max Transform Count")]
        public int MaxTransformCount
        {
            get => m_MaxTransformCount;
            set => m_MaxTransformCount = Mathf.Max(0, value);
        }

        public override IEffect CreateEffect()
        {
            return new MonsterTransformEffect(Radius, m_SourceMonsterTypes, m_TargetMonsterType, m_MaxTransformCount, Shape);
        }
    }
} 
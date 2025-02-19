using UnityEngine;
using RPGMinesweeper;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "MonsterTransformEffectData", menuName = "RPGMinesweeper/Effects/MonsterTransformEffect")]
    public class MonsterTransformEffectData : EffectData
    {
        [Header("Monster Transform Properties")]
        [Tooltip("Type of monster to transform from")]
        [SerializeField]
        private MonsterType m_SourceMonsterType = MonsterType.None;

        [Tooltip("Type of monster to transform into")]
        [SerializeField]
        private MonsterType m_TargetMonsterType = MonsterType.None;

        [OverridableProperty("Source Monster Type")]
        public MonsterType SourceMonsterType
        {
            get => m_SourceMonsterType;
            set => m_SourceMonsterType = value;
        }

        [OverridableProperty("Target Monster Type")]
        public MonsterType TargetMonsterType
        {
            get => m_TargetMonsterType;
            set => m_TargetMonsterType = value;
        }

        public override IEffect CreateEffect()
        {
            return new MonsterTransformEffect(Radius, m_SourceMonsterType, m_TargetMonsterType, Shape);
        }
    }
} 
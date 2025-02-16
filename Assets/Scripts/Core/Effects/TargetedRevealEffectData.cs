using UnityEngine;
using RPGMinesweeper;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "TargetedRevealEffectData", menuName = "RPGMinesweeper/Effects/TargetedRevealEffect")]
    public class TargetedRevealEffectData : EffectData
    {
        [Header("Targeted Reveal Properties")]
        [Tooltip("Type of monster to reveal")]
        [SerializeField]
        private MonsterType m_TargetMonsterType = MonsterType.None;

        [OverridableProperty("Target Monster Type")]
        public MonsterType TargetMonsterType
        {
            get => m_TargetMonsterType;
            set => m_TargetMonsterType = value;
        }

        public override IEffect CreateEffect()
        {
            return new TargetedRevealEffect(Radius, m_TargetMonsterType, Shape);
        }
    }
} 
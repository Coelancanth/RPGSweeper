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
        [SerializeField] private MonsterType m_TargetMonsterType = MonsterType.None;

        public void SetTargetMonsterType(MonsterType type)
        {
            m_TargetMonsterType = type;
        }

        public MonsterType GetTargetMonsterType()
        {
            return m_TargetMonsterType;
        }

        public override IEffect CreateEffect()
        {
            return new TargetedRevealEffect(Radius, m_TargetMonsterType, Shape);
        }
    }
} 
using UnityEngine;
using RPGMinesweeper.Effects;
using RPGMinesweeper;

[CreateAssetMenu(fileName = "TargetedRevealEffectData", menuName = "RPGMinesweeper/Effects/TargetedRevealEffect")]
public class TargetedRevealEffectData : EffectData
{
    [Header("Targeted Reveal Properties")]
    [Tooltip("Duration of the reveal effect")]
    [SerializeField] private float m_Duration = 5f;

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
        //Debug.Log($"Creating TargetedRevealEffect with monster type: {m_TargetMonsterType}, shape: {Shape}, radius: {Radius}");
        return new TargetedRevealEffect(Duration, Radius, m_TargetMonsterType, Shape);
    }
} 
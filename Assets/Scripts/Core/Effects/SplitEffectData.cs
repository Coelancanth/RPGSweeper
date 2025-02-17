using UnityEngine;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;

/*!SECTION
- 分体HP = H x k / n
- H = 原怪物HP
- n = 分裂数量
- k = HP（总）HP调整系数
*/
[CreateAssetMenu(fileName = "SplitEffectData", menuName = "RPGMinesweeper/Effects/Split Effect")]
public class SplitEffectData : EffectData
{
    [SerializeField, Range(1, 10)] 
    private int m_SplitRadius = 2;
    [SerializeField, Range(0.1f, 2f)]
    private float m_HealthModifier = 1;
    
    [SerializeField, Range(2, 4)] 
    private int m_SplitCount = 2;

    [Header("Split Trigger Settings")]
    [SerializeField, Range(0f, 1f), Tooltip("HP ratio threshold that triggers split (for persistent mode)")]
    private float m_DamageThreshold = 0.5f;

    public float SplitRadius => m_SplitRadius;
    public float HealthModifier => m_HealthModifier;
    public int SplitCount => m_SplitCount;
    public float DamageThreshold => m_DamageThreshold;

    public override IEffect CreateEffect()
    {
        var effect = new SplitEffect(m_SplitRadius, Shape, m_HealthModifier, m_SplitCount, m_DamageThreshold);
        return effect;
    }
} 
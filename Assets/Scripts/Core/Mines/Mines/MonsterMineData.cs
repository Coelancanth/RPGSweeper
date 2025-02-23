using UnityEngine;
using RPGMinesweeper;  // For MonsterType
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MonsterMineData", menuName = "RPGMinesweeper/MonsterMineData")]
public class MonsterMineData : MineData
{
    [TitleGroup("Monster Properties")]
    [HorizontalGroup("Monster Properties/Split")]
    [VerticalGroup("Monster Properties/Split/Left"), LabelWidth(100)]
    [Tooltip("Type of monster this mine represents")]
    [SerializeField] private MonsterType m_MonsterType = MonsterType.None;
    
    [BoxGroup("Stats")]
    [HorizontalGroup("Stats/Split")]
    [VerticalGroup("Stats/Split/Left"), LabelWidth(100)]
    [Tooltip("Maximum HP of the monster - determines how many hits it can take before being defeated")]
    [MinValue(1)]
    [SerializeField] private int m_MaxHp = 100;
    
    [VerticalGroup("Stats/Split/Left")]
    [Tooltip("Base damage dealt TO the player when they trigger this monster - this is the damage player receives")]
    [MinValue(0)]
    [SerializeField] private int m_BaseDamage = 20;
    
    [VerticalGroup("Stats/Split/Left")]
    [Tooltip("Damage taken BY the monster when hit by the player - this reduces monster's HP")]
    [MinValue(1)]
    [SerializeField] private int m_DamagePerHit = 25;
    
    [FoldoutGroup("Enrage Properties")]
    [Tooltip("When enabled, monster deals increased damage at low HP (below 30%)")]
    [SerializeField, OnValueChanged("OnEnrageStateChanged")] 
    private bool m_HasEnrageState = true;
    
    [FoldoutGroup("Enrage Properties")]
    [Tooltip("Multiplier applied to base damage when monster is enraged (e.g., 1.5 = 50% more damage)")]
    [ShowIf("m_HasEnrageState")]
    [SerializeField, Range(1f, 3f)] 
    private float m_EnrageDamageMultiplier = 1.5f;

    public MonsterType MonsterType => m_MonsterType;
    public int MaxHp => m_MaxHp;
    public int BaseDamage => m_BaseDamage;
    public int DamagePerHit => m_DamagePerHit;
    public bool HasEnrageState => m_HasEnrageState;
    public float EnrageDamageMultiplier => m_EnrageDamageMultiplier;

    private void OnEnrageStateChanged()
    {
        if (!m_HasEnrageState)
        {
            m_EnrageDamageMultiplier = 1f;
        }
    }

    public int GetDamage(float hpPercentage)
    {
        if (m_HasEnrageState && hpPercentage <= 0.3f)
        {
            return Mathf.RoundToInt(m_BaseDamage * m_EnrageDamageMultiplier);
        }
        return m_BaseDamage;
    }

    [Button("Get Monster Info")]
    public string GetMonsterType()
    {
        return m_MonsterType.ToString();
    }
} 
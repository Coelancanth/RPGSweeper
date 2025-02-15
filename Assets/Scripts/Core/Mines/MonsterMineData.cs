using UnityEngine;
using RPGMinesweeper;  // For MonsterType

[CreateAssetMenu(fileName = "MonsterMineData", menuName = "RPGMinesweeper/MonsterMineData")]
public class MonsterMineData : MineData
{
    [Header("Monster Properties")]
    [Tooltip("Type of monster this mine represents")]
    [SerializeField] private MonsterType m_MonsterType = MonsterType.None;
    
    [Tooltip("Maximum HP of the monster - determines how many hits it can take before being defeated")]
    [SerializeField] private int m_MaxHp = 100;
    
    [Tooltip("Base damage dealt TO the player when they trigger this monster - this is the damage player receives")]
    [SerializeField] private int m_BaseDamage = 20;
    
    [Tooltip("Damage taken BY the monster when hit by the player - this reduces monster's HP")]
    [SerializeField] private int m_DamagePerHit = 25;
    
    [Tooltip("Monster's sprite that appears when the cell is revealed")]
    [SerializeField] private Sprite m_MonsterSprite;
    
    [Tooltip("Color tint applied to the monster's sprite")]
    [SerializeField] private Color m_MonsterTint = Color.white;
    
    [Tooltip("When enabled, monster deals increased damage at low HP (below 30%)")]
    [SerializeField] private bool m_HasEnrageState = true;
    
    [Tooltip("Multiplier applied to base damage when monster is enraged (e.g., 1.5 = 50% more damage)")]
    [SerializeField, Range(1f, 3f)] private float m_EnrageDamageMultiplier = 1.5f;

    public MonsterType MonsterType => m_MonsterType;
    public int MaxHp => m_MaxHp;
    public int BaseDamage => m_BaseDamage;
    public int DamagePerHit => m_DamagePerHit;
    public Sprite MonsterSprite => m_MonsterSprite;
    public Color MonsterTint => m_MonsterTint;
    public bool HasEnrageState => m_HasEnrageState;
    public float EnrageDamageMultiplier => m_EnrageDamageMultiplier;

    public int GetDamage(float hpPercentage)
    {
        if (m_HasEnrageState && hpPercentage <= 0.3f)
        {
            return Mathf.RoundToInt(m_BaseDamage * m_EnrageDamageMultiplier);
        }
        return m_BaseDamage;
    }

    public string GetMonsterType()
    {
        return m_MonsterType.ToString();
    }
} 
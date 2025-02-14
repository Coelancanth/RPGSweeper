using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;

public class MonsterMine : IDamagingMine
{
    #region Private Fields
    private readonly MonsterMineData m_Data;
    private readonly Vector2Int m_Position;
    private readonly List<Vector2Int> m_AffectedPositions;
    private readonly List<IPassiveEffect> m_ActivePassiveEffects = new();
    private int m_CurrentHp;
    private float m_ElapsedTime;
    private bool m_IsEnraged;
    private GameObject m_GameObject;
    
    // Runtime modifiable values
    private int m_MaxHp;
    private int m_BaseDamage;
    private int m_DamagePerHit;
    private float m_EnrageDamageMultiplier;
    private bool m_HasEnrageState;
    private Color m_MonsterTint;
    #endregion

    #region Public Properties
    public MineType Type => m_Data.Type;
    public bool CanDisguise => true;
    public int CurrentHp => m_CurrentHp;
    public float HpPercentage => (float)m_CurrentHp / m_MaxHp;
    public bool IsEnraged => m_IsEnraged;
    public Sprite MonsterSprite => m_Data.MonsterSprite;
    public Color MonsterTint => m_MonsterTint;
    public bool IsCollectable => m_CurrentHp <= 0;
    
    // Modifiable properties
    public int MaxHp
    {
        get => m_MaxHp;
        set
        {
            m_MaxHp = value;
            // Adjust current HP proportionally
            m_CurrentHp = Mathf.RoundToInt((float)m_CurrentHp / m_MaxHp * value);
            OnHpChanged?.Invoke(m_Position, HpPercentage);
        }
    }
    
    public int BaseDamage
    {
        get => m_BaseDamage;
        set => m_BaseDamage = value;
    }
    
    public int DamagePerHit
    {
        get => m_DamagePerHit;
        set => m_DamagePerHit = value;
    }
    
    public float EnrageDamageMultiplier
    {
        get => m_EnrageDamageMultiplier;
        set => m_EnrageDamageMultiplier = Mathf.Clamp(value, 1f, 3f);
    }
    
    public bool HasEnrageState
    {
        get => m_HasEnrageState;
        set => m_HasEnrageState = value;
    }
    #endregion

    #region Events
    // Event for when the monster becomes enraged
    public System.Action<Vector2Int> OnEnraged;
    // Event for when the monster's HP changes
    public System.Action<Vector2Int, float> OnHpChanged;
    #endregion

    #region Constructor
    public MonsterMine(MonsterMineData _data, Vector2Int _position)
    {
        m_Data = _data;
        m_Position = _position;
        
        // Initialize runtime values from data
        m_MaxHp = _data.MaxHp;
        m_CurrentHp = m_MaxHp;
        m_BaseDamage = _data.BaseDamage;
        m_DamagePerHit = _data.DamagePerHit;
        m_EnrageDamageMultiplier = _data.EnrageDamageMultiplier;
        m_HasEnrageState = _data.HasEnrageState;
        m_MonsterTint = _data.MonsterTint;
        
        m_AffectedPositions = GridShapeHelper.GetAffectedPositions(m_Position, m_Data.Shape, m_Data.Radius);
        InitializePassiveEffects();
    }
    #endregion

    #region IDamagingMine Implementation
    public int CalculateDamage()
    {
        // Check if should enter enrage state
        if (!m_IsEnraged && m_HasEnrageState && HpPercentage <= 0.3f)
        {
            m_IsEnraged = true;
            OnEnraged?.Invoke(m_Position);
        }

        // Calculate damage based on current state
        return m_IsEnraged ? 
            Mathf.RoundToInt(m_BaseDamage * m_EnrageDamageMultiplier) : 
            m_BaseDamage;
    }
    #endregion

    #region IMine Implementation
    public void OnTrigger(PlayerComponent _player)
    {
        // Deal damage to player
        _player.TakeDamage(CalculateDamage());
        
        // Take damage
        int previousHp = m_CurrentHp;
        m_CurrentHp -= m_DamagePerHit;
        
        // Notify of HP change
        if (previousHp != m_CurrentHp)
        {
            OnHpChanged?.Invoke(m_Position, HpPercentage);
        }
        
        // Apply effects if monster is still alive
        if (m_CurrentHp > 0)
        {
            if (m_Data.PassiveEffects != null)
            {
                foreach (var effectData in m_Data.PassiveEffects)
                {
                    var effect = effectData.CreateEffect() as IPassiveEffect;
                    if (effect != null)
                    {
                        effect.Apply(_player.gameObject, m_Position);
                        m_ActivePassiveEffects.Add(effect);
                    }
                }
            }
        }
    }

    public void OnDestroy()
    {
        // Only allow destruction if monster is defeated
        if (m_CurrentHp > 0) return;

        if (m_Data.ActiveEffects != null)
        {
            var player = GameObject.FindFirstObjectByType<PlayerComponent>();
            if (player != null)
            {
                foreach (var effectData in m_Data.ActiveEffects)
                {
                    var effect = effectData.CreateEffect();
                    if (effect != null)
                    {
                        effect.Apply(player.gameObject, m_Position);
                    }
                }
            }
        }

        // Clean up passive effects
        foreach (var effect in m_ActivePassiveEffects)
        {
            effect.Remove(GameObject.FindFirstObjectByType<PlayerComponent>()?.gameObject, m_Position);
        }
        m_ActivePassiveEffects.Clear();
    }

    public void Update(float deltaTime)
    {
        m_ElapsedTime += deltaTime;
        
        foreach (var effect in m_ActivePassiveEffects)
        {
            var player = GameObject.FindFirstObjectByType<PlayerComponent>();
            if (player != null)
            {
                effect.OnTick(player.gameObject, m_Position);
            }
        }
    }
    #endregion

    #region Private Methods
    private void InitializePassiveEffects()
    {
        if (m_Data.PassiveEffects == null) return;
        
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player == null) return;

        foreach (var effectData in m_Data.PassiveEffects)
        {
            var effect = effectData.CreateEffect() as IPassiveEffect;
            if (effect != null)
            {
                effect.Apply(player.gameObject, m_Position);
                m_ActivePassiveEffects.Add(effect);
            }
        }
    }
    #endregion
} 
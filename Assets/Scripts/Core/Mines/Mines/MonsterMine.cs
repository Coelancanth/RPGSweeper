using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType
using Minesweeper.Core.DamageSystem;
using Minesweeper.Core.DamageSystem.Initializers;

public class MonsterMine : IDamagingMine
{
    #region Private Fields
    private readonly MonsterMineData m_Data;
    private readonly Vector2Int m_Position;
    private readonly List<Vector2Int> m_AffectedPositions;
    private readonly List<IPersistentEffect> m_ActivePersistentEffects = new();
    private float m_ElapsedTime;
    private bool m_IsDefeated;
    private bool m_IsCollectable = false;
    private GameObject m_GameObject;
    
    // Entity for attribute and damage management
    private MonsterEntity m_Entity;
    #endregion

    #region Public Properties
    public MineType Type => m_Data.Type;
    public bool CanDisguise => true;
    
    // HP management through entity
    public int CurrentHp 
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.CURRENT_HP)?.CurrentValue ?? 0);
        set 
        {
            float oldHp = m_Entity.GetAttribute(AttributeTypes.CURRENT_HP)?.CurrentValue ?? 0;
            m_Entity.GetAttribute(AttributeTypes.CURRENT_HP)?.SetBaseValue(value);
            
            OnHpChanged?.Invoke(m_Position, HpPercentage);
            
            // Check if monster was just defeated
            if (CurrentHp <= 0 && !m_IsDefeated)
            {
                m_IsDefeated = true;
                // Note: Not setting IsCollectable here - that happens on next interaction
                OnDefeated?.Invoke(m_Position);
            }
        }
    }
    
    public float HpPercentage => m_Entity.GetHpPercentage();
    public bool IsEnraged => m_Entity.IsEnraged();
    public bool IsDefeated => m_IsDefeated;
    public bool IsCollectable => m_IsCollectable;
    public MonsterType MonsterType => m_Data.MonsterType;
    
    // Properties still exposed for compatibility
    public int MaxHp
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.MAX_HP)?.CurrentValue ?? 0);
        set
        {
            m_Entity.GetAttribute(AttributeTypes.MAX_HP)?.SetBaseValue(value);
            // Update current HP to match max
            CurrentHp = value;
        }
    }
    
    public int BaseDamage
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0);
        set => m_Entity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.SetBaseValue(value);
    }
    
    public int DamagePerHit
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT)?.CurrentValue ?? 0);
        set => m_Entity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT)?.SetBaseValue(value);
    }
    
    public float EnrageDamageMultiplier
    {
        get => m_Entity.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER)?.CurrentValue ?? 1f;
        set => m_Entity.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER)?.SetBaseValue(Mathf.Clamp(value, 1f, 3f));
    }
    
    public bool HasEnrageState => m_Entity.HasAttribute(AttributeTypes.ENRAGE_MULTIPLIER);

    // Add method to get MineData
    public MineData GetMineData()
    {
        return m_Data;
    }
    
    // Get the entity for external systems
    public MonsterEntity GetEntity()
    {
        return m_Entity;
    }
    #endregion

    #region Events
    // Event for when the monster becomes enraged
    public System.Action<Vector2Int> OnEnraged;
    // Event for when the monster's HP changes
    public System.Action<Vector2Int, float> OnHpChanged;
    // Event for when the monster is defeated
    public System.Action<Vector2Int> OnDefeated;
    #endregion

    #region Constructor
    public MonsterMine(MonsterMineData _data, Vector2Int _position)
    {
        m_Data = _data;
        m_Position = _position;
        
        // Create and initialize the monster entity
        InitializeMonsterEntity();
        
        // Set initial state
        m_IsDefeated = false;
        
        m_AffectedPositions = GridShapeHelper.GetAffectedPositions(m_Position, m_Data.Shape, m_Data.Radius);
        InitializePersistentEffects();
    }
    
    private void InitializeMonsterEntity()
    {
        // Use the adapter to create an entity for this monster type
        m_Entity = MonsterTypeAdapter.CreateMonsterEntity(
            m_Data.MonsterType, 
            $"Monster_{m_Data.MonsterType}_{m_Position.x}_{m_Position.y}"
        );

        // Override the default values with the specific ones from MonsterMineData
        var maxHpAttr = m_Entity.GetAttribute(AttributeTypes.MAX_HP);
        var currentHpAttr = m_Entity.GetAttribute(AttributeTypes.CURRENT_HP);
        var baseDamageAttr = m_Entity.GetAttribute(AttributeTypes.BASE_DAMAGE);
        var damagePerHitAttr = m_Entity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT);
        
        if (maxHpAttr != null)
        {
            maxHpAttr.SetBaseValue(m_Data.MaxHp);
        }
        else
        {
            m_Entity.AddAttribute(AttributeTypes.MAX_HP, m_Data.MaxHp);
        }
        
        if (currentHpAttr != null)
        {
            currentHpAttr.SetBaseValue(m_Data.MaxHp);
        }
        else
        {
            m_Entity.AddAttribute(AttributeTypes.CURRENT_HP, m_Data.MaxHp);
        }
        
        if (baseDamageAttr != null)
        {
            baseDamageAttr.SetBaseValue(m_Data.BaseDamage);
        }
        else
        {
            m_Entity.AddAttribute(AttributeTypes.BASE_DAMAGE, m_Data.BaseDamage);
        }
        
        if (damagePerHitAttr != null)
        {
            damagePerHitAttr.SetBaseValue(m_Data.DamagePerHit);
        }
        else
        {
            m_Entity.AddAttribute(AttributeTypes.DAMAGE_PER_HIT, m_Data.DamagePerHit);
        }
        
        // Handle enrage multiplier
        if (m_Data.HasEnrageState)
        {
            var enrageAttr = m_Entity.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER);
            if (enrageAttr != null)
            {
                enrageAttr.SetBaseValue(m_Data.EnrageDamageMultiplier);
            }
            else
            {
                m_Entity.AddAttribute(AttributeTypes.ENRAGE_MULTIPLIER, m_Data.EnrageDamageMultiplier);
            }
        }
        else
        {
            // Remove enrage attribute if it exists but the monster doesn't have enrage state
            var enrageAttr = m_Entity.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER);
            if (enrageAttr != null)
            {
                m_Entity.RemoveAttribute(AttributeTypes.ENRAGE_MULTIPLIER);
            }
        }
        
        // Set up health constraints
        var currentHp = m_Entity.GetAttribute(AttributeTypes.CURRENT_HP);
        var maxHp = m_Entity.GetAttribute(AttributeTypes.MAX_HP);
        
        if (currentHp != null && maxHp != null)
        {
            currentHp.MinValue = 0;
            currentHp.MaxValue = maxHp.CurrentValue;
            
            // Set up constraint for current HP to never exceed max HP
            maxHp.OnValueChanged += (attr, oldVal, newVal) => {
                currentHp.MaxValue = newVal;
            };
        }
        
        // Subscribe to entity events
        m_Entity.OnAttributeValueChanged += HandleAttributeValueChanged;
    }
    
    private void HandleAttributeValueChanged(Entity entity, Attribute attribute, float oldValue, float newValue)
    {
        // Handle HP changes
        if (attribute.Type == AttributeTypes.CURRENT_HP)
        {
            OnHpChanged?.Invoke(m_Position, HpPercentage);
            
            // Check for enrage state change
            bool wasEnraged = m_Entity.IsEnraged();
            bool enrageChanged = m_Entity.UpdateEnrageState();
            
            // Fire enrage event if state changed to enraged
            if (enrageChanged && m_Entity.IsEnraged() && !wasEnraged)
            {
                OnEnraged?.Invoke(m_Position);
            }
            
            // Check for defeated state
            if (newValue <= 0 && !m_IsDefeated)
            {
                m_IsDefeated = true;
                OnDefeated?.Invoke(m_Position);
            }
        }
    }
    #endregion

    #region IDamagingMine Implementation
    public int CalculateDamage()
    {
        // Check if we should enter enrage state
        m_Entity.UpdateEnrageState();

        // Create damage info for a simulated target
        var damageInfo = new DamageInfo
        {
            Source = m_Entity,
            Type = DamageType.Physical,
            BaseDamage = m_Entity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0,
            IsEnraged = m_Entity.IsEnraged()
        };
        
        // Calculate damage through the damage system
        damageInfo = DamageSystem.CalculateDamage(damageInfo);
        
        // Return calculated damage
        return Mathf.RoundToInt(damageInfo.FinalDamage);
    }
    #endregion

    #region IMine Implementation
    public void OnTrigger(PlayerComponent _player)
    {
        // If already defeated but not yet collectable, mark as collectable
        if (m_IsDefeated && !m_IsCollectable)
        {
            m_IsCollectable = true;
            // Notify of collectable state change
            OnHpChanged?.Invoke(m_Position, 0); // Trigger update with 0 HP
            return;
        }
        
        // If already defeated and collectable, don't do anything here
        if (m_IsDefeated && m_IsCollectable)
        {
            return;
        }
        
        // Deal damage to player
        _player.TakeDamage(CalculateDamage());
        
        // Deal damage to monster
        int previousHp = CurrentHp;
        
        // Apply damage to monster
        var currentHp = m_Entity.GetAttribute(AttributeTypes.CURRENT_HP);
        if (currentHp != null)
        {
            float damageValue = m_Entity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT)?.CurrentValue ?? 0;
            float newHp = currentHp.CurrentValue - damageValue;
            currentHp.SetBaseValue(newHp);
        }
        
        // Notify split effects of HP change if HP changed
        if (previousHp != CurrentHp)
        {
            foreach (var effect in m_ActivePersistentEffects)
            {
                if (effect is SplitEffect splitEffect)
                {
                    splitEffect.OnMonsterDamaged(m_Position, HpPercentage);
                }
            }
        }
        
        // Apply effects if monster is still alive
        if (CurrentHp > 0)
        {
            foreach (var effect in m_Data.CreatePersistentEffects())
            {
                if (effect is IPersistentEffect persistentEffect)
                {
                    persistentEffect.Apply(_player.gameObject, m_Position);
                    m_ActivePersistentEffects.Add(persistentEffect);
                }
                else
                {
                    effect.Apply(_player.gameObject, m_Position);
                }
            }
        }
    }

    public void OnDestroy()
    {
        // Only allow destruction if monster is defeated
        if (CurrentHp > 0) return;

        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player != null)
        {
            foreach (var effect in m_Data.CreateTriggerableEffects())
            {
                effect.Apply(player.gameObject, m_Position);
            }
        }

        // Clean up persistent effects
        foreach (var effect in m_ActivePersistentEffects)
        {
            effect.Remove(player?.gameObject);
        }
        m_ActivePersistentEffects.Clear();
    }
    
    public void OnRemoveEffects()
    {
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player != null)
        {
            foreach (var effect in m_Data.CreateTriggerableEffects())
            {
                effect.Apply(player.gameObject, m_Position);
            }
        }

        // Clean up persistent effects
        foreach (var effect in m_ActivePersistentEffects)
        {
            effect.Remove(player?.gameObject);
        }
        m_ActivePersistentEffects.Clear();
    }

    public void Update(float deltaTime)
    {
        m_ElapsedTime += deltaTime;
        
        // Update persistent effects
        foreach (var effect in m_ActivePersistentEffects)
        {
            effect.Update(deltaTime);
            if (!effect.IsActive)
            {
                var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                if (player != null)
                {
                    effect.Remove(player.gameObject);
                }
            }
        }
        
        // Remove inactive effects
        m_ActivePersistentEffects.RemoveAll(effect => !effect.IsActive);
    }
    #endregion

    #region Private Methods
    private void InitializePersistentEffects()
    {
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player == null) return;

        foreach (var effect in m_Data.CreatePersistentEffects())
        {
            if (effect is IPersistentEffect persistentEffect)
            {
                persistentEffect.Apply(player.gameObject, m_Position);
                m_ActivePersistentEffects.Add(persistentEffect);
            }
            else
            {
                effect.Apply(player.gameObject, m_Position);
            }
        }
    }
    #endregion
} 
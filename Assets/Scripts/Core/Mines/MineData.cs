using UnityEngine;
using System;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType

[Serializable]
public class EffectTemplate
{
    [Header("Effect Reference")]
    [SerializeField] private EffectData m_Template;

    [Header("Custom Values")]
    [SerializeField] private float m_Duration;
    [SerializeField] private float m_Magnitude;
    [SerializeField] private GridShape m_Shape = GridShape.Single;
    [SerializeField] private int m_Radius = 1;
    [SerializeField] private LayerMask m_TargetLayers;
    [SerializeField] private MonsterType m_TargetMonsterType = MonsterType.None;

    // For passive effects
    [SerializeField] private float m_TickInterval = 1f;

    public EffectData CreateInstance()
    {
        if (m_Template == null) return null;

        var instance = ScriptableObject.CreateInstance(m_Template.GetType()) as EffectData;
        if (instance == null) return null;

        // Copy base values
        instance.Type = m_Template.Type;
        instance.Duration = m_Duration;
        instance.Magnitude = m_Magnitude;
        instance.Shape = m_Shape;
        instance.Radius = m_Radius;
        instance.TargetLayers = m_TargetLayers;

        // Handle specific effect type properties
        if (instance is PassiveEffectData passiveInstance)
        {
            passiveInstance.TickInterval = m_TickInterval;
        }
        else if (instance is TargetedRevealEffectData targetedRevealInstance)
        {
            targetedRevealInstance.SetTargetMonsterType(m_TargetMonsterType);
        }

        return instance;
    }

    #if UNITY_EDITOR
    public void OnValidate()
    {
        if (m_Template != null)
        {
            // Initialize values from template if not yet set
            if (m_Duration == 0) m_Duration = m_Template.Duration;
            if (m_Magnitude == 0) m_Magnitude = m_Template.Magnitude;
            if (m_Shape == GridShape.Single) m_Shape = m_Template.Shape;
            if (m_Radius == 1) m_Radius = m_Template.Radius;
            if (m_TargetLayers == 0) m_TargetLayers = m_Template.TargetLayers;

            if (m_Template is PassiveEffectData passiveTemplate)
            {
                if (m_TickInterval == 1f) m_TickInterval = passiveTemplate.TickInterval;
            }
            else if (m_Template is TargetedRevealEffectData targetedRevealTemplate)
            {
                if (m_TargetMonsterType == MonsterType.None) m_TargetMonsterType = targetedRevealTemplate.GetTargetMonsterType();
            }
        }
    }
    #endif
}

[CreateAssetMenu(fileName = "MineData", menuName = "RPGMinesweeper/MineData")]
public class MineData : ScriptableObject
{
    [Header("Basic Properties")]
    public MineType Type;
    [Tooltip("Display value shown on the grid")]
    public int Value;
    
    [Header("Area of Effect")]
    public GridShape Shape;
    public int Radius;

    [Header("Spawn Properties")]
    public MineSpawnStrategyType SpawnStrategy = MineSpawnStrategyType.Random;

    [Header("Visual Properties")]
    public Sprite MineSprite;
    [Tooltip("Color of the displayed mine value text")]
    [SerializeField] private Color m_ValueColor = Color.white;
    [Tooltip("Color of the displayed mine value text when this is a mine cell")]
    [SerializeField] private Color m_MineValueColor = Color.yellow;
    public Color ValueColor => m_ValueColor;
    public Color MineValueColor => m_MineValueColor;
    
    [Header("Effects")]
    [Tooltip("Effects that are always active while the mine exists")]
    [SerializeField] private EffectTemplate[] m_PassiveEffects;
    
    [Tooltip("Effects that trigger when the mine is removed")]
    [SerializeField] private EffectTemplate[] m_ActiveEffects;

    private EffectData[] m_PassiveEffectInstances;
    private EffectData[] m_ActiveEffectInstances;

    public EffectData[] PassiveEffects => m_PassiveEffectInstances;
    public EffectData[] ActiveEffects => m_ActiveEffectInstances;

    private void OnEnable()
    {
        CreateEffectInstances();
    }

    private void CreateEffectInstances()
    {
        // Create passive effect instances
        if (m_PassiveEffects != null)
        {
            m_PassiveEffectInstances = new EffectData[m_PassiveEffects.Length];
            for (int i = 0; i < m_PassiveEffects.Length; i++)
            {
                m_PassiveEffectInstances[i] = m_PassiveEffects[i]?.CreateInstance();
            }
        }

        // Create active effect instances
        if (m_ActiveEffects != null)
        {
            m_ActiveEffectInstances = new EffectData[m_ActiveEffects.Length];
            for (int i = 0; i < m_ActiveEffects.Length; i++)
            {
                m_ActiveEffectInstances[i] = m_ActiveEffects[i]?.CreateInstance();
            }
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        // Validate effect templates
        if (m_PassiveEffects != null)
        {
            foreach (var effect in m_PassiveEffects)
            {
                effect?.OnValidate();
            }
        }

        if (m_ActiveEffects != null)
        {
            foreach (var effect in m_ActiveEffects)
            {
                effect?.OnValidate();
            }
        }
    }
    #endif

    public List<Vector2Int> GetAffectedPositions(Vector2Int center)
    {
        return GridShapeHelper.GetAffectedPositions(center, Shape, Radius);
    }

    public bool IsPositionAffected(Vector2Int position, Vector2Int center)
    {
        return GridShapeHelper.IsPositionAffected(position, center, Shape, Radius);
    }

    public (int value, Color color) GetValueDisplay()
    {
        return (Value, m_ValueColor);
    }
} 
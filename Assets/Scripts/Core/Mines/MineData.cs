using UnityEngine;
using System;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType

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
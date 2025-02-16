using UnityEngine;
using System;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MineData", menuName = "RPGMinesweeper/MineData")]
public class MineData : SerializedScriptableObject
{
    [TitleGroup("Basic Properties")]
    [HorizontalGroup("Basic Properties/Split")]
    [VerticalGroup("Basic Properties/Split/Left"), LabelWidth(100)]
    public MineType Type;

    [VerticalGroup("Basic Properties/Split/Left")]
    [Tooltip("Display value shown on the grid")]
    public int Value;
    
    [BoxGroup("Area of Effect")]
    [HorizontalGroup("Area of Effect/Split")]
    [VerticalGroup("Area of Effect/Split/Left"), LabelWidth(100)]
    public GridShape Shape;

    [VerticalGroup("Area of Effect/Split/Left")]
    public int Radius;

    [FoldoutGroup("Spawn Settings")]
    public MineSpawnStrategyType SpawnStrategy = MineSpawnStrategyType.Random;

    [BoxGroup("Visual Properties")]
    [HorizontalGroup("Visual Properties/Split")]
    [VerticalGroup("Visual Properties/Split/Left"), LabelWidth(100)]
    [PreviewField(55)]
    public Sprite MineSprite;

    [FoldoutGroup("Visual Properties/Colors")]
    [Tooltip("Color of the displayed mine value text")]
    [ColorPalette]
    [SerializeField] private Color m_ValueColor = Color.white;

    [FoldoutGroup("Visual Properties/Colors")]
    [Tooltip("Color of the displayed mine value text when this is a mine cell")]
    [ColorPalette]
    [SerializeField] private Color m_MineValueColor = Color.yellow;

    public Color ValueColor => m_ValueColor;
    public Color MineValueColor => m_MineValueColor;
    
    [TabGroup("Effects", "Passive")]
    [Tooltip("Effects that are always active while the mine exists")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private EffectTemplate[] m_PassiveEffects;
    
    [TabGroup("Effects", "Active")]
    [Tooltip("Effects that trigger when the mine is removed")]
    [ListDrawerSettings(ShowIndexLabels = true)]
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
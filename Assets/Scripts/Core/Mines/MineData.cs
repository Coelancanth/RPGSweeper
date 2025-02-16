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
    
    [BoxGroup("Effects")]
    [TabGroup("Effects/Tabs", "Passive")]
    [Tooltip("Effects that are applied while the mine is active")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private EffectData[] m_PassiveEffects;

    [TabGroup("Effects/Tabs", "Active")]
    [Tooltip("Effects that are applied when the mine is destroyed")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private EffectData[] m_ActiveEffects;

    public EffectData[] PassiveEffects => m_PassiveEffects;
    public EffectData[] ActiveEffects => m_ActiveEffects;

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
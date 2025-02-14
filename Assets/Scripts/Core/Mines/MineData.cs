using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;

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
    public PassiveEffectData[] PassiveEffects;
    
    [Tooltip("Effects that trigger when the mine is removed")]
    public ActiveEffectData[] ActiveEffects;

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
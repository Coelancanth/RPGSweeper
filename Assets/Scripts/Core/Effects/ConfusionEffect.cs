using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;

public class ConfusionEffect : IPassiveEffect
{
    private float m_Duration;
    private int m_Radius;
    private GridShape m_Shape;
    private HashSet<Vector2Int> m_AffectedCells;

    public EffectType Type => EffectType.Confusion;
    public EffectTargetType TargetType => EffectTargetType.Grid;
    public float Duration => m_Duration;

    public ConfusionEffect(float duration, int radius, GridShape shape = GridShape.Square)
    {
        m_Duration = duration;
        m_Radius = radius;
        m_Shape = shape;
        m_AffectedCells = new HashSet<Vector2Int>();
    }

    public void Apply(GameObject source, Vector2Int sourcePosition)
    {
        var gridManager = GameObject.FindFirstObjectByType<GridManager>();
        if (gridManager == null) return;

        // Get affected positions based on shape
        var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);
        
        foreach (var pos in affectedPositions)
        {
            if (gridManager.IsValidPosition(pos))
            {
                m_AffectedCells.Add(pos);
                MineValueModifier.RegisterEffect(pos, this);
            }
        }

        // Trigger value recalculation
        var mineManager = GameObject.FindFirstObjectByType<MineManager>();
        if (mineManager != null)
        {
            MineValuePropagator.PropagateValues(mineManager, gridManager);
        }
    }

    public void Remove(GameObject source, Vector2Int sourcePosition)
    {
        var gridManager = GameObject.FindFirstObjectByType<GridManager>();
        var mineManager = GameObject.FindFirstObjectByType<MineManager>();
        if (gridManager == null || mineManager == null) return;

        // Unregister effect from all affected cells
        foreach (var pos in m_AffectedCells)
        {
            MineValueModifier.UnregisterEffect(pos, this);
        }
        m_AffectedCells.Clear();

        // Trigger value recalculation
        MineValuePropagator.PropagateValues(mineManager, gridManager);
    }

    public void OnTick(GameObject source, Vector2Int sourcePosition)
    {
        // Refresh the confusion effect periodically
        Apply(source, sourcePosition);
    }
} 
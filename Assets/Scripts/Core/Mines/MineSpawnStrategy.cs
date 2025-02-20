using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper
{
    public enum SpawnStrategyPriority
    {
        Lowest = 0,
        Random = 100,
        Center = 200,
        Edge = 300,
        Corner = 400,
        Surrounded = 500,
        Symmetric = 600,
        Highest = 1000
    }

    public interface IMineSpawnStrategy
    {
        Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines);
        SpawnStrategyPriority Priority { get; }
    }

    // Deprecated: Use GridPositionType instead
    [System.Flags]
    public enum MineSpawnStrategyType
    {
        None = 0,
        Random = 1 << 0,
        Edge = 1 << 1,
        Corner = 1 << 2,
        Center = 1 << 3,
        Surrounded = 1 << 4,
        SymmetricHorizontal = 1 << 5,
        SymmetricVertical = 1 << 6,
        All = Random | Edge | Corner | Center | Surrounded | SymmetricHorizontal | SymmetricVertical
    }

    public enum SymmetryDirection
    {
        Horizontal,
        Vertical
    }
} 
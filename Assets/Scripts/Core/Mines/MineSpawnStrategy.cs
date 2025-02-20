using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper
{
    public interface IMineSpawnStrategy
    {
        Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines);
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
        All = Random | Edge | Corner | Center | Surrounded
    }
} 
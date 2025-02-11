using UnityEngine;
using System.Collections.Generic;

public interface IMineSpawnStrategy
{
    Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines);
}

public enum MineSpawnStrategyType
{
    Random,
    Edge
} 
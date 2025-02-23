using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Factory;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public interface IMineSpawner
    {
        //void ValidateSpawnCounts(List<MineTypeSpawnData> spawnData, int gridWidth, int gridHeight);
        void PlaceMines(List<MineTypeSpawnData> spawnData, IMineFactory mineFactory, GridManager gridManager, 
            Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap);
    }
} 
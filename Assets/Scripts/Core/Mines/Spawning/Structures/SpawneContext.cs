using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper.Factory;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class SpawnContext
    {
        public GridManager GridManager { get; }
        public IMineFactory MineFactory { get; }
        public Dictionary<Vector2Int, IMine> ExistingMines { get; }
        public Dictionary<Vector2Int, MineData> MineDataMap { get; }
        public HashSet<Vector2Int> BlockedPositions { get; }
        
        public int GridWidth => GridManager.Width;
        public int GridHeight => GridManager.Height;
        public int RemainingCount { get; private set; }

        public SpawnContext(
            GridManager gridManager,
            IMineFactory mineFactory,
            Dictionary<Vector2Int, IMine> existingMines,
            Dictionary<Vector2Int, MineData> mineDataMap,
            int initialCount)
        {
            GridManager = gridManager;
            MineFactory = mineFactory;
            ExistingMines = existingMines;
            MineDataMap = mineDataMap;
            RemainingCount = initialCount;
            BlockedPositions = new HashSet<Vector2Int>();
        }

        public bool IsValidPosition(Vector2Int position) =>
            position.x >= 0 && position.x < GridWidth &&
            position.y >= 0 && position.y < GridHeight;

        public bool IsPositionAvailable(Vector2Int position) =>
            IsValidPosition(position) &&
            !ExistingMines.ContainsKey(position) &&
            !BlockedPositions.Contains(position);

        public IEnumerable<Vector2Int> GetAvailablePositions() =>
            Enumerable.Range(0, GridWidth)
                .SelectMany(x => Enumerable.Range(0, GridHeight)
                    .Select(y => new Vector2Int(x, y)))
                .Where(IsPositionAvailable);

        public void AddMine(SpawnedMine mine)
        {
            if (!IsValidPosition(mine.Position)) return;
            
            ExistingMines[mine.Position] = mine.Mine;
            MineDataMap[mine.Position] = mine.MineData;
            BlockedPositions.Add(mine.Position);
            RemainingCount--;
        }

        public void AddMines(IEnumerable<SpawnedMine> mines)
        {
            foreach (var mine in mines)
            {
                AddMine(mine);
            }
        }
    }
}

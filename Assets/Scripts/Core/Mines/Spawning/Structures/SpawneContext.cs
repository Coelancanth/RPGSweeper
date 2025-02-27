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
        
        // Maps each position to the priority of the strategy that placed a mine there
        public Dictionary<Vector2Int, int> PositionPriorities { get; private set; }
        
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
            PositionPriorities = new Dictionary<Vector2Int, int>();
        }

        public bool IsValidPosition(Vector2Int position) =>
            position.x >= 0 && position.x < GridWidth &&
            position.y >= 0 && position.y < GridHeight;

        public bool IsPositionAvailable(Vector2Int position) =>
            IsValidPosition(position) &&
            !ExistingMines.ContainsKey(position) &&
            !BlockedPositions.Contains(position);

        // New method to check if a position is available for a specific strategy priority
        public bool IsPositionAvailableForStrategy(Vector2Int position, SpawnStrategyType priority)
        {
            // If position isn't available at all, return false
            if (!IsPositionAvailable(position))
                return false;
                
            // Ensure PositionPriorities is initialized
            if (PositionPriorities == null)
            {
                PositionPriorities = new Dictionary<Vector2Int, int>();
                return true;
            }
                
            // If position isn't claimed by any strategy yet, it's available
            if (!PositionPriorities.TryGetValue(position, out int existingPriority))
                return true;
                
            // Position is only available if this strategy has higher priority 
            // Higher numerical value = higher priority
            return (int)priority > existingPriority;
        }
        
        // Method that returns available positions for a specific strategy priority
        public IEnumerable<Vector2Int> GetAvailablePositionsForStrategy(SpawnStrategyType priority)
        {
            // Ensure PositionPriorities is initialized
            if (PositionPriorities == null)
            {
                PositionPriorities = new Dictionary<Vector2Int, int>();
            }
            
            return GetAvailablePositions().Where(pos => 
                PositionPriorities == null || 
                !PositionPriorities.TryGetValue(pos, out int existingPriority) || 
                (int)priority > existingPriority);
        }

        public IEnumerable<Vector2Int> GetAvailablePositions() =>
            Enumerable.Range(0, GridWidth)
                .SelectMany(x => Enumerable.Range(0, GridHeight)
                    .Select(y => new Vector2Int(x, y)))
                .Where(IsPositionAvailable);

        public void AddMine(SpawnedMine mine, SpawnStrategyType strategyPriority)
        {
            if (!IsValidPosition(mine.Position)) return;
            
            ExistingMines[mine.Position] = mine.Mine;
            MineDataMap[mine.Position] = mine.MineData;
            BlockedPositions.Add(mine.Position);
            
            // Ensure PositionPriorities is initialized
            if (PositionPriorities == null)
            {
                PositionPriorities = new Dictionary<Vector2Int, int>();
            }
            
            PositionPriorities[mine.Position] = (int)strategyPriority;
            RemainingCount--;
        }

        public void AddMines(IEnumerable<SpawnedMine> mines, SpawnStrategyType strategyPriority)
        {
            foreach (var mine in mines)
            {
                AddMine(mine, strategyPriority);
            }
        }
        
        // Keep existing method overloads for backward compatibility
        public void AddMine(SpawnedMine mine)
        {
            AddMine(mine, SpawnStrategyType.Lowest);
        }
        
        public void AddMines(IEnumerable<SpawnedMine> mines)
        {
            AddMines(mines, SpawnStrategyType.Lowest);
        }
    }
}

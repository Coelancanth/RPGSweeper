using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public enum SpawnStrategyType
    {
        Lowest = 0,
        Random = 100,
        Center = 200,
        Edge = 300,
        Corner = 400,
        Surrounded = 500,
        Symmetric = 600,
        Relational = 700,
        Adjacent = 800,
        Highest = 1000
    }

    public interface IMineSpawnStrategy
    {
        SpawnStrategyType Priority { get; }
        
        // New methods
        bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData);
        SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData);
        
        // Legacy method - will be removed after migration
        List<SpawnedMine> GetSpawnedMines(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines, int count);
    }

    public abstract class MineSpawnStrategyBase : IMineSpawnStrategy
    {
        public abstract SpawnStrategyType Priority { get; }

        public virtual bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            return context.RemainingCount > 0 && 
                   spawnData.IsEnabled && 
                   spawnData.SpawnCount > 0 &&
                   context.GetAvailablePositions().Any();
        }

        public abstract SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData);

        // Legacy implementation that delegates to new methods
        public List<SpawnedMine> GetSpawnedMines(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines, int count)
        {
            var context = new SpawnContext(
                gridManager,
                null, // MineFactory will be null for legacy calls
                existingMines,
                null,

                count);

            foreach (var kvp in existingMines)
            {
                context.ExistingMines[kvp.Key] = kvp.Value;
            }

            var spawnData = new MineTypeSpawnData
            {
                SpawnCount = count,
                IsEnabled = true
            };

            var result = Execute(context, spawnData);
            return result.Success ? result.Mines.ToList() : new List<SpawnedMine>();
        }

        protected SpawnedMine CreateMine(SpawnContext context, Vector2Int position, MineTypeSpawnData spawnData, FacingDirection facing = FacingDirection.Up)
        {
            var mine = context.MineFactory?.CreateMine(spawnData.MineData, position);
            return SpawnedMine.Create(position, mine, spawnData.MineData, facing);
        }

        protected bool ValidateSpawnData(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!spawnData.IsEnabled)
            {
                return false;
            }

            if (spawnData.SpawnCount <= 0)
            {
                return false;
            }

            if (context.RemainingCount <= 0)
            {
                return false;
            }

            return true;
        }
    }
} 
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class RandomSpawnStrategy : MineSpawnStrategyBase
    {
        public override SpawnStrategyType Priority => SpawnStrategyType.Random;

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var availablePositions = context.GetAvailablePositions().ToList();
            if (availablePositions.Count == 0)
            {
                return SpawnResult.Failed("No available positions");
            }

            var spawnCount = Mathf.Min(spawnData.SpawnCount, availablePositions.Count);
            var selectedPositions = availablePositions
                .OrderBy(_ => Random.value)
                .Take(spawnCount);

            var mines = selectedPositions
                .Select(pos => CreateMine(context, pos, spawnData))
                .ToList();

            return SpawnResult.Successful(mines);
        }
    }
} 
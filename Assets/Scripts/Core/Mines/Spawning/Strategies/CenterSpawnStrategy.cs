using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class CenterSpawnStrategy : MineSpawnStrategyBase
    {
        public override SpawnStrategyType Priority => SpawnStrategyType.Center;

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            return GetAvailablePositions(context)
                .Any(p => IsCenterPosition(p, context));
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var availableCenterPositions = GetAvailablePositions(context)
                .Where(p => IsCenterPosition(p, context))
                .ToList();

            if (availableCenterPositions.Count == 0)
            {
                return SpawnResult.Failed("No available center positions");
            }

            var spawnCount = Mathf.Min(spawnData.SpawnCount, availableCenterPositions.Count);
            var selectedPositions = availableCenterPositions
                .OrderBy(_ => Random.value)
                .Take(spawnCount);

            var mines = selectedPositions
                .Select(pos => CreateMine(context, pos, spawnData))
                .ToList();

            return SpawnResult.Successful(mines);
        }

        private bool IsCenterPosition(Vector2Int pos, SpawnContext context)
        {
            //int minX = context.GridWidth / 3;
            //int maxX = (context.GridWidth * 2) / 3;
            //int minY = context.GridHeight / 3;
            //int maxY = (context.GridHeight * 2) / 3;

            //return pos.x >= minX && pos.x <= maxX && 
                   //pos.y >= minY && pos.y <= maxY;
            return pos.x == context.GridWidth / 2 && pos.y == context.GridHeight / 2;
        }
    }
}
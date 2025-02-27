using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class CornerSpawnStrategy : MineSpawnStrategyBase
    {
        public override SpawnStrategyType Priority => SpawnStrategyType.Corner;

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            return GetAvailablePositions(context)
                .Any(p => IsCornerPosition(p, context));
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var availableCorners = GetAvailablePositions(context)
                .Where(p => IsCornerPosition(p, context))
                .ToList();

            if (availableCorners.Count == 0)
            {
                return SpawnResult.Failed("No available corner positions");
            }

            var spawnCount = Mathf.Min(spawnData.SpawnCount, availableCorners.Count);
            var selectedPositions = availableCorners
                .OrderBy(_ => Random.value)
                .Take(spawnCount);

            var mines = selectedPositions
                .Select(pos => 
                {
                    var facing = DetermineFacingDirection(pos, context);
                    return CreateMine(context, pos, spawnData, facing);
                })
                .ToList();

            return SpawnResult.Successful(mines);
        }

        private bool IsCornerPosition(Vector2Int pos, SpawnContext context)
        {
            return (pos.x == 0 || pos.x == context.GridWidth - 1) &&
                   (pos.y == 0 || pos.y == context.GridHeight - 1);
        }

        private FacingDirection DetermineFacingDirection(Vector2Int pos, SpawnContext context)
        {
            //// Determine facing direction based on which corner the mine is in
            //if (pos.x == 0)
            //{
                //return pos.y == 0 ? FacingDirection.Right : FacingDirection.Up;
            //}
            //else // pos.x == context.GridWidth - 1
            //{
                //return pos.y == 0 ? FacingDirection.Down : FacingDirection.Left;
            //}
            return FacingDirection.None;
        }
    }
}
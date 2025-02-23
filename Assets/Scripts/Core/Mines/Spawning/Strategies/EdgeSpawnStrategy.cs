using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class EdgeSpawnStrategy : MineSpawnStrategyBase
    {
        public override SpawnStrategyType Priority => SpawnStrategyType.Edge;

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            return context.GetAvailablePositions()
                .Any(p => IsEdgePosition(p, context));
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var availableEdges = context.GetAvailablePositions()
                .Where(p => IsEdgePosition(p, context))
                .ToList();

            if (availableEdges.Count == 0)
            {
                return SpawnResult.Failed("No available edge positions");
            }

            var spawnCount = Mathf.Min(spawnData.SpawnCount, availableEdges.Count);
            var selectedPositions = availableEdges
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

        private bool IsEdgePosition(Vector2Int pos, SpawnContext context) =>
            pos.x == 0 || pos.x == context.GridWidth - 1 ||
            pos.y == 0 || pos.y == context.GridHeight - 1;

        private FacingDirection DetermineFacingDirection(Vector2Int pos, SpawnContext context)
        {
            //if (pos.x == 0) return FacingDirection.Right;
            //if (pos.x == context.GridWidth - 1) return FacingDirection.Left;
            //if (pos.y == 0) return FacingDirection.Up;
            //return FacingDirection.Down;
            return FacingDirection.None;
        }
    }
}
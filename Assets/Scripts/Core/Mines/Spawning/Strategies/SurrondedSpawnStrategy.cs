using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class SurroundedSpawnStrategy : MineSpawnStrategyBase
    {
        private readonly MineType m_TargetMineType;
        private readonly MonsterType? m_TargetMonsterType;
        private static readonly Vector2Int[] s_AdjacentOffsets = new Vector2Int[]
        {
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
            new Vector2Int(-1, 0),                         new Vector2Int(1, 0),
            new Vector2Int(-1, 1),  new Vector2Int(0, 1),  new Vector2Int(1, 1)
        };

        public override SpawnStrategyType Priority => SpawnStrategyType.Surrounded;

        public SurroundedSpawnStrategy(MineType targetMineType, MonsterType? targetMonsterType = null)
        {
            m_TargetMineType = targetMineType;
            m_TargetMonsterType = targetMonsterType;
        }

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            var targetPositions = context.ExistingMines
                .Where(kvp => IsTargetMine(kvp.Value))
                .Select(kvp => kvp.Key);

            return targetPositions.Any() && 
                   targetPositions.SelectMany(pos => GetValidAdjacentPositions(pos, context))
                   .Any();
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var targetPositions = context.ExistingMines
                .Where(kvp => IsTargetMine(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            if (targetPositions.Count == 0)
            {
                return SpawnResult.Failed($"No target mines found of type {m_TargetMineType}" + 
                    (m_TargetMonsterType.HasValue ? $" and monster type {m_TargetMonsterType.Value}" : ""));
            }

            var validPositions = targetPositions
                .SelectMany(pos => GetValidAdjacentPositions(pos, context))
                .Distinct()
                .ToList();

            if (validPositions.Count == 0)
            {
                return SpawnResult.Failed("No valid adjacent positions available");
            }

            var spawnCount = Mathf.Min(spawnData.SpawnCount, validPositions.Count);
            var selectedPositions = validPositions
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

        private IEnumerable<Vector2Int> GetValidAdjacentPositions(Vector2Int center, SpawnContext context)
        {
            return s_AdjacentOffsets
                .Select(offset => center + offset)
                .Where(pos => IsValidPosition(pos, context));
        }

        private bool IsValidPosition(Vector2Int pos, SpawnContext context)
        {
            return pos.x >= 0 && pos.x < context.GridWidth &&
                   pos.y >= 0 && pos.y < context.GridHeight &&
                   !context.ExistingMines.ContainsKey(pos);
        }

        private bool IsTargetMine(IMine mine)
        {
            if (mine == null) return false;

            bool isCorrectType = mine.Type == m_TargetMineType;
            if (!isCorrectType) return false;

            if (m_TargetMonsterType.HasValue && mine is MonsterMine monsterMine)
            {
                return monsterMine.MonsterType == m_TargetMonsterType.Value;
            }

            return true;
        }

        private FacingDirection DetermineFacingDirection(Vector2Int pos, SpawnContext context)
        {
            // Find the closest target mine
            var closestTarget = context.ExistingMines
                .Where(kvp => IsTargetMine(kvp.Value))
                .OrderBy(kvp => Vector2Int.Distance(kvp.Key, pos))
                .FirstOrDefault();

            if (closestTarget.Value == null)
            {
                return FacingDirection.Up;
            }

            var diff = closestTarget.Key - pos;
            
            // Determine facing direction based on relative position to target
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                return diff.x > 0 ? FacingDirection.Right : FacingDirection.Left;
            }
            else
            {
                return diff.y > 0 ? FacingDirection.Up : FacingDirection.Down;
            }
        }
    }
}
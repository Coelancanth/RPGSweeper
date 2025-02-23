using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public enum SymmetryDirection
    {
        AroundHorizontalLine,
        AroundVerticalLine
    }

    public class SymmetricSpawnStrategy : MineSpawnStrategyBase
    {
        private readonly SymmetryDirection m_Direction;
        private readonly bool m_PlaceAdjacent;
        private readonly float m_SymmetryLinePosition; // 0-1 range
        private readonly int m_MinDistanceToLine;
        private readonly int m_MaxDistanceToLine;

        public override SpawnStrategyType Priority => SpawnStrategyType.Symmetric;

        public SymmetricSpawnStrategy(
            SymmetryDirection direction, 
            float symmetryLinePosition = 0.5f,
            int minDistanceToLine = 0,
            int maxDistanceToLine = int.MaxValue,
            bool placeAdjacent = false)
        {
            m_Direction = direction;
            m_PlaceAdjacent = placeAdjacent;
            m_SymmetryLinePosition = Mathf.Clamp01(symmetryLinePosition);
            m_MinDistanceToLine = Mathf.Max(0, minDistanceToLine);
            m_MaxDistanceToLine = maxDistanceToLine;
        }

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            // For symmetric placement, we need at least 2 available positions
            var availablePositions = context.GetAvailablePositions()
                .Where(p => IsWithinDistanceConstraints(p, context))
                .ToList();
                
            return availablePositions.Count >= 2 && 
                   availablePositions.Any(p => HasValidSymmetricPosition(p, context));
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var availablePositions = context.GetAvailablePositions()
                .Where(p => IsWithinDistanceConstraints(p, context) && HasValidSymmetricPosition(p, context))
                .ToList();

            if (availablePositions.Count == 0)
            {
                return SpawnResult.Failed("No valid positions available for symmetric placement");
            }

            var mines = new List<SpawnedMine>();
            int pairsToSpawn = spawnData.SpawnCount / 2;
            int remainingCount = spawnData.SpawnCount % 2;

            while (mines.Count < pairsToSpawn * 2 && availablePositions.Count > 0)
            {
                int index = Random.Range(0, availablePositions.Count);
                var pos = availablePositions[index];
                var symmetricPos = GetSymmetricPosition(pos, context);

                if (!context.ExistingMines.ContainsKey(symmetricPos) && IsWithinDistanceConstraints(symmetricPos, context))
                {
                    var facing1 = GetFacingDirection(pos, context);
                    var facing2 = GetFacingDirection(symmetricPos, context);

                    mines.Add(CreateMine(context, pos, spawnData, facing1));
                    mines.Add(CreateMine(context, symmetricPos, spawnData, facing2));

                    availablePositions.RemoveAll(p => 
                        p == pos || p == symmetricPos || 
                        (m_PlaceAdjacent && (IsAdjacent(p, pos) || IsAdjacent(p, symmetricPos))));
                }
                else
                {
                    availablePositions.RemoveAt(index);
                }
            }

            // Handle odd count if needed
            if (remainingCount > 0 && availablePositions.Count > 0)
            {
                var centerPos = GetLinePosition(context);
                if (IsValidPosition(centerPos, context) && IsWithinDistanceConstraints(centerPos, context))
                {
                    mines.Add(CreateMine(context, centerPos, spawnData, GetFacingDirection(centerPos, context)));
                }
            }

            if (mines.Count == 0)
            {
                return SpawnResult.Failed("Could not place any symmetric mines");
            }

            return SpawnResult.Successful(mines);
        }

        private Vector2Int GetSymmetricPosition(Vector2Int pos, SpawnContext context)
        {
            var linePos = GetLinePosition(context);
            
            return m_Direction switch
            {
                SymmetryDirection.AroundHorizontalLine => new Vector2Int(
                    pos.x,
                    2 * linePos.y - pos.y
                ),
                SymmetryDirection.AroundVerticalLine => new Vector2Int(
                    2 * linePos.x - pos.x,
                    pos.y
                ),
                _ => pos
            };
        }

        private bool HasValidSymmetricPosition(Vector2Int pos, SpawnContext context)
        {
            var symmetricPos = GetSymmetricPosition(pos, context);
            return IsValidPosition(symmetricPos, context) && 
                   IsWithinDistanceConstraints(symmetricPos, context);
        }

        private Vector2Int GetLinePosition(SpawnContext context)
        {
            return m_Direction switch
            {
                SymmetryDirection.AroundHorizontalLine => new Vector2Int(
                    context.GridWidth / 2,
                    Mathf.RoundToInt(context.GridHeight * m_SymmetryLinePosition)
                ),
                SymmetryDirection.AroundVerticalLine => new Vector2Int(
                    Mathf.RoundToInt(context.GridWidth * m_SymmetryLinePosition),
                    context.GridHeight / 2
                ),
                _ => new Vector2Int(context.GridWidth / 2, context.GridHeight / 2)
            };
        }

        private bool IsValidPosition(Vector2Int pos, SpawnContext context)
        {
            return pos.x >= 0 && pos.x < context.GridWidth &&
                   pos.y >= 0 && pos.y < context.GridHeight &&
                   !context.ExistingMines.ContainsKey(pos);
        }

        private bool IsWithinDistanceConstraints(Vector2Int pos, SpawnContext context)
        {
            var linePos = GetLinePosition(context);
            float distance = m_Direction == SymmetryDirection.AroundHorizontalLine
                ? Mathf.Abs(pos.y - linePos.y)
                : Mathf.Abs(pos.x - linePos.x);

            return distance >= m_MinDistanceToLine && distance <= m_MaxDistanceToLine;
        }

        private FacingDirection GetFacingDirection(Vector2Int pos, SpawnContext context)
        {
            var linePos = GetLinePosition(context);
            
            return m_Direction switch
            {
                SymmetryDirection.AroundHorizontalLine => 
                    pos.y > linePos.y ? FacingDirection.Down : FacingDirection.Up,
                SymmetryDirection.AroundVerticalLine => 
                    pos.x > linePos.x ? FacingDirection.Left : FacingDirection.Right,
                _ => FacingDirection.Up
            };
        }

        private bool IsAdjacent(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) <= 1 && Mathf.Abs(a.y - b.y) <= 1;
        }
    }
}
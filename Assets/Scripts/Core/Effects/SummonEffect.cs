using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For GridPositionType

namespace RPGMinesweeper.Effects
{
    public class SummonEffect : IInstantEffect
    {
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly MineType m_MineType;
        private readonly MonsterType m_MonsterType;
        private readonly int m_Count;
        private readonly Vector2Int? m_TriggerPosition;
        private readonly GridPositionType m_TriggerPositionType;

        public EffectTargetType TargetType => EffectTargetType.Grid;

        public SummonEffect(float radius, GridShape shape, MineType mineType, MonsterType monsterType, int count, Vector2Int? triggerPosition = null, GridPositionType triggerPositionType = GridPositionType.Source)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_MineType = mineType;
            m_MonsterType = monsterType;
            m_Count = count;
            m_TriggerPosition = triggerPosition;
            m_TriggerPositionType = triggerPositionType;
        }

        private Vector2Int GetEffectivePosition(Vector2Int sourcePosition, GridManager gridManager)
        {
            if (m_TriggerPosition.HasValue) return m_TriggerPosition.Value;
            
            if (m_TriggerPositionType == GridPositionType.Source) return sourcePosition;
            
            // Get a position based on the trigger type
            var gridSize = new Vector2Int(gridManager.Width, gridManager.Height);
            return m_TriggerPositionType switch
            {
                GridPositionType.Random => new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y)),
                GridPositionType.Edge => GetRandomEdgePosition(gridSize),
                GridPositionType.Corner => GetRandomCornerPosition(gridSize),
                GridPositionType.Center => new Vector2Int(gridSize.x / 2, gridSize.y / 2),
                _ => sourcePosition
            };
        }

        private Vector2Int GetRandomEdgePosition(Vector2Int gridSize)
        {
            bool isHorizontalEdge = Random.value < 0.5f;
            if (isHorizontalEdge)
            {
                int x = Random.Range(0, gridSize.x);
                int y = Random.value < 0.5f ? 0 : gridSize.y - 1;
                return new Vector2Int(x, y);
            }
            else
            {
                int x = Random.value < 0.5f ? 0 : gridSize.x - 1;
                int y = Random.Range(0, gridSize.y);
                return new Vector2Int(x, y);
            }
        }

        private Vector2Int GetRandomCornerPosition(Vector2Int gridSize)
        {
            int x = Random.value < 0.5f ? 0 : gridSize.x - 1;
            int y = Random.value < 0.5f ? 0 : gridSize.y - 1;
            return new Vector2Int(x, y);
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            var effectivePosition = GetEffectivePosition(sourcePosition, gridManager);
            var affectedPositions = GridShapeHelper.GetAffectedPositions(effectivePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            var validPositions = new List<Vector2Int>();

            // Filter for valid and empty positions
            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos) && !mineManager.HasMineAt(pos))
                {
                    validPositions.Add(pos);
                }
            }

            // Randomly select positions to place mines
            int minesToPlace = Mathf.Min(m_Count, validPositions.Count);
            for (int i = 0; i < minesToPlace; i++)
            {
                if (validPositions.Count == 0) break;

                // Randomly select a position from remaining valid positions
                int randomIndex = Random.Range(0, validPositions.Count);
                Vector2Int selectedPos = validPositions[randomIndex];
                validPositions.RemoveAt(randomIndex);

                // Attempt to add the mine
                if (m_MineType == MineType.Monster)
                {
                    GameEvents.RaiseMineAddAttempted(selectedPos, m_MineType, m_MonsterType);
                }
                else
                {
                    GameEvents.RaiseMineAddAttempted(selectedPos, m_MineType, null);
                }
            }

            // Recalculate values for all affected cells
            MineValuePropagator.PropagateValues(mineManager, gridManager);
        }
    }
} 
using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class SummonEffect : IInstantEffect
    {
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly MineType m_MineType;
        private readonly MonsterType m_MonsterType;
        private readonly int m_Count;

        public EffectTargetType TargetType => EffectTargetType.Grid;

        public SummonEffect(float radius, GridShape shape, MineType mineType, MonsterType monsterType, int count)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_MineType = mineType;
            m_MonsterType = monsterType;
            m_Count = count;
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            // Get all possible positions within the shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
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
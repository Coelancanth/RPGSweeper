using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using Random = UnityEngine.Random;
using RPGMinesweeper;  // For GridPositionType

namespace RPGMinesweeper.Effects
{
    public class RangeRevealEffect : ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly Vector2Int? m_TriggerPosition;
        private readonly GridPositionType m_TriggerPositionType;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Range Reveal";
        #endregion

        public RangeRevealEffect(float radius, GridShape shape = GridShape.Square, Vector2Int? triggerPosition = null, GridPositionType triggerPositionType = GridPositionType.Source)
        {
            m_Radius = radius;
            m_Shape = shape;
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

        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            var effectivePosition = GetEffectivePosition(sourcePosition, gridManager);
            var affectedPositions = GridShapeHelper.GetAffectedPositions(effectivePosition, m_Shape, Mathf.RoundToInt(m_Radius));

            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    var cellObject = gridManager.GetCellObject(pos);
                    if (cellObject != null)
                    {
                        var cellView = cellObject.GetComponent<CellView>();
                        if (cellView != null)
                        {
                            // If there's a mine at this position, show its sprite first
                            if (mineManager.HasMineAt(pos))
                            {
                                var mineData = mineManager.GetMineDataAt(pos);
                                if (mineData != null)
                                {
                                    cellView.ShowMineSprite(mineData.MineSprite, mineManager.GetMines()[pos], mineData);
                                }
                            }
                            // Then reveal the cell
                            cellView.UpdateVisuals(true);
                        }
                    }
                }
            }
        }
    }
} 
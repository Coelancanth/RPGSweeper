using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class FreezeEffect : IDurationalEffect
    {
        private readonly float m_Duration;
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private readonly HashSet<Vector2Int> m_FrozenCells;

        public float Duration => m_Duration;
        public EffectTargetType TargetType => EffectTargetType.Grid;

        public FreezeEffect(float duration, int radius, GridShape shape)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_Shape = shape;
            m_FrozenCells = new HashSet<Vector2Int>();
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);

            // Freeze each valid cell
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
                            cellView.SetFrozen(true);
                            m_FrozenCells.Add(pos);
                        }
                    }
                }
            }
        }

        public void Remove(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            // Unfreeze all affected cells
            foreach (var pos in m_FrozenCells)
            {
                var cellObject = gridManager.GetCellObject(pos);
                if (cellObject != null)
                {
                    var cellView = cellObject.GetComponent<CellView>();
                    if (cellView != null)
                    {
                        cellView.SetFrozen(false);
                    }
                }
            }
            m_FrozenCells.Clear();
        }
    }
} 
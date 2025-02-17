using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class UnfreezeEffect : IInstantEffect
    {
        private readonly int m_Radius;
        private readonly GridShape m_Shape;

        public EffectTargetType TargetType => EffectTargetType.Grid;

        public UnfreezeEffect(int radius, GridShape shape)
        {
            m_Radius = radius;
            m_Shape = shape;
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);

            // Unfreeze each valid cell
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
                            cellView.SetFrozen(false);
                        }
                    }
                }
            }
        }
    }
} 
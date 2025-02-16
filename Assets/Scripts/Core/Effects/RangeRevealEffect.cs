using UnityEngine;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class RangeRevealEffect : IInstantEffect
    {
        private readonly float m_Radius;
        private readonly GridShape m_Shape;

        public EffectTargetType TargetType => EffectTargetType.Grid;

        public RangeRevealEffect(float radius, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_Shape = shape;
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));

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
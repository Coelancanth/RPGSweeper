using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class MineVisualManager : IMineVisualManager
    {
        private readonly GridManager m_GridManager;

        public MineVisualManager(GridManager gridManager)
        {
            m_GridManager = gridManager;
        }

        public void UpdateCellView(Vector2Int position, MineData mineData, IMine mine)
        {
            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject == null) return;

            var cellView = cellObject.GetComponent<CellView>();
            if (cellView == null) return;

            cellView.SetValue(mineData.Value, mineData.ValueColor);
            
            if (cellView.IsRevealed)
            {
                cellView.ShowMineSprite(mineData.MineSprite, mine, mineData);
                cellView.UpdateVisuals(true);
            }
        }

        public void ShowMineSprite(Vector2Int position, Sprite sprite, IMine mine, MineData mineData)
        {
            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject == null) return;

            var cellView = cellObject.GetComponent<CellView>();
            if (cellView == null) return;

            cellView.ShowMineSprite(sprite, mine, mineData);
            cellView.UpdateVisuals(true);
        }

        public void HandleMineRemoval(Vector2Int position)
        {
            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject == null) return;

            var cellView = cellObject.GetComponent<CellView>();
            if (cellView == null) return;

            cellView.HandleMineRemoval();
        }
    }
} 
using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class MineVisualManager : IMineVisualManager
    {
        private readonly GridManager m_GridManager;

        public MineVisualManager(GridManager gridManager)
        {
            m_GridManager = gridManager ?? throw new System.ArgumentNullException(nameof(gridManager));
        }

        public void UpdateCellView(Vector2Int position, MineData mineData, IMine mine)
        {
            if (mineData == null)
            {
                Debug.LogWarning($"MineVisualManager: Attempted to update cell view with null MineData at position {position}");
                return;
            }

            if (mine == null)
            {
                Debug.LogWarning($"MineVisualManager: Attempted to update cell view with null Mine at position {position}");
                return;
            }

            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject == null)
            {
                Debug.LogWarning($"MineVisualManager: No cell object found at position {position}");
                return;
            }

            var cellView = cellObject.GetComponent<CellView>();
            if (cellView == null)
            {
                Debug.LogWarning($"MineVisualManager: CellView component not found on cell object at position {position}");
                return;
            }

            cellView.SetValue(mineData.Value, mineData.ValueColor);
            
            if (cellView.IsRevealed)
            {
                cellView.ShowMineSprite(mineData.MineSprite, mine, mineData);
                cellView.UpdateVisuals(true);
            }
        }

        public void ShowMineSprite(Vector2Int position, Sprite sprite, IMine mine, MineData mineData)
        {
            if (mine == null)
            {
                Debug.LogWarning($"MineVisualManager: Attempted to show sprite for null Mine at position {position}");
                return;
            }

            if (mineData == null)
            {
                Debug.LogWarning($"MineVisualManager: Attempted to show sprite with null MineData at position {position}");
                return;
            }

            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject == null)
            {
                Debug.LogWarning($"MineVisualManager: No cell object found at position {position}");
                return;
            }

            var cellView = cellObject.GetComponent<CellView>();
            if (cellView == null)
            {
                Debug.LogWarning($"MineVisualManager: CellView component not found on cell object at position {position}");
                return;
            }

            cellView.ShowMineSprite(sprite, mine, mineData);
            cellView.UpdateVisuals(true);
        }

        public void HandleMineRemoval(Vector2Int position)
        {
            if (!m_GridManager)
            {
                Debug.LogError("MineVisualManager: GridManager is null");
                return;
            }

            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject == null)
            {
                Debug.LogWarning($"MineVisualManager: No cell object found at position {position}");
                return;
            }

            var cellView = cellObject.GetComponent<CellView>();
            if (cellView == null)
            {
                Debug.LogWarning($"MineVisualManager: CellView component not found on cell object at position {position}");
                return;
            }

            cellView.HandleMineRemoval();
        }
    }
} 
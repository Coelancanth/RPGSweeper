using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class MineVisualManager : IMineVisualManager
    {
        private readonly GridManager m_GridManager;
        private readonly MineTypeSpawnData[] m_SpawnData;

        public MineVisualManager(GridManager gridManager, MineTypeSpawnData[] spawnData)
        {
            m_GridManager = gridManager ?? throw new System.ArgumentNullException(nameof(gridManager));
            m_SpawnData = spawnData;
        }

        private Sprite GetDirectionalSprite(MineData mineData)
        {
            //Debug.Log($"Getting directional sprite for {mineData.name}");
            if (mineData == null) return null;

            // First try to get the sprite from spawn data if available
            if (m_SpawnData != null)
            {
                foreach (var spawnData in m_SpawnData)
                {
                    if (spawnData.MineData == mineData)
                    {
                        var spawnDataSprite = spawnData.GetDirectionalSprite(mineData.FacingDirection);
                        if (spawnDataSprite != null)
                        {
                            return spawnDataSprite;
                        }
                        break; // Found matching spawn data but no directional sprite, stop searching
                    }
                }
            }

            // Fallback to MineData's directional sprite
            return mineData.GetDirectionalSprite();
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
            //Debug.Log($"CellView is revealed: {cellView.IsRevealed}");
            if (cellView.IsRevealed)
            {
                //Debug.Log($"Getting directional sprite for {mineData.name}");
                var sprite = GetDirectionalSprite(mineData);
                cellView.ShowMineSprite(sprite ?? mineData.MineSprite, mine, mineData);
                cellView.UpdateVisuals(true);
            }
        }

        public void ShowMineSprite(Vector2Int position, Sprite sprite, IMine mine, MineData mineData)
        {
            //Debug.Log($"Showing mine sprite for {mineData.name}");
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

            // Use the provided sprite or get the directional sprite
            //var finalSprite = sprite ?? GetDirectionalSprite(mineData) ?? mineData.MineSprite;
            //Debug.Log($"sprite: {sprite?.name}");
            //Debug.Log($"directional sprite: {GetDirectionalSprite(mineData)?.name}");
            //Debug.Log($"mineData sprite: {mineData.MineSprite?.name}");
            //Debug.Log($"Showing mine sprite for {position}: {finalSprite?.name}");
            var finalSprite = GetDirectionalSprite(mineData) ?? sprite;;
            cellView.ShowMineSprite(finalSprite, mine, mineData);
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
using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public interface IMineVisualManager
    {
        void UpdateCellView(Vector2Int position, MineData mineData, IMine mine);
        void ShowMineSprite(Vector2Int position, Sprite sprite, IMine mine, MineData mineData);
        void HandleMineRemoval(Vector2Int position);
    }
} 
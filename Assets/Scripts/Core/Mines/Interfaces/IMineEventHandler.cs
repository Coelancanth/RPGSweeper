using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public interface IMineEventHandler
    {
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
        void HandleCellRevealed(Vector2Int position);
        void HandleMineRemoval(Vector2Int position);
        void HandleMineAdd(Vector2Int position, MineType type, MonsterType? monsterType);
    }
} 
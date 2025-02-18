using UnityEngine;

namespace RPGMinesweeper.Input
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        Vector2Int Position { get; }
        void OnInteract();
    }
} 
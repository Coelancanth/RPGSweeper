using UnityEngine;

namespace RPGMinesweeper
{
    /// <summary>
    /// Interface for cell visual updates.
    /// Provides methods to update and manipulate the visual representation of a cell.
    /// </summary>
    public interface ICellVisualUpdater
    {
        void UpdateVisuals();
        void ShowDebugHighlight(Color highlightColor);
        void HideDebugHighlight();
        void ApplyFrozenVisual(bool frozen);
    }
} 
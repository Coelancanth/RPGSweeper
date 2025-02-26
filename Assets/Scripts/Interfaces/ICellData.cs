using UnityEngine;
using RPGMinesweeper;

namespace RPGMinesweeper
{
    /// <summary>
    /// Read-only interface for accessing cell data properties.
    /// Provides a clean, immutable view of a cell's state.
    /// </summary>
    public interface ICellData
    {
        Vector2Int Position { get; }
        bool IsRevealed { get; }
        bool IsFrozen { get; }
        bool HasMine { get; }
        int CurrentValue { get; }
        Color CurrentValueColor { get; }
        IMine CurrentMine { get; }
        MineData CurrentMineData { get; }
        CellMarkType MarkType { get; }
    }
} 
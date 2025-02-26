using UnityEngine;
using RPGMinesweeper;

namespace RPGMinesweeper
{
    /// <summary>
    /// Interface for modifying cell state.
    /// Provides methods to change the state of a cell while encapsulating the implementation details.
    /// </summary>
    public interface ICellStateModifier
    {
        void SetRevealed(bool revealed);
        void SetFrozen(bool frozen);
        void SetValue(int value, Color color);
        void SetMine(IMine mine, MineData mineData);
        void RemoveMine();
        void SetMarkType(CellMarkType markType);
    }
} 
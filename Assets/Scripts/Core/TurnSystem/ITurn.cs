using UnityEngine;

namespace RPGMinesweeper.TurnSystem
{
    public interface ITurn
    {
        bool IsComplete { get; }
        void Begin();
        void Update();
        void End();
    }
} 
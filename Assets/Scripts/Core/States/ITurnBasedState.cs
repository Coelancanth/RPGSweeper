using UnityEngine;

namespace RPGMinesweeper.States
{
    public interface ITurnBasedState : IState
    {
        int TurnsRemaining { get; }
        void OnTurnEnd();
    }
} 
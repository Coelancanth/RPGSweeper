using UnityEngine;

namespace RPGMinesweeper.States
{
    public interface IState
    {
        string Name { get; }
        float Duration { get; }
        bool IsExpired { get; }
        void Enter(GameObject target);
        void Update(float deltaTime);
        void Exit(GameObject target);
    }
} 
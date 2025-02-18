using UnityEngine;

namespace RPGMinesweeper.States
{
    public interface IState
    {
        string Name { get; }
        float Duration { get; }
        bool IsExpired { get; }
        StateTarget Target { get; }
        object TargetId { get; } // This will store the specific target identifier (e.g., Vector2Int for cells)
        void Enter(GameObject target);
        void Update(float deltaTime);
        void Exit(GameObject target);
    }
} 
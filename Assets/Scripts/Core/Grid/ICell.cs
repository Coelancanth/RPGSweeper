using UnityEngine;
public interface ICell
{
    Vector2Int Position { get; }
    bool IsRevealed { get; }
    void Reveal();
    void Reset();
} 
using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<Vector2Int> OnCellRevealed;
    public static event Action<MineType> OnMineTriggered;
    public static event Action<Vector2Int> OnEffectApplied;
    public static event Action<int> OnExperienceGained;
    public static event Action<Vector2Int> OnMineRemovalAttempted;
    public static event Action<int> OnShieldChanged;

    public static void RaiseCellRevealed(Vector2Int position)
    {
        OnCellRevealed?.Invoke(position);
    }

    public static void RaiseMineTriggered(MineType type)
    {
        OnMineTriggered?.Invoke(type);
    }

    public static void RaiseEffectApplied(Vector2Int position)
    {
        OnEffectApplied?.Invoke(position);
    }

    public static void RaiseExperienceGained(int amount)
    {
        OnExperienceGained?.Invoke(amount);
    }

    public static void RaiseMineRemovalAttempted(Vector2Int position)
    {
        OnMineRemovalAttempted?.Invoke(position);
    }

    public static void RaiseShieldChanged(int newShieldAmount)
    {
        OnShieldChanged?.Invoke(newShieldAmount);
    }
} 
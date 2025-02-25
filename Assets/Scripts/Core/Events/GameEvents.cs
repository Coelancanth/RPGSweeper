using System;
using UnityEngine;
using RPGMinesweeper;

public static class GameEvents
{
    public static event Action<Vector2Int> OnCellRevealed;
    public static event Action<MineType> OnMineTriggered;
    public static event Action<Vector2Int> OnEffectApplied;
    public static event Action<int> OnExperienceGained;
    public static event Action<Vector2Int, MineType, MonsterType?> OnMineAddAttempted;
    public static event Action<Vector2Int> OnMineRemovalAttempted;
    public static event Action<Vector2Int> OnEffectsRemoved;
    public static event Action<int> OnShieldChanged;
    public static event Action OnRoundAdvanced;
    public static event Action<Vector2Int, CellMarkType> OnCellMarked;

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

    public static void RaiseMineAddAttempted(Vector2Int position, MineType type, MonsterType? monsterType = null)
    {
        OnMineAddAttempted?.Invoke(position, type, monsterType);
    }

    public static void RaiseMineRemovalAttempted(Vector2Int position)
    {
        OnMineRemovalAttempted?.Invoke(position);
    }

    public static void RaiseEffectsRemoved(Vector2Int position)
    {
        OnEffectsRemoved?.Invoke(position);
    }

    public static void RaiseShieldChanged(int newShieldAmount)
    {
        OnShieldChanged?.Invoke(newShieldAmount);
    }

    public static void RaiseRoundAdvanced()
    {
        OnRoundAdvanced?.Invoke();
    }
    
    public static void RaiseCellMarked(Vector2Int position, CellMarkType markType)
    {
        OnCellMarked?.Invoke(position, markType);
    }
} 
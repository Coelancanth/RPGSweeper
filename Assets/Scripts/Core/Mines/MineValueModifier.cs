using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

public static class MineValueModifier
{
    #region Private Fields
    private static Dictionary<Vector2Int, HashSet<IEffect>> s_RegisteredEffects = new Dictionary<Vector2Int, HashSet<IEffect>>();
    #endregion

    #region Public Methods
    public static void RegisterEffect(Vector2Int position, IEffect effect)
    {
        if (!s_RegisteredEffects.ContainsKey(position))
        {
            s_RegisteredEffects[position] = new HashSet<IEffect>();
        }
        s_RegisteredEffects[position].Add(effect);
    }

    public static void UnregisterEffect(Vector2Int position, IEffect effect)
    {
        if (s_RegisteredEffects.ContainsKey(position))
        {
            s_RegisteredEffects[position].Remove(effect);
            if (s_RegisteredEffects[position].Count == 0)
            {
                s_RegisteredEffects.Remove(position);
            }
        }
    }

    public static int ModifyValue(Vector2Int position, int baseValue)
    {
        int modifiedValue = baseValue;

        if (!s_RegisteredEffects.ContainsKey(position))
        {
            return modifiedValue;
        }

        // Check for confusion effects
        foreach (var effect in s_RegisteredEffects[position])
        {
            if (effect is ConfusionEffect)
            {
                return -1; // Show "?" if any confusion effect is active
            }
        }

        return modifiedValue;
    }

    public static (int value, Color color) ModifyValueAndGetColor(Vector2Int position, int baseValue)
    {
        int modifiedValue = baseValue;
        Color color = Color.white;

        if (!s_RegisteredEffects.ContainsKey(position))
        {
            return (modifiedValue, color);
        }

        // Check for confusion effects
        foreach (var effect in s_RegisteredEffects[position])
        {
            if (effect is ConfusionEffect)
            {
                return (-1, new Color(1f, 0.4f, 0.7f)); // Pink color for confusion
            }
        }

        return (modifiedValue, color);
    }

    public static void Clear()
    {
        s_RegisteredEffects.Clear();
    }
    #endregion
} 
using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

public class MineValueModifier
{
    private static Dictionary<Vector2Int, HashSet<IEffect>> s_ActiveEffects = new Dictionary<Vector2Int, HashSet<IEffect>>();

    public static void RegisterEffect(Vector2Int position, IEffect effect)
    {
        if (!s_ActiveEffects.ContainsKey(position))
        {
            s_ActiveEffects[position] = new HashSet<IEffect>();
        }
        s_ActiveEffects[position].Add(effect);
    }

    public static void UnregisterEffect(Vector2Int position, IEffect effect)
    {
        if (s_ActiveEffects.ContainsKey(position))
        {
            s_ActiveEffects[position].Remove(effect);
            if (s_ActiveEffects[position].Count == 0)
            {
                s_ActiveEffects.Remove(position);
            }
        }
    }

    public static int ModifyValue(Vector2Int position, int originalValue)
    {
        if (!s_ActiveEffects.ContainsKey(position))
        {
            return originalValue;
        }

        // Check for confusion effects
        foreach (var effect in s_ActiveEffects[position])
        {
            if (effect is ConfusionEffect)
            {
                return -1; // Show "?" if any confusion effect is active
            }
        }

        return originalValue;
    }

    public static (int value, Color color) ModifyValueAndGetColor(Vector2Int position, int originalValue)
    {
        if (!s_ActiveEffects.ContainsKey(position))
        {
            return (originalValue, Color.white);
        }

        // Check for confusion effects
        foreach (var effect in s_ActiveEffects[position])
        {
            if (effect is ConfusionEffect)
            {
                return (-1, new Color(1f, 0.4f, 0.7f)); // Pink color for confusion
            }
        }

        return (originalValue, Color.white);
    }

    public static void Clear()
    {
        s_ActiveEffects.Clear();
    }
} 
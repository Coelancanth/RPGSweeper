using UnityEngine;

namespace RPGMinesweeper.Effects
{
    public enum EffectType
    {
        Persistent,    // Long-lasting effects that persist (e.g., Poison, Curse, Confusion)
        Triggerable    // One-time effects that trigger on certain conditions (e.g., Summon, Split)
    }

    // Base interface for all effects
    public interface IEffect
    {
        EffectType Type { get; }
        string Name { get; }
        void Apply(GameObject target, Vector2Int sourcePosition);
    }

    // For persistent effects that maintain state
    public interface IPersistentEffect : IEffect
    {
        bool IsActive { get; }
        void Update(float deltaTime);
        void Remove(GameObject target);
    }

    // For triggerable effects that apply once
    public interface ITriggerableEffect : IEffect
    {
        // Just inherits Apply from IEffect
        // Triggerable effects are one-time triggers that may apply states
    }

    // For effects that need to react to specific game events
    public interface IReactiveEffect : IEffect
    {
        void OnTrigger(GameObject source, Vector2Int sourcePosition);
    }
} 
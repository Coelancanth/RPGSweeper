using UnityEngine;

namespace RPGMinesweeper.Effects
{
    public enum EffectTargetType
    {
        Player,     // Effects that target the player (heal, damage, shield)
        Grid,       // Effects that target grid cells (reveal)
        Area,       // Effects that target an area (explosion, chain reaction)
        Global      // Effects that affect the game state (time slow, difficulty change)
    }

    // Base interface for all effects
    public interface IEffect
    {
        EffectTargetType TargetType { get; }
        void Apply(GameObject source, Vector2Int sourcePosition);
    }

    // For effects that need duration and cleanup
    public interface IDurationalEffect : IEffect
    {
        float Duration { get; }
        void Remove(GameObject source, Vector2Int sourcePosition);
    }

    // For instant effects (no cleanup needed)
    public interface IInstantEffect : IEffect
    {
        // Just inherits Apply from IEffect
    }

    // For effects that react to mine removal
    public interface IMineReactiveEffect : IEffect
    {
        void OnMineRemoved(GameObject source, Vector2Int sourcePosition);
    }
} 
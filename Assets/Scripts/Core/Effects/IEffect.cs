using UnityEngine;

namespace RPGMinesweeper.Effects
{
    public enum EffectTargetType
    {
        Player,     // Effects that target the player (heal, damage, shield, speed)
        Grid,       // Effects that target grid cells (reveal)
        Area,       // Effects that target an area (explosion, chain reaction)
        Global      // Effects that affect the game state (time slow, difficulty change)
    }

    public interface IEffect
    {
        EffectType Type { get; }
        EffectTargetType TargetType { get; }
        float Duration { get; }
        void Apply(GameObject source, Vector2Int sourcePosition);
        void Remove(GameObject source, Vector2Int sourcePosition);
    }

    public interface IPassiveEffect : IEffect
    {
        void OnTick(GameObject source, Vector2Int sourcePosition);
    }

    public interface IActiveEffect : IEffect
    {
        void OnMineRemoved(GameObject source, Vector2Int sourcePosition);
    }

    public enum EffectType
    {
        None,
        Reveal,
        Transform,
        Heal,
        SpawnItem,
        Damage,
        Shield,
        Speed,
        Confusion    // Obscures mine values with question marks
    }
} 
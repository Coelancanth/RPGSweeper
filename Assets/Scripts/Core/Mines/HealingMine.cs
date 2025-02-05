using UnityEngine;

public class HealingMine : BaseMine
{
    public HealingMine(MineData _data, Vector2Int _position) : base(_data, _position)
    {
    }

    public override void OnTrigger(Player _player)
    {
        if (m_IsDestroyed) return;

        // Healing mines don't cause damage
        _player.GainExperience(m_Data.Damage); // Use damage value as healing amount
        GameEvents.RaiseMineTriggered(Type);
        
        foreach (var effect in m_Data.Effects)
        {
            ApplyEffect(effect);
        }

        OnDestroy();
    }

    protected override void ApplyEffect(EffectData _effect)
    {
        if (_effect.Type == EffectType.Heal)
        {
            GameEvents.RaiseEffectApplied(m_Position);
        }
    }
} 
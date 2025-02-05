using UnityEngine;

public class ExperienceMine : BaseMine
{
    public override bool CanDisguise => true; // Can look like a treasure chest

    public ExperienceMine(MineData _data, Vector2Int _position) : base(_data, _position)
    {
    }

    public override void OnTrigger(Player _player)
    {
        if (m_IsDestroyed) return;

        // Experience mines give experience instead of damage
        _player.GainExperience(m_Data.Damage);
        GameEvents.RaiseMineTriggered(Type);
        
        foreach (var effect in m_Data.Effects)
        {
            ApplyEffect(effect);
        }

        OnDestroy();
    }
} 
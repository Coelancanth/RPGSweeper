using UnityEngine;
using System.Collections.Generic;

public abstract class BaseMine : IMine
{
    protected MineData m_Data;
    protected Vector2Int m_Position;
    protected bool m_IsDestroyed;

    public MineType Type => m_Data.Type;
    public virtual bool CanDisguise => false;

    protected BaseMine(MineData _data, Vector2Int _position)
    {
        m_Data = _data;
        m_Position = _position;
        m_IsDestroyed = false;
    }

    public virtual void OnTrigger(Player _player)
    {
        if (m_IsDestroyed) return;

        int damage = CalculateDamage(_player);
        _player.TakeDamage(damage);
        
        GameEvents.RaiseMineTriggered(Type);
        
        foreach (var effect in m_Data.Effects)
        {
            ApplyEffect(effect);
        }
    }

    protected virtual int CalculateDamage(Player _player)
    {
        return m_Data.Damage;
    }

    protected virtual void ApplyEffect(EffectData _effect)
    {
        // Base effect application logic
    }

    public virtual void OnDestroy()
    {
        m_IsDestroyed = true;
    }
} 
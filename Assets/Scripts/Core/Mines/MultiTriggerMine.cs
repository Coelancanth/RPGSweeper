using UnityEngine;

public class MultiTriggerMine : BaseMine
{
    private int m_TriggerCount;
    private readonly int m_RequiredTriggers;

    public MultiTriggerMine(MineData _data, Vector2Int _position, int _requiredTriggers = 3) 
        : base(_data, _position)
    {
        m_RequiredTriggers = _requiredTriggers;
        m_TriggerCount = 0;
    }

    public override void OnTrigger(Player _player)
    {
        if (m_IsDestroyed) return;

        m_TriggerCount++;
        
        float damageMultiplier = 1f / m_RequiredTriggers;
        int damage = Mathf.RoundToInt(m_Data.Damage * damageMultiplier);
        
        _player.TakeDamage(damage);
        GameEvents.RaiseMineTriggered(Type);

        if (m_TriggerCount >= m_RequiredTriggers)
        {
            foreach (var effect in m_Data.Effects)
            {
                ApplyEffect(effect);
            }
            OnDestroy();
        }
    }
} 
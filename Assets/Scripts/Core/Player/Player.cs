using System;

public class Player
{
    private PlayerStats m_Stats;
    private readonly int m_BaseMaxHP = 20;
    private readonly int m_HPIncreasePerLevel = 5;

    public event Action OnDeath;

    // Public properties to expose stats
    public int CurrentHp => m_Stats.CurrentHP;
    public int MaxHp => m_Stats.MaxHP;
    public int Level => m_Stats.Level;
    public int Experience => m_Stats.Experience;

    public Player()
    {
        m_Stats = new PlayerStats(m_BaseMaxHP);
        
        m_Stats.OnLevelUp += HandleLevelUp;
    }

    public void TakeDamage(int _damage)
    {
        if (_damage <= 0) return;

        m_Stats.ModifyHP(-_damage);

        if (m_Stats.CurrentHP <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    private void HandleLevelUp(int _newLevel)
    {
        int newMaxHP = m_BaseMaxHP + (_newLevel - 1) * m_HPIncreasePerLevel;
        m_Stats.SetMaxHP(newMaxHP);
        m_Stats.RestoreFullHP();
    }

    public void GainExperience(int _amount)
    {
        if (_amount <= 0) return;
        m_Stats.AddExperience(_amount);
    }
} 
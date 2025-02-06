using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStats m_Stats;
    private readonly int m_BaseMaxHP = 20;
    private readonly int m_HPIncreasePerLevel = 5;

    public PlayerStats Stats => m_Stats;
    public event Action OnDeath;

    private void Awake()
    {
        m_Stats = new PlayerStats(m_BaseMaxHP);
        m_Stats.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        if (m_Stats != null)
        {
            m_Stats.OnLevelUp -= HandleLevelUp;
        }
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
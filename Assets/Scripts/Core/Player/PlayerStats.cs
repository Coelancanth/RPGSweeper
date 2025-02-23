using System;
using UnityEngine;


public class PlayerStats
{
    private int m_CurrentHP;
    private int m_MaxHP;
    private int m_Experience;
    private int m_Level;
    
    private readonly int m_BaseExperienceToLevel = 100;
    private readonly float m_ExperienceMultiplier = 1.5f;

    public int CurrentHP => m_CurrentHP;
    public int MaxHP => m_MaxHP;
    public int Experience => m_Experience;
    public int Level => m_Level;

    public event Action<int> OnLevelUp;
    public event Action<int> OnHPChanged;
    public event Action<int> OnExperienceChanged;

    public PlayerStats(int _initialMaxHP)
    {
        m_MaxHP = _initialMaxHP;
        m_CurrentHP = m_MaxHP;
        m_Level = 1;
        m_Experience = 0;
    }

    public void ModifyHP(int _amount)
    {
        m_CurrentHP = Mathf.Clamp(m_CurrentHP + _amount, 0, m_MaxHP);
        OnHPChanged?.Invoke(m_CurrentHP);
    }

    public void SetMaxHP(int _newMax)
    {
        m_MaxHP = Mathf.Max(1, _newMax);
        m_CurrentHP = Mathf.Min(m_CurrentHP, m_MaxHP);
        OnHPChanged?.Invoke(m_CurrentHP);
    }

    public void RestoreFullHP()
    {
        m_CurrentHP = m_MaxHP;
        OnHPChanged?.Invoke(m_CurrentHP);
    }

    public void AddExperience(int _amount)
    {
        m_Experience += _amount;
        OnExperienceChanged?.Invoke(m_Experience);

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int experienceRequired = CalculateExperienceForLevel(m_Level + 1);
        
        while (m_Experience >= experienceRequired)
        {
            m_Level++;
            OnLevelUp?.Invoke(m_Level);
            experienceRequired = CalculateExperienceForLevel(m_Level + 1);
        }
    }

    private int CalculateExperienceForLevel(int _level)
    {
        return Mathf.RoundToInt(m_BaseExperienceToLevel * Mathf.Pow(m_ExperienceMultiplier, _level - 1));
    }
} 
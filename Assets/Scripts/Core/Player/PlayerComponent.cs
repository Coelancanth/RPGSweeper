using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    [Header("Player State")]
    [SerializeField, Tooltip("Current shield amount, reduces incoming damage")]
    private int m_CurrentShield = 0;

    private Player m_Player;

    private void Awake()
    {
        m_Player = new Player();
    }

    public void TakeDamage(int damage)
    {
        // Apply shield reduction first
        if (m_CurrentShield > 0)
        {
            int shieldDamage = Mathf.Min(m_CurrentShield, damage);
            m_CurrentShield -= shieldDamage;
            damage -= shieldDamage;
            
            // Notify shield change
            GameEvents.RaiseShieldChanged(m_CurrentShield);
        }

        if (damage > 0)
        {
            m_Player.TakeDamage(damage);
        }
    }

    public void GainExperience(int amount)
    {
        m_Player.GainExperience(amount);
    }

    public void AddShield(int amount)
    {
        m_CurrentShield += amount;
        GameEvents.RaiseShieldChanged(m_CurrentShield);
    }

    public void RemoveShield(int amount)
    {
        m_CurrentShield = Mathf.Max(0, m_CurrentShield - amount);
        GameEvents.RaiseShieldChanged(m_CurrentShield);
    }

    // Public getters for UI/display purposes
    public int CurrentShield => m_CurrentShield;
    public int CurrentHp => m_Player.CurrentHp;
    public int MaxHp => m_Player.MaxHp;
    public int Level => m_Player.Level;
    public int Experience => m_Player.Experience;
} 
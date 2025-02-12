using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    private Player m_Player;
    private float m_BaseSpeed = 5f;
    private float m_CurrentSpeedModifier = 1f;
    private int m_CurrentShield = 0;

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
    }

    public void RemoveShield(int amount)
    {
        m_CurrentShield = Mathf.Max(0, m_CurrentShield - amount);
    }

    public void ModifySpeed(float modifier)
    {
        m_CurrentSpeedModifier += modifier;
        UpdateMovementSpeed();
    }

    private void UpdateMovementSpeed()
    {
        // Apply speed to character controller or rigidbody
        float currentSpeed = m_BaseSpeed * m_CurrentSpeedModifier;
        // Implementation depends on your movement system
    }
} 
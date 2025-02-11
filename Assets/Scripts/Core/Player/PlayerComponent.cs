using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    private Player m_Player;

    private void Awake()
    {
        m_Player = new Player();
    }

    public void TakeDamage(int damage)
    {
        m_Player.TakeDamage(damage);
    }

    public void GainExperience(int amount)
    {
        m_Player.GainExperience(amount);
    }
} 
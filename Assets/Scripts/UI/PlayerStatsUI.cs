using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField, Tooltip("Displays current/max HP (e.g., 'HP: 80/100')")]
    private TextMeshProUGUI m_HpText;
    
    [SerializeField, Tooltip("Displays current shield amount")]
    private TextMeshProUGUI m_ShieldText;
    
    [SerializeField, Tooltip("Displays current level")]
    private TextMeshProUGUI m_LevelText;
    
    [SerializeField, Tooltip("Displays current experience")]
    private TextMeshProUGUI m_ExperienceText;

    [Header("HP Bar")]
    [SerializeField, Tooltip("Visual representation of HP")]
    private Image m_HpFillBar;
    
    [SerializeField, Tooltip("Color when HP is above 50%")]
    private Color m_HealthyColor = Color.green;
    
    [SerializeField, Tooltip("Color when HP is between 25% and 50%")]
    private Color m_CautionColor = Color.yellow;
    
    [SerializeField, Tooltip("Color when HP is below 25%")]
    private Color m_DangerColor = Color.red;

    private PlayerComponent m_PlayerComponent;

    private void Start()
    {
        m_PlayerComponent = FindFirstObjectByType<PlayerComponent>();
        if (m_PlayerComponent == null)
        {
            Debug.LogError("PlayerStatsUI: Could not find PlayerComponent!");
            enabled = false;
            return;
        }

        // Subscribe to events
        GameEvents.OnShieldChanged += UpdateShieldText;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        GameEvents.OnShieldChanged -= UpdateShieldText;
    }

    private void Update()
    {
        if (m_PlayerComponent == null) return;

        UpdateHPDisplay();
        UpdateLevelAndExperience();
    }

    private void UpdateHPDisplay()
    {
        int currentHp = m_PlayerComponent.CurrentHp;
        int maxHp = m_PlayerComponent.MaxHp;

        // Update HP text
        if (m_HpText != null)
        {
            m_HpText.text = $"HP: {currentHp}/{maxHp}";
        }

        // Update HP bar
        if (m_HpFillBar != null)
        {
            float hpPercentage = (float)currentHp / maxHp;
            m_HpFillBar.fillAmount = hpPercentage;

            // Update color based on HP percentage
            if (hpPercentage > 0.5f)
                m_HpFillBar.color = m_HealthyColor;
            else if (hpPercentage > 0.25f)
                m_HpFillBar.color = m_CautionColor;
            else
                m_HpFillBar.color = m_DangerColor;
        }
    }

    private void UpdateShieldText(int shieldAmount)
    {
        if (m_ShieldText != null)
        {
            m_ShieldText.text = shieldAmount > 0 ? $"Shield: {shieldAmount}" : string.Empty;
        }
    }

    private void UpdateLevelAndExperience()
    {
        // Update level text
        if (m_LevelText != null)
        {
            m_LevelText.text = $"Level: {m_PlayerComponent.Level}";
        }

        // Update experience text
        if (m_ExperienceText != null)
        {
            m_ExperienceText.text = $"EXP: {m_PlayerComponent.Experience}";
        }
    }
} 
using UnityEngine;
using TMPro;

public class CharacterUIView : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private CharacterUIConfig m_Config;
    
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI m_HPText;
    [SerializeField] private TextMeshProUGUI m_EXPText;
    [SerializeField] private TextMeshProUGUI m_LevelText;
    #endregion

    #region Private Fields
    private PlayerStats m_PlayerStats;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeUI();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Initialization
    private void InitializeUI()
    {
        // Set initial colors
        m_HPText.color = m_Config.HPColor;
        m_EXPText.color = m_Config.EXPColor;
        m_LevelText.color = m_Config.LevelColor;

        // Get player stats reference
        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            m_PlayerStats = player.Stats;
            UpdateAllStats();
        }
        else
        {
            Debug.LogError("Player not found in scene!");
        }
    }

    private void SubscribeToEvents()
    {
        if (m_PlayerStats != null)
        {
            m_PlayerStats.OnHPChanged += UpdateHP;
            m_PlayerStats.OnExperienceChanged += UpdateExperience;
            m_PlayerStats.OnLevelUp += UpdateLevel;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (m_PlayerStats != null)
        {
            m_PlayerStats.OnHPChanged -= UpdateHP;
            m_PlayerStats.OnExperienceChanged -= UpdateExperience;
            m_PlayerStats.OnLevelUp -= UpdateLevel;
        }
    }
    #endregion

    #region Update Methods
    private void UpdateAllStats()
    {
        UpdateHP(m_PlayerStats.CurrentHP);
        UpdateExperience(m_PlayerStats.Experience);
        UpdateLevel(m_PlayerStats.Level);
    }

    private void UpdateHP(int _currentHP)
    {
        m_HPText.text = string.Format(m_Config.HPFormat, _currentHP, m_PlayerStats.MaxHP);
    }

    private void UpdateExperience(int _experience)
    {
        int nextLevelExp = CalculateExperienceForNextLevel();
        m_EXPText.text = string.Format(m_Config.EXPFormat, _experience, nextLevelExp);
    }

    private void UpdateLevel(int _level)
    {
        m_LevelText.text = string.Format(m_Config.LevelFormat, _level);
    }
    #endregion

    #region Helper Methods
    private int CalculateExperienceForNextLevel()
    {
        // This should match the calculation in PlayerStats
        int currentLevel = m_PlayerStats.Level;
        float baseExp = 100; // Base experience needed for level 2
        float multiplier = 1.5f; // Experience multiplier per level
        
        return Mathf.RoundToInt(baseExp * Mathf.Pow(multiplier, currentLevel - 1));
    }
    #endregion
} 
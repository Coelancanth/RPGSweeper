using UnityEngine;
using TMPro;

public class MonsterMineDisplayStrategy : IMineDisplayStrategy
{
    private TextMeshPro m_ValueText;
    private TextMeshPro m_StatsText;
    private TextMeshPro m_MineValueText;
    private MonsterMine m_MonsterMine;
    private MineDisplayConfig m_Config;
    private bool m_IsRevealed;

    public void SetupDisplay(GameObject cellObject, TextMeshPro valueText)
    {
        m_ValueText = valueText;
        m_Config = cellObject.GetComponent<CellView>().DisplayConfig;
        
        // Create stats text (HP) component
        var statsGO = new GameObject("MonsterStats");
        statsGO.transform.SetParent(cellObject.transform, false);
        m_StatsText = statsGO.AddComponent<TextMeshPro>();
        m_StatsText.fontSize = valueText.fontSize;
        m_StatsText.alignment = TextAlignmentOptions.Center;
        m_StatsText.sortingOrder = valueText.sortingOrder;
        m_StatsText.transform.localPosition = m_Config.HPPosition;
        m_StatsText.enabled = false;

        // Create mine value text component
        var mineValueGO = new GameObject("MineValue");
        mineValueGO.transform.SetParent(cellObject.transform, false);
        m_MineValueText = mineValueGO.AddComponent<TextMeshPro>();
        m_MineValueText.fontSize = valueText.fontSize;
        m_MineValueText.alignment = TextAlignmentOptions.Center;
        m_MineValueText.sortingOrder = valueText.sortingOrder;
        m_MineValueText.transform.localPosition = m_Config.ValuePosition;
        m_MineValueText.enabled = false;

        // Position damage text
        m_ValueText.transform.localPosition = m_Config.DamagePosition;
    }

    public void UpdateDisplay(IMine mine, MineData mineData, bool isRevealed)
    {
        // Unsubscribe from previous monster's events if any
        if (m_MonsterMine != null)
        {
            m_MonsterMine.OnHpChanged -= HandleHpChanged;
            m_MonsterMine.OnEnraged -= HandleEnraged;
        }

        m_IsRevealed = isRevealed;
        if (!isRevealed)
        {
            m_ValueText.enabled = false;
            m_StatsText.enabled = false;
            m_MineValueText.enabled = false;
            return;
        }

        m_MonsterMine = mine as MonsterMine;
        if (m_MonsterMine == null) return;

        // Subscribe to new monster's events
        m_MonsterMine.OnHpChanged += HandleHpChanged;
        m_MonsterMine.OnEnraged += HandleEnraged;

        UpdateDamageDisplay();
        UpdateHPDisplay();
        UpdateMineValueDisplay(mineData);
    }

    private void HandleHpChanged(Vector2Int position, float hpPercentage)
    {
        if (!m_IsRevealed || m_MonsterMine == null) return;
        UpdateHPDisplay();
    }

    private void HandleEnraged(Vector2Int position)
    {
        if (!m_IsRevealed || m_MonsterMine == null) return;
        UpdateDamageDisplay();
    }

    private void UpdateDamageDisplay()
    {
        m_ValueText.enabled = true;
        m_ValueText.text = m_MonsterMine.CalculateDamage().ToString();
        m_ValueText.color = m_MonsterMine.IsEnraged ? m_Config.EnragedColor : m_Config.DefaultValueColor;
        m_ValueText.fontStyle = m_MonsterMine.IsEnraged ? FontStyles.Bold : FontStyles.Normal;
    }

    private void UpdateHPDisplay()
    {
        m_StatsText.enabled = true;
        m_StatsText.text = $"{m_MonsterMine.CurrentHp}/{m_MonsterMine.MaxHp}";
        m_StatsText.color = Color.Lerp(m_Config.HPLowColor, m_Config.HPHighColor, m_MonsterMine.HpPercentage);
    }

    private void UpdateMineValueDisplay(MineData mineData)
    {
        m_MineValueText.enabled = true;
        m_MineValueText.text = mineData.Value.ToString();
        m_MineValueText.color = mineData.ValueColor;
    }

    public void CleanupDisplay()
    {
        // Unsubscribe from events
        if (m_MonsterMine != null)
        {
            m_MonsterMine.OnHpChanged -= HandleHpChanged;
            m_MonsterMine.OnEnraged -= HandleEnraged;
            m_MonsterMine = null;
        }

        if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
        }
        if (m_StatsText != null)
        {
            Object.Destroy(m_StatsText.gameObject);
        }
        if (m_MineValueText != null)
        {
            Object.Destroy(m_MineValueText.gameObject);
        }
    }
} 
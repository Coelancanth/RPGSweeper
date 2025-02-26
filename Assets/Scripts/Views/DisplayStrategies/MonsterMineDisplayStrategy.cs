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
        m_MineValueText.transform.localPosition = m_Config.MonsterMineValuePosition;
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
            m_MonsterMine.OnDefeated -= HandleDefeated;
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
        m_MonsterMine.OnDefeated += HandleDefeated;

        // Update text positions
        UpdateTextPositions();
        
        // If monster is already defeated, update to show defeated state
        if (m_MonsterMine.IsDefeated)
        {
            UpdateDefeatedDisplay(mineData);
        }
        else
        {
            // Update displays for normal monster state
            UpdateDamageDisplay();
            UpdateHPDisplay();
            UpdateMineValueDisplay(mineData);
        }
    }

    private void UpdateTextPositions()
    {
        if (m_StatsText != null)
        {
            m_StatsText.transform.localPosition = m_Config.HPPosition;
        }
        if (m_ValueText != null)
        {
            m_ValueText.transform.localPosition = m_Config.DamagePosition;
        }
        if (m_MineValueText != null)
        {
            m_MineValueText.transform.localPosition = m_Config.MonsterMineValuePosition;
        }
    }

    private void HandleHpChanged(Vector2Int position, float hpPercentage)
    {
        if (!m_IsRevealed || m_MonsterMine == null) return;
        
        if (m_MonsterMine.IsDefeated)
        {
            // If monster is now defeated, update to defeated display
            UpdateDefeatedDisplay(null);
            
            // Notify cell view to update its visuals (e.g., change background sprite)
            var cellView = m_StatsText.transform.parent.GetComponent<CellView>();
            if (cellView != null)
            {
                cellView.UpdateVisuals(m_IsRevealed);
            }
        }
        else
        {
            // Normal HP update
            UpdateHPDisplay();
        }
    }

    private void HandleDefeated(Vector2Int position)
    {
        if (!m_IsRevealed || m_MonsterMine == null) return;
        
        // Update to show defeated state
        UpdateDefeatedDisplay(null);
        
        // Notify cell view to update its visuals (e.g., change background sprite)
        var cellView = m_StatsText.transform.parent.GetComponent<CellView>();
        if (cellView != null)
        {
            cellView.UpdateVisuals(m_IsRevealed);
        }
    }

    private void HandleEnraged(Vector2Int position)
    {
        if (!m_IsRevealed || m_MonsterMine == null || m_MonsterMine.IsDefeated) return;
        UpdateDamageDisplay();
    }

    private void UpdateDamageDisplay()
    {
        if (m_ValueText == null) return;
        
        m_ValueText.enabled = true;
        int damage = m_MonsterMine.CalculateDamage();
        m_ValueText.text = damage.ToString();
        m_ValueText.color = m_MonsterMine.IsEnraged ? m_Config.EnragedColor : m_Config.MonsterPowerColor;
        m_ValueText.fontStyle = m_MonsterMine.IsEnraged ? FontStyles.Bold : FontStyles.Normal;
        m_ValueText.transform.localPosition = m_Config.DamagePosition;
    }

    private void UpdateHPDisplay()
    {
        if (m_StatsText == null) return;
        
        m_StatsText.enabled = true;
        m_StatsText.text = $"{m_MonsterMine.CurrentHp}/{m_MonsterMine.MaxHp}";
        m_StatsText.color = Color.Lerp(m_Config.HPLowColor, m_Config.HPHighColor, m_MonsterMine.HpPercentage);
        m_StatsText.transform.localPosition = m_Config.HPPosition;
    }

    private void UpdateMineValueDisplay(MineData mineData)
    {
        if (m_MineValueText == null || mineData == null) return;
        
        // Only show mine value if it's greater than 0
        if (mineData.Value > 0)
        {
            m_MineValueText.enabled = true;
            m_MineValueText.text = mineData.Value.ToString();
            m_MineValueText.color = mineData.MineValueColor; // Use mine-specific color from MineData
            m_MineValueText.transform.localPosition = m_Config.MonsterMineValuePosition;
        }
        else
        {
            m_MineValueText.enabled = false;
        }
    }

    private void UpdateDefeatedDisplay(MineData mineData)
    {
        // Change the display for a defeated monster
        MineData data = mineData ?? m_MonsterMine.GetMineData();
        
        // Update HP display to show 0/MaxHP
        if (m_StatsText != null)
        {
            m_StatsText.enabled = true;
            m_StatsText.text = $"0/{m_MonsterMine.MaxHp}";
            m_StatsText.color = m_Config.HPLowColor;
        }
        
        // Show appropriate text based on whether monster is collectable yet
        if (m_ValueText != null)
        {
            m_ValueText.enabled = true;
            
            // Different text based on whether the monster is collectable
            if (m_MonsterMine.IsCollectable)
            {
                m_ValueText.text = "Collect!";
                m_ValueText.color = Color.yellow;
            }
            else
            {
                m_ValueText.text = "Defeated";
                m_ValueText.color = Color.white;
            }
            
            m_ValueText.fontStyle = FontStyles.Bold;
        }
        
        // Always show the rewards (mine value) for defeated monsters
        if (m_MineValueText != null && data != null && data.Value > 0)
        {
            m_MineValueText.enabled = true;
            m_MineValueText.text = $"+{data.Value}";
            m_MineValueText.color = data.MineValueColor;
            m_MineValueText.fontStyle = FontStyles.Bold;
        }
    }

    public void CleanupDisplay()
    {
        // Unsubscribe from events
        if (m_MonsterMine != null)
        {
            m_MonsterMine.OnHpChanged -= HandleHpChanged;
            m_MonsterMine.OnEnraged -= HandleEnraged;
            m_MonsterMine.OnDefeated -= HandleDefeated;
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
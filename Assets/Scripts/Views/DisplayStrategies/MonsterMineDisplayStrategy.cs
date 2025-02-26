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
        m_StatsText = CreateTextComponent(cellObject, "MonsterStats", m_Config.HPPosition);
        
        // Create mine value text component
        m_MineValueText = CreateTextComponent(cellObject, "MineValue", m_Config.MonsterMineValuePosition);
        
        // Position damage text
        m_ValueText.transform.localPosition = m_Config.DamagePosition;
    }
    
    private TextMeshPro CreateTextComponent(GameObject parent, string name, Vector3 position)
    {
        var gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent.transform, false);
        
        var textComponent = gameObject.AddComponent<TextMeshPro>();
        textComponent.fontSize = m_ValueText.fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.sortingOrder = m_ValueText.sortingOrder;
        textComponent.transform.localPosition = position;
        textComponent.enabled = false;
        
        return textComponent;
    }

    public void UpdateDisplay(IMine mine, MineData mineData, bool isRevealed)
    {
        UnsubscribeFromEvents();
        m_IsRevealed = isRevealed;
        
        if (!isRevealed)
        {
            SetTextComponentsEnabled(false);
            return;
        }

        m_MonsterMine = mine as MonsterMine;
        if (m_MonsterMine == null) return;

        SubscribeToEvents();
        UpdateTextPositions();
        
        // Update display based on monster state
        if (m_MonsterMine.IsDefeated)
        {
            UpdateDefeatedDisplay(mineData);
        }
        else
        {
            UpdateActiveMonsterDisplay(mineData);
        }
    }

    private void UpdateActiveMonsterDisplay(MineData mineData)
    {
        UpdateDamageDisplay();
        UpdateHPDisplay();
        UpdateMineValueDisplay(mineData);
    }

    private void SetTextComponentsEnabled(bool enabled)
    {
        if (m_ValueText != null) m_ValueText.enabled = enabled;
        if (m_StatsText != null) m_StatsText.enabled = enabled;
        if (m_MineValueText != null) m_MineValueText.enabled = enabled;
    }

    private void SubscribeToEvents()
    {
        m_MonsterMine.OnHpChanged += HandleHpChanged;
        m_MonsterMine.OnEnraged += HandleEnraged;
        m_MonsterMine.OnDefeated += HandleDefeated;
    }

    private void UnsubscribeFromEvents()
    {
        if (m_MonsterMine != null)
        {
            m_MonsterMine.OnHpChanged -= HandleHpChanged;
            m_MonsterMine.OnEnraged -= HandleEnraged;
            m_MonsterMine.OnDefeated -= HandleDefeated;
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
        if (!ShouldHandleEvent()) return;
        
        if (m_MonsterMine.IsDefeated)
        {
            UpdateDefeatedDisplay(null);
            NotifyCellViewToUpdate();
        }
        else
        {
            UpdateHPDisplay();
        }
    }

    private void HandleDefeated(Vector2Int position)
    {
        if (!ShouldHandleEvent()) return;
        
        UpdateDefeatedDisplay(null);
        NotifyCellViewToUpdate();
    }

    private void HandleEnraged(Vector2Int position)
    {
        if (!ShouldHandleEvent() || m_MonsterMine.IsDefeated) return;
        UpdateDamageDisplay();
    }
    
    private bool ShouldHandleEvent()
    {
        return m_IsRevealed && m_MonsterMine != null;
    }
    
    private void NotifyCellViewToUpdate()
    {
        var cellView = m_StatsText?.transform.parent.GetComponent<CellView>();
        cellView?.UpdateVisuals(m_IsRevealed);
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
        
        m_MineValueText.enabled = mineData.Value > 0;
        
        if (mineData.Value > 0)
        {
            m_MineValueText.text = mineData.Value.ToString();
            m_MineValueText.color = mineData.MineValueColor;
            m_MineValueText.transform.localPosition = m_Config.MonsterMineValuePosition;
        }
    }

    private void UpdateDefeatedDisplay(MineData mineData)
    {
        MineData data = mineData ?? m_MonsterMine.GetMineData();
        
        // Update HP display to show 0/MaxHP
        UpdateDefeatedHPDisplay();
        
        // Show appropriate text based on whether monster is collectable
        UpdateDefeatedStatusDisplay();
        
        // Always show the rewards (mine value) for defeated monsters
        UpdateDefeatedValueDisplay(data);
    }
    
    private void UpdateDefeatedHPDisplay()
    {
        if (m_StatsText == null) return;
        
        m_StatsText.enabled = true;
        m_StatsText.text = $"0/{m_MonsterMine.MaxHp}";
        m_StatsText.color = m_Config.HPLowColor;
    }
    
    private void UpdateDefeatedStatusDisplay()
    {
        if (m_ValueText == null) return;
        
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
    
    private void UpdateDefeatedValueDisplay(MineData data)
    {
        if (m_MineValueText == null || data == null || data.Value <= 0) return;
        
        m_MineValueText.enabled = true;
        m_MineValueText.text = $"+{data.Value}";
        m_MineValueText.color = data.MineValueColor;
        m_MineValueText.fontStyle = FontStyles.Bold;
    }

    public void CleanupDisplay()
    {
        UnsubscribeFromEvents();
        m_MonsterMine = null;

        if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
        }
        
        DestroyTextComponent(m_StatsText);
        DestroyTextComponent(m_MineValueText);
    }
    
    private void DestroyTextComponent(TextMeshPro textComponent)
    {
        if (textComponent != null)
        {
            Object.Destroy(textComponent.gameObject);
        }
    }
} 
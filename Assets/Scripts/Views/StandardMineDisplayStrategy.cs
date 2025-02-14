using UnityEngine;
using TMPro;

public class StandardMineDisplayStrategy : IMineDisplayStrategy
{
    private TextMeshPro m_ValueText;
    private MineDisplayConfig m_Config;

    public void SetupDisplay(GameObject cellObject, TextMeshPro valueText)
    {
        m_ValueText = valueText;
        m_Config = cellObject.GetComponent<CellView>().DisplayConfig;
        
        // Position value text
        if (m_ValueText != null)
        {
            m_ValueText.transform.localPosition = m_Config.StandardMineValuePosition;
        }
    }

    public void UpdateDisplay(IMine mine, MineData mineData, bool isRevealed)
    {
        if (!isRevealed || m_ValueText == null)
        {
            if (m_ValueText != null)
            {
                m_ValueText.enabled = false;
            }
            return;
        }

        // Show mine value with mine-specific color
        m_ValueText.enabled = mineData.Value > 0;
        m_ValueText.text = mineData.Value > 0 ? mineData.Value.ToString() : "";
        m_ValueText.color = mineData.MineValueColor; // Use mine-specific color from MineData
        m_ValueText.transform.localPosition = m_Config.StandardMineValuePosition;
    }

    public void CleanupDisplay()
    {
        if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
        }
    }
} 
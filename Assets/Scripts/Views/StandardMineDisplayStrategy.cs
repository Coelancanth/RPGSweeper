using UnityEngine;
using TMPro;

public class StandardMineDisplayStrategy : IMineDisplayStrategy
{
    private TextMeshPro m_ValueText;
    private Color m_MineValueColor = Color.white;
    private int m_RawValue;

    public void SetupDisplay(GameObject cellObject, TextMeshPro valueText)
    {
        m_ValueText = valueText;
    }

    public void UpdateDisplay(IMine mine, MineData mineData, bool isRevealed)
    {
        if (!isRevealed)
        {
            m_ValueText.enabled = false;
            return;
        }

        m_RawValue = mineData.Value;
        m_MineValueColor = mineData.MineValueColor;

        m_ValueText.enabled = m_RawValue > 0;
        m_ValueText.text = m_RawValue > 0 ? m_RawValue.ToString() : "";
        m_ValueText.color = m_MineValueColor;
    }

    public void CleanupDisplay()
    {
        if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
        }
    }
} 
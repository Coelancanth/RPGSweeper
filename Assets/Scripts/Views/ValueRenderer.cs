using UnityEngine;
using TMPro;

public class ValueRenderer : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_ValueText;
    [SerializeField] private int m_ValueSortingOrder = 2;

    public void Initialize()
    {
        if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
            m_ValueText.sortingOrder = m_ValueSortingOrder;
        }
    }

    public void UpdateValue(int value, Color color, Vector3 position)
    {
        if (m_ValueText == null) return;

        if (value == 0)
        {
            m_ValueText.enabled = false;
            return;
        }

        m_ValueText.enabled = true;
        m_ValueText.text = value == -1 ? "?" : value.ToString();
        m_ValueText.color = color;
        m_ValueText.transform.localPosition = position;
    }
} 
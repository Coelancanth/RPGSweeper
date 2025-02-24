using UnityEngine;

public class CellConfigManager : MonoBehaviour
{
    [SerializeField] private MineDisplayConfig m_DisplayConfig;
    
    public event System.Action OnConfigChanged;
    
    private void OnEnable()
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged += HandleConfigChanged;
        }
    }

    private void OnDisable()
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged -= HandleConfigChanged;
        }
    }

    private void HandleConfigChanged()
    {
        OnConfigChanged?.Invoke();
    }

    public void UpdateConfig(MineDisplayConfig newConfig)
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged -= HandleConfigChanged;
        }

        m_DisplayConfig = newConfig;
        
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged += HandleConfigChanged;
        }

        HandleConfigChanged();
    }

    public Vector3 GetMineOffset() => m_DisplayConfig?.MineOffset ?? Vector3.zero;
    public Vector3 GetEmptyCellValuePosition() => m_DisplayConfig?.EmptyCellValuePosition ?? Vector3.zero;
    public Color GetDefaultValueColor() => m_DisplayConfig?.DefaultValueColor ?? Color.white;
} 
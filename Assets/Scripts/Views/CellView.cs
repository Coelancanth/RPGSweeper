using UnityEngine;

public class CellView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer m_BackgroundRenderer;
    [SerializeField] private SpriteRenderer m_MineRenderer;
    
    [Header("Colors")]
    [SerializeField] private Color m_HiddenColor = Color.gray;
    [SerializeField] private Color m_RevealedColor = Color.white;
    
    private Vector2Int m_Position;
    private bool m_IsRevealed;
    private Color m_OriginalBackgroundColor;
    private Sprite m_MineSprite;

    public Vector2Int GridPosition => m_Position;

    private void Awake()
    {
        // Hide mine sprite initially
        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = false;
        }

        if (m_BackgroundRenderer != null)
        {
            m_OriginalBackgroundColor = m_BackgroundRenderer.color;
        }
    }

    public void Initialize(Vector2Int position)
    {
        m_Position = position;
        m_IsRevealed = false;
        m_MineSprite = null;
        UpdateVisuals(false);
    }

    public void UpdateVisuals(bool revealed)
    {
        m_IsRevealed = revealed;
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.color = revealed ? m_RevealedColor : m_HiddenColor;
            m_OriginalBackgroundColor = m_BackgroundRenderer.color;
        }

        // Show mine sprite if it exists and cell is revealed
        if (m_MineRenderer != null && m_MineSprite != null)
        {
            m_MineRenderer.enabled = revealed;
        }
    }

    public void ShowMineSprite(Sprite mineSprite)
    {
        if (m_MineRenderer != null && mineSprite != null)
        {
            m_MineSprite = mineSprite;
            m_MineRenderer.sprite = mineSprite;
            m_MineRenderer.sortingOrder = 1; // Ensure mine appears above background
            m_MineRenderer.enabled = m_IsRevealed; // Only show if cell is revealed
        }
    }

    public void ShowDebugHighlight(Color highlightColor)
    {
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.color = highlightColor;
        }
    }

    public void HideDebugHighlight()
    {
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.color = m_IsRevealed ? m_RevealedColor : m_HiddenColor;
        }
    }

    public void ApplyEffect()
    {
        // Add visual effect (e.g., particle system)
        Debug.Log($"Effect applied at position {m_Position}");
    }

    private void OnMouseDown()
    {
        if (!m_IsRevealed)
        {
            GameEvents.RaiseCellRevealed(m_Position);
        }
    }
} 
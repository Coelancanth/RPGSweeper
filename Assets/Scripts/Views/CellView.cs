using UnityEngine;

public class CellView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer m_BackgroundRenderer;
    [SerializeField] private SpriteRenderer m_MineRenderer;
    
    [Header("Colors")]
    [SerializeField] private Color m_HiddenColor = Color.gray;
    [SerializeField] private Color m_RevealedColor = Color.white;

    [Header("Mine Sprite Settings")]
    [SerializeField] private float m_MineScale = 0.8f; // Scale relative to cell size
    [SerializeField] private Vector3 m_MineOffset = new Vector3(0f, 0f, -0.1f); // Offset to appear above background
    
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
            m_MineRenderer.transform.localPosition = m_MineOffset;
            SetMineScale(m_MineScale);
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
            SetMineScale(m_MineScale);
        }
    }

    private void SetMineScale(float scale)
    {
        if (m_MineRenderer != null)
        {
            // Calculate the scale based on the cell size and desired relative scale
            float cellSize = transform.localScale.x;
            float targetScale = cellSize * scale;
            
            // If we have a sprite, adjust scale based on sprite size to maintain proportions
            if (m_MineRenderer.sprite != null)
            {
                float spriteSize = m_MineRenderer.sprite.bounds.size.x;
                float scaleMultiplier = targetScale / spriteSize;
                m_MineRenderer.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1f);
            }
            else
            {
                m_MineRenderer.transform.localScale = new Vector3(targetScale, targetScale, 1f);
            }
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
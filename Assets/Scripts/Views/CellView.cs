using UnityEngine;
using TMPro;

public class CellView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer m_BackgroundRenderer;
    [SerializeField] private SpriteRenderer m_MineRenderer;
    [SerializeField] private TextMeshPro m_ValueText;
    
    [Header("Sprites")]
    [SerializeField] private Sprite m_HiddenSprite;
    [SerializeField] private Sprite m_RevealedEmptySprite;
    [SerializeField] private Sprite m_RevealedMineSprite;

    [Header("Visual Settings")]
    [SerializeField] private float m_CellSize = 1f;
    [SerializeField] private float m_MineScale = 0.8f;
    [SerializeField] private Vector3 m_MineOffset = new Vector3(0f, 0f, -0.1f);
    [SerializeField] private int m_BackgroundSortingOrder = 0;
    [SerializeField] private int m_MineSortingOrder = 1;
    
    private Vector2Int m_Position;
    private bool m_IsRevealed;
    private Sprite m_CurrentMineSprite;
    private bool m_HasMine;

    // Public properties
    public Vector2Int GridPosition => m_Position;
    public bool IsRevealed => m_IsRevealed;
    public bool HasMine => m_HasMine;
    
    // Properties needed by MineDebugger
    public SpriteRenderer BackgroundRenderer => m_BackgroundRenderer;
    public SpriteRenderer MineRenderer => m_MineRenderer;

    private void Awake()
    {
        SetupRenderers();
        ResetCell();
    }

    private void SetupRenderers()
    {
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.enabled = true;
            m_BackgroundRenderer.sortingOrder = m_BackgroundSortingOrder;
            SetSpriteScale(m_BackgroundRenderer, m_CellSize);
        }

        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = false;
            m_MineRenderer.sortingOrder = m_MineSortingOrder;
            m_MineRenderer.transform.localPosition = m_MineOffset;
        }
    }

    private void SetSpriteScale(SpriteRenderer renderer, float targetWorldSize)
    {
        if (renderer.sprite != null)
        {
            float pixelsPerUnit = renderer.sprite.pixelsPerUnit;
            float spriteSize = renderer.sprite.rect.width / pixelsPerUnit;
            float scale = targetWorldSize / spriteSize;
            renderer.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    public void Initialize(Vector2Int position)
    {
        m_Position = position;
        ResetCell();
    }

    private void ResetCell()
    {
        m_IsRevealed = false;
        m_HasMine = false;
        m_CurrentMineSprite = null;
        UpdateVisuals();
    }

    public void UpdateVisuals(bool revealed)
    {
        if (m_IsRevealed == revealed) return;
        m_IsRevealed = revealed;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Update background sprite
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.enabled = true;
            if (!m_IsRevealed)
            {
                m_BackgroundRenderer.sprite = m_HiddenSprite;
            }
            else if (m_HasMine)
            {
                m_BackgroundRenderer.sprite = m_RevealedMineSprite;
            }
            else
            {
                m_BackgroundRenderer.sprite = m_RevealedEmptySprite;
            }
        }

        // Update mine sprite
        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = m_IsRevealed && m_HasMine && m_CurrentMineSprite != null;
        }
    }

    public void ShowMineSprite(Sprite mineSprite)
    {
        if (m_MineRenderer == null || mineSprite == null) return;

        m_HasMine = true;
        m_CurrentMineSprite = mineSprite;
        m_MineRenderer.sprite = mineSprite;
        
        float targetMineSize = m_CellSize * m_MineScale;
        SetSpriteScale(m_MineRenderer, targetMineSize);

        UpdateVisuals();
    }

    public void HandleMineRemoval()
    {
        if (!m_HasMine || !m_IsRevealed) return;

        m_HasMine = false;
        m_CurrentMineSprite = null;
        UpdateVisuals();
    }

    public void SetValue(int value)
    {
        if (m_ValueText != null)
        {
            m_ValueText.text = value > 0 ? value.ToString() : "";
            m_ValueText.enabled = value > 0;
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
            m_BackgroundRenderer.color = Color.white;
            UpdateVisuals();
        }
    }

    private void OnMouseDown()
    {
        if (!m_IsRevealed)
        {
            GameEvents.RaiseCellRevealed(m_Position);
        }
        else if (m_HasMine)
        {
            GameEvents.RaiseMineRemovalAttempted(m_Position);
        }
    }

    public void ApplyEffect()
    {
        Debug.Log($"Effect applied at position {m_Position}");
    }
} 
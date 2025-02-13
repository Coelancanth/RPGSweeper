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
    [SerializeField] private Vector3 m_MineOffset = new Vector3(0f, 0.2f, -0.1f);
    [SerializeField] private Vector3 m_ValueOffset = new Vector3(0f, -0.2f, -0.1f);
    [SerializeField] private int m_BackgroundSortingOrder = 0;
    [SerializeField] private int m_MineSortingOrder = 1;
    [SerializeField] private int m_ValueSortingOrder = 2;
    
    private Vector2Int m_Position;
    private bool m_IsRevealed;
    private Sprite m_CurrentMineSprite;
    private bool m_HasMine;
    private int m_CurrentValue;
    private Color m_CurrentValueColor = Color.white;
    private int m_RawValue; // Store the unmodified value from MineData
    private Color m_MineValueColor = Color.white; // Store the mine value color

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

        if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
            m_ValueText.sortingOrder = m_ValueSortingOrder;
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
        m_CurrentValue = 0;
        UpdateVisuals();
    }

    public void UpdateVisuals(bool revealed)
    {
        if (m_IsRevealed == revealed) return;
        m_IsRevealed = revealed;
        UpdateVisuals();
    }

    private void UpdateValueTextPosition()
    {
        if (m_ValueText != null)
        {
            // If has mine with non-zero value, position below mine sprite, otherwise center
            m_ValueText.transform.localPosition = (m_HasMine && m_RawValue > 0) ? m_ValueOffset : Vector3.zero;
        }
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
                if (m_ValueText != null)
                {
                    m_ValueText.enabled = false;
                }
            }
            else if (m_HasMine)
            {
                m_BackgroundRenderer.sprite = m_RevealedMineSprite;
                if (m_ValueText != null)
                {
                    // For mine cells, always show raw value in white
                    m_ValueText.enabled = m_RawValue > 0;
                    m_ValueText.text = m_RawValue > 0 ? m_RawValue.ToString() : "";
                    m_ValueText.color = Color.white;
                }
            }
            else
            {
                m_BackgroundRenderer.sprite = m_RevealedEmptySprite;
                if (m_ValueText != null)
                {
                    if (m_CurrentValue == -1)
                    {
                        m_ValueText.enabled = true;
                        m_ValueText.text = "?";
                        m_ValueText.color = m_CurrentValueColor;
                    }
                    else
                    {
                        m_ValueText.enabled = m_CurrentValue > 0;
                        m_ValueText.text = m_CurrentValue > 0 ? m_CurrentValue.ToString() : "";
                        m_ValueText.color = m_CurrentValueColor;
                    }
                }
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

        // Center mine sprite if value is 0
        m_MineRenderer.transform.localPosition = m_RawValue > 0 ? m_MineOffset : Vector3.zero;

        UpdateValueTextPosition();
        UpdateVisuals();
    }

    public void HandleMineRemoval()
    {
        if (!m_HasMine || !m_IsRevealed) return;

        m_HasMine = false;
        m_CurrentMineSprite = null;
        UpdateValueTextPosition();
        UpdateVisuals();
    }

    public void SetValue(int value)
    {
        SetValue(value, Color.white);
    }

    public void SetValue(int value, Color color)
    {
        m_CurrentValue = value;
        m_CurrentValueColor = color;
        if (m_ValueText != null)
        {
            // Show value if the cell is revealed
            if (m_IsRevealed)
            {
                if (m_HasMine)
                {
                    // For mine cells, show raw value in mine value color
                    m_ValueText.enabled = m_RawValue > 0;
                    m_ValueText.text = m_RawValue > 0 ? m_RawValue.ToString() : "";
                    m_ValueText.color = m_MineValueColor;
                }
                else if (value == -1)
                {
                    m_ValueText.enabled = true;
                    m_ValueText.text = "?";
                    m_ValueText.color = color;
                }
                else
                {
                    m_ValueText.enabled = value > 0;
                    m_ValueText.text = value > 0 ? value.ToString() : "";
                    m_ValueText.color = color;
                }
            }
            else
            {
                m_ValueText.enabled = false;
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

    public void SetRawValue(int value, Color mineValueColor)
    {
        m_RawValue = value;
        m_MineValueColor = mineValueColor;
        if (m_HasMine)
        {
            // Update mine sprite position if already placed
            if (m_MineRenderer != null)
            {
                m_MineRenderer.transform.localPosition = value > 0 ? m_MineOffset : Vector3.zero;
            }
            UpdateVisuals();
        }
    }

    // Keep the old method for backward compatibility
    public void SetRawValue(int value)
    {
        SetRawValue(value, Color.white);
    }
} 
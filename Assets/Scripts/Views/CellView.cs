using UnityEngine;
using RPGMinesweeper.Views.States;
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
    private ICellState m_CurrentState;
    private Sprite m_CurrentMineSprite;
    private bool m_HasMine;

    // Properties for state classes
    public SpriteRenderer BackgroundRenderer => m_BackgroundRenderer;
    public SpriteRenderer MineRenderer => m_MineRenderer;
    public Sprite HiddenSprite => m_HiddenSprite;
    public Sprite RevealedEmptySprite => m_RevealedEmptySprite;
    public Sprite RevealedMineSprite => m_RevealedMineSprite;
    public Sprite CurrentMineSprite => m_CurrentMineSprite;

    public Vector2Int GridPosition => m_Position;

    private void Awake()
    {
        SetupRenderers();
        SetState(HiddenCellState.Instance);
    }

    private void SetupRenderers()
    {
        if (m_BackgroundRenderer != null)
        {
            // Set background sprite renderer properties
            m_BackgroundRenderer.enabled = true;
            m_BackgroundRenderer.sortingOrder = m_BackgroundSortingOrder;
            SetSpriteScale(m_BackgroundRenderer, m_CellSize);
        }

        if (m_MineRenderer != null)
        {
            // Set mine sprite renderer properties
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
        m_IsRevealed = false;
        m_CurrentMineSprite = null;
        m_HasMine = false;
        SetState(HiddenCellState.Instance);
    }

    public void UpdateVisuals(bool revealed)
    {
        // If already in the desired state, do nothing
        if (m_IsRevealed == revealed)
        {
            return;
        }

        m_IsRevealed = revealed;
        
        if (revealed)
        {
            if (m_HasMine)
            {
                SetState(RevealedMineCellState.Instance);
            }
            else
            {
                SetState(RevealedEmptyCellState.Instance);
            }
        }
        else
        {
            SetState(HiddenCellState.Instance);
        }
    }

    public void ShowMineSprite(Sprite mineSprite)
    {
        if (m_MineRenderer != null && mineSprite != null)
        {
            m_HasMine = true;
            m_CurrentMineSprite = mineSprite;
            m_MineRenderer.sprite = mineSprite;
            
            // Calculate mine scale
            float targetMineSize = m_CellSize * m_MineScale;
            SetSpriteScale(m_MineRenderer, targetMineSize);

            // Show mine sprite if cell is revealed
            m_MineRenderer.enabled = m_IsRevealed;
            
            // Update state if revealed
            if (m_IsRevealed)
            {
                SetState(RevealedMineCellState.Instance);
            }
        }
    }

    private void SetState(ICellState newState)
    {
        m_CurrentState = newState;
        m_CurrentState.Enter(this);
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
            m_CurrentState.UpdateVisuals(this);
        }
    }

    public void ApplyEffect()
    {
        Debug.Log($"Effect applied at position {m_Position}");
    }

    private void OnMouseDown()
    {
        if (!m_IsRevealed)
        {
            GameEvents.RaiseCellRevealed(m_Position);
        }
    }

    public void SetValue(int value)
    {
        if (m_ValueText != null)
        {
            m_ValueText.text = value > 0 ? value.ToString() : "";
            m_ValueText.enabled = value > 0;
        }
    }
} 
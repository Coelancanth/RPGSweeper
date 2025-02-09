using UnityEngine;
using RPGMinesweeper.Core.Mines;

public class CellView : MonoBehaviour
{
    #region Serialized Fields
    [Header("Child Components")]
    [Tooltip("SpriteRenderer for the cell's frame")]
    [SerializeField] private SpriteRenderer m_FrameRenderer;
    
    [Tooltip("SpriteRenderer for the cell's background")]
    [SerializeField] private SpriteRenderer m_BackgroundRenderer;
    
    [Tooltip("SpriteRenderer for displaying the mine")]
    [SerializeField] private SpriteRenderer m_MineRenderer;

    [Tooltip("TextMeshPro component for displaying the cell value")]
    [SerializeField] private TMPro.TextMeshPro m_ValueText;

    [Header("Visual Settings")]
    [Tooltip("Color of the background when cell is hidden")]
    [SerializeField] private Color m_HiddenColor = Color.gray;
    
    [Tooltip("Color of the background when cell is revealed")]
    [SerializeField] private Color m_RevealedColor = Color.white;
    
    [Tooltip("Default sprite for the frame")]
    [SerializeField] private Sprite m_FrameSprite;
    
    [Tooltip("Default sprite for empty cells")]
    [SerializeField] private Sprite m_DefaultSprite;

    [Tooltip("Color for the number display")]
    [SerializeField] private Color m_NumberColor = Color.red;
    #endregion

    #region Private Fields
    private Vector2Int m_Position;
    private Sprite m_MineSprite;
    private bool m_HasMine;
    private MineManager m_MineManager;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        // Ensure proper layering
        if (m_FrameRenderer != null) m_FrameRenderer.sortingOrder = 2;
        if (m_MineRenderer != null) m_MineRenderer.sortingOrder = 1;
        if (m_BackgroundRenderer != null) m_BackgroundRenderer.sortingOrder = 0;
        if (m_ValueText != null) 
        {
            m_ValueText.sortingOrder = 3;
            m_ValueText.enabled = false;
        }

        m_MineManager = FindObjectOfType<MineManager>();
    }
    #endregion

    #region Public Methods
    public void Initialize(Vector2Int position)
    {
        m_Position = position;
        m_HasMine = false;
        m_MineSprite = null;
        
        // Set up initial visuals
        if (m_FrameRenderer != null) m_FrameRenderer.sprite = m_FrameSprite;
        if (m_MineRenderer != null) m_MineRenderer.sprite = null;
        if (m_ValueText != null) m_ValueText.enabled = false;
        
        UpdateVisuals(false);
    }

    public void SetMine(MineData mineData)
    {
        if (mineData != null)
        {
            m_HasMine = true;
            m_MineSprite = mineData.MineSprite;
        }
    }

    public void UpdateVisuals(bool revealed)
    {
        // Update background
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.color = revealed ? m_RevealedColor : m_HiddenColor;
        }
        
        // Update mine visibility
        if (m_MineRenderer != null)
        {
            if (revealed && m_HasMine && m_MineSprite != null)
            {
                m_MineRenderer.sprite = m_MineSprite;
                m_MineRenderer.enabled = true;
            }
            else
            {
                m_MineRenderer.sprite = null;
                m_MineRenderer.enabled = false;
            }
        }

        // Update value text
        if (m_ValueText != null && revealed && !m_HasMine)
        {
            int value = m_MineManager.CalculateCellValue(m_Position);
            if (value > 0)
            {
                m_ValueText.text = value.ToString();
                m_ValueText.color = m_NumberColor;
                m_ValueText.enabled = true;
            }
            else
            {
                m_ValueText.enabled = false;
            }
        }
        else if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
        }
    }

    public void ApplyEffect()
    {
        // Add visual effect (e.g., particle system)
        Debug.Log($"Effect applied at position {m_Position}");
    }

    #region Debug Methods
    public bool HasMine()
    {
        return m_HasMine;
    }

    public SpriteRenderer GetBackgroundRenderer()
    {
        return m_BackgroundRenderer;
    }
    #endregion
    #endregion

    #region Private Methods
    private void OnMouseDown()
    {
        GameEvents.RaiseCellRevealed(m_Position);
    }
    #endregion
} 
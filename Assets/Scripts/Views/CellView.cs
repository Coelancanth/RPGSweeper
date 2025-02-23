using UnityEngine;
using TMPro;
using RPGMinesweeper.Input;

public class CellView : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private SpriteRenderer m_BackgroundRenderer;
    [SerializeField] private SpriteRenderer m_MineRenderer;
    [SerializeField] private TextMeshPro m_ValueText;
    
    [Header("Sprites")]
    [SerializeField] private Sprite m_HiddenSprite;
    [SerializeField] private Sprite m_RevealedEmptySprite;
    [SerializeField] private Sprite m_RevealedMineSprite;
    [SerializeField] private Sprite m_DefeatedMonsterSprite;

    [Header("Visual Settings")]
    [SerializeField] private float m_CellSize = 1f;
    [SerializeField] private float m_MineScale = 0.8f;
    [SerializeField] private Vector3 m_MineOffset = new Vector3(0f, 0.2f, -0.1f);
    [SerializeField] private MineDisplayConfig m_DisplayConfig;
    [SerializeField] private int m_BackgroundSortingOrder = 0;
    [SerializeField] private int m_MineSortingOrder = 1;
    [SerializeField] private int m_ValueSortingOrder = 2;
    
    [Header("Debug")]
    [SerializeField] private bool m_DebugMode = false;

    private Vector2Int m_Position;
    private bool m_IsRevealed;
    private bool m_IsFrozen;
    private Sprite m_CurrentMineSprite;
    private bool m_HasMine;
    private int m_CurrentValue;
    private Color m_CurrentValueColor = Color.white;
    private IMineDisplayStrategy m_DisplayStrategy;
    private IMine m_CurrentMine;
    private MineData m_CurrentMineData;

    // Public properties
    public Vector2Int GridPosition => m_Position;
    public bool IsRevealed => m_IsRevealed;
    public bool HasMine => m_HasMine;
    
    // Properties needed by MineDebugger and display strategies
    public SpriteRenderer BackgroundRenderer => m_BackgroundRenderer;
    public SpriteRenderer MineRenderer => m_MineRenderer;
    public MineDisplayConfig DisplayConfig => m_DisplayConfig;

    public bool CanInteract => !m_IsFrozen && !m_IsRevealed;
    public Vector2Int Position => m_Position;
    public bool IsFrozen => m_IsFrozen;

    private void Awake()
    {
        SetupRenderers();
        ResetCell();
    }

    private void OnEnable()
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged += HandleDisplayConfigChanged;
        }
    }

    private void OnDisable()
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged -= HandleDisplayConfigChanged;
        }
    }

    private void OnDestroy()
    {
        m_DisplayStrategy?.CleanupDisplay();
    }

    private void HandleDisplayConfigChanged()
    {
        if (m_MineRenderer != null)
        {
            m_MineRenderer.transform.localPosition = m_DisplayConfig.MineOffset;
            float targetMineSize = m_CellSize * m_MineScale;
            SetSpriteScale(m_MineRenderer, targetMineSize);
        }

        // Update display strategy with new config
        if (m_HasMine && m_IsRevealed && m_DisplayStrategy != null)
        {
            m_DisplayStrategy.UpdateDisplay(m_CurrentMine, m_CurrentMineData, true);
        }
        else if (m_ValueText != null)
        {
            UpdateValueTextPosition();
        }
    }

    private void UpdateValueTextPosition()
    {
        if (m_ValueText != null)
        {
            // For empty cells, use EmptyCellValuePosition
            if (!m_HasMine)
            {
                m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
            }
            // For mines, let the display strategy handle positioning
        }
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
        m_IsFrozen = false;
        m_HasMine = false;
        m_CurrentMineSprite = null;
        m_CurrentValue = 0;
        m_DisplayStrategy?.CleanupDisplay();
        m_DisplayStrategy = null;
        UpdateVisuals();
    }

    public void UpdateVisuals(bool revealed)
    {
        bool stateChanged = m_IsRevealed != revealed;
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
                if (m_ValueText != null)
                {
                    m_ValueText.enabled = false;
                }
            }
            else if (m_HasMine)
            {
                // Check if it's a monster mine and is defeated
                var monsterMine = m_CurrentMine as MonsterMine;
                if (monsterMine != null && monsterMine.IsCollectable)
                {
                    m_BackgroundRenderer.sprite = m_DefeatedMonsterSprite;
                }
                else
                {
                    m_BackgroundRenderer.sprite = m_RevealedMineSprite;
                }
                
                // Update display strategy without triggering another visual update
                m_DisplayStrategy?.UpdateDisplay(m_CurrentMine, m_CurrentMineData, m_IsRevealed);
            }
            else // Empty cell
            {
                m_BackgroundRenderer.sprite = m_RevealedEmptySprite;
                if (m_ValueText != null)
                {
                    if (m_CurrentValue == -1)
                    {
                        // Confusion effect
                        m_ValueText.enabled = true;
                        m_ValueText.text = "?";
                        m_ValueText.color = m_CurrentValueColor;
                        m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
                    }
                    else if (m_CurrentValue > 0)
                    {
                        // Show surrounding mine count
                        m_ValueText.enabled = true;
                        m_ValueText.text = m_CurrentValue.ToString();
                        m_ValueText.color = m_DisplayConfig.DefaultValueColor;
                        m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
                    }
                    else
                    {
                        m_ValueText.enabled = false;
                    }
                }
            }
        }

        // Update mine sprite - keep it visible for all revealed mines, including defeated monsters
        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = m_IsRevealed && m_HasMine && m_CurrentMineSprite != null;
        }
    }

    public void ShowMineSprite(Sprite mineSprite, IMine mine, MineData mineData)
    {
        if (m_MineRenderer == null || mineSprite == null) return;

        m_HasMine = true;
        m_CurrentMineSprite = mineSprite;
        m_MineRenderer.sprite = mineSprite;
        m_CurrentMine = mine;
        m_CurrentMineData = mineData;
        
        float targetMineSize = m_CellSize * m_MineScale;
        SetSpriteScale(m_MineRenderer, targetMineSize);
        m_MineRenderer.transform.localPosition = m_DisplayConfig.MineOffset;

        // Set up appropriate display strategy
        m_DisplayStrategy?.CleanupDisplay();
        m_DisplayStrategy = CreateDisplayStrategy(mine);
        m_DisplayStrategy?.SetupDisplay(gameObject, m_ValueText);

        // Let UpdateVisuals control the enabled state
        m_MineRenderer.enabled = false;
        UpdateVisuals();
    }

    private IMineDisplayStrategy CreateDisplayStrategy(IMine mine)
    {
        if (mine is MonsterMine)
        {
            return new MonsterMineDisplayStrategy();
        }
        return new StandardMineDisplayStrategy();
    }

    public void HandleMineRemoval()
    {
        if (!m_HasMine || !m_IsRevealed) return;

        m_HasMine = false;
        m_CurrentMineSprite = null;
        m_DisplayStrategy?.CleanupDisplay();
        m_DisplayStrategy = null;
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
        UpdateVisuals();
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

    public void SetFrozen(bool frozen)
    {
        m_IsFrozen = frozen;
        if (m_BackgroundRenderer != null)
        {
            // Add a blue tint to indicate frozen state
            m_BackgroundRenderer.color = frozen ? new Color(0.8f, 0.8f, 1f) : Color.white;
        }
    }

    public void OnInteract()
    {
        if (CanInteract)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellView] Revealing cell at {m_Position}, CanInteract: {CanInteract}, IsFrozen: {m_IsFrozen}, IsRevealed: {m_IsRevealed}");
            }
            GameEvents.RaiseCellRevealed(m_Position);
        }
        else if (m_IsRevealed && m_HasMine)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellView] Interacting with revealed mine at {m_Position}, IsCollectable: {(m_CurrentMine as MonsterMine)?.IsCollectable}");
            }
            // For monster mines, process interaction on every click
            var monsterMine = m_CurrentMine as MonsterMine;
            if (monsterMine != null)
            {
                if (monsterMine.IsCollectable)
                {
                    GameEvents.RaiseMineRemovalAttempted(m_Position);
                }
                else
                {
                    var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                    if (player != null)
                    {
                        monsterMine.OnTrigger(player);
                    }
                }
            }
            else
            {
                GameEvents.RaiseMineRemovalAttempted(m_Position);
            }
        }
        else
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellView] Cannot interact with cell at {m_Position}, CanInteract: {CanInteract}, IsFrozen: {m_IsFrozen}, IsRevealed: {m_IsRevealed}");
            }
        }
    }

    public void ApplyEffect()
    {
        if (m_DebugMode)
        {
            Debug.Log($"Effect applied at position {m_Position}");
        }
    }

    public void UpdateDisplayConfig(MineDisplayConfig newConfig)
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged -= HandleDisplayConfigChanged;
        }

        m_DisplayConfig = newConfig;
        
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged += HandleDisplayConfigChanged;
        }

        HandleDisplayConfigChanged();
    }
} 
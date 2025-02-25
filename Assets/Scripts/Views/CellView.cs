using UnityEngine;
using TMPro;
using RPGMinesweeper.Input;
using RPGMinesweeper;

// Interface for cell state management
public interface ICellState
{
    Vector2Int Position { get; }
    bool IsRevealed { get; }
    bool IsFrozen { get; }
    bool HasMine { get; }
    int CurrentValue { get; }
    IMine CurrentMine { get; }
    MineData CurrentMineData { get; }
    
    void SetRevealed(bool revealed);
    void SetFrozen(bool frozen);
    void SetValue(int value, Color color);
    void SetMine(IMine mine, MineData mineData);
    void RemoveMine();
}

// Interface for cell visual management
public interface ICellVisual
{
    void UpdateVisuals();
    void ShowDebugHighlight(Color highlightColor);
    void HideDebugHighlight();
    void ApplyFrozenVisual(bool frozen);
}

// Class responsible for managing cell state
public class CellState : ICellState
{
    private Vector2Int m_Position;
    private bool m_IsRevealed;
    private bool m_IsFrozen;
    private bool m_HasMine;
    private int m_CurrentValue;
    private Color m_CurrentValueColor = Color.white;
    private IMine m_CurrentMine;
    private MineData m_CurrentMineData;
    
    public event System.Action OnStateChanged;
    
    public Vector2Int Position => m_Position;
    public bool IsRevealed => m_IsRevealed;
    public bool IsFrozen => m_IsFrozen;
    public bool HasMine => m_HasMine;
    public int CurrentValue => m_CurrentValue;
    public Color CurrentValueColor => m_CurrentValueColor;
    public IMine CurrentMine => m_CurrentMine;
    public MineData CurrentMineData => m_CurrentMineData;
    
    public CellState(Vector2Int position)
    {
        m_Position = position;
    }
    
    public void SetRevealed(bool revealed)
    {
        if (m_IsRevealed != revealed)
        {
            m_IsRevealed = revealed;
            OnStateChanged?.Invoke();
        }
    }
    
    public void SetFrozen(bool frozen)
    {
        if (m_IsFrozen != frozen)
        {
            m_IsFrozen = frozen;
            OnStateChanged?.Invoke();
        }
    }
    
    public void SetValue(int value, Color color)
    {
        m_CurrentValue = value;
        m_CurrentValueColor = color;
        OnStateChanged?.Invoke();
    }
    
    public void SetMine(IMine mine, MineData mineData)
    {
        m_HasMine = true;
        m_CurrentMine = mine;
        m_CurrentMineData = mineData;
        OnStateChanged?.Invoke();
    }
    
    public void RemoveMine()
    {
        if (m_HasMine)
        {
            m_HasMine = false;
            m_CurrentMine = null;
            m_CurrentMineData = null;
            OnStateChanged?.Invoke();
        }
    }
}

// Strategy factory for creating mine display strategies
public interface IMineDisplayStrategyFactory
{
    IMineDisplayStrategy CreateStrategy(IMine mine);
}

// Default implementation of mine display strategy factory
public class DefaultMineDisplayStrategyFactory : IMineDisplayStrategyFactory
{
    public IMineDisplayStrategy CreateStrategy(IMine mine)
    {
        if (mine is DisguisedMonsterMine)
        {
            return new DisguisedMonsterMineDisplayStrategy();
        }
        else if (mine is MonsterMine)
        {
            return new MonsterMineDisplayStrategy();
        }
        return new StandardMineDisplayStrategy();
    }
}

// Main CellView class focused on visualization and interaction
public class CellView : MonoBehaviour, IInteractable, ICellVisual
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

    private ICellState m_CellState;
    private IMineDisplayStrategy m_DisplayStrategy;
    private IMineDisplayStrategyFactory m_StrategyFactory;
    private Sprite m_CurrentMineSprite;
    
    // Public properties
    public Vector2Int GridPosition => m_CellState.Position;
    public bool CanInteract => !m_CellState.IsFrozen && !m_CellState.IsRevealed;
    public Vector2Int Position => m_CellState.Position;
    public bool IsRevealed => m_CellState.IsRevealed;
    public bool HasMine => m_CellState.HasMine;
    
    // Properties needed by MineDebugger and display strategies
    public SpriteRenderer BackgroundRenderer => m_BackgroundRenderer;
    public SpriteRenderer MineRenderer => m_MineRenderer;
    public MineDisplayConfig DisplayConfig => m_DisplayConfig;
    public bool IsFrozen => m_CellState.IsFrozen;

    private void Awake()
    {
        // Create dependencies with default implementations
        m_StrategyFactory = new DefaultMineDisplayStrategyFactory();
        m_CellState = new CellState(Vector2Int.zero);
        
        SetupRenderers();
        
        // Subscribe to state changes
        ((CellState)m_CellState).OnStateChanged += UpdateVisuals;
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
        
        // Unsubscribe from state changes to prevent memory leaks
        if (m_CellState is CellState cellState)
        {
            cellState.OnStateChanged -= UpdateVisuals;
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
        if (m_CellState.HasMine && m_CellState.IsRevealed && m_DisplayStrategy != null)
        {
            m_DisplayStrategy.UpdateDisplay(m_CellState.CurrentMine, m_CellState.CurrentMineData, true);
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
            if (!m_CellState.HasMine)
            {
                m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
            }
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
        // Clean up old state if any
        if (m_CellState is CellState cellState)
        {
            cellState.OnStateChanged -= UpdateVisuals;
        }
        
        // Create a new state with the specified position
        m_CellState = new CellState(position);
        ((CellState)m_CellState).OnStateChanged += UpdateVisuals;
        
        UpdateVisuals();
    }

    public void UpdateVisuals(bool revealed)
    {
        m_CellState.SetRevealed(revealed);
    }

    public void UpdateVisuals()
    {
        // Update background sprite
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.enabled = true;
            if (!m_CellState.IsRevealed)
            {
                m_BackgroundRenderer.sprite = m_HiddenSprite;
                if (m_ValueText != null)
                {
                    m_ValueText.enabled = false;
                }
                if (m_MineRenderer != null)
                {
                    m_MineRenderer.enabled = false;
                }
            }
            else if (m_CellState.HasMine)
            {
                // Check if it's a monster mine and is defeated
                var monsterMine = m_CellState.CurrentMine as MonsterMine;
                var disguisedMonsterMine = m_CellState.CurrentMine as DisguisedMonsterMine;
                bool isDefeatedMonster = (monsterMine != null && monsterMine.IsCollectable) || 
                                         (disguisedMonsterMine != null && disguisedMonsterMine.IsCollectable);
                
                m_BackgroundRenderer.sprite = isDefeatedMonster ? m_DefeatedMonsterSprite : m_RevealedMineSprite;
                
                // Update display strategy
                m_DisplayStrategy?.UpdateDisplay(m_CellState.CurrentMine, m_CellState.CurrentMineData, m_CellState.IsRevealed);
                
                // Show mine sprite
                if (m_MineRenderer != null && m_CurrentMineSprite != null)
                {
                    m_MineRenderer.enabled = true;
                }
            }
            else // Empty cell
            {
                m_BackgroundRenderer.sprite = m_RevealedEmptySprite;
                if (m_ValueText != null)
                {
                    if (m_CellState.CurrentValue == -1)
                    {
                        // Confusion effect
                        m_ValueText.enabled = true;
                        m_ValueText.text = "?";
                        m_ValueText.color = ((CellState)m_CellState).CurrentValueColor;
                        m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
                    }
                    else if (m_CellState.CurrentValue > 0)
                    {
                        // Show surrounding mine count
                        m_ValueText.enabled = true;
                        m_ValueText.text = m_CellState.CurrentValue.ToString();
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
    }

    public void ShowMineSprite(Sprite mineSprite, IMine mine, MineData mineData)
    {
        if (m_MineRenderer == null || mineSprite == null) return;

        m_CurrentMineSprite = mineSprite;
        m_MineRenderer.sprite = mineSprite;
        
        // Set up mine in the cell state
        m_CellState.SetMine(mine, mineData);
        
        float targetMineSize = m_CellSize * m_MineScale;
        SetSpriteScale(m_MineRenderer, targetMineSize);
        m_MineRenderer.transform.localPosition = m_DisplayConfig.MineOffset;

        // Set up appropriate display strategy
        m_DisplayStrategy?.CleanupDisplay();
        m_DisplayStrategy = m_StrategyFactory.CreateStrategy(mine);
        m_DisplayStrategy?.SetupDisplay(gameObject, m_ValueText);

        UpdateVisuals();
    }

    public void HandleMineRemoval()
    {
        if (!m_CellState.HasMine || !m_CellState.IsRevealed) return;

        m_CurrentMineSprite = null;
        m_CellState.RemoveMine();
        
        if (m_DisplayStrategy != null)
        {
            m_DisplayStrategy.CleanupDisplay();
            m_DisplayStrategy = null;
        }
        
        UpdateVisuals();
    }

    public void SetValue(int value)
    {
        m_CellState.SetValue(value, Color.white);
    }

    public void SetValue(int value, Color color)
    {
        m_CellState.SetValue(value, color);
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
        m_CellState.SetFrozen(frozen);
        ApplyFrozenVisual(frozen);
    }
    
    public void ApplyFrozenVisual(bool frozen)
    {
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
                Debug.Log($"[CellView] Revealing cell at {m_CellState.Position}, CanInteract: {CanInteract}, IsFrozen: {m_CellState.IsFrozen}, IsRevealed: {m_CellState.IsRevealed}");
            }
            GameEvents.RaiseCellRevealed(m_CellState.Position);
        }
        else if (m_CellState.IsRevealed && m_CellState.HasMine)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellView] Interacting with revealed mine at {m_CellState.Position}, Mine type: {m_CellState.CurrentMine?.GetType().Name}");
            }
            
            // Special handling for disguised monster mines
            var disguisedMonsterMine = m_CellState.CurrentMine as DisguisedMonsterMine;
            if (disguisedMonsterMine != null)
            {
                if (disguisedMonsterMine.IsDisguised)
                {
                    // Let the mine reveal itself and update visuals
                    var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                    if (player != null)
                    {
                        disguisedMonsterMine.OnTrigger(player);
                    }
                    
                    // Force update mine display after revealing
                    if (m_DisplayStrategy != null)
                    {
                        m_DisplayStrategy.UpdateDisplay(m_CellState.CurrentMine, m_CellState.CurrentMineData, true);
                        UpdateVisuals();
                    }
                    
                    return;
                }
                else if (disguisedMonsterMine.IsCollectable)
                {
                    GameEvents.RaiseMineRemovalAttempted(m_CellState.Position);
                    return;
                }
                else
                {
                    var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                    if (player != null)
                    {
                        disguisedMonsterMine.OnTrigger(player);
                    }
                    return;
                }
            }
            
            // For regular monster mines, process interaction on every click
            var monsterMine = m_CellState.CurrentMine as MonsterMine;
            if (monsterMine != null)
            {
                if (monsterMine.IsCollectable)
                {
                    GameEvents.RaiseMineRemovalAttempted(m_CellState.Position);
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
                GameEvents.RaiseMineRemovalAttempted(m_CellState.Position);
            }
        }
        else
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellView] Cannot interact with cell at {m_CellState.Position}, CanInteract: {CanInteract}, IsFrozen: {m_CellState.IsFrozen}, IsRevealed: {m_CellState.IsRevealed}");
            }
        }
    }

    public void ApplyEffect()
    {
        if (m_DebugMode)
        {
            Debug.Log($"Effect applied at position {m_CellState.Position}");
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
using UnityEngine;
using TMPro;
using RPGMinesweeper.Input;
using RPGMinesweeper;

// Class responsible for managing cell state
public class CellState : ICellData, ICellStateModifier
{
    private Vector2Int m_Position;
    private bool m_IsRevealed;
    private bool m_IsFrozen;
    private bool m_HasMine;
    private int m_CurrentValue;
    private Color m_CurrentValueColor = Color.white;
    private IMine m_CurrentMine;
    private MineData m_CurrentMineData;
    private CellMarkType m_MarkType = CellMarkType.None;
    
    public event System.Action OnStateChanged;
    
    public Vector2Int Position => m_Position;
    public bool IsRevealed => m_IsRevealed;
    public bool IsFrozen => m_IsFrozen;
    public bool HasMine => m_HasMine;
    public int CurrentValue => m_CurrentValue;
    public Color CurrentValueColor => m_CurrentValueColor;
    public IMine CurrentMine => m_CurrentMine;
    public MineData CurrentMineData => m_CurrentMineData;
    public CellMarkType MarkType => m_MarkType;
    
    public CellState(Vector2Int position)
    {
        m_Position = position;
    }
    
    public void SetRevealed(bool revealed)
    {
        if (m_IsRevealed != revealed)
        {
            m_IsRevealed = revealed;
            
            // Clear any marks when revealing a cell
            if (revealed && m_MarkType != CellMarkType.None)
            {
                m_MarkType = CellMarkType.None;
            }
            
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
    
    public void SetMarkType(CellMarkType markType)
    {
        if (m_MarkType != markType && !m_IsRevealed)
        {
            m_MarkType = markType;
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
public class CellView : MonoBehaviour, IInteractable, ICellVisualUpdater
{
    [Header("References")]
    [SerializeField] private SpriteRenderer m_BackgroundRenderer;
    [SerializeField] private SpriteRenderer m_MineRenderer;
    [SerializeField] private TextMeshPro m_ValueText;
    [SerializeField] private SpriteRenderer m_MarkRenderer;
    
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
    [SerializeField] private RPGMinesweeper.MarkPanelDisplayConfig m_MarkDisplayConfig;
    [SerializeField] private int m_BackgroundSortingOrder = 0;
    [SerializeField] private int m_MineSortingOrder = 1;
    [SerializeField] private int m_ValueSortingOrder = 2;
    [SerializeField] private int m_MarkSortingOrder = 3;
    
    [Header("Debug")]
    [SerializeField] private bool m_DebugMode = false;

    // Encapsulated components
    private CellState m_CellState;
    private CellVisualizer m_Visualizer;
    private CellInteractionHandler m_InteractionHandler;
    private IMineDisplayStrategyFactory m_StrategyFactory;
    
    // Public properties that implement IInteractable
    public Vector2Int GridPosition => m_CellState?.Position ?? Vector2Int.zero;
    public bool CanInteract => m_InteractionHandler?.CanInteract ?? false;
    public Vector2Int Position => m_CellState?.Position ?? Vector2Int.zero;
    public bool IsRevealed => m_CellState?.IsRevealed ?? false;
    public bool HasMine => m_CellState?.HasMine ?? false;
    
    // Properties needed by MineDebugger and display strategies
    public SpriteRenderer BackgroundRenderer => m_BackgroundRenderer;
    public SpriteRenderer MineRenderer => m_MineRenderer;
    public MineDisplayConfig DisplayConfig => m_DisplayConfig;
    public bool IsFrozen => m_CellState?.IsFrozen ?? false;

    private void Awake()
    {
        // Create dependencies with default implementations
        m_StrategyFactory = new DefaultMineDisplayStrategyFactory();
        m_CellState = new CellState(Vector2Int.zero);
        
        // Set up visualizer
        m_Visualizer = new CellVisualizer(
            m_BackgroundRenderer,
            m_MineRenderer,
            m_ValueText,
            m_MarkRenderer,
            m_HiddenSprite,
            m_RevealedEmptySprite,
            m_RevealedMineSprite,
            m_DefeatedMonsterSprite,
            m_DisplayConfig,
            m_MarkDisplayConfig,
            m_CellSize,
            m_MineScale,
            m_BackgroundSortingOrder,
            m_MineSortingOrder,
            m_ValueSortingOrder,
            m_MarkSortingOrder,
            m_DebugMode,
            m_CellState
        );
        
        // Set up handlers AFTER visualizer, so we can pass the visualizer for visual updates
        m_InteractionHandler = new CellInteractionHandler(m_CellState, m_DebugMode, this);
        
        // Subscribe to state changes
        m_CellState.OnStateChanged += HandleStateChanged;
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
        if (m_CellState != null)
        {
            m_CellState.OnStateChanged -= HandleStateChanged;
        }
    }

    private void HandleStateChanged()
    {
        // Update visualizer when state changes
        m_Visualizer?.UpdateVisuals();
    }

    private void HandleDisplayConfigChanged()
    {
        // Update visualizer when display config changes
        m_Visualizer?.UpdateDisplayConfig(m_DisplayConfig);
    }

    public void Initialize(Vector2Int position)
    {
        // Clean up previous state if it exists
        if (m_CellState != null)
        {
            m_CellState.OnStateChanged -= HandleStateChanged;
        }
        
        // Create new state
        m_CellState = new CellState(position);
        m_CellState.OnStateChanged += HandleStateChanged;
        
        // Re-initialize visualizer with new state
        m_Visualizer = new CellVisualizer(
            m_BackgroundRenderer,
            m_MineRenderer,
            m_ValueText,
            m_MarkRenderer,
            m_HiddenSprite,
            m_RevealedEmptySprite,
            m_RevealedMineSprite,
            m_DefeatedMonsterSprite,
            m_DisplayConfig,
            m_MarkDisplayConfig,
            m_CellSize,
            m_MineScale,
            m_BackgroundSortingOrder,
            m_MineSortingOrder,
            m_ValueSortingOrder,
            m_MarkSortingOrder,
            m_DebugMode,
            m_CellState
        );
        
        // Update dependencies with new state - pass this as visualizer 
        m_InteractionHandler = new CellInteractionHandler(m_CellState, m_DebugMode, this);
        
        // Force visual update
        m_Visualizer.UpdateVisuals();
    }

    public void UpdateVisuals(bool revealed)
    {
        m_CellState.SetRevealed(revealed);
    }

    public void UpdateVisuals()
    {
        m_Visualizer.UpdateVisuals();
    }

    public void ShowMineSprite(Sprite mineSprite, IMine mine, MineData mineData)
    {
        // Set state first
        m_CellState.SetMine(mine, mineData);

        // Update visualizer
        m_Visualizer.ShowMineSprite(mineSprite, mine, mineData, gameObject, m_StrategyFactory);
    }

    public void HandleMineRemoval()
    {
        // Update state
        m_CellState.RemoveMine();
        
        // Update visualizer
        m_Visualizer.HandleMineRemoval();
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
        m_Visualizer.ShowDebugHighlight(highlightColor);
    }

    public void HideDebugHighlight()
    {
        m_Visualizer.HideDebugHighlight();
    }

    public void SetFrozen(bool frozen)
    {
        m_CellState.SetFrozen(frozen);
        m_Visualizer.ApplyFrozenVisual(frozen);
    }
    
    public void ApplyFrozenVisual(bool frozen)
    {
        m_Visualizer.ApplyFrozenVisual(frozen);
    }

    public void OnInteract()
    {
        m_InteractionHandler.OnInteract();
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
        m_DisplayConfig = newConfig;
        m_Visualizer.UpdateDisplayConfig(newConfig);
    }

    public void SetMarkType(CellMarkType markType)
    {
        if (m_CellState != null)
        {
            // Add debug statements to check what's null
            if (m_DebugMode)
            {
                Debug.Log($"[CellView] SetMarkType called with markType: {markType}");
                Debug.Log($"[CellView] m_MarkDisplayConfig is {(m_MarkDisplayConfig == null ? "NULL" : "NOT NULL")}");
                
                if (m_MarkDisplayConfig != null)
                {
                    Debug.Log($"[CellView] CustomInputMarkSprite is {(m_MarkDisplayConfig.CustomInputMarkSprite == null ? "NULL" : "NOT NULL")}");
                    Debug.Log($"[CellView] FlagMarkSprite is {(m_MarkDisplayConfig.FlagMarkSprite == null ? "NULL" : "NOT NULL")}");
                    Debug.Log($"[CellView] QuestionMarkSprite is {(m_MarkDisplayConfig.QuestionMarkSprite == null ? "NULL" : "NOT NULL")}");
                    Debug.Log($"[CellView] NumbersMarkSprite is {(m_MarkDisplayConfig.NumbersMarkSprite == null ? "NULL" : "NOT NULL")}");
                }
            }
            
            m_CellState.SetMarkType(markType);
        }
    }
}
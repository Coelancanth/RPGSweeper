using UnityEngine;
using TMPro;
using RPGMinesweeper;

public class CellVisualizer : ICellVisualUpdater
{
    private SpriteRenderer m_BackgroundRenderer;
    private SpriteRenderer m_MineRenderer;
    private TextMeshPro m_ValueText;
    private SpriteRenderer m_MarkRenderer;
    
    private Sprite m_HiddenSprite;
    private Sprite m_RevealedEmptySprite;
    private Sprite m_RevealedMineSprite;
    private Sprite m_DefeatedMonsterSprite;
    
    private MineDisplayConfig m_DisplayConfig;
    private MarkPanelDisplayConfig m_MarkDisplayConfig;
    
    private float m_CellSize;
    private float m_MineScale;
    
    private ICellData m_CellData;
    private IMineDisplayStrategy m_DisplayStrategy;
    private Sprite m_CurrentMineSprite;
    
    private int m_BackgroundSortingOrder;
    private int m_MineSortingOrder;
    private int m_ValueSortingOrder;
    private int m_MarkSortingOrder;
    
    private bool m_DebugMode;

    public CellVisualizer(
        SpriteRenderer backgroundRenderer, 
        SpriteRenderer mineRenderer, 
        TextMeshPro valueText, 
        SpriteRenderer markRenderer,
        Sprite hiddenSprite,
        Sprite revealedEmptySprite,
        Sprite revealedMineSprite,
        Sprite defeatedMonsterSprite,
        MineDisplayConfig displayConfig,
        MarkPanelDisplayConfig markDisplayConfig,
        float cellSize,
        float mineScale,
        int backgroundSortingOrder,
        int mineSortingOrder,
        int valueSortingOrder,
        int markSortingOrder,
        bool debugMode,
        ICellData cellData)
    {
        m_BackgroundRenderer = backgroundRenderer;
        m_MineRenderer = mineRenderer;
        m_ValueText = valueText;
        m_MarkRenderer = markRenderer;
        
        m_HiddenSprite = hiddenSprite;
        m_RevealedEmptySprite = revealedEmptySprite;
        m_RevealedMineSprite = revealedMineSprite;
        m_DefeatedMonsterSprite = defeatedMonsterSprite;
        
        m_DisplayConfig = displayConfig;
        m_MarkDisplayConfig = markDisplayConfig;
        
        m_CellSize = cellSize;
        m_MineScale = mineScale;
        
        m_BackgroundSortingOrder = backgroundSortingOrder;
        m_MineSortingOrder = mineSortingOrder;
        m_ValueSortingOrder = valueSortingOrder;
        m_MarkSortingOrder = markSortingOrder;
        
        m_DebugMode = debugMode;
        m_CellData = cellData;
        
        SetupRenderers();
    }

    private void SetupRenderers()
    {
        // Setup background renderer
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.sprite = m_HiddenSprite;
            m_BackgroundRenderer.sortingOrder = m_BackgroundSortingOrder;
            SetSpriteScale(m_BackgroundRenderer, m_CellSize);
        }
        
        // Setup mine renderer
        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = false;
            m_MineRenderer.sortingOrder = m_MineSortingOrder;
            
            if (m_DisplayConfig != null)
            {
                m_MineRenderer.transform.localPosition = m_DisplayConfig.MineOffset;
            }
            
            float targetMineSize = m_CellSize * m_MineScale;
            SetSpriteScale(m_MineRenderer, targetMineSize);
        }
        
        // Setup value text
        if (m_ValueText != null)
        {
            m_ValueText.enabled = false;
            m_ValueText.sortingOrder = m_ValueSortingOrder;
        }
        
        // Setup mark renderer
        if (m_MarkRenderer != null)
        {
            m_MarkRenderer.enabled = false;
            m_MarkRenderer.sortingOrder = m_MarkSortingOrder;
            
            // Use the mark scale from config if available, otherwise fallback to default
            float markScale = (m_MarkDisplayConfig != null) ? m_MarkDisplayConfig.MarkScale : 0.7f;
            SetSpriteScale(m_MarkRenderer, m_CellSize * markScale);
            
            // Apply mark offset if config is available
            if (m_MarkDisplayConfig != null && m_MarkRenderer.transform != null)
            {
                m_MarkRenderer.transform.localPosition = new Vector3(
                    m_MarkDisplayConfig.MarkOffset.x,
                    m_MarkDisplayConfig.MarkOffset.y,
                    m_MarkRenderer.transform.localPosition.z
                );
            }
        }
    }

    private void SetSpriteScale(SpriteRenderer renderer, float targetWorldSize)
    {
        if (renderer != null && renderer.sprite != null)
        {
            float pixelsPerUnit = renderer.sprite.pixelsPerUnit;
            float spriteSize = renderer.sprite.rect.width / pixelsPerUnit;
            float scale = targetWorldSize / spriteSize;
            renderer.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    public void UpdateVisuals()
    {
        // Update background sprite
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.enabled = true;
            if (!m_CellData.IsRevealed)
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
                
                UpdateMarkRenderer();
            }
            else if (m_CellData.HasMine)
            {
                // Check if it's a monster mine and is defeated
                var monsterMine = m_CellData.CurrentMine as MonsterMine;
                var disguisedMonsterMine = m_CellData.CurrentMine as DisguisedMonsterMine;
                bool isDefeatedMonster = (monsterMine != null && monsterMine.IsDefeated) || 
                                         (disguisedMonsterMine != null && disguisedMonsterMine.IsDefeated);
                
                // Use the defeated monster sprite for defeated monsters
                m_BackgroundRenderer.sprite = isDefeatedMonster ? m_DefeatedMonsterSprite : m_RevealedMineSprite;
                
                // Update display strategy
                m_DisplayStrategy?.UpdateDisplay(m_CellData.CurrentMine, m_CellData.CurrentMineData, m_CellData.IsRevealed);
                
                // Show mine sprite
                if (m_MineRenderer != null && m_CurrentMineSprite != null)
                {
                    // For defeated monsters, flip the sprite horizontally
                    if (isDefeatedMonster)
                    {
                        m_MineRenderer.flipX = true;
                    }
                    else
                    {
                        m_MineRenderer.flipX = false;
                    }
                    
                    m_MineRenderer.enabled = true;
                }
                
                // Hide mark if revealed
                if (m_MarkRenderer != null)
                {
                    m_MarkRenderer.enabled = false;
                }
            }
            else // Empty cell
            {
                m_BackgroundRenderer.sprite = m_RevealedEmptySprite;
                if (m_ValueText != null)
                {
                    if (m_CellData.CurrentValue == -1)
                    {
                        // Confusion effect
                        m_ValueText.enabled = true;
                        m_ValueText.text = "?";
                        m_ValueText.color = m_CellData.CurrentValueColor;
                        if (m_DisplayConfig != null)
                        {
                            m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
                        }
                    }
                    else if (m_CellData.CurrentValue > 0)
                    {
                        m_ValueText.enabled = true;
                        m_ValueText.text = m_CellData.CurrentValue.ToString();
                        m_ValueText.color = m_CellData.CurrentValueColor;
                        if (m_DisplayConfig != null)
                        {
                            m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
                        }
                    }
                    else
                    {
                        m_ValueText.enabled = false;
                    }
                }
                
                // Hide mark if revealed
                if (m_MarkRenderer != null)
                {
                    m_MarkRenderer.enabled = false;
                }
            }
        }
        
        // Set the value text
        if (m_ValueText != null)
        {
            if (!m_CellData.IsRevealed)
            {
                m_ValueText.enabled = false;
            }
            else
            {
                // Show the value if there is no mine
                if (!m_CellData.HasMine)
                {
                    if (m_CellData.CurrentValue == 0)
                    {
                        // Empty cell with no surrounding mines
                        m_ValueText.enabled = false;
                    }
                    else if (m_CellData.CurrentValue > 0)
                    {
                        m_ValueText.enabled = true;
                        m_ValueText.text = m_CellData.CurrentValue.ToString();
                        m_ValueText.color = m_CellData.CurrentValueColor;
                        if (m_DisplayConfig != null)
                        {
                            m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
                        }
                    }
                }
            }
        }
    }

    private void UpdateMarkRenderer()
    {
        if (m_MarkRenderer != null)
        {
            if (m_MarkDisplayConfig != null && m_CellData.MarkType != CellMarkType.None)
            {
                m_MarkRenderer.enabled = true;
                
                // Get mark sprite using GetMarkSprite
                Sprite markSprite = m_MarkDisplayConfig.GetMarkSprite(m_CellData.MarkType);
                if (markSprite != null)
                {
                    m_MarkRenderer.sprite = markSprite;
                    
                    // Scale the mark based on config
                    float targetMarkSize = m_CellSize * m_MarkDisplayConfig.MarkScale;
                    SetSpriteScale(m_MarkRenderer, targetMarkSize);
                    
                    // Apply offset from config
                    m_MarkRenderer.transform.localPosition = new Vector3(
                        m_MarkDisplayConfig.MarkOffset.x,
                        m_MarkDisplayConfig.MarkOffset.y,
                        m_MarkRenderer.transform.localPosition.z
                    );
                    
                    if (m_DebugMode)
                    {
                        Debug.Log($"[CellVisualizer] Applied mark type {m_CellData.MarkType} with sprite {markSprite.name}");
                    }
                }
                else
                {
                    if (m_DebugMode)
                    {
                        Debug.LogWarning($"[CellVisualizer] No sprite found for mark type {m_CellData.MarkType}. Config: {(m_MarkDisplayConfig != null ? m_MarkDisplayConfig.name : "None")}");
                        
                        // Check which sprites are actually assigned
                        if (m_MarkDisplayConfig.FlagMarkSprite == null) Debug.LogWarning("[CellVisualizer] Flag mark sprite is null in the config");
                        if (m_MarkDisplayConfig.QuestionMarkSprite == null) Debug.LogWarning("[CellVisualizer] Question mark sprite is null in the config");
                        if (m_MarkDisplayConfig.NumbersMarkSprite == null) Debug.LogWarning("[CellVisualizer] Numbers mark sprite is null in the config");
                        if (m_MarkDisplayConfig.CustomInputMarkSprite == null) Debug.LogWarning("[CellVisualizer] Custom mark sprite is null in the config");
                    }
                    
                    // Show with a placeholder color to indicate missing sprite
                    m_MarkRenderer.color = new Color(1f, 0.5f, 0.5f);
                    m_MarkRenderer.enabled = true;
                }
            }
            else if (m_CellData.MarkType != CellMarkType.None)
            {
                // Fallback if m_MarkDisplayConfig is null but we have a mark type
                m_MarkRenderer.enabled = true;
                if (m_DebugMode)
                {
                    Debug.LogWarning($"[CellVisualizer] MarkDisplayConfig is null, using default rendering for mark type {m_CellData.MarkType}");
                }
                
                // Set a default color or appearance without using the config
                m_MarkRenderer.color = Color.yellow;
            }
            else
            {
                m_MarkRenderer.enabled = false;
            }
        }
        else if (m_DebugMode && m_CellData.MarkType != CellMarkType.None)
        {
            Debug.LogWarning("[CellVisualizer] Cannot show mark - m_MarkRenderer is null");
        }
    }

    public void ShowMineSprite(Sprite mineSprite, IMine mine, MineData mineData, GameObject cellObject, IMineDisplayStrategyFactory strategyFactory)
    {
        if (m_MineRenderer == null || mineSprite == null) return;

        m_CurrentMineSprite = mineSprite;
        m_MineRenderer.sprite = mineSprite;
        
        float targetMineSize = m_CellSize * m_MineScale;
        SetSpriteScale(m_MineRenderer, targetMineSize);
        
        if (m_DisplayConfig != null)
        {
            m_MineRenderer.transform.localPosition = m_DisplayConfig.MineOffset;
        }

        // Subscribe to state changes for monster mines
        if (mine is MonsterMine monsterMine)
        {
            // Unsubscribe first to avoid duplicate handlers
            monsterMine.OnHpChanged -= HandleMonsterHpChanged;
            monsterMine.OnHpChanged += HandleMonsterHpChanged;
            
            monsterMine.OnEnraged -= HandleMonsterEnraged;
            monsterMine.OnEnraged += HandleMonsterEnraged;
        }
        else if (mine is DisguisedMonsterMine)
        {
            // DisguisedMonsterMine doesn't have a direct MonsterMine property
            // Instead, we'll rely on its events and state changes
            // The visual updates will happen when interacting with it through CellInteractionHandler
            // Or when the state changes from ICellData
        }

        // Set up appropriate display strategy
        m_DisplayStrategy?.CleanupDisplay();
        m_DisplayStrategy = strategyFactory.CreateStrategy(mine);
        m_DisplayStrategy?.SetupDisplay(cellObject, m_ValueText);

        UpdateVisuals();
    }

    // Event handlers for monster state changes
    private void HandleMonsterHpChanged(Vector2Int position, float newHp)
    {
        // Only update if this is our cell
        if (m_CellData.Position == position)
        {
            UpdateVisuals();
        }
    }

    private void HandleMonsterEnraged(Vector2Int position)
    {
        // Only update if this is our cell
        if (m_CellData.Position == position)
        {
            UpdateVisuals();
        }
    }

    public void HandleMineRemoval()
    {
        // Only check if the cell is revealed, since we need to clean up visual traces
        // even if HasMine is already false (which is likely the case during teleportation)
        if (!m_CellData.IsRevealed) return;

        // Unsubscribe from any monster state changes
        if (m_CellData.CurrentMine is MonsterMine monsterMine)
        {
            monsterMine.OnHpChanged -= HandleMonsterHpChanged;
            monsterMine.OnEnraged -= HandleMonsterEnraged;
        }
        // No need to unsubscribe from DisguisedMonsterMine as we're not directly subscribing to it

        m_CurrentMineSprite = null;
        
        if (m_DisplayStrategy != null)
        {
            m_DisplayStrategy.CleanupDisplay();
            m_DisplayStrategy = null;
        }
        
        // Make sure the mine renderer is disabled
        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = false;
        }
        
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

    public void ApplyFrozenVisual(bool frozen)
    {
        if (m_BackgroundRenderer != null)
        {
            // Add a blue tint to indicate frozen state
            m_BackgroundRenderer.color = frozen ? new Color(0.8f, 0.8f, 1f) : Color.white;
        }
    }

    public void UpdateDisplayConfig(MineDisplayConfig newConfig)
    {
        m_DisplayConfig = newConfig;
        
        if (m_MineRenderer != null && m_DisplayConfig != null)
        {
            m_MineRenderer.transform.localPosition = m_DisplayConfig.MineOffset;
            float targetMineSize = m_CellSize * m_MineScale;
            SetSpriteScale(m_MineRenderer, targetMineSize);
        }

        // Update display strategy with new config
        if (m_CellData.HasMine && m_CellData.IsRevealed && m_DisplayStrategy != null)
        {
            m_DisplayStrategy.UpdateDisplay(m_CellData.CurrentMine, m_CellData.CurrentMineData, true);
        }
        else if (m_ValueText != null)
        {
            UpdateValueTextPosition();
        }
    }
    
    private void UpdateValueTextPosition()
    {
        if (m_ValueText != null && m_DisplayConfig != null)
        {
            // For empty cells, use EmptyCellValuePosition
            if (!m_CellData.HasMine)
            {
                m_ValueText.transform.localPosition = m_DisplayConfig.EmptyCellValuePosition;
            }
        }
    }
} 
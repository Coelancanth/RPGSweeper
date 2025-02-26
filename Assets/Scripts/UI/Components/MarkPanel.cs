using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using RPGMinesweeper.Input;
using RPGMinesweeper;

public class MarkPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform m_PanelRect;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Button m_FlagButton;
    [SerializeField] private Button m_QuestionButton;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_NumbersButton;
    [SerializeField] private Button m_CustomInputButton;
    [SerializeField] private Image m_PanelBackground;
    [SerializeField] private GraphicRaycaster m_Raycaster;
    
    [Header("Configuration")]
    [SerializeField] private RPGMinesweeper.MarkPanelDisplayConfig m_DisplayConfig;
    [SerializeField] private bool m_DebugMode = false;
    
    private Vector2Int m_CurrentCellPosition;
    private Camera m_MainCamera;
    private CellView m_CurrentCellView;
    private InputManager m_InputManager;
    
    private void Awake()
    {
        // Get main camera
        m_MainCamera = Camera.main;
        
        // Set up raycaster if not assigned
        if (m_Raycaster == null)
        {
            m_Raycaster = GetComponentInParent<GraphicRaycaster>();
        }
        
        // Find input manager
        m_InputManager = FindFirstObjectByType<InputManager>();
        
        // Apply configuration
        ApplyConfiguration();
        
        // Setup buttons
        if (m_FlagButton != null)
            m_FlagButton.onClick.AddListener(OnFlagButtonClicked);
            
        if (m_QuestionButton != null)
            m_QuestionButton.onClick.AddListener(OnQuestionButtonClicked);
            
        if (m_NumbersButton != null)
            m_NumbersButton.onClick.AddListener(OnNumbersButtonClicked);
            
        if (m_CustomInputButton != null)
            m_CustomInputButton.onClick.AddListener(OnCustomInputButtonClicked);
            
        if (m_CloseButton != null)
            m_CloseButton.onClick.AddListener(HidePanel);
            
        // Initialize panel hidden
        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
        }
            
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged += ApplyConfiguration;
        }
        
        // Disable input while panel is active to prevent interaction with cells
        if (m_InputManager != null)
        {
            if (m_DebugMode) Debug.Log("[MarkPanel] Disabling input manager interactions");
            m_InputManager.EnableInput(false);
        }
    }
    
    private void OnDisable()
    {
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged -= ApplyConfiguration;
        }
        
        // Re-enable input when panel is hidden
        if (m_InputManager != null)
        {
            if (m_DebugMode) Debug.Log("[MarkPanel] Enabling input manager interactions");
            m_InputManager.EnableInput(true);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (m_FlagButton != null)
            m_FlagButton.onClick.RemoveListener(OnFlagButtonClicked);
            
        if (m_QuestionButton != null)
            m_QuestionButton.onClick.RemoveListener(OnQuestionButtonClicked);
            
        if (m_NumbersButton != null)
            m_NumbersButton.onClick.RemoveListener(OnNumbersButtonClicked);
            
        if (m_CustomInputButton != null)
            m_CustomInputButton.onClick.RemoveListener(OnCustomInputButtonClicked);
            
        if (m_CloseButton != null)
            m_CloseButton.onClick.RemoveListener(HidePanel);
            
        if (m_DisplayConfig != null)
        {
            m_DisplayConfig.OnConfigChanged -= ApplyConfiguration;
        }
    }
    
    private void ApplyConfiguration()
    {
        if (m_DisplayConfig == null) return;
        
        // Apply panel size
        if (m_PanelRect != null)
        {
            m_PanelRect.sizeDelta = m_DisplayConfig.PanelSize;
        }
        
        // Apply panel background color
        if (m_PanelBackground != null)
        {
            m_PanelBackground.color = m_DisplayConfig.PanelBackgroundColor;
        }
        
        // Configure button sizes and colors
        ConfigureButton(m_FlagButton, "Flag");
        ConfigureButton(m_QuestionButton, "?");
        ConfigureButton(m_NumbersButton, "123");
        ConfigureButton(m_CustomInputButton, "...");
        ConfigureButton(m_CloseButton, "X");
        
        if (m_DebugMode) Debug.Log("[MarkPanel] Configuration applied");
    }
    
    private void ConfigureButton(Button button, string label)
    {
        if (button == null || m_DisplayConfig == null) return;
        
        // Set button size
        var rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = m_DisplayConfig.ButtonSize;
        }
        
        // Set button colors
        var colors = button.colors;
        colors.normalColor = m_DisplayConfig.ButtonNormalColor;
        colors.highlightedColor = m_DisplayConfig.ButtonHoverColor;
        button.colors = colors;
        
        // Set button label
        var textComponent = button.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = label;
        }
        else
        {
            var legacyText = button.GetComponentInChildren<Text>();
            if (legacyText != null)
            {
                legacyText.text = label;
            }
        }
    }
    
    public void ShowAtCell(CellView cellView)
    {
        if (cellView == null)
        {
            if (m_DebugMode) Debug.LogWarning("[MarkPanel] Cannot show panel: Cell view is null");
            return;
        }
        
        if (cellView.IsRevealed)
        {
            if (m_DebugMode) Debug.LogWarning("[MarkPanel] Cannot show panel: Cell is already revealed");
            return;
        }
        
        if (m_DisplayConfig == null)
        {
            Debug.LogError("[MarkPanel] Cannot show panel: Display config is null. Please assign a MarkPanelDisplayConfig in the inspector.");
            return;
        }
        
        m_CurrentCellView = cellView;
        m_CurrentCellPosition = cellView.Position;
        
        // Position the panel relative to the cell in screen space
        Vector3 cellWorldPos = cellView.transform.position;
        Vector3 screenPos = m_MainCamera.WorldToScreenPoint(cellWorldPos);
        
        // Apply offset to avoid overlapping the cell
        Vector2 offset = m_DisplayConfig.Offset;
        screenPos += new Vector3(offset.x, offset.y, 0);
        
        // Convert to canvas space if using a canvas scaler
        RectTransform canvasRect = transform.parent.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPos, 
                null, 
                out Vector2 localPos);
                
            m_PanelRect.anchoredPosition = localPos;
            
            if (m_DebugMode) Debug.Log($"[MarkPanel] Positioned at local position: {localPos}");
        }
        else
        {
            if (m_DebugMode) Debug.LogWarning("[MarkPanel] Parent canvas rect not found");
        }
        
        // Show the panel
        gameObject.SetActive(true);
        
        // Enable interaction and blocking
        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;
            
            if (m_DebugMode) Debug.Log("[MarkPanel] Canvas group configured for interaction");
        }
        else
        {
            Debug.LogWarning("[MarkPanel] Canvas group reference is missing");
        }
        
        // Disable input manager to prevent interactions with cells underneath
        if (m_InputManager != null)
        {
            if (m_DebugMode) Debug.Log("[MarkPanel] Disabling input manager interactions");
            m_InputManager.EnableInput(false);
        }
        else
        {
            Debug.LogWarning("[MarkPanel] Input manager reference is missing");
        }
        
        if (m_DebugMode) Debug.Log($"[MarkPanel] Showing at cell: {m_CurrentCellPosition}");
    }
    
    public void HidePanel()
    {
        // Disable panel visually
        gameObject.SetActive(false);

        // Clear current cell reference
        m_CurrentCellView = null;
        m_CurrentCellPosition = Vector2Int.zero;

        // Re-enable input manager
        if (m_InputManager != null)
        {
            if (m_DebugMode) Debug.Log("[MarkPanel] Re-enabling input manager interactions");
            m_InputManager.EnableInput(true);
        }

        if (m_DebugMode) Debug.Log("[MarkPanel] Panel hidden");
    }
    
    private void OnFlagButtonClicked()
    {
        if (m_CurrentCellView != null)
        {
            if (m_DebugMode) Debug.Log($"[MarkPanel] Flag button clicked for cell: {m_CurrentCellPosition}");
            
            // Apply flag marker
            GameEvents.RaiseCellMarked(m_CurrentCellPosition, CellMarkType.Flag);
            HidePanel();
        }
    }
    
    private void OnQuestionButtonClicked()
    {
        if (m_CurrentCellView != null)
        {
            if (m_DebugMode) Debug.Log($"[MarkPanel] Question button clicked for cell: {m_CurrentCellPosition}");
            
            // Apply question marker
            GameEvents.RaiseCellMarked(m_CurrentCellPosition, CellMarkType.Question);
            HidePanel();
        }
    }
    
    private void OnNumbersButtonClicked()
    {
        if (m_CurrentCellView != null)
        {
            if (m_DebugMode) Debug.Log($"[MarkPanel] Numbers button clicked for cell: {m_CurrentCellPosition}");
            
            // Apply numbers marker
            GameEvents.RaiseCellMarked(m_CurrentCellPosition, CellMarkType.Numbers);
            HidePanel();
        }
    }
    
    private void OnCustomInputButtonClicked()
    {
        if (m_CurrentCellView != null)
        {
            if (m_DebugMode) Debug.Log($"[MarkPanel] Custom input button clicked for cell: {m_CurrentCellPosition}");
            
            // Apply custom input marker
            GameEvents.RaiseCellMarked(m_CurrentCellPosition, CellMarkType.CustomInput);
            HidePanel();
        }
    }
} 
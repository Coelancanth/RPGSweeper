using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform m_PanelRect;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Button m_FlagButton;
    [SerializeField] private Button m_QuestionButton;
    [SerializeField] private Button m_CloseButton;
    
    [Header("Settings")]
    [SerializeField] private float m_ShowDuration = 0.25f;
    [SerializeField] private Vector2 m_PanelSize = new Vector2(120f, 150f);
    [SerializeField] private Vector2 m_Offset = new Vector2(20f, 20f);
    
    private Vector2Int m_CurrentCellPosition;
    private Camera m_MainCamera;
    private CellView m_CurrentCellView;
    
    private void Awake()
    {
        // Get main camera
        m_MainCamera = Camera.main;
        
        // Setup buttons
        if (m_FlagButton != null)
            m_FlagButton.onClick.AddListener(OnFlagButtonClicked);
            
        if (m_QuestionButton != null)
            m_QuestionButton.onClick.AddListener(OnQuestionButtonClicked);
            
        if (m_CloseButton != null)
            m_CloseButton.onClick.AddListener(HidePanel);
            
        // Initialize panel hidden
        if (m_CanvasGroup != null)
            m_CanvasGroup.alpha = 0f;
            
        // Set panel size
        if (m_PanelRect != null)
            m_PanelRect.sizeDelta = m_PanelSize;
            
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (m_FlagButton != null)
            m_FlagButton.onClick.RemoveListener(OnFlagButtonClicked);
            
        if (m_QuestionButton != null)
            m_QuestionButton.onClick.RemoveListener(OnQuestionButtonClicked);
            
        if (m_CloseButton != null)
            m_CloseButton.onClick.RemoveListener(HidePanel);
    }
    
    public void ShowAtCell(CellView cellView)
    {
        if (cellView == null || cellView.IsRevealed)
            return;
            
        m_CurrentCellView = cellView;
        m_CurrentCellPosition = cellView.Position;
        
        // Position the panel relative to the cell in screen space
        Vector3 cellWorldPos = cellView.transform.position;
        Vector3 screenPos = m_MainCamera.WorldToScreenPoint(cellWorldPos);
        
        // Apply offset to avoid overlapping the cell
        screenPos += new Vector3(m_Offset.x, m_Offset.y, 0);
        
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
        }
        
        // Show the panel
        gameObject.SetActive(true);
        //LeanTween.alphaCanvas(m_CanvasGroup, 1f, m_ShowDuration).setEaseOutQuad();
    }
    
    public void HidePanel()
    {
        // Hide with animation
        //LeanTween.alphaCanvas(m_CanvasGroup, 0f, m_ShowDuration)
            //.setEaseInQuad()
            //.setOnComplete(() => gameObject.SetActive(false));
            
        m_CurrentCellView = null;
    }
    
    private void OnFlagButtonClicked()
    {
        if (m_CurrentCellView != null)
        {
            // Apply flag marker
            GameEvents.RaiseCellMarked(m_CurrentCellPosition, CellMarkType.Flag);
            HidePanel();
        }
    }
    
    private void OnQuestionButtonClicked()
    {
        if (m_CurrentCellView != null)
        {
            // Apply question marker
            GameEvents.RaiseCellMarked(m_CurrentCellPosition, CellMarkType.Question);
            HidePanel();
        }
    }
} 
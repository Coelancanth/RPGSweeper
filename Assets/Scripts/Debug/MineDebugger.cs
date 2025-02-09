using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MineDebugger : MonoBehaviour
{
    #region Serialized Fields
    [Header("Debug Settings")]
    [Tooltip("Color to highlight mine cells")]
    [SerializeField] private Color m_MineHighlightColor = new Color(1f, 0f, 0f, 0.5f);
    
    [Tooltip("Key to toggle mine visibility")]
    [SerializeField] private KeyCode m_DebugKeyCode = KeyCode.F1;
    #endregion

    #region Private Fields
    private CellView[] m_AllCells;
    private bool m_IsDebugVisible;
    private Color[] m_OriginalColors;
    #endregion

    #region Unity Methods
    private void Start()
    {
        m_AllCells = FindObjectsOfType<CellView>();
        m_OriginalColors = new Color[m_AllCells.Length];
        m_IsDebugVisible = false;
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(m_DebugKeyCode))
        {
            ToggleMineVisibility();
        }
        #endif
    }
    #endregion

    #region Public Methods
    public void ToggleMineVisibility()
    {
        m_IsDebugVisible = !m_IsDebugVisible;
        
        if (m_AllCells == null || m_AllCells.Length == 0)
        {
            m_AllCells = FindObjectsOfType<CellView>();
            m_OriginalColors = new Color[m_AllCells.Length];
        }

        for (int i = 0; i < m_AllCells.Length; i++)
        {
            var cell = m_AllCells[i];
            var backgroundRenderer = cell.GetBackgroundRenderer();
            
            if (backgroundRenderer != null)
            {
                if (m_IsDebugVisible)
                {
                    // Store original color and highlight mines
                    m_OriginalColors[i] = backgroundRenderer.color;
                    if (cell.HasMine())
                    {
                        backgroundRenderer.color = m_MineHighlightColor;
                    }
                }
                else
                {
                    // Restore original color
                    backgroundRenderer.color = m_OriginalColors[i];
                }
            }
        }
    }
    #endregion

    #region Editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(MineDebugger))]
    public class MineDebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MineDebugger debugger = (MineDebugger)target;
            if (GUILayout.Button("Toggle Mine Visibility"))
            {
                debugger.ToggleMineVisibility();
            }
        }
    }
    #endif
    #endregion
} 
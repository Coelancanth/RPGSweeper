using UnityEngine;
using RPGMinesweeper;

public class MineAdditionTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private GridManager m_GridManager;
    [Tooltip("Key to add a Bat monster")]
    [SerializeField] private KeyCode m_AddBatKey = KeyCode.B;
    [Tooltip("Key to add a Beholder monster")]
    [SerializeField] private KeyCode m_AddBeholderKey = KeyCode.H;

    private void Start()
    {
        if (m_GridManager == null)
        {
            m_GridManager = FindFirstObjectByType<GridManager>();
            if (m_GridManager == null)
            {
                Debug.LogError("MineAdditionTest: Could not find GridManager!");
                enabled = false;
                return;
            }
        }
    }

    private void Update()
    {
        // Get mouse position and convert to grid position
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            var cellView = hit.collider.GetComponent<CellView>();
            if (cellView != null)
            {
                Vector2Int gridPosition = cellView.GridPosition;

                // Test Bat monster creation
                if (Input.GetKeyDown(m_AddBatKey))
                {
                    //Debug.Log($"Attempting to add Bat at position {gridPosition}");
                    GameEvents.RaiseMineAddAttempted(gridPosition, MineType.Monster, MonsterType.Bat);
                }

                // Test Beholder monster creation
                if (Input.GetKeyDown(m_AddBeholderKey))
                {
                    Debug.Log($"Attempting to add Beholder at position {gridPosition}");
                    GameEvents.RaiseMineAddAttempted(gridPosition, MineType.Monster, MonsterType.Beholder);
                }
            }
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_AddBatKey == m_AddBeholderKey)
        {
            Debug.LogWarning("MineAdditionTest: Bat and Beholder keys should be different!");
        }
    }
    #endif
} 
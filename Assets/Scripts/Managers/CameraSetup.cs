using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private GridManager m_GridManager;
    [SerializeField] private float m_Padding = 1f; // Extra space around the grid
    
    private Camera m_Camera;

    private void Start()
    {
        m_Camera = Camera.main;
        if (m_Camera != null)
        {
            SetupCamera();
        }
    }

    private void SetupCamera()
    {
        // Get grid dimensions
        float gridWidth = m_GridManager.Width * m_GridManager.CellSize;
        float gridHeight = m_GridManager.Height * m_GridManager.CellSize;

        // Calculate required orthographic size
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = gridWidth / gridHeight;

        float orthographicSize;
        if (screenRatio >= targetRatio)
        {
            // Height is the limiting factor
            orthographicSize = (gridHeight + m_Padding * 2) / 2f;
        }
        else
        {
            // Width is the limiting factor
            orthographicSize = (gridWidth / screenRatio + m_Padding * 2) / 2f;
        }

        // Set camera position and size
        m_Camera.orthographicSize = orthographicSize;
        m_Camera.transform.position = new Vector3(0f, 0f, -10f);
    }
} 
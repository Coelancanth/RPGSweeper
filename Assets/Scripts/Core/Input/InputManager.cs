using UnityEngine;
using System;

namespace RPGMinesweeper.Input
{
    public class InputManager : MonoBehaviour
    {
        #region Events
        public static event Action<Vector2Int> OnCellClicked;
        #endregion

        #region Private Fields
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private LayerMask m_CellLayer;
        [SerializeField] private bool m_DebugMode;
        private bool m_IsInputEnabled = true;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (m_MainCamera == null)
            {
                m_MainCamera = Camera.main;
                Debug.Log($"[InputManager] Using main camera: {m_MainCamera != null}");
            }
            
            if (m_CellLayer.value == 0)
            {
                Debug.LogError("[InputManager] Cell layer mask is not set!");
            }
        }

        private void Update()
        {
            if (!m_IsInputEnabled)
            {
                if (m_DebugMode && UnityEngine.Input.GetMouseButtonDown(0))
                {
                    Debug.Log("[InputManager] Input is currently disabled");
                }
                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0)) // Left click
            {
                if (m_DebugMode) Debug.Log("[InputManager] Mouse button clicked");
                HandleMouseClick();
            }
        }
        #endregion

        #region Public Methods
        public void EnableInput(bool enable)
        {
            m_IsInputEnabled = enable;
            if (m_DebugMode) Debug.Log($"[InputManager] Input {(enable ? "enabled" : "disabled")}");
        }
        #endregion

        #region Private Methods
        private void HandleMouseClick()
        {
            if (m_MainCamera == null)
            {
                Debug.LogError("[InputManager] No camera assigned!");
                return;
            }

            Ray ray = m_MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (m_DebugMode) Debug.Log($"[InputManager] Casting ray from {ray.origin} in direction {ray.direction}");
            
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, m_CellLayer);

            if (hit.collider != null)
            {
                if (m_DebugMode) Debug.Log($"[InputManager] Hit object: {hit.collider.gameObject.name}");
                var cellView = hit.collider.GetComponent<CellView>();
                if (cellView != null)
                {
                    if (m_DebugMode) Debug.Log($"[InputManager] Found CellView at position {cellView.GridPosition}");
                    if (!cellView.IsFrozen)
                    {
                        if (m_DebugMode) Debug.Log($"[InputManager] Invoking OnCellClicked for position {cellView.GridPosition}");
                        OnCellClicked?.Invoke(cellView.GridPosition);
                    }
                    else if (m_DebugMode)
                    {
                        Debug.Log($"[InputManager] Cell at {cellView.GridPosition} is frozen - ignoring click");
                    }
                }
                else if (m_DebugMode)
                {
                    Debug.Log("[InputManager] Hit object does not have CellView component");
                }
            }
            else if (m_DebugMode)
            {
                Debug.Log("[InputManager] No object hit by raycast");
            }
        }
        #endregion
    }
} 
using UnityEngine;
using RPGMinesweeper.Grid;
using System.Collections.Generic;

namespace RPGMinesweeper.States
{
    public class FrozenState : BaseTurnState
    {
        #region Private Fields
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private readonly Vector2Int m_SourcePosition;
        [SerializeField] private bool m_DebugMode = false;
        private List<Vector2Int> m_AffectedPositions;
        #endregion

        #region Constructor
        public FrozenState(int turns, int radius, Vector2Int sourcePosition, GridShape shape = GridShape.Square) 
            : base("Frozen", turns, StateTarget.Cell, sourcePosition)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_SourcePosition = sourcePosition;
            
            if (m_DebugMode)
            {
                Debug.Log($"[FrozenState] Created new frozen state at {sourcePosition} with radius {radius}, duration {turns} turns");
            }
        }
        #endregion

        #region Public Methods
        public override void Enter(GameObject target)
        {
            base.Enter(target);
            if (m_DebugMode)
            {
                Debug.Log($"[FrozenState] Entering frozen state at {m_SourcePosition}, {TurnsRemaining} turns remaining");
            }
            ApplyFrozenState();
        }

        public override void Exit(GameObject target)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[FrozenState] Exiting frozen state at {m_SourcePosition}");
            }
            RemoveFrozenState();
            base.Exit(target);
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            if (m_DebugMode)
            {
                Debug.Log($"[FrozenState] Turn ended, {TurnsRemaining} turns remaining at {m_SourcePosition}");
            }
        }
        #endregion

        #region Private Methods
        private void ApplyFrozenState()
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                if (m_DebugMode)
                {
                    Debug.LogError("[FrozenState] Could not find GridManager!");
                }
                return;
            }

            m_AffectedPositions = GridShapeHelper.GetAffectedPositions(m_SourcePosition, m_Shape, m_Radius);
            foreach (var pos in m_AffectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    var cellObject = gridManager.GetCellObject(pos);
                    if (cellObject != null)
                    {
                        var cellView = cellObject.GetComponent<CellView>();
                        if (cellView != null)
                        {
                            cellView.SetFrozen(true);
                            if (m_DebugMode)
                            {
                                Debug.Log($"[FrozenState] Froze cell at {pos}");
                            }
                        }
                    }
                }
            }
        }

        private void RemoveFrozenState()
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                if (m_DebugMode)
                {
                    Debug.LogError("[FrozenState] Could not find GridManager during cleanup!");
                }
                return;
            }

            if (m_AffectedPositions != null)
            {
                foreach (var pos in m_AffectedPositions)
                {
                    if (gridManager.IsValidPosition(pos))
                    {
                        var cellObject = gridManager.GetCellObject(pos);
                        if (cellObject != null)
                        {
                            var cellView = cellObject.GetComponent<CellView>();
                            if (cellView != null)
                            {
                                cellView.SetFrozen(false);
                                if (m_DebugMode)
                                {
                                    Debug.Log($"[FrozenState] Unfroze cell at {pos}");
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
} 
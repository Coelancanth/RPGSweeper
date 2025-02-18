using UnityEngine;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.States
{
    public class FrozenState : BaseState
    {
        #region Private Fields
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly Vector2Int m_SourcePosition;
        #endregion

        #region Constructor
        public FrozenState(float duration, float radius, Vector2Int sourcePosition, GridShape shape = GridShape.Square) 
            : base("Frozen", duration)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_SourcePosition = sourcePosition;
        }
        #endregion

        #region Public Methods
        public override void Enter(GameObject target)
        {
            base.Enter(target);
            ApplyFrozenState();
        }

        public override void Exit(GameObject target)
        {
            base.Exit(target);
            RemoveFrozenState();
        }
        #endregion

        #region Private Methods
        private void ApplyFrozenState()
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            var affectedPositions = GridShapeHelper.GetAffectedPositions(m_SourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            foreach (var pos in affectedPositions)
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
                        }
                    }
                }
            }
        }

        private void RemoveFrozenState()
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            var affectedPositions = GridShapeHelper.GetAffectedPositions(m_SourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            foreach (var pos in affectedPositions)
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
                        }
                    }
                }
            }
        }
        #endregion
    }
} 
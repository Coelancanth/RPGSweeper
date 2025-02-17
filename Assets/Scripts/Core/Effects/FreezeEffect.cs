using UnityEngine;
using System.Collections;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class FreezeEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Duration;
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private bool m_IsActive;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Freeze";
        #endregion

        public FreezeEffect(float duration, float radius, GridShape shape = GridShape.Square)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_Shape = shape;
            m_IsActive = false;
            m_CurrentMode = EffectType.Persistent; // Default to persistent mode
        }

        #region Protected Methods
        protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            ApplyFreezeToArea(sourcePosition);
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            ApplyFreezeToArea(sourcePosition);
            
            // Auto-remove after duration for triggerable mode
            GameObject.FindFirstObjectByType<MonoBehaviour>()?.StartCoroutine(RemoveAfterDelay());
        }
        #endregion

        #region Public Methods
        public void Update(float deltaTime)
        {
            // Update freeze effect (e.g., visual feedback)
            if (m_CurrentMode == EffectType.Persistent)
            {
                // Add any persistent-specific update logic here
            }
        }

        public void Remove(GameObject target)
        {
            m_IsActive = false;
            RemoveFreezeFromArea();
        }
        #endregion

        #region Private Methods
        private void ApplyFreezeToArea(Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    m_AffectedCells.Add(pos);
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

        private void RemoveFreezeFromArea()
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            foreach (var pos in m_AffectedCells)
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
            m_AffectedCells.Clear();
        }

        private IEnumerator RemoveAfterDelay()
        {
            yield return new WaitForSeconds(m_Duration);
            if (m_IsActive)
            {
                Remove(null);
            }
        }
        #endregion
    }
} 
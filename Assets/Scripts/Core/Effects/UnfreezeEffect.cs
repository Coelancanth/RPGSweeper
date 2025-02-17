using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.States;

namespace RPGMinesweeper.Effects
{
    public class UnfreezeEffect : ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Unfreeze";
        #endregion

        public UnfreezeEffect(float radius, GridShape shape)
        {
            m_Radius = radius;
            m_Shape = shape;
        }

        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));

            // Remove frozen state from each valid cell
            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    var cellObject = gridManager.GetCellObject(pos);
                    if (cellObject != null)
                    {
                        var stateManager = cellObject.GetComponent<StateManager>();
                        if (stateManager != null)
                        {
                            stateManager.RemoveState("Frozen");
                        }
                    }
                }
            }
        }
    }
} 
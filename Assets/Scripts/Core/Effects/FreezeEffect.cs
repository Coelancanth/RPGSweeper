using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.States;

namespace RPGMinesweeper.Effects
{
    public class FreezeEffect : ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Duration;
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Freeze";
        #endregion

        public FreezeEffect(float duration, int radius, GridShape shape)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_Shape = shape;
        }

        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);

            // Apply frozen state to each valid cell
            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    var cellObject = gridManager.GetCellObject(pos);
                    if (cellObject != null)
                    {
                        var stateManager = cellObject.GetComponent<StateManager>();
                        if (stateManager == null)
                        {
                            stateManager = cellObject.AddComponent<StateManager>();
                        }
                        stateManager.AddState(new FrozenState(m_Duration));
                    }
                }
            }
        }
    }

    // Define the FrozenState
    public class FrozenState : BaseState
    {
        public FrozenState(float duration) : base("Frozen", duration)
        {
        }

        public override void Enter(GameObject target)
        {
            base.Enter(target);
            var cellView = target.GetComponent<CellView>();
            if (cellView != null)
            {
                cellView.SetFrozen(true);
            }
        }

        public override void Exit(GameObject target)
        {
            base.Exit(target);
            var cellView = target.GetComponent<CellView>();
            if (cellView != null)
            {
                cellView.SetFrozen(false);
            }
        }
    }
} 
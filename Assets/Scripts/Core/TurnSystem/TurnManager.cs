using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper.TurnSystem
{
    public class TurnManager : MonoBehaviour
    {
        #region Events
        public static event System.Action<ITurn> OnTurnStarted;
        public static event System.Action<ITurn> OnTurnEnded;
        #endregion

        #region Private Fields
        private Queue<ITurn> m_PendingTurns = new Queue<ITurn>();
        private ITurn m_CurrentTurn;
        private bool m_IsProcessingTurns;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            GameEvents.OnCellRevealed += HandleCellRevealed;
            GameEvents.OnMineTriggered += HandleMineTriggered;
            GameEvents.OnEffectApplied += HandleEffectApplied;
        }

        private void OnDestroy()
        {
            GameEvents.OnCellRevealed -= HandleCellRevealed;
            GameEvents.OnMineTriggered -= HandleMineTriggered;
            GameEvents.OnEffectApplied -= HandleEffectApplied;
        }

        private void Update()
        {
            ProcessTurns();
        }
        #endregion

        #region Public Methods
        public void QueueTurn(ITurn turn)
        {
            m_PendingTurns.Enqueue(turn);
            if (!m_IsProcessingTurns)
            {
                ProcessTurns();
            }
        }
        #endregion

        #region Private Methods
        private void ProcessTurns()
        {
            if (m_IsProcessingTurns) return;
            m_IsProcessingTurns = true;

            try
            {
                // Process current turn if it exists
                if (m_CurrentTurn != null)
                {
                    m_CurrentTurn.Update();
                    if (m_CurrentTurn.IsComplete)
                    {
                        EndCurrentTurn();
                    }
                }

                // Start new turn if none is active
                if (m_CurrentTurn == null && m_PendingTurns.Count > 0)
                {
                    StartNextTurn();
                }
            }
            finally
            {
                m_IsProcessingTurns = false;
            }
        }

        private void StartNextTurn()
        {
            if (m_PendingTurns.Count == 0) return;

            m_CurrentTurn = m_PendingTurns.Dequeue();
            m_CurrentTurn.Begin();
            OnTurnStarted?.Invoke(m_CurrentTurn);
        }

        private void EndCurrentTurn()
        {
            if (m_CurrentTurn == null) return;

            m_CurrentTurn.End();
            OnTurnEnded?.Invoke(m_CurrentTurn);
            m_CurrentTurn = null;
        }

        private void HandleCellRevealed(Vector2Int position)
        {
            // Queue player turn when cell is revealed
            QueueTurn(new PlayerTurn(position));
        }

        private void HandleMineTriggered(MineType type)
        {
            // Queue reaction turn when mine is triggered
            QueueTurn(new ReactionTurn(type));
        }

        private void HandleEffectApplied(Vector2Int position)
        {
            // Queue effect turn when effect is applied
            QueueTurn(new EffectTurn(position));
        }
        #endregion
    }
} 
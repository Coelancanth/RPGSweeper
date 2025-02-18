using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.TurnSystem;

namespace RPGMinesweeper.TurnSystem
{
    public class TurnManager : MonoBehaviour
    {
        #region Events
        public static event System.Action<ITurn> OnTurnStarted;
        public static event System.Action<ITurn> OnTurnEnded;
        public static event System.Action<int> OnTurnCountChanged;
        #endregion

        #region Private Fields
        private Queue<ITurn> m_PendingTurns = new Queue<ITurn>();
        private ITurn m_CurrentTurn;
        private bool m_IsProcessingTurns;
        private int m_TurnCount;
        [SerializeField] private bool m_DebugMode;
        #endregion

        #region Public Properties
        public int CurrentTurn => m_TurnCount;
        public ITurn ActiveTurn => m_CurrentTurn;
        public int PendingTurnsCount => m_PendingTurns.Count;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            m_TurnCount = 0;
            GameEvents.OnCellRevealed += HandleCellRevealed;
            GameEvents.OnMineTriggered += HandleMineTriggered;
            GameEvents.OnEffectApplied += HandleEffectApplied;
            
            if (m_DebugMode)
            {
                Debug.Log("[TurnManager] Initialized. Turn count: 0");
            }
        }

        private void OnDestroy()
        {
            GameEvents.OnCellRevealed -= HandleCellRevealed;
            GameEvents.OnMineTriggered -= HandleMineTriggered;
            GameEvents.OnEffectApplied -= HandleEffectApplied;
        }
        #endregion

        #region Public Methods
        public void QueueTurn(ITurn turn)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[TurnManager] Turn {m_TurnCount}: Queueing {turn.GetType().Name}, Queue size: {m_PendingTurns.Count}");
            }
            
            m_PendingTurns.Enqueue(turn);
            ProcessTurns();
        }

        public void CompleteTurn()
        {
            if (m_CurrentTurn != null)
            {
                if (m_DebugMode)
                {
                    Debug.Log($"[TurnManager] Turn {m_TurnCount}: Completing {m_CurrentTurn.GetType().Name}");
                }
                EndCurrentTurn();
                ProcessTurns();
            }
        }

        public void ResetTurnCount()
        {
            m_TurnCount = 0;
            if (m_DebugMode)
            {
                Debug.Log("[TurnManager] Turn count reset to 0");
            }
            OnTurnCountChanged?.Invoke(m_TurnCount);
        }
        #endregion

        #region Private Methods
        private void ProcessTurns()
        {
            if (m_IsProcessingTurns) return;
            m_IsProcessingTurns = true;

            try
            {
                if (m_DebugMode)
                {
                    Debug.Log($"[TurnManager] Turn {m_TurnCount}: Processing turns | Current: {m_CurrentTurn?.GetType().Name ?? "None"} | Pending: {m_PendingTurns.Count}");
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

            m_TurnCount++;
            m_CurrentTurn = m_PendingTurns.Dequeue();
            m_CurrentTurn.Begin();
            
            if (m_DebugMode)
            {
                Debug.Log($"[TurnManager] Turn {m_TurnCount}: Starting {m_CurrentTurn.GetType().Name} | Pending: {m_PendingTurns.Count}");
            }
            
            OnTurnStarted?.Invoke(m_CurrentTurn);
            OnTurnCountChanged?.Invoke(m_TurnCount);
        }

        private void EndCurrentTurn()
        {
            if (m_CurrentTurn == null) return;

            if (m_DebugMode)
            {
                Debug.Log($"[TurnManager] Turn {m_TurnCount}: Ending {m_CurrentTurn.GetType().Name}");
            }
            
            m_CurrentTurn.End();
            OnTurnEnded?.Invoke(m_CurrentTurn);
            m_CurrentTurn = null;
        }

        private void HandleCellRevealed(Vector2Int position)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[TurnManager] Turn {m_TurnCount}: Cell revealed at {position}, queueing PlayerTurn");
            }
            // Queue player turn when cell is revealed
            QueueTurn(new PlayerTurn(position));
        }

        private void HandleMineTriggered(MineType type)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[TurnManager] Turn {m_TurnCount}: Mine of type {type} triggered, queueing ReactionTurn");
            }
            // Queue reaction turn when mine is triggered
            QueueTurn(new ReactionTurn(type));
        }

        private void HandleEffectApplied(Vector2Int position)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[TurnManager] Turn {m_TurnCount}: Effect applied at {position}, queueing EffectTurn");
            }
            // Queue effect turn when effect is applied
            QueueTurn(new EffectTurn(position));
        }
        #endregion
    }
} 
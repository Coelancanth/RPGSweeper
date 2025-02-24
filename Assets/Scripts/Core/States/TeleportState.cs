using UnityEngine;
using RPGMinesweeper.Grid;
using System.Collections.Generic;

namespace RPGMinesweeper.States
{
    public class TeleportState : BaseTurnState
    {
        #region Private Fields
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private readonly Vector2Int m_SourcePosition;
        private List<Vector2Int> m_AffectedPositions;
        private bool m_DebugMode = false;
        #endregion

        #region Constructor
        public TeleportState(int turns, int radius, Vector2Int sourcePosition, GridShape shape = GridShape.Square) 
            : base("Teleport", turns, StateTarget.Cell, sourcePosition)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_SourcePosition = sourcePosition;
            
            if (m_DebugMode)
            {
                Debug.Log($"[TeleportState] Created new teleport state at {sourcePosition} with radius {radius}, duration {turns} turns");
            }
        }
        #endregion

        #region Public Methods
        public override void Enter(GameObject target)
        {
            base.Enter(target);
            if (m_DebugMode)
            {
                Debug.Log($"[TeleportState] Entering teleport state at {m_SourcePosition}, {TurnsRemaining} turns remaining");
            }
            InitializeAffectedPositions();
        }

        public override void Exit(GameObject target)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[TeleportState] Exiting teleport state at {m_SourcePosition}");
            }
            base.Exit(target);
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            if (m_DebugMode)
            {
                Debug.Log($"[TeleportState] Turn ended, {TurnsRemaining} turns remaining at {m_SourcePosition}");
            }
            TeleportCells();
        }
        #endregion

        #region Private Methods
        private void InitializeAffectedPositions()
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                if (m_DebugMode)
                {
                    Debug.LogError("[TeleportState] Could not find GridManager!");
                }
                return;
            }

            m_AffectedPositions = GridShapeHelper.GetAffectedPositions(m_SourcePosition, m_Shape, m_Radius);
            m_AffectedPositions.RemoveAll(pos => !gridManager.IsValidPosition(pos));
        }

        private void TeleportCells()
        {
            if (m_AffectedPositions == null || m_AffectedPositions.Count == 0) return;

            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (gridManager == null || mineManager == null)
            {
                Debug.LogError("[TeleportState] Could not find required managers!");
                return;
            }

            // Get all valid positions on the grid
            var validPositions = new List<Vector2Int>();
            for (int x = 0; x < gridManager.Width; x++)
            {
                for (int y = 0; y < gridManager.Height; y++)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }

            // Remove positions that have mines
            validPositions.RemoveAll(pos => mineManager.HasMineAt(pos));

            // Store positions to teleport to avoid modifying collection during iteration
            var teleportOperations = new List<(Vector2Int source, Vector2Int target)>();

            // For each affected position that has a mine
            foreach (var sourcePos in m_AffectedPositions)
            {
                if (!mineManager.HasMineAt(sourcePos)) continue;

                // If there are no more valid positions, stop teleporting
                if (validPositions.Count == 0) break;

                // Pick a random valid position
                int randomIndex = Random.Range(0, validPositions.Count);
                Vector2Int targetPos = validPositions[randomIndex];

                // Store the operation
                teleportOperations.Add((sourcePos, targetPos));
                // Remove the target position from valid positions
                validPositions.RemoveAt(randomIndex);
            }

            // Execute teleport operations
            foreach (var (sourcePos, targetPos) in teleportOperations)
            {
                mineManager.TeleportMine(sourcePos, targetPos);

                if (m_DebugMode)
                {
                    Debug.Log($"[TeleportState] Teleported mine from {sourcePos} to {targetPos}");
                }
            }
        }
        #endregion
    }
} 
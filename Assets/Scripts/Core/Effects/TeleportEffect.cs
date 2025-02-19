using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType and GameEvents

namespace RPGMinesweeper.Effects
{
    public class TeleportEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
    {
        #region Private Fields
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private readonly GridPositionType m_TargetPositionType;
        private readonly float m_TeleportInterval;
        private float m_TimeUntilNextTeleport;
        private bool m_IsActive;
        private bool m_DebugMode = false;
        private Vector2Int m_CurrentPosition;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Teleport";
        #endregion

        public TeleportEffect(int radius, GridShape shape = GridShape.Square, GridPositionType targetPositionType = GridPositionType.Random, float teleportInterval = 5f)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_TargetPositionType = targetPositionType;
            m_TeleportInterval = teleportInterval;
            m_TimeUntilNextTeleport = teleportInterval;
            m_IsActive = false;
            m_CurrentMode = EffectType.Triggerable;
        }

        #region Protected Methods
        protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            m_CurrentPosition = sourcePosition;
            m_TimeUntilNextTeleport = m_TeleportInterval;
            RegisterEffectInArea(sourcePosition, m_Radius, m_Shape);
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            PerformTeleport(sourcePosition);
            m_IsActive = false;
        }
        #endregion

        #region Public Methods
        public void Update(float deltaTime)
        {
            if (!m_IsActive || m_CurrentMode != EffectType.Persistent) return;

            m_TimeUntilNextTeleport -= deltaTime;
            if (m_TimeUntilNextTeleport <= 0)
            {
                PerformTeleport(m_CurrentPosition);
                m_TimeUntilNextTeleport = m_TeleportInterval;
            }
        }

        public void Remove(GameObject target)
        {
            m_IsActive = false;
            UnregisterEffectFromArea();
        }
        #endregion

        #region Private Methods
        private void PerformTeleport(Vector2Int sourcePosition)
        {
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            
            if (mineManager == null || gridManager == null) return;

            var mines = mineManager.GetMines();
            if (!mines.TryGetValue(sourcePosition, out var sourceMine)) return;

            // Get the effective target position based on GridPositionType
            var effectivePosition = GetEffectivePosition(sourcePosition, gridManager);

            // Get all valid positions within the grid
            var validPositions = GetValidTeleportPositions(gridManager, effectivePosition);
            if (validPositions.Count == 0) return;
            Debug.Log($"[TeleportEffect] Valid positions: {string.Join(", ", validPositions)}");

            // Select a random position from valid positions
            var randomIndex = Random.Range(0, validPositions.Count);
            var targetPosition = validPositions[randomIndex];

            // Perform the teleport
            if (m_DebugMode)
            {
                Debug.Log($"[TeleportEffect] Teleporting mine from {sourcePosition} to {targetPosition}");
            }

            // Preserve mine state
            var mineData = mineManager.GetMineDataAt(sourcePosition);
            if (mineData == null) return;

            // Store monster state if it's a monster mine
            int? currentHp = null;
            if (sourceMine is MonsterMine monsterMine)
            {
                currentHp = monsterMine.CurrentHp;
            }

            // Remove mine from current position
            GameEvents.RaiseMineRemovalAttempted(sourcePosition);

            // Add mine to new position
            MonsterType? monsterType = null;
            if (sourceMine is MonsterMine monsterMine2)
            {
                monsterType = monsterMine2.MonsterType;
            }
            GameEvents.RaiseMineAddAttempted(targetPosition, sourceMine.Type, monsterType);

            // Restore monster state if applicable
            if (currentHp.HasValue && mineManager.GetMines().TryGetValue(targetPosition, out var newMine) && newMine is MonsterMine newMonsterMine)
            {
                newMonsterMine.CurrentHp = currentHp.Value;
            }

            // Update grid values
            PropagateValueChanges();

            // Update current position for persistent mode
            if (m_CurrentMode == EffectType.Persistent)
            {
                m_CurrentPosition = targetPosition;
                UnregisterEffectFromArea();
                RegisterEffectInArea(targetPosition, m_Radius, m_Shape);
            }
        }

        private Vector2Int GetEffectivePosition(Vector2Int sourcePosition, GridManager gridManager)
        {
            var gridSize = new Vector2Int(gridManager.Width, gridManager.Height);
            
            return m_TargetPositionType switch
            {
                GridPositionType.Random => new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y)),
                GridPositionType.Edge => GetRandomEdgePosition(gridSize),
                GridPositionType.Corner => GetRandomCornerPosition(gridSize),
                GridPositionType.Center => new Vector2Int(gridSize.x / 2, gridSize.y / 2),
                _ => sourcePosition // Default to source position
            };
        }

        private Vector2Int GetRandomEdgePosition(Vector2Int gridSize)
        {
            bool isHorizontalEdge = Random.value < 0.5f;
            if (isHorizontalEdge)
            {
                int x = Random.Range(0, gridSize.x);
                int y = Random.value < 0.5f ? 0 : gridSize.y - 1;
                return new Vector2Int(x, y);
            }
            else
            {
                int x = Random.value < 0.5f ? 0 : gridSize.x - 1;
                int y = Random.Range(0, gridSize.y);
                return new Vector2Int(x, y);
            }
        }

        private Vector2Int GetRandomCornerPosition(Vector2Int gridSize)
        {
            int x = Random.value < 0.5f ? 0 : gridSize.x - 1;
            int y = Random.value < 0.5f ? 0 : gridSize.y - 1;
            return new Vector2Int(x, y);
        }

        private List<Vector2Int> GetValidTeleportPositions(GridManager gridManager, Vector2Int sourcePosition)
        {
            var validPositions = new List<Vector2Int>();
            var gridSize = new Vector2Int(gridManager.Width, gridManager.Height);

            // Get positions within radius based on shape
            var potentialPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);

            // Filter valid positions
            foreach (var pos in potentialPositions)
            {
                if (IsValidTeleportPosition(pos, gridManager, sourcePosition))
                {
                    validPositions.Add(pos);
                }
            }

            return validPositions;
        }

        private bool IsValidTeleportPosition(Vector2Int position, GridManager gridManager, Vector2Int sourcePosition)
        {
            // Must be different from source position
            if (position == sourcePosition) return false;

            // Must be within grid bounds
            if (!gridManager.IsValidPosition(position)) return false;

            // Must be an unrevealed cell
            var cellObject = gridManager.GetCellObject(position);
            if (cellObject == null) return false;
            
            var cellView = cellObject.GetComponent<CellView>();
            if (cellView == null || cellView.IsRevealed) return false;

            // Must not have a mine already
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (mineManager == null) return false;

            return !mineManager.HasMineAt(position);
        }
        #endregion
    }
} 
using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For GridPositionType

namespace RPGMinesweeper.Effects
{
    public class SummonEffect : ITriggerableEffect
    {
        #region Private Fields
        // Static flag to prevent recursive summoning
        private static bool s_IsSummoning = false;
        private static HashSet<MonsterType> s_CurrentlySummoningTypes = new HashSet<MonsterType>();

        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly MineType m_MineType;
        private readonly MonsterType m_MonsterType;
        private readonly int m_Count;
        private readonly Vector2Int? m_TriggerPosition;
        private readonly GridPositionType m_TriggerPositionType;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Summon";
        #endregion

        public SummonEffect(float radius, GridShape shape, MineType mineType, MonsterType monsterType, int count, Vector2Int? triggerPosition = null, GridPositionType triggerPositionType = GridPositionType.Source)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_MineType = mineType;
            m_MonsterType = monsterType;
            m_Count = count;
            m_TriggerPosition = triggerPosition;
            m_TriggerPositionType = triggerPositionType;
        }

        private Vector2Int GetEffectivePosition(Vector2Int sourcePosition, GridManager gridManager)
        {
            return m_TriggerPosition.HasValue 
                ? m_TriggerPosition.Value
                : GetPositionBasedOnType(sourcePosition, gridManager);
        }
        
        private Vector2Int GetPositionBasedOnType(Vector2Int sourcePosition, GridManager gridManager)
        {
            var gridSize = new Vector2Int(gridManager.Width, gridManager.Height);
            
            return m_TriggerPositionType switch
            {
                GridPositionType.Source => sourcePosition,
                GridPositionType.Random => GetRandomPosition(gridSize),
                GridPositionType.Edge => GetRandomEdgePosition(gridSize),
                GridPositionType.Corner => GetRandomCornerPosition(gridSize),
                GridPositionType.Center => GetCenterPosition(gridSize),
                _ => sourcePosition
            };
        }
        
        private Vector2Int GetRandomPosition(Vector2Int gridSize)
        {
            return new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        }
        
        private Vector2Int GetCenterPosition(Vector2Int gridSize)
        {
            return new Vector2Int(gridSize.x / 2, gridSize.y / 2);
        }

        private Vector2Int GetRandomEdgePosition(Vector2Int gridSize)
        {
            bool isHorizontalEdge = Random.value < 0.5f;
            
            return isHorizontalEdge
                ? new Vector2Int(Random.Range(0, gridSize.x), Random.value < 0.5f ? 0 : gridSize.y - 1)
                : new Vector2Int(Random.value < 0.5f ? 0 : gridSize.x - 1, Random.Range(0, gridSize.y));
        }

        private Vector2Int GetRandomCornerPosition(Vector2Int gridSize)
        {
            int x = Random.value < 0.5f ? 0 : gridSize.x - 1;
            int y = Random.value < 0.5f ? 0 : gridSize.y - 1;
            return new Vector2Int(x, y);
        }

        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            // Prevent recursive summoning of the same monster type
            if (IsRecursiveSummoning())
            {
                Debug.LogWarning($"Prevented recursive summoning of monster type {m_MonsterType}");
                return;
            }

            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            try
            {
                BeginSummoning();
                PlaceMines(sourcePosition, gridManager, mineManager);
                
                // Recalculate values for all affected cells
                MineValuePropagator.PropagateValues(mineManager, gridManager);
            }
            finally
            {
                EndSummoning();
            }
        }
        
        private bool IsRecursiveSummoning()
        {
            return s_IsSummoning && m_MineType == MineType.Monster && s_CurrentlySummoningTypes.Contains(m_MonsterType);
        }
        
        private void BeginSummoning()
        {
            s_IsSummoning = true;
            if (m_MineType == MineType.Monster)
            {
                s_CurrentlySummoningTypes.Add(m_MonsterType);
            }
        }
        
        private void EndSummoning()
        {
            if (m_MineType == MineType.Monster)
            {
                s_CurrentlySummoningTypes.Remove(m_MonsterType);
            }
            if (s_CurrentlySummoningTypes.Count == 0)
            {
                s_IsSummoning = false;
            }
        }
        
        private void PlaceMines(Vector2Int sourcePosition, GridManager gridManager, MineManager mineManager)
        {
            var effectivePosition = GetEffectivePosition(sourcePosition, gridManager);
            var affectedPositions = GridShapeHelper.GetAffectedPositions(effectivePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            
            // Get valid positions for mine placement
            var validPositions = GetValidPositions(affectedPositions, gridManager, mineManager);
            
            // Place the mines at random valid positions
            PlaceRandomMines(validPositions, Mathf.Min(m_Count, validPositions.Count));
        }
        
        private List<Vector2Int> GetValidPositions(IEnumerable<Vector2Int> positions, GridManager gridManager, MineManager mineManager)
        {
            var validPositions = new List<Vector2Int>();
            
            foreach (var pos in positions)
            {
                if (gridManager.IsValidPosition(pos) && !mineManager.HasMineAt(pos))
                {
                    validPositions.Add(pos);
                }
            }
            
            return validPositions;
        }
        
        private void PlaceRandomMines(List<Vector2Int> validPositions, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (validPositions.Count == 0) break;

                int randomIndex = Random.Range(0, validPositions.Count);
                Vector2Int selectedPos = validPositions[randomIndex];
                validPositions.RemoveAt(randomIndex);

                // Raise the appropriate event based on mine type
                if (m_MineType == MineType.Monster)
                {
                    GameEvents.RaiseMineAddAttempted(selectedPos, m_MineType, m_MonsterType);
                }
                else
                {
                    GameEvents.RaiseMineAddAttempted(selectedPos, m_MineType, null);
                }
            }
        }
    }
} 
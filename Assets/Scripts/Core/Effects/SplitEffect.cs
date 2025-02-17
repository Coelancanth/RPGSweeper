using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class SplitEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_HealthModifier;
        private readonly int m_SplitCount;
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly float m_DamageThreshold; // HP ratio threshold that triggers split
        private bool m_IsActive;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Split";
        #endregion

        public SplitEffect(float radius, GridShape shape, float healthModifier, int splitCount, float damageThreshold = 0.5f)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_HealthModifier = healthModifier;
            m_SplitCount = splitCount;
            m_DamageThreshold = Mathf.Clamp01(damageThreshold); // Ensure threshold is between 0 and 1
        }

        #region Protected Methods
        protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            RegisterEffectInArea(sourcePosition, m_Radius, m_Shape);
            // In persistent mode, the actual split happens when the monster is attacked/defeated
            // This is handled by the monster's damage/defeat logic
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            PerformSplit(sourcePosition);
            Remove(target); // Remove after splitting in triggerable mode
        }
        #endregion

        #region Public Methods
        public void Update(float deltaTime)
        {
            // Update is mainly for persistent mode visual feedback if needed
            if (m_CurrentMode == EffectType.Persistent)
            {
                // Add any persistent-specific update logic here
            }
        }

        public void Remove(GameObject target)
        {
            m_IsActive = false;
            UnregisterEffectFromArea();
        }

        // This method can be called by the monster when it takes damage in persistent mode
        public void OnMonsterDamaged(Vector2Int position, float currentHPRatio)
        {
            if (m_CurrentMode != EffectType.Persistent || !m_IsActive) return;
            
            // Only split if the monster's HP ratio falls below the threshold
            if (currentHPRatio <= m_DamageThreshold)
            {
                PerformSplit(position);
                Remove(null); // Remove the effect after splitting
            }
        }
        #endregion

        #region Private Methods
        private void PerformSplit(Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            // Get the source monster's current HP
            if (!mineManager.GetMines().TryGetValue(sourcePosition, out IMine sourceMine) || 
                sourceMine is not MonsterMine sourceMonster) return;

            int currentHP = sourceMonster.CurrentHp;
            int maxHP = sourceMonster.MaxHp;
            
            // Only split if monster has enough HP to survive the split
            if (currentHP <= 0) return;
            
            // Calculate HP after potential split
            int splitHP = Mathf.RoundToInt(currentHP * m_HealthModifier / m_SplitCount);
            
            // Don't split if:
            // 1. Split would result in 0 HP monsters
            // 2. Current HP ratio is already less than what it would be after split
            float currentHPRatio = (float)currentHP / maxHP;
            float splitHPRatio = (float)splitHP / maxHP;
            
            if (splitHP <= 0 || currentHPRatio <= splitHPRatio)
            {
                return;
            }

            // Get valid positions for new monsters
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            var validPositions = new List<Vector2Int>();

            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos) && !mineManager.HasMineAt(pos))
                {
                    validPositions.Add(pos);
                }
            }

            // Spawn new monsters
            for (int i = 0; i < m_SplitCount - 1; i++)  // -1 because original monster counts as one
            {
                if (validPositions.Count == 0) break;

                int randomIndex = Random.Range(0, validPositions.Count);
                Vector2Int selectedPos = validPositions[randomIndex];
                validPositions.RemoveAt(randomIndex);

                // Spawn new monster with modified HP
                GameEvents.RaiseMineAddAttempted(selectedPos, MineType.Monster, sourceMonster.MonsterType);
                
                // Update HP of newly spawned monster
                if (mineManager.GetMines().TryGetValue(selectedPos, out IMine newMine) && 
                    newMine is MonsterMine newMonster)
                {
                    newMonster.MaxHp = splitHP;
                }
            }

            // Update original monster's HP
            sourceMonster.MaxHp = splitHP;

            // Recalculate values for all affected cells
            MineValuePropagator.PropagateValues(mineManager, gridManager);
        }
        #endregion
    }
} 
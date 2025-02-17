using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class SplitEffect : ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly float m_HealthModifier;
        private readonly int m_SplitCount;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Split";
        #endregion

        public SplitEffect(float radius, GridShape shape, float healthModifier, int splitCount)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_HealthModifier = healthModifier;
            m_SplitCount = splitCount;
        }

        public void Apply(GameObject target, Vector2Int sourcePosition)
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
            // This prevents splitting when monster is already damaged significantly
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
    }
} 
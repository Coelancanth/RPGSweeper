using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class SplitEffect : IInstantEffect
    {
        private readonly int m_Radius;
        private readonly float m_HealthModifier;
        private readonly int m_SplitCount;

        public EffectTargetType TargetType => EffectTargetType.Grid;

        public SplitEffect(int radius, float healthModifier, int splitCount)
        {
            m_Radius = radius;
            m_HealthModifier = healthModifier;
            m_SplitCount = splitCount;
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
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
                //Debug.Log($"Split cancelled - Current HP ratio ({currentHPRatio:F2}) <= Split HP ratio ({splitHPRatio:F2})");
                return;
            }
            
            //Debug.Log($"Split Effect - Current HP: {currentHP}, Max HP: {maxHP}, New HP: {splitHP}, Health Modifier: {m_HealthModifier}, Split Count: {m_SplitCount}");
            
            // Get valid positions in radius for spawning split monsters
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, GridShape.Diamond, Mathf.RoundToInt(m_Radius));
            var validPositions = new List<Vector2Int>();

            foreach (var pos in affectedPositions)
            {
                if (pos == sourcePosition) continue; // Skip source position
                if (gridManager.IsValidPosition(pos) && !mineManager.HasMineAt(pos))
                {
                    validPositions.Add(pos);
                }
            }

            // Spawn split monsters
            int monstersToSpawn = Mathf.Min(m_SplitCount - 1, validPositions.Count); // -1 because source transforms into one
            
            // Update source monster's HP
            sourceMonster.MaxHp = splitHP;

            for (int i = 0; i < monstersToSpawn; i++)
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

            // Recalculate values for all affected cells
            MineValuePropagator.PropagateValues(mineManager, gridManager);
        }
    }
} 
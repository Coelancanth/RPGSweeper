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
            
            // Only split if monster has enough HP to survive the split
            if (currentHP <= 0) return;
            
            int newHP = Mathf.RoundToInt(currentHP * m_HealthModifier);
            if (newHP <= 0) return; // Don't split if it would result in 0 HP monsters
            
            Debug.Log($"Split Effect - Current HP: {currentHP}, New HP: {newHP}");
            
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
            // NOTE: not sure how this trick work, so far it works
            int monstersToSpawn = Mathf.Min(m_SplitCount - 1, validPositions.Count); // -1 because source transforms into one
            
            // Update source monster's HP
            // Since setting MaxHp adjusts current HP proportionally, we need to set it to double what we want
            // This way when it's halved by the proportion, it will be the correct value
            sourceMonster.MaxHp = newHP * 2;
            // Now set it to the actual max we want, which will set current HP to newHP
            sourceMonster.MaxHp = newHP;

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
                    // Same trick for the new monsters - set double then actual to get correct current HP
                    newMonster.MaxHp = newHP * 2;
                    newMonster.MaxHp = newHP;
                }
            }

            // Recalculate values for all affected cells
            MineValuePropagator.PropagateValues(mineManager, gridManager);
        }
    }
} 
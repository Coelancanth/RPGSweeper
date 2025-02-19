using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class MonsterTransformEffect : ITriggerableEffect
    {
        #region Private Fields
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private readonly MonsterType m_SourceMonsterType;
        private readonly MonsterType m_TargetMonsterType;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Monster Transform";
        #endregion

        public MonsterTransformEffect(int radius, MonsterType sourceMonsterType, MonsterType targetMonsterType, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_SourceMonsterType = sourceMonsterType;
            m_TargetMonsterType = targetMonsterType;
            m_Shape = shape;
        }

        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);

            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    var cellObject = gridManager.GetCellObject(pos);
                    if (cellObject != null)
                    {
                        var cellView = cellObject.GetComponent<CellView>();
                        if (cellView != null && mineManager.HasMineAt(pos))
                        {
                            var mines = mineManager.GetMines();
                            if (mines.TryGetValue(pos, out var mine) && mine is MonsterMine monsterMine)
                            {
                                if (monsterMine.MonsterType == m_SourceMonsterType)
                                {
                                    // First remove the old mine
                                    GameEvents.RaiseMineRemovalAttempted(pos);
                                    
                                    // Then create the new mine of the target type
                                    GameEvents.RaiseMineAddAttempted(pos, MineType.Monster, m_TargetMonsterType);
                                    
                                    // Notify that an effect was applied at this position
                                    GameEvents.RaiseEffectApplied(pos);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Remove(GameObject source, Vector2Int sourcePosition)
        {
            // Transformation is permanent, no removal logic needed
        }
    }
} 
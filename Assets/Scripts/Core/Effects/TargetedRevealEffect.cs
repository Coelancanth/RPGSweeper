using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class TargetedRevealEffect : ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly MonsterType m_TargetMonsterType;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Targeted Reveal";
        #endregion

        public TargetedRevealEffect(float radius, MonsterType targetMonsterType, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_TargetMonsterType = targetMonsterType;
            m_Shape = shape;
            //Debug.Log($"TargetedRevealEffect created with monster type: {targetMonsterType}, shape: {shape}, radius: {radius}");
        }

        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            //Debug.Log($"TargetedRevealEffect affecting {affectedPositions.Count} positions with shape {m_Shape}");

            // Reveal matching monster cells
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
                            var mine = mineManager.GetMines()[pos];
                            if (mine is MonsterMine monsterMine && monsterMine.MonsterType == m_TargetMonsterType)
                            {
                                var mineData = mineManager.GetMineDataAt(pos);
                                if (mineData != null)
                                {
                                    cellView.ShowMineSprite(mineData.MineSprite, mine, mineData);
                                    cellView.UpdateVisuals(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Remove(GameObject source, Vector2Int sourcePosition)
        {
            // Revealing is permanent, no need for removal logic
        }
    }
} 
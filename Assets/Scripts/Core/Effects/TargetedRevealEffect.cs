using UnityEngine;
using RPGMinesweeper.Effects;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class TargetedRevealEffect : IEffect
    {
        private readonly float m_Duration;
        private readonly float m_Radius;
        private readonly MonsterType m_TargetMonsterType;

        public EffectType Type => EffectType.Reveal;
        public EffectTargetType TargetType => EffectTargetType.Grid;
        public float Duration => m_Duration;

        public TargetedRevealEffect(float duration, float radius, MonsterType targetMonsterType)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_TargetMonsterType = targetMonsterType;
            Debug.Log($"TargetedRevealEffect created with monster type: {targetMonsterType}");
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            //Debug.Log($"TargetedRevealEffect.Apply - Looking for monster type: {m_TargetMonsterType}");
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            for (int x = -Mathf.RoundToInt(m_Radius); x <= m_Radius; x++)
            {
                for (int y = -Mathf.RoundToInt(m_Radius); y <= m_Radius; y++)
                {
                    Debug.Log("Radius: " + m_Radius);
                    //Debug.Log("x: " + x + " y: " + y);
                    var pos = sourcePosition + new Vector2Int(x, y);
                    if (gridManager.IsValidPosition(pos))
                    {
                        var cellObject = gridManager.GetCellObject(pos);
                        if (cellObject != null)
                        {
                            var cellView = cellObject.GetComponent<CellView>();
                            if (cellView != null && mineManager.HasMineAt(pos))
                            {
                                var mine = mineManager.GetMines()[pos];
                                if (mine is MonsterMine monsterMine)
                                {
                                    //Debug.Log($"Found monster mine at {pos} with type: {monsterMine.MonsterType}");
                                    if (monsterMine.MonsterType == m_TargetMonsterType)
                                    {
                                        //Debug.Log($"Revealing monster mine at {pos}");
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
            }
        }

        public void Remove(GameObject source, Vector2Int sourcePosition)
        {
            // Revealing is permanent, no need for removal logic
        }
    }
} 
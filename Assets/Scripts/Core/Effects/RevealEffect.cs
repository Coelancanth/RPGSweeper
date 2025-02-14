using UnityEngine;

namespace RPGMinesweeper.Effects
{
    public class RevealEffect : IEffect
    {
        private readonly float m_Duration;
        private readonly float m_Radius;

        public EffectType Type => EffectType.Reveal;
        public EffectTargetType TargetType => EffectTargetType.Grid;
        public float Duration => m_Duration;

        public RevealEffect(float duration, float radius)
        {
            m_Duration = duration;
            m_Radius = radius;
        }

        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            
            if (gridManager == null || mineManager == null) return;

            for (int x = -Mathf.RoundToInt(m_Radius); x <= m_Radius; x++)
            {
                for (int y = -Mathf.RoundToInt(m_Radius); y <= m_Radius; y++)
                {
                    var pos = sourcePosition + new Vector2Int(x, y);
                    if (gridManager.IsValidPosition(pos))
                    {
                        var cellObject = gridManager.GetCellObject(pos);
                        if (cellObject != null)
                        {
                            var cellView = cellObject.GetComponent<CellView>();
                            if (cellView != null)
                            {
                                // If there's a mine at this position, show its sprite first
                                if (mineManager.HasMineAt(pos))
                                {
                                    var mineData = mineManager.GetMineDataAt(pos);
                                    if (mineData != null)
                                    {
                                        cellView.ShowMineSprite(mineData.MineSprite, mineManager.GetMines()[pos], mineData);
                                    }
                                }
                                // Then reveal the cell
                                cellView.UpdateVisuals(true);
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
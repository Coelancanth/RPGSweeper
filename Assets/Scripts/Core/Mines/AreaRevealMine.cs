using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class AreaRevealMine : BaseMine
    {
        public override bool CanDisguise => true; // Can look like a radar device

        public AreaRevealMine(MineData _data, Vector2Int _position) : base(_data, _position)
        {
        }

        public override void OnTrigger(Player _player)
        {
            if (m_IsDestroyed) return;

            RevealArea();
            GameEvents.RaiseMineTriggered(Type);
            OnDestroy();
        }

        private void RevealArea()
        {
            int radius = Mathf.RoundToInt(m_Data.TriggerRadius);
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        Vector2Int position = m_Position + new Vector2Int(x, y);
                        GameEvents.RaiseCellRevealed(position);
                    }
                }
            }
        }
    }
} 
using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class StandardMine : BaseMine
    {
        public StandardMine(MineData _data, Vector2Int _position) : base(_data, _position)
        {
        }

        public override void OnTrigger(Player _player)
        {
            if (m_IsDestroyed) return;

            _player.TakeDamage(m_Data.Value);
            GameEvents.RaiseMineTriggered(Type);

            foreach (var effect in m_Data.Effects)
            {
                ApplyEffect(effect);
            }

            OnDestroy();
        }
    }
} 
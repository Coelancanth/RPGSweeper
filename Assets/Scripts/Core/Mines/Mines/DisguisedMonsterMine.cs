using UnityEngine;
using RPGMinesweeper;
using System.Collections.Generic;

namespace RPGMinesweeper
{
    public class DisguisedMonsterMine : MonsterMine
    {
        private readonly DisguisedMonsterMineData m_DisguisedData;
        private bool m_IsDisguised;
        private readonly Vector2Int m_Position;
        
        // Additional Events
        public System.Action<Vector2Int, bool> OnDisguiseChanged;

        public DisguisedMonsterMine(DisguisedMonsterMineData data, Vector2Int position) 
            : base(data, position)
        {
            m_DisguisedData = data;
            m_IsDisguised = data.IsDisguised;
            m_Position = position;
        }
        
        // Instead of overriding, we'll use a new property for our own use
        public new MineType Type => MineType.DisguisedMonster;
        
        public bool IsDisguised => m_IsDisguised;
        public Sprite DisguiseSprite => m_DisguisedData.DisguiseSprite;
        public int DisguisedValue => m_DisguisedData.DisguisedValue;
        public Color DisguisedValueColor => m_DisguisedData.DisguisedValueColor;
        
        public void RevealTrueForm(Vector2Int position)
        {
            if (!m_IsDisguised) return;
            
            m_IsDisguised = false;
            OnDisguiseChanged?.Invoke(position, false);
        }
        
        // Instead of overriding, we'll use a new implementation
        public new void OnTrigger(PlayerComponent player)
        {
            // If disguised, just reveal true form on first interaction
            // and don't perform standard monster behavior
            if (m_IsDisguised)
            {
                RevealTrueForm(m_Position);
                return;
            }
            
            // If already revealed, behave like a normal monster
            base.OnTrigger(player);
        }
    }
} 
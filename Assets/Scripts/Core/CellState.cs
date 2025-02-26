using UnityEngine;
using RPGMinesweeper;

namespace RPGMinesweeper
{
    // Class responsible for managing cell state
    public class CellState : ICellData, ICellStateModifier
    {
        private Vector2Int m_Position;
        private bool m_IsRevealed;
        private bool m_IsFrozen;
        private bool m_HasMine;
        private int m_CurrentValue;
        private Color m_CurrentValueColor = Color.white;
        private IMine m_CurrentMine;
        private MineData m_CurrentMineData;
        private CellMarkType m_MarkType = CellMarkType.None;
        
        public event System.Action OnStateChanged;
        
        public Vector2Int Position => m_Position;
        public bool IsRevealed => m_IsRevealed;
        public bool IsFrozen => m_IsFrozen;
        public bool HasMine => m_HasMine;
        public int CurrentValue => m_CurrentValue;
        public Color CurrentValueColor => m_CurrentValueColor;
        public IMine CurrentMine => m_CurrentMine;
        public MineData CurrentMineData => m_CurrentMineData;
        public CellMarkType MarkType => m_MarkType;
        
        public CellState(Vector2Int position)
        {
            m_Position = position;
        }
        
        public void SetRevealed(bool revealed)
        {
            if (m_IsRevealed != revealed)
            {
                m_IsRevealed = revealed;
                
                // Clear any marks when revealing a cell
                if (revealed && m_MarkType != CellMarkType.None)
                {
                    m_MarkType = CellMarkType.None;
                }
                
                OnStateChanged?.Invoke();
            }
        }
        
        public void SetFrozen(bool frozen)
        {
            if (m_IsFrozen != frozen)
            {
                m_IsFrozen = frozen;
                OnStateChanged?.Invoke();
            }
        }
        
        public void SetValue(int value, Color color)
        {
            m_CurrentValue = value;
            m_CurrentValueColor = color;
            OnStateChanged?.Invoke();
        }
        
        public void SetMine(IMine mine, MineData mineData)
        {
            m_HasMine = true;
            m_CurrentMine = mine;
            m_CurrentMineData = mineData;
            OnStateChanged?.Invoke();
        }
        
        public void RemoveMine()
        {
            if (m_HasMine)
            {
                m_HasMine = false;
                m_CurrentMine = null;
                m_CurrentMineData = null;
                OnStateChanged?.Invoke();
            }
        }
        
        public void SetMarkType(CellMarkType markType)
        {
            if (m_MarkType != markType && !m_IsRevealed)
            {
                m_MarkType = markType;
                OnStateChanged?.Invoke();
            }
        }
    }
} 
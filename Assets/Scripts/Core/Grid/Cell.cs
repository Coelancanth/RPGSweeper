using UnityEngine;

public class Cell : ICell
{
    private Vector2Int m_Position;
    private bool m_IsRevealed;

    public Vector2Int Position => m_Position;
    public bool IsRevealed => m_IsRevealed;

    public Cell(Vector2Int _position)
    {
        m_Position = _position;
        m_IsRevealed = false;
    }

    public void Reveal()
    {
        if (!m_IsRevealed)
        {
            m_IsRevealed = true;
            GameEvents.RaiseCellRevealed(m_Position);
        }
    }

    public void Reset()
    {
        m_IsRevealed = false;
    }
} 
using UnityEngine;

public class Grid
{
    private readonly ICell[,] m_Cells;
    private readonly int m_Width;
    private readonly int m_Height;

    public int Width => m_Width;
    public int Height => m_Height;

    public Grid(int _width, int _height)
    {
        m_Width = _width;
        m_Height = _height;
        m_Cells = new ICell[m_Width, m_Height];
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                m_Cells[x, y] = new Cell(new Vector2Int(x, y));
            }
        }
    }

    public bool IsValidPosition(Vector2Int _position)
    {
        return _position.x >= 0 && _position.x < m_Width &&
               _position.y >= 0 && _position.y < m_Height;
    }

    public ICell GetCell(Vector2Int _position)
    {
        if (!IsValidPosition(_position))
            throw new System.ArgumentOutOfRangeException(nameof(_position));
            
        return m_Cells[_position.x, _position.y];
    }
} 
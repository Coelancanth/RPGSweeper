using UnityEngine;
using System.Collections.Generic;

public class RandomMineSpawnStrategy : IMineSpawnStrategy
{
    public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
    {
        Vector2Int position;
        do
        {
            position = new Vector2Int(
                Random.Range(0, gridManager.Width),
                Random.Range(0, gridManager.Height)
            );
        } while (existingMines.ContainsKey(position));

        return position;
    }
}

public class EdgeMineSpawnStrategy : IMineSpawnStrategy
{
    public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
    {
        List<Vector2Int> edgePositions = new List<Vector2Int>();
        
        // Add top and bottom edges
        for (int x = 0; x < gridManager.Width; x++)
        {
            edgePositions.Add(new Vector2Int(x, 0));
            edgePositions.Add(new Vector2Int(x, gridManager.Height - 1));
        }
        
        // Add left and right edges (excluding corners already added)
        for (int y = 1; y < gridManager.Height - 1; y++)
        {
            edgePositions.Add(new Vector2Int(0, y));
            edgePositions.Add(new Vector2Int(gridManager.Width - 1, y));
        }
        
        // Filter out positions that already have mines
        edgePositions.RemoveAll(pos => existingMines.ContainsKey(pos));
        
        // If no edge positions are available, fallback to random position
        if (edgePositions.Count == 0)
        {
            return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
        }
        
        // Return random edge position
        return edgePositions[Random.Range(0, edgePositions.Count)];
    }
} 
using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper
{
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

    public class CornerMineSpawnStrategy : IMineSpawnStrategy
    {
        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            List<Vector2Int> cornerPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),                                     // Bottom-left
                new Vector2Int(gridManager.Width - 1, 0),                // Bottom-right
                new Vector2Int(0, gridManager.Height - 1),               // Top-left
                new Vector2Int(gridManager.Width - 1, gridManager.Height - 1)  // Top-right
            };

            // Filter out positions that already have mines
            cornerPositions.RemoveAll(pos => existingMines.ContainsKey(pos));

            // If no corner positions are available, fallback to random position
            if (cornerPositions.Count == 0)
            {
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }

            // Return random corner position
            return cornerPositions[Random.Range(0, cornerPositions.Count)];
        }
    }

    public class CenterMineSpawnStrategy : IMineSpawnStrategy
    {
        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            // Define the center region (middle 1/3 of the grid)
            int minX = gridManager.Width / 3;
            int maxX = (gridManager.Width * 2) / 3;
            int minY = gridManager.Height / 3;
            int maxY = (gridManager.Height * 2) / 3;

            List<Vector2Int> centerPositions = new List<Vector2Int>();
            
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (!existingMines.ContainsKey(pos))
                    {
                        centerPositions.Add(pos);
                    }
                }
            }

            // If no center positions are available, fallback to random position
            if (centerPositions.Count == 0)
            {
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }

            // Return random center position
            return centerPositions[Random.Range(0, centerPositions.Count)];
        }
    }
} 
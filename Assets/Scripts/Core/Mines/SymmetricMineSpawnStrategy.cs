using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper
{
    public class SymmetricMineSpawnStrategy : IMineSpawnStrategy
    {
        private readonly SymmetryDirection m_Direction;
        private readonly bool m_PlaceAdjacent;
        private Vector2Int? m_PendingPairPosition;
        private int m_CurrentSymmetryLine;

        public SpawnStrategyPriority Priority => SpawnStrategyPriority.Symmetric;

        public SymmetricMineSpawnStrategy(SymmetryDirection direction, bool placeAdjacent = false)
        {
            m_Direction = direction;
            m_PlaceAdjacent = placeAdjacent;
        }

        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            // If we have a pending pair position, return it and clear the pending
            if (m_PendingPairPosition.HasValue)
            {
                Vector2Int position = m_PendingPairPosition.Value;
                m_PendingPairPosition = null;
                return position;
            }

            // Get a random symmetry line position between 1 and gridSize-1
            m_CurrentSymmetryLine = m_Direction == SymmetryDirection.Horizontal
                ? Random.Range(1, gridManager.Height - 1)
                : Random.Range(1, gridManager.Width - 1);
            Debug.Log($"SymmetricMineSpawnStrategy: Current symmetry line: {m_CurrentSymmetryLine}");

            List<(Vector2Int pos1, Vector2Int pos2)> validPairs = new List<(Vector2Int, Vector2Int)>();

            // Find valid positions for symmetric pairs
            if (m_Direction == SymmetryDirection.Horizontal)
            {
                // For horizontal symmetry, mirror across a horizontal line
                for (int x = 0; x < gridManager.Width; x++)
                {
                    // Only search in the lower half of the grid
                    for (int y = 0; y < m_CurrentSymmetryLine; y++)
                    {
                        Vector2Int pos1 = new Vector2Int(x, y);
                        Vector2Int pos2;
                        
                        if (m_PlaceAdjacent)
                        {
                            // Place the second position just below the symmetry line if we're above it,
                            // or just above if we're below it
                            if (y < m_CurrentSymmetryLine - 1)
                            {
                                pos2 = new Vector2Int(x, y + 1);
                            }
                            else
                            {
                                continue; // Skip positions right next to the symmetry line when placing adjacent
                            }
                        }
                        else
                        {
                            // Mirror position: if y is distance D below the line, its pair should be distance D above the line
                            pos2 = new Vector2Int(x, m_CurrentSymmetryLine + (m_CurrentSymmetryLine - y));
                        }

                        if (IsValidPair(pos1, pos2, gridManager, existingMines))
                        {
                            validPairs.Add((pos1, pos2));
                        }
                    }
                }
            }
            else // Vertical symmetry
            {
                // For vertical symmetry, mirror across a vertical line
                // Only search in the left half of the grid
                for (int x = 0; x < m_CurrentSymmetryLine; x++)
                {
                    for (int y = 0; y < gridManager.Height; y++)
                    {
                        Vector2Int pos1 = new Vector2Int(x, y);
                        Vector2Int pos2;

                        if (m_PlaceAdjacent)
                        {
                            // Place the second position just to the right of the first position
                            if (x < m_CurrentSymmetryLine - 1)
                            {
                                pos2 = new Vector2Int(x + 1, y);
                            }
                            else
                            {
                                continue; // Skip positions right next to the symmetry line when placing adjacent
                            }
                        }
                        else
                        {
                            // Mirror position: if x is distance D left of the line, its pair should be distance D right of the line
                            pos2 = new Vector2Int(m_CurrentSymmetryLine + (m_CurrentSymmetryLine - x), y);
                        }

                        if (IsValidPair(pos1, pos2, gridManager, existingMines))
                        {
                            validPairs.Add((pos1, pos2));
                        }
                    }
                }
            }

            if (validPairs.Count == 0)
            {
                Debug.LogWarning($"SymmetricMineSpawnStrategy: No valid positions found for {m_Direction} symmetry at line {m_CurrentSymmetryLine}");
                return Vector2Int.one * -1;
            }

            // Select a random valid pair
            var selectedPair = validPairs[Random.Range(0, validPairs.Count)];
            
            // Store the second position of the pair
            m_PendingPairPosition = selectedPair.pos2;

            return selectedPair.pos1;
        }

        private bool IsValidPair(Vector2Int pos1, Vector2Int pos2, GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            return IsValidPosition(pos1, gridManager) &&
                   IsValidPosition(pos2, gridManager) &&
                   !existingMines.ContainsKey(pos1) &&
                   !existingMines.ContainsKey(pos2);
        }

        private bool IsValidPosition(Vector2Int position, GridManager gridManager)
        {
            return position.x >= 0 && position.x < gridManager.Width &&
                   position.y >= 0 && position.y < gridManager.Height;
        }
    }
} 
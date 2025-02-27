using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper.Factory;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    // Simple priority queue implementation for older C# versions
    internal class SimplePriorityQueue<T>
    {
        private readonly List<(T Item, int Priority)> _items = new();

        public int Count => _items.Count;

        public void Enqueue(T item, int priority)
        {
            _items.Add((item, priority));
            // Sort in descending order so highest priority comes first
            _items.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public T Dequeue()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            var item = _items[0].Item;
            _items.RemoveAt(0);
            return item;
        }

        public void Clear() => _items.Clear();
    }

    public class MineSpawner : IMineSpawner
    {
        private readonly SimplePriorityQueue<(IMineSpawnStrategy Strategy, MineTypeSpawnData Data)> _strategyQueue;
        private readonly Dictionary<SpawnStrategyType, IMineSpawnStrategy> _strategies;

        public event Action<Vector2Int, IMine, MineData> OnMineSpawned;
        
        public MineSpawner()
        {
            _strategyQueue = new SimplePriorityQueue<(IMineSpawnStrategy, MineTypeSpawnData)>();
            _strategies = InitializeBaseStrategies();
        }

        private Dictionary<SpawnStrategyType, IMineSpawnStrategy> InitializeBaseStrategies()
        {
            return new Dictionary<SpawnStrategyType, IMineSpawnStrategy>
            {
                { SpawnStrategyType.Random, new RandomSpawnStrategy() },
                { SpawnStrategyType.Edge, new EdgeSpawnStrategy() },
                { SpawnStrategyType.Center, new CenterSpawnStrategy()},
                { SpawnStrategyType.Corner, new CornerSpawnStrategy()},
                { SpawnStrategyType.Surrounded, new SurroundedSpawnStrategy(MineType.Monster)},
                { SpawnStrategyType.Adjacent, new AdjacentSpawnStrategy()},
                // Other strategies will be implemented later:
                // Corner, Center, Surrounded, Symmetric
            };
        }

        public void PlaceMines(
            List<MineTypeSpawnData> spawnData, 
            IMineFactory mineFactory, 
            GridManager gridManager,
            Dictionary<Vector2Int, IMine> mines, 
            Dictionary<Vector2Int, MineData> mineDataMap)
        {
            var context = new SpawnContext(
                gridManager,
                mineFactory,
                mines,
                mineDataMap,
                spawnData.Where(d => d.IsEnabled).Sum(d => d.SpawnCount)
            );

            InitializeStrategies(spawnData);
            
            // Track all positions that have been successfully spawned to prevent overwriting
            var occupiedPositions = new HashSet<Vector2Int>(mines.Keys);
            
            while (_strategyQueue.Count > 0)
            {
                var (strategy, data) = _strategyQueue.Dequeue();
                
                if (!strategy.CanExecute(context, data))
                {
                    Debug.LogWarning($"Strategy {strategy.Priority} cannot execute - not enough valid positions or requirements not met");
                    continue;
                }

                var result = strategy.Execute(context, data);
                if (!result.Success)
                {
                    Debug.LogWarning($"Strategy {strategy.Priority} failed: {result.ErrorMessage}");
                    continue;
                }

                int successfulSpawns = 0;
                foreach (var spawnedMine in result.Mines)
                {
                    // Skip if this position has already been taken by a higher-priority strategy
                    if (occupiedPositions.Contains(spawnedMine.Position))
                    {
                        // Remove debug log and just skip silently
                        continue;
                    }
                    
                    // Add to context with the strategy's priority and track occupied position
                    context.AddMine(spawnedMine, strategy.Priority);
                    occupiedPositions.Add(spawnedMine.Position);
                    
                    // Add to the provided dictionaries
                    mines[spawnedMine.Position] = spawnedMine.Mine;
                    mineDataMap[spawnedMine.Position] = spawnedMine.MineData;
                    OnMineSpawned?.Invoke(spawnedMine.Position, spawnedMine.Mine, spawnedMine.MineData);
                    successfulSpawns++;
                }
                
                // Log summary of spawning results
                if (successfulSpawns == 0 && result.Mines.Count > 0)
                {
                    Debug.LogWarning($"Strategy {strategy.Priority} found {result.Mines.Count} positions but all were already occupied by higher priority strategies");
                }
                else if (successfulSpawns < data.SpawnCount)
                {
                    Debug.Log($"Strategy {strategy.Priority} spawned {successfulSpawns}/{data.SpawnCount} mines");
                }
            }
        }

        private void InitializeStrategies(List<MineTypeSpawnData> spawnData)
        {
            _strategyQueue.Clear();

            foreach (var data in spawnData.Where(d => d.IsEnabled))
            {
                IMineSpawnStrategy strategy;
                
                if (data.SpawnStrategy == SpawnStrategyType.Surrounded)
                {
                    strategy = new SurroundedSpawnStrategy(data.TargetMineType, data.TargetMonsterType);
                }
                else if (data.SpawnStrategy == SpawnStrategyType.Symmetric)
                {
                    int maxDistance = data.MaxDistanceToLine == 0 ? int.MaxValue : data.MaxDistanceToLine;
                    strategy = new SymmetricSpawnStrategy(
                        data.SymmetryDirection,
                        data.SymmetryLinePosition,
                        data.MinDistanceToLine,
                        maxDistance);
                }
                else if (data.SpawnStrategy == SpawnStrategyType.Adjacent)
                {
                    strategy = new AdjacentSpawnStrategy(
                        maxDistance: data.MaxDistance,
                        minDistance: 2,  // Default minimum distance between clusters
                        allowDiagonal: data.AllowDiagonal,
                        minClusterSize: data.MinClusterSize,
                        maxClusterSize: data.MaxClusterSize,
                        direction: data.AdjacencyDirection);
                }
                else if (data.SpawnStrategy == SpawnStrategyType.Neighbor)
                {
                    strategy = new NeighborSpawnStrategy(data);
                }
                else if (!_strategies.TryGetValue(data.SpawnStrategy, out strategy))
                {
                    Debug.LogWarning($"No strategy found for {data.SpawnStrategy}, falling back to random strategy");
                    strategy = _strategies[SpawnStrategyType.Random];
                }

                // Special handling for monster mines
                if (data.MineData is MonsterMineData monsterData)
                {
                    strategy = new MonsterSpawnStrategy(strategy, monsterData.MonsterType);
                }

                // Use the numeric value of enum for priority - higher values = higher priority
                _strategyQueue.Enqueue((strategy, data), (int)strategy.Priority);
            }
        }
    }
}
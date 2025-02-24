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
            _items.Sort((a, b) => a.Priority.CompareTo(b.Priority));
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
            
            while (_strategyQueue.Count > 0)
            {
                var (strategy, data) = _strategyQueue.Dequeue();
                
                if (!strategy.CanExecute(context, data))
                {
                    continue;
                }

                var result = strategy.Execute(context, data);
                if (!result.Success)
                {
                    Debug.LogWarning($"Strategy failed: {result.ErrorMessage}");
                    continue;
                }

                foreach (var spawnedMine in result.Mines)
                {
                    context.AddMine(spawnedMine);
                    // Add to the provided dictionaries
                    mines[spawnedMine.Position] = spawnedMine.Mine;
                    mineDataMap[spawnedMine.Position] = spawnedMine.MineData;
                    OnMineSpawned?.Invoke(spawnedMine.Position, spawnedMine.Mine, spawnedMine.MineData);
                    //Debug.Log($"Added mine at {spawnedMine.Position} with facing {spawnedMine.MineData.FacingDirection}");
                }
            }
        }

        private void InitializeStrategies(List<MineTypeSpawnData> spawnData)
        {
            //Debug.Log("Initializing strategies");
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
                    // Create a specialized strategy for monster mines if needed
                    //Debug.Log($"Creating monster spawn strategy for {monsterData.MonsterType}");
                    //Debug.Log($"Strategy: {strategy}");
                    strategy = new MonsterSpawnStrategy(strategy, monsterData.MonsterType);
                }

                _strategyQueue.Enqueue((strategy, data), (int)data.SpawnStrategy);
            }
        }
    }
}
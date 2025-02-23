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
                { SpawnStrategyType.Surrounded, new SurroundedSpawnStrategy(MineType.Monster)}, // This will be replaced dynamically
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

                foreach (var mine in result.Mines)
                {
                    context.AddMine(mine);
                    OnMineSpawned?.Invoke(mine.Position, mine.Mine, mine.MineData);
                }
            }
        }

        private void InitializeStrategies(List<MineTypeSpawnData> spawnData)
        {
            Debug.Log("Initializing strategies");
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
                else if (!_strategies.TryGetValue(data.SpawnStrategy, out strategy))
                {
                    Debug.LogWarning($"No strategy found for {data.SpawnStrategy}, falling back to random strategy");
                    strategy = _strategies[SpawnStrategyType.Random];
                }

                // Special handling for monster mines
                if (data.MineData is MonsterMineData monsterData)
                {
                    // Create a specialized strategy for monster mines if needed
                    Debug.Log($"Creating monster spawn strategy for {monsterData.MonsterType}");
                    Debug.Log($"Strategy: {strategy}");
                    strategy = new MonsterSpawnStrategy(strategy, monsterData.MonsterType);
                }

                _strategyQueue.Enqueue((strategy, data), (int)data.SpawnStrategy);
            }
        }
    }
}
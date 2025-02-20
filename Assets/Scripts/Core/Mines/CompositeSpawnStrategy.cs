using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper
{
    public class CompositeSpawnStrategy : IMineSpawnStrategy
    {
        private readonly MineSpawnStrategyType m_Strategies;
        private readonly Dictionary<MineSpawnStrategyType, IMineSpawnStrategy> m_StrategyMap;

        public SpawnStrategyPriority Priority
        {
            get
            {
                // Return the highest priority among active strategies
                var activeStrategies = System.Enum.GetValues(typeof(MineSpawnStrategyType))
                    .Cast<MineSpawnStrategyType>()
                    .Where(s => s != MineSpawnStrategyType.None && s != MineSpawnStrategyType.All && (m_Strategies & s) != 0)
                    .Select(s => m_StrategyMap[s].Priority)
                    .DefaultIfEmpty(SpawnStrategyPriority.Random);

                return activeStrategies.Max();
            }
        }

        public CompositeSpawnStrategy(MineSpawnStrategyType strategies, MineType targetMineType = MineType.Standard, MonsterType? targetMonsterType = null)
        {
            m_Strategies = strategies;
            m_StrategyMap = new Dictionary<MineSpawnStrategyType, IMineSpawnStrategy>
            {
                { MineSpawnStrategyType.Random, new RandomMineSpawnStrategy() },
                { MineSpawnStrategyType.Edge, new EdgeMineSpawnStrategy() },
                { MineSpawnStrategyType.Corner, new CornerMineSpawnStrategy() },
                { MineSpawnStrategyType.Center, new CenterMineSpawnStrategy() },
                { MineSpawnStrategyType.Surrounded, new SurroundedMineSpawnStrategy(targetMineType, targetMonsterType) }
            };
        }

        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            // If it's a single strategy (not a combination), use it directly
            if (m_StrategyMap.ContainsKey(m_Strategies))
            {
                return m_StrategyMap[m_Strategies].GetSpawnPosition(gridManager, existingMines);
            }

            // Get all active strategies based on flags
            var activeStrategies = System.Enum.GetValues(typeof(MineSpawnStrategyType))
                .Cast<MineSpawnStrategyType>()
                .Where(s => s != MineSpawnStrategyType.None && s != MineSpawnStrategyType.All && (m_Strategies & s) != 0)
                .ToList();

            if (activeStrategies.Count == 0)
            {
                // Fallback to random if no strategies are selected
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }

            // Randomly select one of the active strategies
            var selectedStrategy = activeStrategies[Random.Range(0, activeStrategies.Count)];
            if (m_StrategyMap.TryGetValue(selectedStrategy, out var strategy))
            {
                return strategy.GetSpawnPosition(gridManager, existingMines);
            }

            // Fallback to random if strategy not found
            return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
        }
    }
} 
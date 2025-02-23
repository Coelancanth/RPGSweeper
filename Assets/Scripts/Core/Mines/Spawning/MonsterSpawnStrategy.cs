using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class MonsterSpawnStrategy : MineSpawnStrategyBase
    {
        private readonly IMineSpawnStrategy _baseStrategy;
        private readonly MonsterType _monsterType;

        public override SpawnStrategyType Priority => _baseStrategy.Priority;

        public MonsterSpawnStrategy(IMineSpawnStrategy baseStrategy, MonsterType monsterType)
        {
            _baseStrategy = baseStrategy;
            _monsterType = monsterType;
        }

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData data)
        {
            // Ensure we're working with monster mine data
            if (!(data.MineData is MonsterMineData monsterData) || monsterData.MonsterType != _monsterType)
            {
                return false;
            }

            return base.CanExecute(context, data) && _baseStrategy.CanExecute(context, data);
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData data)
        {
            // Additional validation for monster-specific requirements
            if (!(data.MineData is MonsterMineData monsterData) || monsterData.MonsterType != _monsterType)
            {
                return SpawnResult.Failed("Invalid monster mine data");
            }

            // Let the base strategy handle the actual spawning
            return _baseStrategy.Execute(context, data);
        }

        // Use base implementation from MineSpawnStrategyBase
        public new List<SpawnedMine> GetSpawnedMines(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines, int count)
        {
            return base.GetSpawnedMines(gridManager, existingMines, count);
        }
    }
} 
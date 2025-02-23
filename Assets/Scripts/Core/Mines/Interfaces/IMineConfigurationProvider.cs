using System.Collections.Generic;
using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class MineConfiguration
    {
        public List<MineTypeSpawnData> SpawnData { get; }
        public IReadOnlyDictionary<MineType, MineData> MineDataMap { get; }

        public MineConfiguration(List<MineTypeSpawnData> spawnData, IReadOnlyDictionary<MineType, MineData> mineDataMap)
        {
            SpawnData = spawnData;
            MineDataMap = mineDataMap;
        }
    }

    public interface IMineConfigurationProvider
    {
        MineConfiguration GetConfiguration();
        void ValidateConfiguration(int gridWidth, int gridHeight);
    }
} 
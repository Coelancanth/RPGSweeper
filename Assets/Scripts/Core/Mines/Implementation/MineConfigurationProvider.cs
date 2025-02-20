using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines
{
    public class MineConfigurationProvider : IMineConfigurationProvider
    {
        private readonly List<MineTypeSpawnData> m_MineSpawnData;
        private MineConfiguration m_CachedConfiguration;

        public MineConfigurationProvider(List<MineTypeSpawnData> mineSpawnData)
        {
            m_MineSpawnData = mineSpawnData;
        }

        public MineConfiguration GetConfiguration()
        {
            if (m_CachedConfiguration != null) return m_CachedConfiguration;

            // Group by MineType and take the first enabled entry for each type
            var mineDataMap = m_MineSpawnData
                .Where(data => data.IsEnabled)
                .GroupBy(data => data.MineData.Type)
                .ToDictionary(
                    group => group.Key,
                    group => group.First().MineData
                );

            m_CachedConfiguration = new MineConfiguration(m_MineSpawnData, mineDataMap);
            return m_CachedConfiguration;
        }

        public void ValidateConfiguration(int gridWidth, int gridHeight)
        {
            if (m_MineSpawnData == null || m_MineSpawnData.Count == 0)
            {
                Debug.LogError("MineConfigurationProvider: No mine spawn data configured!");
                return;
            }

            int totalMines = m_MineSpawnData
                .Where(data => data.IsEnabled)
                .Sum(data => data.SpawnCount);

            int maxPossibleMines = gridWidth * gridHeight;

            if (totalMines > maxPossibleMines)
            {
                Debug.LogWarning($"MineConfigurationProvider: Total mine count ({totalMines}) exceeds grid capacity ({maxPossibleMines}). Mines will be scaled down proportionally.");
                float scale = (float)maxPossibleMines / totalMines;
                foreach (var data in m_MineSpawnData.Where(d => d.IsEnabled))
                {
                    data.SpawnCount = Mathf.Max(1, Mathf.FloorToInt(data.SpawnCount * scale));
                }
            }

            // Clear cached configuration since we modified the data
            m_CachedConfiguration = null;
        }
    }
} 
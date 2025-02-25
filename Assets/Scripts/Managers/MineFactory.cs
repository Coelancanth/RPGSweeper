using UnityEngine;
using RPGMinesweeper;
using System;

// Add references to the new classes
using RPGMinesweeper.Core.Mines;

namespace RPGMinesweeper.Factory
{
    public interface IMineFactory
    {
        IMine CreateMine(MineData mineData, Vector2Int position);
    }

    public class MineFactory : IMineFactory
    {
        public IMine CreateMine(MineData mineData, Vector2Int position)
        {
            return mineData.Type switch
            {
                MineType.Standard => new StandardMine(mineData, position),
                MineType.Monster => CreateMonsterMine(mineData, position),
                MineType.DisguisedMonster => CreateDisguisedMonsterMine(mineData, position),
                _ => HandleUnknownMineType(mineData, position)
            };
        }

        private IMine CreateMonsterMine(MineData mineData, Vector2Int position)
        {
            if (mineData is MonsterMineData monsterData)
            {
                return new MonsterMine(monsterData, position);
            }
            
            Debug.LogError($"MineManager: MineData for Monster type must be MonsterMineData!");
            return new StandardMine(mineData, position);
        }
        
        private IMine CreateDisguisedMonsterMine(MineData mineData, Vector2Int position)
        {
            if (mineData is DisguisedMonsterMineData disguisedData)
            {
                return new DisguisedMonsterMine(disguisedData, position);
            }
            
            Debug.LogError($"MineManager: MineData for DisguisedMonster type must be DisguisedMonsterMineData!");
            return new StandardMine(mineData, position);
        }

        private IMine HandleUnknownMineType(MineData mineData, Vector2Int position)
        {
            Debug.LogWarning($"Mine type {mineData.Type} not implemented yet. Using StandardMine as fallback.");
            return new StandardMine(mineData, position);
        }
    }
}

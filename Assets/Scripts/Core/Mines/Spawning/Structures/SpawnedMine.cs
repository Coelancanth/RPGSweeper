using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper.Factory;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public struct SpawnedMine
    {
        public Vector2Int Position { get; private set; }
        public IMine Mine { get; private set; }
        public MineData MineData { get; private set; }
        public string DebugInfo { get; set; }

        public static SpawnedMine Create(
            Vector2Int position, 
            IMine mine, 
            MineData mineData,
            FacingDirection facing = FacingDirection.None)
        {
            // Create a copy of the MineData to avoid modifying the original
            var mineDataInstance = UnityEngine.Object.Instantiate(mineData);
            mineDataInstance.InitializeRuntimeFacingDirection();
            if (facing != FacingDirection.None)
            {
                mineDataInstance.FacingDirection = facing;
            }

            return new SpawnedMine
            {
                Position = position,
                Mine = mine,
                MineData = mineDataInstance,
                DebugInfo = string.Empty
            };
        }
    }
}

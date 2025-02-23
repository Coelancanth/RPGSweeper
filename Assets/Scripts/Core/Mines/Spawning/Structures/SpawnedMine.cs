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
        public FacingDirection FacingDirection { get; private set; }

        public static SpawnedMine Create(
            Vector2Int position, 
            IMine mine, 
            MineData mineData,
            FacingDirection facing = FacingDirection.Up)
        {
            return new SpawnedMine
            {
                Position = position,
                Mine = mine,
                MineData = mineData,
                FacingDirection = facing
            };
        }
    }
}

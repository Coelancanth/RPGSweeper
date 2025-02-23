using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper.Factory;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public struct SpawnResult
    {
        public IReadOnlyList<SpawnedMine> Mines { get; private set; }
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }

        public static SpawnResult Successful(IReadOnlyList<SpawnedMine> mines)
        {
            return new SpawnResult
            {
                Mines = mines,
                Success = true,
                ErrorMessage = string.Empty
            };
        }

        public static SpawnResult Failed(string error)
        {
            return new SpawnResult
            {
                Mines = Array.Empty<SpawnedMine>(),
                Success = false,
                ErrorMessage = error
            };
        }
    }
}

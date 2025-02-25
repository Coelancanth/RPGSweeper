// This script ensures all our custom mine types are referenced in the project
using UnityEngine;

namespace RPGMinesweeper
{
    public static class RPGMinesweeperTypes
    {
        // Force the compiler to include these types by referencing them
        private static void EnsureTypesAreIncluded()
        {
            // Log a reference to each type we need included
            Debug.Log(typeof(DisguisedMonsterMine));
            Debug.Log(typeof(DisguisedMonsterMineData));
            Debug.Log(typeof(DisguisedMonsterMineDisplayStrategy));
        }
    }
}

public enum GameState
{
    None,
    Ready,
    Playing,
    Win,
    GameOver
}

public enum CellMarkType
{
    None,
    Flag,
    Question, 
    Numbers,
    CustomInput
}

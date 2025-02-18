namespace RPGMinesweeper.States
{
    public enum StateTarget
    {
        Global,     // Affects the entire game state
        Cell,       // Affects a specific cell
        Mine,       // Affects a specific mine
        Player      // Affects the player
    }
} 
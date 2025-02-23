using System;

namespace RPGMinesweeper
{
    [Flags]
    public enum GridPositionType
    {
        None = 0,
        Random = 1 << 0,
        Edge = 1 << 1,
        Corner = 1 << 2,
        Center = 1 << 3,
        Source = 1 << 4,  // The position where the effect/action originated
        All = Random | Edge | Corner | Center
    }
} 
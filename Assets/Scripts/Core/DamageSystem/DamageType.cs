namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Represents the different types of damage that can be dealt.
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Lightning,
        Poison,
        True  // Ignores resistances
    }
} 
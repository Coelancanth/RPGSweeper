namespace Minesweeper.Core.DamageSystem.Calculation
{
    /// <summary>
    /// Interface for a step in the damage calculation pipeline
    /// </summary>
    public interface IDamageCalculationStep
    {
        /// <summary>
        /// Processes a damage info object and returns the modified version
        /// </summary>
        /// <param name="damageInfo">The damage info to process</param>
        /// <returns>The processed damage info</returns>
        DamageInfo Process(DamageInfo damageInfo);
    }
} 
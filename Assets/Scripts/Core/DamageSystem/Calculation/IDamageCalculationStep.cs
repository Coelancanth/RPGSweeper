namespace Minesweeper.Core.DamageSystem.Calculation
{
    /// <summary>
    /// Interface for steps in the damage calculation pipeline.
    /// Each step processes a damage info object and returns an updated version.
    /// </summary>
    public interface IDamageCalculationStep
    {
        /// <summary>
        /// Process a damage info object and update its values
        /// </summary>
        /// <param name="info">The damage info to process</param>
        /// <returns>The processed damage info</returns>
        DamageInfo Process(DamageInfo info);
    }
} 
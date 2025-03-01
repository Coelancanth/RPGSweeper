namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Interface for processors in the damage calculation pipeline
    /// Each processor handles a specific aspect of damage calculation
    /// </summary>
    public interface IDamageProcessor
    {
        /// <summary>
        /// Process a damage info object, updating its values as needed
        /// </summary>
        /// <param name="damageInfo">The damage info to process</param>
        /// <returns>The processed damage info</returns>
        DamageInfo Process(DamageInfo damageInfo);
    }
    
    /// <summary>
    /// Interface for damage appliers in the Processors namespace
    /// to avoid conflicts with the main IDamageApplier interface
    /// </summary>
    public interface IDamageApplier
    {
        /// <summary>
        /// Apply damage to a target based on damage info
        /// </summary>
        /// <param name="damageInfo">Information about the damage to apply</param>
        /// <returns>True if damage was applied successfully</returns>
        bool ApplyDamage(DamageInfo damageInfo);
    }
} 
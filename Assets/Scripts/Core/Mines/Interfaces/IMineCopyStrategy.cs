using UnityEngine;

namespace RPGMinesweeper.Mines
{
    /// <summary>
    /// Interface defining how to copy properties from one mine to another
    /// </summary>
    public interface IMineCopyStrategy
    {
        /// <summary>
        /// Copies properties from source mine to target mine
        /// </summary>
        /// <param name="sourceMine">The source mine to copy from</param>
        /// <param name="targetMine">The target mine to copy to</param>
        void CopyMineProperties(IMine sourceMine, IMine targetMine);
    }
} 
using System;
using Minesweeper.Core.DamageSystem.Calculation;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Service locator for damage related systems
    /// </summary>
    public static class DamageSystem
    {
        private static IDamageCalculator _calculator;
        private static IDamageApplier _applier;
        
        /// <summary>
        /// Static constructor to initialize the default services
        /// </summary>
        static DamageSystem()
        {
            // Default initialization
            _calculator = new DamageCalculator();
            _applier = new DamageApplier(_calculator);
        }
        
        /// <summary>
        /// The current damage calculator
        /// </summary>
        public static IDamageCalculator Calculator => _calculator;
        
        /// <summary>
        /// The current damage applier
        /// </summary>
        public static IDamageApplier Applier => _applier;
        
        /// <summary>
        /// Set a custom damage calculator
        /// </summary>
        public static void SetCalculator(IDamageCalculator calculator)
        {
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        }
        
        /// <summary>
        /// Set a custom damage applier
        /// </summary>
        public static void SetApplier(IDamageApplier applier)
        {
            _applier = applier ?? throw new ArgumentNullException(nameof(applier));
        }
        
        /// <summary>
        /// Calculate damage without applying it
        /// </summary>
        public static DamageInfo CalculateDamage(DamageInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
                
            return _calculator.Calculate(info);
        }
        
        /// <summary>
        /// Apply pre-calculated damage to a target
        /// </summary>
        public static void ApplyDamage(DamageInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
                
            _applier.ApplyDamage(info);
        }
        
        /// <summary>
        /// Calculate and apply damage in one step
        /// </summary>
        public static void CalculateAndApplyDamage(DamageInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
                
            info = _calculator.Calculate(info);
            _applier.ApplyDamage(info);
        }
    }
} 
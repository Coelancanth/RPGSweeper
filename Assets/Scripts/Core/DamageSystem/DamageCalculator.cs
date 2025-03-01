using System;
using System.Collections.Generic;
using Minesweeper.Core.DamageSystem.Calculation;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Default implementation of IDamageCalculator using a pipeline of calculation steps
    /// </summary>
    public class DamageCalculator : IDamageCalculator
    {
        private readonly List<IDamageCalculationStep> _calculationSteps;
        
        /// <summary>
        /// Creates a default damage calculator with standard calculation steps
        /// </summary>
        public DamageCalculator()
        {
            _calculationSteps = new List<IDamageCalculationStep>
            {
                new BaseDamageProcessor(),
                new SpecialStateProcessor(),
                new ResistanceProcessor()
            };
        }
        
        /// <summary>
        /// Creates a damage calculator with custom calculation steps
        /// </summary>
        /// <param name="steps">The calculation steps to use</param>
        public DamageCalculator(IEnumerable<IDamageCalculationStep> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));
                
            _calculationSteps = new List<IDamageCalculationStep>(steps);
        }
        
        /// <summary>
        /// Add a calculation step to the pipeline
        /// </summary>
        /// <param name="step">The step to add</param>
        public void AddCalculationStep(IDamageCalculationStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
                
            _calculationSteps.Add(step);
        }
        
        /// <summary>
        /// Calculate damage using the calculation pipeline
        /// </summary>
        /// <param name="damageInfo">The damage info to calculate</param>
        /// <returns>The updated damage info with calculated values</returns>
        public DamageInfo Calculate(DamageInfo damageInfo)
        {
            if (damageInfo == null)
                throw new ArgumentNullException(nameof(damageInfo));
                
            // Process through each step in the pipeline
            foreach (var step in _calculationSteps)
            {
                damageInfo = step.Process(damageInfo);
            }
            
            return damageInfo;
        }
    }
} 
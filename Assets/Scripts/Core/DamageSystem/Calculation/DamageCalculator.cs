using System;
using System.Collections.Generic;

namespace Minesweeper.Core.DamageSystem.Calculation
{
    /// <summary>
    /// Main damage calculator that processes a damage info through a pipeline
    /// </summary>
    public class DamageCalculator : IDamageCalculator
    {
        private readonly List<IDamageCalculationStep> _calculationSteps = new List<IDamageCalculationStep>();
        
        // Event for monitoring damage calculations
        public event Action<DamageInfo> OnDamageCalculated;
        
        /// <summary>
        /// Creates a new damage calculator with default calculation steps
        /// </summary>
        public DamageCalculator()
        {
            // Add default calculation steps
            _calculationSteps.Add(new SpecialStateProcessor());
            _calculationSteps.Add(new BaseDamageProcessor());
            _calculationSteps.Add(new ResistanceProcessor());
        }
        
        /// <summary>
        /// Add a custom calculation step to the pipeline
        /// </summary>
        public void AddCalculationStep(IDamageCalculationStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
                
            _calculationSteps.Add(step);
        }
        
        /// <summary>
        /// Calculate the final damage for a given DamageInfo
        /// </summary>
        public DamageInfo Calculate(DamageInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
                
            // Process through all calculation steps
            foreach (var step in _calculationSteps)
            {
                info = step.Process(info);
            }
            
            // Notify listeners
            OnDamageCalculated?.Invoke(info);
            
            return info;
        }
        
        /// <summary>
        /// Clear all calculation steps
        /// </summary>
        public void ClearCalculationSteps()
        {
            _calculationSteps.Clear();
        }
        
        /// <summary>
        /// Insert a calculation step at a specific index
        /// </summary>
        public void InsertCalculationStep(int index, IDamageCalculationStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
                
            _calculationSteps.Insert(index, step);
        }
        
        /// <summary>
        /// Remove a calculation step
        /// </summary>
        public bool RemoveCalculationStep(IDamageCalculationStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
                
            return _calculationSteps.Remove(step);
        }
    }
} 
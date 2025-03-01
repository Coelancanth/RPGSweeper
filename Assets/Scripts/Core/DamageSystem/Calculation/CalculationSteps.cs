using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Calculation
{
    /// <summary>
    /// Processes special states like enrage and critical hits
    /// </summary>
    public class SpecialStateProcessor : IDamageCalculationStep
    {
        public DamageInfo Process(DamageInfo info)
        {
            // Handle enrage state for monsters
            if (info.Source is MonsterEntity monster && info.IsEnraged)
            {
                float enrageMultiplier = monster.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER)?.CurrentValue ?? 1.0f;
                info.EnrageMultiplier = enrageMultiplier;
                info.DamageMultiplier *= enrageMultiplier;
            }
            
            // Handle critical hits
            if (info.IsCritical)
            {
                info.DamageMultiplier *= info.CriticalMultiplier;
            }
            
            return info;
        }
    }

    /// <summary>
    /// Calculates the base damage amount
    /// </summary>
    public class BaseDamageProcessor : IDamageCalculationStep
    {
        public DamageInfo Process(DamageInfo info)
        {
            // Calculate base damage with multipliers
            float rawDamage = (info.BaseDamage + info.DamageAddition) * info.DamageMultiplier;
            
            // Store intermediate result in metadata for debugging
            info.Metadata["RawDamage"] = rawDamage;
            
            return info;
        }
    }

    /// <summary>
    /// Applies resistance calculations to determine final damage
    /// </summary>
    public class ResistanceProcessor : IDamageCalculationStep
    {
        public DamageInfo Process(DamageInfo info)
        {
            if (info.Type == DamageType.True)
            {
                // True damage ignores resistances
                info.FinalDamage = info.Metadata.ContainsKey("RawDamage") 
                    ? (float)info.Metadata["RawDamage"] 
                    : info.BaseDamage * info.DamageMultiplier;
                return info;
            }
                
            float resistanceValue = 0;
            float rawDamage = info.Metadata.ContainsKey("RawDamage") 
                ? (float)info.Metadata["RawDamage"] 
                : info.BaseDamage * info.DamageMultiplier;
            
            // Get the appropriate resistance attribute based on damage type
            if (info.Target != null && AttributeTypes.ResistanceTypes.TryGetValue(info.Type, out var resistanceType))
            {
                resistanceValue = info.Target.GetAttribute(resistanceType)?.CurrentValue ?? 0;
            }
            
            // Apply resistance formula: 
            // Each point of resistance reduces damage by 0.5% (can be adjusted)
            info.ResistanceMultiplier = 1f - (resistanceValue * 0.005f);
            info.ResistanceMultiplier = Mathf.Clamp(info.ResistanceMultiplier, 0.1f, 2.0f); // Resistance can reduce damage by at most 90%
            
            // Calculate final damage
            float finalDamage = rawDamage * info.ResistanceMultiplier;
            
            // For monsters, we want integers only
            if (info.Target is MonsterEntity)
            {
                finalDamage = Mathf.RoundToInt(finalDamage);
            }
            
            info.FinalDamage = Mathf.Max(0, finalDamage);
            return info;
        }
    }
} 
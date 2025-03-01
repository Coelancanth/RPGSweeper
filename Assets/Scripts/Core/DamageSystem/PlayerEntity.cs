using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Entity class specialized for the player character with additional functionality
    /// </summary>
    public class PlayerEntity : Entity
    {
        /// <summary>
        /// Creates a new player entity with the given name
        /// </summary>
        /// <param name="name">The name of the player</param>
        public PlayerEntity(string name = "Player") : base(name)
        {
            InitializeDefaultAttributes();
        }
        
        /// <summary>
        /// Initialize player with default attributes
        /// </summary>
        private void InitializeDefaultAttributes()
        {
            // Core attributes
            AddAttribute(AttributeTypes.MAX_HP, 100);
            AddAttribute(AttributeTypes.CURRENT_HP, 100);
            AddAttribute(AttributeTypes.BASE_DAMAGE, 10);
            
            // Combat attributes
            AddAttribute(AttributeTypes.ATTACK_POWER, 10);
            AddAttribute(AttributeTypes.DEFENSE, 5);
            AddAttribute(AttributeTypes.CRITICAL_CHANCE, 5);   // 5% chance
            AddAttribute(AttributeTypes.CRITICAL_DAMAGE, 150); // 150% damage
            
            // Resistances
            AddAttribute(AttributeTypes.PHYSICAL_RESISTANCE, 0);
            AddAttribute(AttributeTypes.FIRE_RESISTANCE, 0);
            AddAttribute(AttributeTypes.ICE_RESISTANCE, 0);
            AddAttribute(AttributeTypes.POISON_RESISTANCE, 0);
            
            // Set up constraints for current HP
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            var maxHp = GetAttribute(AttributeTypes.MAX_HP);
            
            if (currentHp != null && maxHp != null)
            {
                currentHp.MinValue = 0;
                currentHp.MaxValue = maxHp.CurrentValue;
                
                // Update max value when max HP changes
                maxHp.OnValueChanged += (attr, oldVal, newVal) => {
                    currentHp.MaxValue = newVal;
                };
            }
        }
        
        /// <summary>
        /// Take damage from an external source
        /// </summary>
        /// <param name="damage">Amount of damage to take</param>
        /// <returns>Actual amount of damage taken</returns>
        public float TakeDamage(float damage)
        {
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            if (currentHp == null)
            {
                return 0f;
            }
            
            float oldHp = currentHp.CurrentValue;
            float newHp = Mathf.Max(0, oldHp - damage);
            
            currentHp.SetBaseValue(newHp);
            
            return oldHp - newHp; // Return actual damage taken
        }
        
        /// <summary>
        /// Heal the player by a specified amount
        /// </summary>
        /// <param name="amount">Amount to heal</param>
        /// <returns>Actual amount healed</returns>
        public float Heal(float amount)
        {
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            var maxHp = GetAttribute(AttributeTypes.MAX_HP);
            
            if (currentHp == null || maxHp == null)
            {
                return 0f;
            }
            
            float oldHp = currentHp.CurrentValue;
            float newHp = Mathf.Min(maxHp.CurrentValue, oldHp + amount);
            
            currentHp.SetBaseValue(newHp);
            
            return newHp - oldHp; // Return actual amount healed
        }
        
        /// <summary>
        /// Calculate damage against a target monster
        /// </summary>
        /// <param name="target">Target monster entity</param>
        /// <returns>Calculated damage info</returns>
        public DamageInfo CalculateDamageAgainst(MonsterEntity target)
        {
            return DamageInfoFactory.CreateEntityToEntityDamage(
                this, 
                target, 
                GetAttribute(AttributeTypes.ATTACK_POWER)?.CurrentValue ?? 10
            );
        }
        
        /// <summary>
        /// Deal damage to a target monster
        /// </summary>
        /// <param name="target">Target monster entity</param>
        /// <returns>The final damage dealt</returns>
        public float DealDamageTo(MonsterEntity target)
        {
            if (target == null)
            {
                return 0f;
            }
            
            // Calculate damage
            var damageInfo = CalculateDamageAgainst(target);
            
            // Process and apply damage
            damageInfo = DamageSystem.CalculateAndApplyDamage(damageInfo);
            
            return damageInfo.FinalDamage;
        }
    }
} 
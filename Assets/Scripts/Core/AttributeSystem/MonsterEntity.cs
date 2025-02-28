using UnityEngine;
using RPGMinesweeper;  // For MonsterType

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Entity implementation for monsters
    /// </summary>
    public class MonsterEntity : Entity
    {
        /// <summary>
        /// The monster type
        /// </summary>
        public MonsterType MonsterType { get; }
        
        /// <summary>
        /// Reference to the original monster mine
        /// </summary>
        private readonly object _owner;
        
        /// <summary>
        /// Creates a new monster entity
        /// </summary>
        /// <param name="name">The name of the monster</param>
        /// <param name="monsterType">The type of monster</param>
        /// <param name="owner">The owning MonsterMine instance</param>
        public MonsterEntity(string name, MonsterType monsterType, object owner) : base(name)
        {
            MonsterType = monsterType;
            _owner = owner;
        }
        
        protected override void InitializeAttributes()
        {
            // Base attributes are initialized by MonsterMine through SetAttribute calls
        }
        
        /// <summary>
        /// Applies attributes based on monster type
        /// </summary>
        /// <param name="type">The monster type to apply attributes for</param>
        //public void ApplyMonsterTypeAttributes(MonsterType type)
        //{
            //switch (type)
            //{
                //case MonsterType.Normal:
                    //// No changes to base attributes
                    //break;
                //case MonsterType.Elite:
                    //// Apply elite monster attribute modifiers
                    //var eliteHealthMod = AttributeModifier.CreateMultiplier(AttributeType.MaxHealth, 1.5f, this);
                    //var eliteAttackMod = AttributeModifier.CreateMultiplier(AttributeType.Attack, 1.2f, this);
                    //var eliteDefenseMod = AttributeModifier.CreateMultiplier(AttributeType.Defense, 1.2f, this);
                    //
                    //AddModifier(eliteHealthMod);
                    //AddModifier(eliteAttackMod);
                    //AddModifier(eliteDefenseMod);
                    //break;
                //case MonsterType.Boss:
                    //// Apply boss monster attribute modifiers
                    //var bossHealthMod = AttributeModifier.CreateMultiplier(AttributeType.MaxHealth, 2.5f, this);
                    //var bossAttackMod = AttributeModifier.CreateMultiplier(AttributeType.Attack, 1.5f, this);
                    //var bossDefenseMod = AttributeModifier.CreateMultiplier(AttributeType.Defense, 1.5f, this);
                    //
                    //AddModifier(bossHealthMod);
                    //AddModifier(bossAttackMod);
                    //AddModifier(bossDefenseMod);
                    //break;
            //}
            
            // Update current health to max health
            //var maxHealth = GetAttribute(AttributeType.MaxHealth);
            //var currentHealth = GetAttribute(AttributeType.CurrentHealth);
            //if (maxHealth != null && currentHealth != null)
            //{
                //currentHealth.SetBaseValue(maxHealth.CurrentValue);
            //}
        //}
        
        /// <summary>
        /// Applies enrage state modifiers
        /// </summary>
        public void ApplyEnrageState(float damageMultiplier)
        {
            // Remove any existing enrage modifiers
            RemoveModifiersFromSource("Enrage");
            
            // Apply new enrage damage modifier
            var enrageMod = AttributeModifier.CreateMultiplier(
                AttributeType.Attack, 
                damageMultiplier, 
                "Enrage", 
                100 // High priority
            );
            
            AddModifier(enrageMod);
        }
        
        /// <summary>
        /// Removes enrage state modifiers
        /// </summary>
        public void RemoveEnrageState()
        {
            RemoveModifiersFromSource("Enrage");
        }
        
        public override void ModifyHealth(int amount)
        {
            base.ModifyHealth(amount);
            
            // Sync with owner's HP properties if needed
            // This would be implemented if we want to call back to the owner
        }
    }
} 
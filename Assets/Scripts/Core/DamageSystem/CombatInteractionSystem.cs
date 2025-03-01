using UnityEngine;
using System;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// System for handling complete combat interactions between entities.
    /// This handles bidirectional damage calculation and application.
    /// </summary>
    public static class CombatInteractionSystem
    {
        /// <summary>
        /// Event triggered when a monster damages a player
        /// </summary>
        public static event Action<MonsterEntity, PlayerComponent, float> OnMonsterDamagePlayer;
        
        /// <summary>
        /// Event triggered when a player damages a monster
        /// </summary>
        public static event Action<PlayerComponent, MonsterEntity, float> OnPlayerDamageMonster;
        
        /// <summary>
        /// Event triggered when a monster is defeated
        /// </summary>
        public static event Action<MonsterEntity> OnMonsterDefeated;

        /// <summary>
        /// Defines who attacks first in a combat interaction
        /// </summary>
        public enum AttackOrder
        {
            PlayerFirst,
            MonsterFirst
        }
        
        /// <summary>
        /// Handles a complete combat interaction between a monster and a player.
        /// This represents a player interacting with (clicking on) a monster mine.
        /// </summary>
        /// <param name="monsterEntity">The monster entity</param>
        /// <param name="player">The player component</param>
        /// <param name="attackOrder">Determines who attacks first in the interaction</param>
        /// <returns>A CombatResult object containing information about the interaction</returns>
        public static CombatResult HandleMonsterPlayerInteraction(
            MonsterEntity monsterEntity, 
            PlayerComponent player, 
            AttackOrder attackOrder = AttackOrder.MonsterFirst)
        {
            if (monsterEntity == null || player == null)
            {
                Debug.LogWarning("Invalid entities in HandleMonsterPlayerInteraction");
                return null;
            }
            
            var result = new CombatResult();
            result.AttackOrder = attackOrder;
            
            // Process the combat based on attack order
            if (attackOrder == AttackOrder.PlayerFirst)
            {
                // Player attacks first
                ApplyPlayerAttackToMonster(player, monsterEntity, result);
                
                // Early return if monster was defeated by the player's attack
                if (result.MonsterDefeated)
                {
                    return result;
                }
                
                // Monster counter-attacks
                ApplyMonsterAttackToPlayer(monsterEntity, player, result);
            }
            else
            {
                // Monster attacks first
                ApplyMonsterAttackToPlayer(monsterEntity, player, result);
                
                // Player counter-attacks
                ApplyPlayerAttackToMonster(player, monsterEntity, result);
            }
            
            return result;
        }
        
        /// <summary>
        /// Applies monster's attack against the player and updates the combat result
        /// </summary>
        private static void ApplyMonsterAttackToPlayer(MonsterEntity monsterEntity, PlayerComponent player, CombatResult result)
        {
            // Create damage info for monster attacking player
            var monsterToPlayerDamageInfo = DamageInfoFactory.CreateMonsterToPlayerDamage(
                monsterEntity, 
                player,
                DamageType.Physical
            );
            
            // Calculate damage through the damage system
            monsterToPlayerDamageInfo = DamageSystem.CalculateDamage(monsterToPlayerDamageInfo);
            
            // Apply to player
            int damageToPlayer = Mathf.RoundToInt(monsterToPlayerDamageInfo.FinalDamage);
            player.TakeDamage(damageToPlayer);
            
            // Record in result
            result.DamageToPlayer = damageToPlayer;
            
            // Trigger event
            OnMonsterDamagePlayer?.Invoke(monsterEntity, player, damageToPlayer);
        }
        
        /// <summary>
        /// Applies player's attack against the monster and updates the combat result
        /// </summary>
        private static void ApplyPlayerAttackToMonster(PlayerComponent player, MonsterEntity monsterEntity, CombatResult result)
        {
            // Calculate player damage to monster
            float playerDamageValue = monsterEntity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT)?.CurrentValue ?? 0;
            result.DamageToMonster = Mathf.RoundToInt(playerDamageValue);
            
            // Apply damage to monster
            float actualDamage = monsterEntity.ApplyDamage(playerDamageValue);
            
            // Update result with actual damage
            result.ActualDamageToMonster = Mathf.RoundToInt(actualDamage);
            
            // Trigger event
            OnPlayerDamageMonster?.Invoke(player, monsterEntity, actualDamage);
            
            // Check if monster was defeated
            bool wasDefeated = monsterEntity.GetAttribute(AttributeTypes.CURRENT_HP)?.CurrentValue <= 0;
            result.MonsterDefeated = wasDefeated;
            
            // Trigger defeat event if monster was defeated
            if (wasDefeated)
            {
                OnMonsterDefeated?.Invoke(monsterEntity);
            }
            
            // Update enrage state and record in result
            bool enrageChanged = monsterEntity.UpdateEnrageState();
            result.EnrageStateChanged = enrageChanged;
            result.IsEnraged = monsterEntity.IsEnraged();
        }
    }
    
    /// <summary>
    /// Contains the results of a combat interaction
    /// </summary>
    public class CombatResult
    {
        /// <summary>
        /// The order in which attacks occurred
        /// </summary>
        public CombatInteractionSystem.AttackOrder AttackOrder { get; set; }
        
        /// <summary>
        /// Damage dealt to the player
        /// </summary>
        public int DamageToPlayer { get; set; }
        
        /// <summary>
        /// Expected damage dealt to the monster
        /// </summary>
        public int DamageToMonster { get; set; }
        
        /// <summary>
        /// Actual damage dealt to the monster after calculations
        /// </summary>
        public int ActualDamageToMonster { get; set; }
        
        /// <summary>
        /// Whether the monster was defeated in this interaction
        /// </summary>
        public bool MonsterDefeated { get; set; }
        
        /// <summary>
        /// Whether the monster's enrage state changed
        /// </summary>
        public bool EnrageStateChanged { get; set; }
        
        /// <summary>
        /// The current enrage state of the monster
        /// </summary>
        public bool IsEnraged { get; set; }
    }
} 
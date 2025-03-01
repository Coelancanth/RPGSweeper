using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Basic damage applier that applies damage to entity targets
    /// </summary>
    public class BasicDamageApplier : IDamageApplier
    {
        public bool ApplyDamage(DamageInfo damageInfo)
        {
            if (damageInfo == null || damageInfo.FinalDamage <= 0)
            {
                return false;
            }
            
            // Apply damage to MonsterEntity targets
            if (damageInfo.Target is MonsterEntity monsterEntity)
            {
                float damageApplied = monsterEntity.ApplyDamage(damageInfo.FinalDamage);
                return damageApplied > 0;
            }
            
            // Handle other entity types if needed
            if (damageInfo.Target is Entity entity)
            {
                var currentHp = entity.GetAttribute(AttributeTypes.CURRENT_HP);
                if (currentHp != null)
                {
                    float newHp = Mathf.Max(0, currentHp.CurrentValue - damageInfo.FinalDamage);
                    currentHp.SetBaseValue(newHp);
                    return true;
                }
            }
            
            return false;
        }
    }
} 
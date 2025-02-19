using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class MonsterTransformEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
    {
        #region Private Fields
        private readonly int m_Radius;
        private readonly List<MonsterType> m_SourceMonsterTypes;
        private readonly MonsterType m_TargetMonsterType;
        private readonly int m_MaxTransformCount;
        private readonly GridShape m_Shape;
        private bool m_IsActive;
        private Vector2Int m_CurrentPosition;
        private Dictionary<MonsterType, float> m_SourceHpPercentages = new();
        private bool m_DebugMode = false;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        public EffectType Type => EffectType.Triggerable;
        public string Name => "MonsterTransform";
        #endregion

        #region Constructor
        public MonsterTransformEffect(int radius, List<MonsterType> sourceMonsterTypes, MonsterType targetMonsterType, int maxTransformCount, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_SourceMonsterTypes = sourceMonsterTypes;
            m_TargetMonsterType = targetMonsterType;
            m_MaxTransformCount = maxTransformCount;
            m_Shape = shape;
            m_IsActive = false;
            m_CurrentMode = EffectType.Persistent;
        }
        #endregion

        #region Protected Methods
        protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            m_CurrentPosition = sourcePosition;
            
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (mineManager == null) return;

            var mines = mineManager.GetMines();
            if (!mines.TryGetValue(sourcePosition, out var sourceMine) || !(sourceMine is MonsterMine triggerMonster)) return;

            // Get the HP percentage from the trigger monster (Water Element in this case)
            float triggerHpPercentage = triggerMonster.HpPercentage;
            Debug.Log($"[MonsterTransformEffect] Trigger monster HP percentage: {triggerHpPercentage}");

            TransformMonsters(mineManager, mines, triggerHpPercentage);
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            m_CurrentPosition = sourcePosition;
            
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (mineManager == null) return;

            var mines = mineManager.GetMines();
            if (!mines.TryGetValue(sourcePosition, out var sourceMine) || !(sourceMine is MonsterMine triggerMonster)) return;

            // Get the HP percentage from the trigger monster (Water Element in this case)
            float triggerHpPercentage = triggerMonster.HpPercentage;
            Debug.Log($"[MonsterTransformEffect] Trigger monster HP percentage: {triggerHpPercentage}");

            TransformMonsters(mineManager, mines, triggerHpPercentage);
        }
        #endregion

        #region Public Methods
        public void Update(float deltaTime)
        {
            // No continuous update needed for this effect
        }

        public void Remove(GameObject target)
        {
            m_IsActive = false;
            m_SourceHpPercentages.Clear();
        }
        #endregion

        #region Private Methods
        private void TransformMonsters(MineManager mineManager, IReadOnlyDictionary<Vector2Int, IMine> mines, float triggerHpPercentage)
        {
            Debug.Log($"[MonsterTransformEffect] TransformMonsters called at position {m_CurrentPosition}");
            var affectedPositions = GridShapeHelper.GetAffectedPositions(m_CurrentPosition, m_Shape, m_Radius);
            
            // Get all valid monsters that can be transformed
            var transformableMines = affectedPositions
                .Where(position => mines.TryGetValue(position, out var mine) 
                    && mine is MonsterMine monsterMine 
                    && m_SourceMonsterTypes.Contains(monsterMine.MonsterType))
                .Select(position => (position, mine: mines[position] as MonsterMine))
                .ToList();

            // If max transform count is set, randomly select that many monsters
            if (m_MaxTransformCount > 0 && transformableMines.Count > m_MaxTransformCount)
            {
                // Fisher-Yates shuffle
                for (int i = transformableMines.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    var temp = transformableMines[i];
                    transformableMines[i] = transformableMines[j];
                    transformableMines[j] = temp;
                }
                transformableMines = transformableMines.Take(m_MaxTransformCount).ToList();
            }

            // Transform selected monsters
            foreach (var (position, monsterMine) in transformableMines)
            {
                //Debug.Log($"[MonsterTransformEffect] Transforming monster at {position}");
                //Debug.Log($"[MonsterTransformEffect] Using trigger monster's HP percentage: {triggerHpPercentage}");
                //Debug.Log($"[MonsterTransformEffect] Monster type: {monsterMine.MonsterType}");

                // Remove the old mine
                GameEvents.RaiseMineRemovalAttempted(position);
                
                // Create the new mine of the target type with the trigger monster's HP percentage
                GameEvents.RaiseMineAddAttempted(position, MineType.Monster, m_TargetMonsterType);

                // Set HP percentage on the new monster using the trigger monster's HP percentage
                if (mines.TryGetValue(position, out var newMine) && newMine is MonsterMine newMonsterMine)
                {
                    int newHp = Mathf.RoundToInt(newMonsterMine.MaxHp * triggerHpPercentage);
                    newMonsterMine.CurrentHp = newHp;
                    
                    if (m_DebugMode)
                    {
                        //Debug.Log($"[MonsterTransformEffect] Transformed monster at {position} from {monsterMine.MonsterType} to {m_TargetMonsterType} with HP: {newHp}/{newMonsterMine.MaxHp} ({triggerHpPercentage:P0})");
                    }
                }
            }
        }
        #endregion
    }
} 
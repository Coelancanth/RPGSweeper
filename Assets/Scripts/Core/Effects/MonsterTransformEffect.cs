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
        private readonly MonsterType m_SourceMonsterType;
        private readonly MonsterType m_TargetMonsterType;
        private readonly GridShape m_Shape;
        private bool m_IsActive;
        private Vector2Int m_CurrentPosition;
        private float m_SourceHpPercentage;
        private bool m_DebugMode = false;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        public EffectType Type => EffectType.Triggerable;
        public string Name => "MonsterTransform";
        #endregion

        #region Constructor
        public MonsterTransformEffect(int radius, MonsterType sourceMonsterType, MonsterType targetMonsterType, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_SourceMonsterType = sourceMonsterType;
            m_TargetMonsterType = targetMonsterType;
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
            
            // Get the source monster's HP percentage
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (mineManager == null) return;

            var mines = mineManager.GetMines();
            if (!mines.TryGetValue(sourcePosition, out var sourceMine) || !(sourceMine is MonsterMine monsterMine)) return;

            m_SourceHpPercentage = monsterMine.HpPercentage;
            TransformMonsters(mineManager, mines);
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            m_CurrentPosition = sourcePosition;
            
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (mineManager == null) return;

            var mines = mineManager.GetMines();
            TransformMonsters(mineManager, mines);
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
        }
        #endregion

        #region Private Methods
        private void TransformMonsters(MineManager mineManager, IReadOnlyDictionary<Vector2Int, IMine> mines)
        {
            var affectedPositions = GridShapeHelper.GetAffectedPositions(m_CurrentPosition, m_Shape, m_Radius);
            
            foreach (var position in affectedPositions)
            {
                if (!mines.TryGetValue(position, out var mine) || !(mine is MonsterMine monsterMine)) continue;
                if (monsterMine.MonsterType != m_SourceMonsterType) continue;

                // Remove the old mine
                GameEvents.RaiseMineRemovalAttempted(position);
                
                // Create the new mine of the target type
                GameEvents.RaiseMineAddAttempted(position, MineType.Monster, m_TargetMonsterType);

                if (m_DebugMode)
                {
                    Debug.Log($"[MonsterTransformEffect] Transformed monster at {position} from {m_SourceMonsterType} to {m_TargetMonsterType}");
                }
            }
        }
        #endregion
    }
} 
using UnityEngine;
using System;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType

namespace RPGMinesweeper.Effects
{
    [Serializable]
    public class EffectTemplate
    {
        [Header("Effect Reference")]
        [SerializeField] private EffectData m_Template;

        [Header("Custom Values")]
        [SerializeField] private float m_Duration;
        [SerializeField] private float m_Magnitude;
        [SerializeField] private GridShape m_Shape = GridShape.Single;
        [SerializeField] private int m_Radius = 1;
        [SerializeField] private LayerMask m_TargetLayers;
        [SerializeField] private MonsterType m_TargetMonsterType = MonsterType.None;

        // For passive effects
        [SerializeField] private float m_TickInterval = 1f;

        public EffectData CreateInstance()
        {
            if (m_Template == null) return null;

            var instance = ScriptableObject.CreateInstance(m_Template.GetType()) as EffectData;
            if (instance == null) return null;

            // Copy base values
            instance.Type = m_Template.Type;
            instance.Duration = m_Duration;
            instance.Magnitude = m_Magnitude;
            instance.Shape = m_Shape;
            instance.Radius = m_Radius;
            instance.TargetLayers = m_TargetLayers;

            // Handle specific effect type properties
            if (instance is PassiveEffectData passiveInstance)
            {
                passiveInstance.TickInterval = m_TickInterval;
            }
            else if (instance is TargetedRevealEffectData targetedRevealInstance)
            {
                targetedRevealInstance.SetTargetMonsterType(m_TargetMonsterType);
            }

            return instance;
        }

        #if UNITY_EDITOR
        public void OnValidate()
        {
            if (m_Template != null)
            {
                // Initialize values from template if not yet set
                if (m_Duration == 0) m_Duration = m_Template.Duration;
                if (m_Magnitude == 0) m_Magnitude = m_Template.Magnitude;
                if (m_Shape == GridShape.Single) m_Shape = m_Template.Shape;
                if (m_Radius == 1) m_Radius = m_Template.Radius;
                if (m_TargetLayers == 0) m_TargetLayers = m_Template.TargetLayers;

                if (m_Template is PassiveEffectData passiveTemplate)
                {
                    if (m_TickInterval == 1f) m_TickInterval = passiveTemplate.TickInterval;
                }
                else if (m_Template is TargetedRevealEffectData targetedRevealTemplate)
                {
                    if (m_TargetMonsterType == MonsterType.None) m_TargetMonsterType = targetedRevealTemplate.GetTargetMonsterType();
                }
            }
        }
        #endif
    }
} 
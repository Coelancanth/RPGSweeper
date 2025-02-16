using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    public abstract class EffectData : ScriptableObject
    {
        [Header("Area of Effect")]
        [SerializeField]
        private GridShape m_Shape = GridShape.Single;
        [SerializeField]
        private int m_Radius = 1;
        [SerializeField]
        private LayerMask m_TargetLayers;
        [SerializeField]
        private float m_Duration;
        [SerializeField]
        private float m_Magnitude;

        [Tooltip("Shape of the effect area")]
        public GridShape Shape
        {
            get => m_Shape;
            set => m_Shape = value;
        }
        
        [Tooltip("Radius of the effect")]
        public int Radius
        {
            get => m_Radius;
            set => m_Radius = value;
        }
        
        [Tooltip("Layers that this effect can target")]
        public LayerMask TargetLayers
        {
            get => m_TargetLayers;
            set => m_TargetLayers = value;
        }

        [Tooltip("Duration of the effect (if applicable)")]
        public float Duration
        {
            get => m_Duration;
            set => m_Duration = value;
        }
        
        [Tooltip("Magnitude/strength of the effect (if applicable)")]
        public float Magnitude
        {
            get => m_Magnitude;
            set => m_Magnitude = value;
        }

        public List<Vector2Int> GetAffectedPositions(Vector2Int center)
        {
            return GridShapeHelper.GetAffectedPositions(center, Shape, Radius);
        }

        public bool IsPositionAffected(Vector2Int position, Vector2Int center)
        {
            return GridShapeHelper.IsPositionAffected(position, center, Shape, Radius);
        }

        public abstract IEffect CreateEffect();
    }
}

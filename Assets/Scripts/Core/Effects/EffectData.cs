using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    public abstract class EffectData : ScriptableObject
    {
        [Header("Area of Effect")]
        [Tooltip("Shape of the effect area")]
        public GridShape Shape = GridShape.Single;
        
        [Tooltip("Radius of the effect")]
        public int Radius = 1;
        
        [Header("Target Settings")]
        [Tooltip("Layers that this effect can target")]
        public LayerMask TargetLayers;

        [Header("Optional Properties")]
        [Tooltip("Duration of the effect (if applicable)")]
        public float Duration;
        
        [Tooltip("Magnitude/strength of the effect (if applicable)")]
        public float Magnitude;

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

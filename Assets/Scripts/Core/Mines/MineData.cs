using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper.Core.Mines
{
    [CreateAssetMenu(fileName = "MineData", menuName = "RPGMinesweeper/MineData")]
    public class MineData : ScriptableObject
    {
        [Header("Basic Settings")]
        [Tooltip("Type of the mine")]
        public MineType Type;

        [Tooltip("Value of the mine (damage/healing/experience)")]
        public int Value;

        [Tooltip("Radius for triggering area effects")]
        [Min(0)]
        public float TriggerRadius;

        [Header("Effects")]
        [Tooltip("List of effects applied when triggered")]
        public List<EffectData> Effects = new List<EffectData>();

        [Header("Shape Settings")]
        [Tooltip("Shape pattern of the mine")]
        public MineShape Shape = MineShape.Single;

        [Tooltip("Range of the shape pattern")]
        [Min(1)]
        public int ShapeRange = 1;

        [Header("Visual Settings")]
        [Tooltip("Sprite to display when the mine is revealed")]
        [SerializeField] private Sprite m_MineSprite;

        public Sprite MineSprite => m_MineSprite;

        public List<Vector2Int> GetShapePositions(Vector2Int center)
        {
            return MineShapeHelper.GetShapePositions(center, Shape, ShapeRange);
        }

        public bool IsPositionInShape(Vector2Int position, Vector2Int center)
        {
            return MineShapeHelper.IsPositionInShape(position, center, Shape, ShapeRange);
        }
    }
} 
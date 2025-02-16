using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    public abstract class EffectData : ScriptableObject
    {
        [Header("Basic Properties")]
        public EffectType Type;
        public float Duration;
        public float Magnitude;
        
        [Header("Area of Effect")]
        public GridShape Shape = GridShape.Single;
        public int Radius = 1;
        public LayerMask TargetLayers;

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

    //[CreateAssetMenu(fileName = "PassiveEffect", menuName = "RPGMinesweeper/Effects/PassiveEffect")]
    //public class PassiveEffectData : EffectData
    //{
        //[Header("Passive Behavior")]
        //public float TickInterval = 1f;

        //public override IEffect CreateEffect()
        //{
            ////switch (Type)
            ////{
                ////case EffectType.Confusion:
                    ////return new ConfusionEffect(Duration, Radius, Shape);
                ////default:
                    ////Debug.LogWarning($"Effect type {Type} not implemented as passive effect");
                    ////return null;
            ////}
        //}
    }

    //[CreateAssetMenu(fileName = "ActiveEffect", menuName = "RPGMinesweeper/Effects/ActiveEffect")]
    //public class ActiveEffectData : EffectData
    //{
        //[Header("Monster Targeting")]
        //[Tooltip("For TargetedReveal effect, specify which monster type to reveal")]
        //[SerializeField] private MonsterType m_TargetMonsterType = MonsterType.None;

        //public override IEffect CreateEffect()
        //{
            //switch (Type)
            //{
                //case EffectType.Reveal:
                    //return new RevealEffect(Duration, Radius);
                ////case EffectType.TargetedReveal:
                    ////return new TargetedRevealEffect(Duration, Radius, m_TargetMonsterType);
                //default:
                    //Debug.LogWarning($"Effect type {Type} not implemented as active effect");
                    //return null;
            //}
        //}
    //}
//} 
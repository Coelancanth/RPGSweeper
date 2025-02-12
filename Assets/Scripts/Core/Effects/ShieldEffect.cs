//using UnityEngine;

//namespace RPGMinesweeper.Effects
//{
    //public class ShieldEffect : IPassiveEffect
    //{
        //private readonly float m_Duration;
        //private readonly float m_Magnitude;
        //private float m_ElapsedTime;

        //public EffectType Type => EffectType.Shield;
        //public float Duration => m_Duration;

        //public ShieldEffect(float duration, float magnitude)
        //{
            //m_Duration = duration;
            //m_Magnitude = magnitude;
            //m_ElapsedTime = 0f;
        //}

        //public void Apply(GameObject target)
        //{
            //if (target.TryGetComponent<PlayerComponent>(out var player))
            //{
                //player.AddShield(Mathf.RoundToInt(m_Magnitude));
            //}
        //}

        //public void Remove(GameObject target)
        //{
            //if (target.TryGetComponent<PlayerComponent>(out var player))
            //{
                //player.RemoveShield(Mathf.RoundToInt(m_Magnitude));
            //}
        //}

        //public void OnTick(GameObject target)
        //{
            //m_ElapsedTime += Time.deltaTime;
            //if (m_ElapsedTime >= m_Duration)
            //{
                //Remove(target);
            //}
        //}
    //}
//} 
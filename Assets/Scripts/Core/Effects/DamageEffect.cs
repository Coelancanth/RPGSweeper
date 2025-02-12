//using UnityEngine;

//namespace RPGMinesweeper.Effects
//{
    //public class DamageEffect : IPassiveEffect
    //{
        //private readonly float m_Duration;
        //private readonly float m_Magnitude;
        //private float m_ElapsedTime;

        //public EffectType Type => EffectType.Damage;
        //public float Duration => m_Duration;

        //public DamageEffect(float duration, float magnitude)
        //{
            //m_Duration = duration;
            //m_Magnitude = magnitude;
            //m_ElapsedTime = 0f;
        //}

        //public void Apply(GameObject target)
        //{
            //// Initial damage
            //if (target.TryGetComponent<PlayerComponent>(out var player))
            //{
                //player.TakeDamage(Mathf.RoundToInt(m_Magnitude));
            //}
        //}

        //public void Remove(GameObject target)
        //{
            //// Damage is already applied, no need for removal logic
        //}

        //public void OnTick(GameObject target)
        //{
            //if (target.TryGetComponent<PlayerComponent>(out var player))
            //{
                //player.TakeDamage(Mathf.RoundToInt(m_Magnitude * Time.deltaTime));
            //}
        //}
    //}
//} 
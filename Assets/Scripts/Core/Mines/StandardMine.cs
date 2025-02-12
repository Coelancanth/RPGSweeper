using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;

public class StandardMine : IMine
{
    #region Private Fields
    private readonly MineData m_Data;
    private readonly Vector2Int m_Position;
    private readonly List<Vector2Int> m_AffectedPositions;
    private readonly List<IPassiveEffect> m_ActivePassiveEffects = new();
    private float m_ElapsedTime;
    private GameObject m_GameObject;
    #endregion

    #region Public Properties
    public MineType Type => m_Data.Type;
    public bool CanDisguise => true;
    #endregion

    #region Constructor
    public StandardMine(MineData _data, Vector2Int _position)
    {
        m_Data = _data;
        m_Position = _position;
        m_AffectedPositions = GridShapeHelper.GetAffectedPositions(m_Position, m_Data.Shape, m_Data.Radius);
        InitializePassiveEffects();
    }
    #endregion

    #region IMine Implementation
    public void OnTrigger(PlayerComponent _player)
    {
        _player.TakeDamage(m_Data.Value);
        
        if (m_Data.PassiveEffects != null)
        {
            foreach (var effectData in m_Data.PassiveEffects)
            {
                var effect = effectData.CreateEffect() as IPassiveEffect;
                if (effect != null)
                {
                    effect.Apply(_player.gameObject, m_Position);
                    m_ActivePassiveEffects.Add(effect);
                }
            }
        }
    }

    public void OnDestroy()
    {
        if (m_Data.ActiveEffects != null)
        {
            //Debug.Log("StandardMine: OnDestroy");
            var player = GameObject.FindFirstObjectByType<PlayerComponent>();
            if (player != null)
            {
                //Debug.Log("StandardMine: OnDestroy: Player found");
                foreach (var effectData in m_Data.ActiveEffects)
                {
                    var effect = effectData.CreateEffect();
                    if (effect != null)
                    {
                        effect.Apply(player.gameObject, m_Position);
                    }
                }
            }
        }

        // Clean up passive effects
        foreach (var effect in m_ActivePassiveEffects)
        {
            effect.Remove(GameObject.FindFirstObjectByType<PlayerComponent>()?.gameObject, m_Position);
        }
        m_ActivePassiveEffects.Clear();
    }

    public void Update(float deltaTime)
    {
        m_ElapsedTime += deltaTime;
        
        foreach (var effect in m_ActivePassiveEffects)
        {
            var player = GameObject.FindFirstObjectByType<PlayerComponent>();
            if (player != null)
            {
                effect.OnTick(player.gameObject, m_Position);
            }
        }
    }
    #endregion

    #region Private Methods
    private void InitializePassiveEffects()
    {
        if (m_Data.PassiveEffects == null) return;
        
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player == null) return;

        foreach (var effectData in m_Data.PassiveEffects)
        {
            var effect = effectData.CreateEffect() as IPassiveEffect;
            if (effect != null)
            {
                effect.Apply(player.gameObject, m_Position);
                m_ActivePassiveEffects.Add(effect);
            }
        }
    }
    #endregion
}
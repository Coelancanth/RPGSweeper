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
    private readonly List<IDurationalEffect> m_ActiveDurationalEffects = new();
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
        InitializeDurationalEffects();
    }
    #endregion

    #region IMine Implementation
    public void OnTrigger(PlayerComponent _player)
    {
        // Standard mines only apply effects, they don't deal damage
        foreach (var effect in m_Data.CreatePassiveEffects())
        {
            if (effect is IDurationalEffect durationalEffect)
            {
                durationalEffect.Apply(_player.gameObject, m_Position);
                m_ActiveDurationalEffects.Add(durationalEffect);
            }
            else
            {
                effect.Apply(_player.gameObject, m_Position);
            }
        }
    }

    public void OnDestroy()
    {
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player != null)
        {
            foreach (var effect in m_Data.CreateActiveEffects())
            {
                effect.Apply(player.gameObject, m_Position);
            }
        }

        // Clean up durational effects
        foreach (var effect in m_ActiveDurationalEffects)
        {
            effect.Remove(GameObject.FindFirstObjectByType<PlayerComponent>()?.gameObject, m_Position);
        }
        m_ActiveDurationalEffects.Clear();
    }

    public void Update(float deltaTime)
    {
        m_ElapsedTime += deltaTime;
        
        // Check for expired effects
        for (int i = m_ActiveDurationalEffects.Count - 1; i >= 0; i--)
        {
            var effect = m_ActiveDurationalEffects[i];
            if (m_ElapsedTime >= effect.Duration)
            {
                var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                if (player != null)
                {
                    effect.Remove(player.gameObject, m_Position);
                }
                m_ActiveDurationalEffects.RemoveAt(i);
            }
        }
    }
    #endregion

    #region Private Methods
    private void InitializeDurationalEffects()
    {
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player == null) return;

        foreach (var effect in m_Data.CreatePassiveEffects())
        {
            if (effect is IDurationalEffect durationalEffect)
            {
                durationalEffect.Apply(player.gameObject, m_Position);
                m_ActiveDurationalEffects.Add(durationalEffect);
            }
            else
            {
                effect.Apply(player.gameObject, m_Position);
            }
        }
    }
    #endregion
}
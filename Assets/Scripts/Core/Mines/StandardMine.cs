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
    private readonly List<IPersistentEffect> m_ActivePersistentEffects = new();
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
        InitializePersistentEffects();
    }
    #endregion

    #region IMine Implementation
    public void OnTrigger(PlayerComponent _player)
    {
        // Standard mines only apply effects, they don't deal damage
        foreach (var effect in m_Data.CreatePersistentEffects())
        {
            if (effect is IPersistentEffect persistentEffect)
            {
                persistentEffect.Apply(_player.gameObject, m_Position);
                m_ActivePersistentEffects.Add(persistentEffect);
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
            foreach (var effect in m_Data.CreateTriggerableEffects())
            {
                effect.Apply(player.gameObject, m_Position);
            }
        }

        // Clean up persistent effects
        foreach (var effect in m_ActivePersistentEffects)
        {
            effect.Remove(player?.gameObject);
        }
        m_ActivePersistentEffects.Clear();
    }

    public void Update(float deltaTime)
    {
        m_ElapsedTime += deltaTime;
        
        // Update persistent effects
        foreach (var effect in m_ActivePersistentEffects)
        {
            effect.Update(deltaTime);
            if (!effect.IsActive)
            {
                var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                if (player != null)
                {
                    effect.Remove(player.gameObject);
                }
            }
        }
        
        // Remove inactive effects
        m_ActivePersistentEffects.RemoveAll(effect => !effect.IsActive);
    }
    #endregion

    #region Private Methods
    private void InitializePersistentEffects()
    {
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player == null) return;

        foreach (var effect in m_Data.CreatePersistentEffects())
        {
            if (effect is IPersistentEffect persistentEffect)
            {
                persistentEffect.Apply(player.gameObject, m_Position);
                m_ActivePersistentEffects.Add(persistentEffect);
            }
            else
            {
                effect.Apply(player.gameObject, m_Position);
            }
        }
    }
    #endregion
}
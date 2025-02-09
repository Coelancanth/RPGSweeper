using UnityEngine;
using System.Collections.Generic;

public class StandardMine : IMine
{
    #region Private Fields
    private readonly MineData m_Data;
    private readonly Vector2Int m_Position;
    private readonly List<Vector2Int> m_AffectedPositions;
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
        m_AffectedPositions = MineShapeHelper.GetShapePositions(m_Position, m_Data.Shape, m_Data.Radius);
    }
    #endregion

    #region IMine Implementation
    public void OnTrigger(Player _player)
    {
        _player.TakeDamage(m_Data.Value);
        
        if (m_Data.Effects != null)
        {
            foreach (var effect in m_Data.Effects)
            {
                ApplyEffect(_player, effect);
            }
        }
    }

    public void OnDestroy()
    {
        // Standard mine has no special destroy behavior
    }
    #endregion

    #region Private Methods
    private void ApplyEffect(Player _player, EffectData _effect)
    {
        switch (_effect.Type)
        {
            case EffectType.Heal:
                _player.TakeDamage(-Mathf.RoundToInt(_effect.Magnitude));
                break;
            case EffectType.Transform:
                // Transform effects should be handled by a separate system
                break;
            case EffectType.Reveal:
                // Reveal effects should be handled by the grid system
                break;
            case EffectType.SpawnItem:
                // Item spawning should be handled by an item system
                break;
        }
    }
    #endregion
}
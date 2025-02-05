using UnityEngine;

public class AreaRevealMine : BaseMine
{
    public AreaRevealMine(MineData _data, Vector2Int _position) : base(_data, _position)
    {
    }

    public override void OnTrigger(Player _player)
    {
        if (m_IsDestroyed) return;

        base.OnTrigger(_player);
        RevealArea();
    }

    private void RevealArea()
    {
        int radius = Mathf.RoundToInt(m_Data.TriggerRadius);
        
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int position = m_Position + new Vector2Int(x, y);
                GameEvents.RaiseEffectApplied(position);
            }
        }
    }
} 
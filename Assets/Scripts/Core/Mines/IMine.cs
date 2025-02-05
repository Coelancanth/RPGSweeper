using UnityEngine;

public interface IMine
{
    MineType Type { get; }
    bool CanDisguise { get; }
    void OnTrigger(Player player);
    void OnDestroy();
} 
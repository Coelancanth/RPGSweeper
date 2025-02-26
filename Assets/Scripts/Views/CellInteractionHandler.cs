using UnityEngine;
using RPGMinesweeper;

public interface ICellInteractionHandler
{
    bool CanInteract { get; }
    void OnInteract();
}

public class CellInteractionHandler : ICellInteractionHandler
{
    private ICellData m_CellData;
    private bool m_DebugMode;
    private ICellVisualUpdater m_VisualUpdater;
    
    public bool CanInteract => !m_CellData.IsFrozen && !m_CellData.IsRevealed;
    
    public CellInteractionHandler(ICellData cellData, bool debugMode, ICellVisualUpdater visualUpdater = null)
    {
        m_CellData = cellData;
        m_DebugMode = debugMode;
        m_VisualUpdater = visualUpdater;
    }
    
    public void OnInteract()
    {
        if (CanInteract)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellInteractionHandler] Revealing cell at {m_CellData.Position}, CanInteract: {CanInteract}, IsFrozen: {m_CellData.IsFrozen}, IsRevealed: {m_CellData.IsRevealed}");
            }
            GameEvents.RaiseCellRevealed(m_CellData.Position);
        }
        else if (m_CellData.IsRevealed && m_CellData.HasMine)
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellInteractionHandler] Interacting with revealed mine at {m_CellData.Position}, Mine type: {m_CellData.CurrentMine?.GetType().Name}");
            }
            
            // Special handling for disguised monster mines
            var disguisedMonsterMine = m_CellData.CurrentMine as DisguisedMonsterMine;
            if (disguisedMonsterMine != null)
            {
                if (disguisedMonsterMine.IsDisguised)
                {
                    // Let the mine reveal itself and update visuals
                    var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                    if (player != null)
                    {
                        disguisedMonsterMine.OnTrigger(player);
                        
                        // Explicitly update visuals after the monster is triggered
                        // This ensures the visual state is updated when a monster is defeated
                        m_VisualUpdater?.UpdateVisuals();
                    }
                    
                    return;
                }
                else if (disguisedMonsterMine.IsDefeated)
                {
                    // For defeated disguised monsters
                    if (disguisedMonsterMine.IsCollectable)
                    {
                        // Only collect rewards and remove if it's collectable
                        Debug.Log($"[CellInteractionHandler] Collecting rewards for defeated disguised monster at {m_CellData.Position}");
                        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                        if (player != null)
                        {
                            disguisedMonsterMine.OnDestroy();
                        }
                        
                        // Then raise the event to remove the mine
                        GameEvents.RaiseMineRemovalAttempted(m_CellData.Position);
                    }
                    else
                    {
                        // Otherwise just trigger to make it collectable
                        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                        if (player != null)
                        {
                            disguisedMonsterMine.OnTrigger(player);
                            m_VisualUpdater?.UpdateVisuals();
                        }
                    }
                    return;
                }
                else
                {
                    var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                    if (player != null)
                    {
                        disguisedMonsterMine.OnTrigger(player);
                        
                        // Explicitly update visuals after the monster is triggered
                        m_VisualUpdater?.UpdateVisuals();
                    }
                    return;
                }
            }
            
            // For regular monster mines, process interaction on every click
            var monsterMine = m_CellData.CurrentMine as MonsterMine;
            if (monsterMine != null)
            {
                if (monsterMine.IsDefeated)
                {
                    // For defeated monsters
                    if (monsterMine.IsCollectable)
                    {
                        // Only collect rewards and remove if it's collectable
                        Debug.Log($"[CellInteractionHandler] Collecting rewards for defeated monster at {m_CellData.Position}");
                        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                        if (player != null)
                        {
                            monsterMine.OnDestroy();
                        }
                        
                        // Then raise the event to remove the mine
                        GameEvents.RaiseMineRemovalAttempted(m_CellData.Position);
                    }
                    else
                    {
                        // Otherwise just trigger to make it collectable
                        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                        if (player != null)
                        {
                            monsterMine.OnTrigger(player);
                            m_VisualUpdater?.UpdateVisuals();
                        }
                    }
                }
                else
                {
                    var player = GameObject.FindFirstObjectByType<PlayerComponent>();
                    if (player != null)
                    {
                        monsterMine.OnTrigger(player);
                        
                        // Explicitly update visuals after the monster is triggered
                        // This helps update the visual state if the monster becomes collectable
                        m_VisualUpdater?.UpdateVisuals();
                    }
                }
            }
            else
            {
                GameEvents.RaiseMineRemovalAttempted(m_CellData.Position);
            }
        }
        else
        {
            if (m_DebugMode)
            {
                Debug.Log($"[CellInteractionHandler] Cannot interact with cell at {m_CellData.Position}, CanInteract: {CanInteract}, IsFrozen: {m_CellData.IsFrozen}, IsRevealed: {m_CellData.IsRevealed}");
            }
        }
    }
} 
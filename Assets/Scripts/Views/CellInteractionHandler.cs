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
        // Handle different interaction scenarios based on cell state
        switch (GetInteractionType())
        {
            case InteractionType.RevealCell:
                LogDebugMessage($"Revealing cell at {m_CellData.Position}, CanInteract: {CanInteract}, IsFrozen: {m_CellData.IsFrozen}, IsRevealed: {m_CellData.IsRevealed}");
                GameEvents.RaiseCellRevealed(m_CellData.Position);
                break;
                
            case InteractionType.DisguisedMonsterTrigger:
                HandleDisguisedMonsterInteraction();
                break;
                
            case InteractionType.MonsterTrigger:
                HandleMonsterInteraction();
                break;
                
            case InteractionType.StandardMineTrigger:
                HandleStandardMineInteraction();
                break;
                
            case InteractionType.None:
                LogDebugMessage($"Cannot interact with cell at {m_CellData.Position}, CanInteract: {CanInteract}, IsFrozen: {m_CellData.IsFrozen}, IsRevealed: {m_CellData.IsRevealed}");
                break;
        }
    }
    
    private enum InteractionType
    {
        None,
        RevealCell,
        DisguisedMonsterTrigger,
        MonsterTrigger,
        StandardMineTrigger
    }
    
    private InteractionType GetInteractionType()
    {
        if (CanInteract)
            return InteractionType.RevealCell;
            
        if (m_CellData.IsRevealed && m_CellData.HasMine)
        {
            if (m_CellData.CurrentMine is DisguisedMonsterMine)
                return InteractionType.DisguisedMonsterTrigger;
                
            if (m_CellData.CurrentMine is MonsterMine)
                return InteractionType.MonsterTrigger;
                
            if (m_CellData.CurrentMine is StandardMine)
                return InteractionType.StandardMineTrigger;
        }
        
        return InteractionType.None;
    }
    
    private void HandleDisguisedMonsterInteraction()
    {
        LogDebugMessage($"Interacting with disguised monster at {m_CellData.Position}");
        
        var disguisedMonsterMine = m_CellData.CurrentMine as DisguisedMonsterMine;
        var player = GetPlayer();
        
        if (disguisedMonsterMine.IsDisguised)
        {
            disguisedMonsterMine.OnTrigger(player);
            m_VisualUpdater?.UpdateVisuals();
        }
        else if (disguisedMonsterMine.IsDefeated)
        {
            if (disguisedMonsterMine.IsCollectable)
            {
                CollectAndRemoveMine(disguisedMonsterMine);
            }
            else
            {
                TriggerMonster(disguisedMonsterMine, player);
            }
        }
        else
        {
            TriggerMonster(disguisedMonsterMine, player);
        }
    }
    
    private void HandleMonsterInteraction()
    {
        var monsterMine = m_CellData.CurrentMine as MonsterMine;
        var player = GetPlayer();
        
        if (monsterMine.IsDefeated)
        {
            if (monsterMine.IsCollectable)
            {
                CollectAndRemoveMine(monsterMine);
            }
            else
            {
                TriggerMonster(monsterMine, player);
            }
        }
        else
        {
            TriggerMonster(monsterMine, player);
        }
    }
    
    private void HandleStandardMineInteraction()
    {
        LogDebugMessage($"Interacting with standard mine at {m_CellData.Position}");
        
        var standardMine = m_CellData.CurrentMine as StandardMine;
        var player = GetPlayer();
        
        if (player != null)
        {
            standardMine.OnTrigger(player);
            m_VisualUpdater?.UpdateVisuals();
        }
    }
    
    private void TriggerMonster(MonsterMine monsterMine, PlayerComponent player)
    {
        if (player != null)
        {
            monsterMine.OnTrigger(player);
            m_VisualUpdater?.UpdateVisuals();
        }
    }
    
    private void CollectAndRemoveMine(MonsterMine monsterMine)
    {
        LogDebugMessage($"Collecting rewards for defeated monster at {m_CellData.Position}");
        var player = GetPlayer();
        
        if (player != null)
        {
            monsterMine.OnDestroy();
        }
        
        GameEvents.RaiseMineRemovalAttempted(m_CellData.Position);
    }
    
    private PlayerComponent GetPlayer()
    {
        return GameObject.FindFirstObjectByType<PlayerComponent>();
    }
    
    private void LogDebugMessage(string message)
    {
        if (m_DebugMode)
        {
            Debug.Log($"[CellInteractionHandler] {message}");
        }
    }
} 
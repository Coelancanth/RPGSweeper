using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public class MineEventHandler : IMineEventHandler
    {
        private readonly MineManager m_MineManager;
        private readonly IMineVisualManager m_VisualManager;

        public MineEventHandler(MineManager mineManager, IMineVisualManager visualManager)
        {
            m_MineManager = mineManager;
            m_VisualManager = visualManager;
        }

        public void SubscribeToEvents()
        {
            GameEvents.OnCellRevealed += HandleCellRevealed;
            GameEvents.OnMineRemovalAttempted += HandleMineRemoval;
            GameEvents.OnMineAddAttempted += HandleMineAdd;
            GameEvents.OnEffectsRemoved += HandleEffectsRemoved;
        }

        public void UnsubscribeFromEvents()
        {
            GameEvents.OnCellRevealed -= HandleCellRevealed;
            GameEvents.OnMineRemovalAttempted -= HandleMineRemoval;
            GameEvents.OnMineAddAttempted -= HandleMineAdd;
            GameEvents.OnEffectsRemoved -= HandleEffectsRemoved;
        }

        public void HandleCellRevealed(Vector2Int position)
        {
            if (m_MineManager.TryGetMine(position, out IMine mine, out MineData mineData))
            {
                m_VisualManager.ShowMineSprite(position, mineData.MineSprite, mine, mineData);

                //var playerComponent = Object.FindFirstObjectByType<PlayerComponent>();
                //if (playerComponent != null)
                //{
                    //mine.OnTrigger(playerComponent);
                //}
            }
        }

        public void HandleMineRemoval(Vector2Int position)
        {
            if (m_MineManager.TryGetMine(position, out IMine mine, out _))
            {
                mine.OnDestroy();
                m_MineManager.RemoveMineAt(position);
                m_VisualManager.HandleMineRemoval(position);
                m_MineManager.UpdateAllGridValues();
            }
        }

        public void HandleMineAdd(Vector2Int position, MineType type, MonsterType? monsterType)
        {
            m_MineManager.AddMine(position, type, monsterType);
        }

        public void HandleEffectsRemoved(Vector2Int position)
        {
            if (m_MineManager.TryGetMine(position, out IMine mine, out _))
            {
                if (mine is MonsterMine monsterMine)
                {
                    monsterMine.OnRemoveEffects();
                }
            }
        }
    }
} 
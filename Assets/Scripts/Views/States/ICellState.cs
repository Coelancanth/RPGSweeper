using UnityEngine;

namespace RPGMinesweeper.Views.States
{
    public interface ICellState
    {
        void Enter(CellView cell);
        void UpdateVisuals(CellView cell);
    }

    public class HiddenCellState : ICellState
    {
        private static readonly HiddenCellState s_Instance = new HiddenCellState();
        public static HiddenCellState Instance => s_Instance;

        public void Enter(CellView cell)
        {
            if (cell.BackgroundRenderer != null)
            {
                cell.BackgroundRenderer.enabled = true;
                cell.BackgroundRenderer.sprite = cell.HiddenSprite;
                cell.BackgroundRenderer.color = Color.white;
            }
            if (cell.MineRenderer != null)
            {
                cell.MineRenderer.enabled = false;
            }
        }

        public void UpdateVisuals(CellView cell)
        {
            if (cell.BackgroundRenderer != null)
            {
                cell.BackgroundRenderer.enabled = true;
                cell.BackgroundRenderer.sprite = cell.HiddenSprite;
                cell.BackgroundRenderer.color = Color.white;
            }
        }
    }

    public class RevealedMineCellState : ICellState
    {
        private static readonly RevealedMineCellState s_Instance = new RevealedMineCellState();
        public static RevealedMineCellState Instance => s_Instance;

        public void Enter(CellView cell)
        {
            if (cell.BackgroundRenderer != null)
            {
                cell.BackgroundRenderer.enabled = true;
                cell.BackgroundRenderer.sprite = cell.RevealedMineSprite;
                cell.BackgroundRenderer.color = Color.white;
            }
            if (cell.MineRenderer != null && cell.CurrentMineSprite != null)
            {
                cell.MineRenderer.sprite = cell.CurrentMineSprite;
                cell.MineRenderer.enabled = true;
                cell.MineRenderer.color = Color.white;
            }
        }

        public void UpdateVisuals(CellView cell)
        {
            if (cell.BackgroundRenderer != null)
            {
                cell.BackgroundRenderer.enabled = true;
                cell.BackgroundRenderer.sprite = cell.RevealedMineSprite;
                cell.BackgroundRenderer.color = Color.white;
            }
        }
    }

    public class RevealedEmptyCellState : ICellState
    {
        private static readonly RevealedEmptyCellState s_Instance = new RevealedEmptyCellState();
        public static RevealedEmptyCellState Instance => s_Instance;

        public void Enter(CellView cell)
        {
            if (cell.BackgroundRenderer != null)
            {
                cell.BackgroundRenderer.enabled = true;
                cell.BackgroundRenderer.sprite = cell.RevealedEmptySprite;
                cell.BackgroundRenderer.color = Color.white;
            }
            if (cell.MineRenderer != null)
            {
                cell.MineRenderer.enabled = false;
            }
        }

        public void UpdateVisuals(CellView cell)
        {
            if (cell.BackgroundRenderer != null)
            {
                cell.BackgroundRenderer.enabled = true;
                cell.BackgroundRenderer.sprite = cell.RevealedEmptySprite;
                cell.BackgroundRenderer.color = Color.white;
            }
        }
    }
} 
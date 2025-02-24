using UnityEngine;

public class MineRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_MineRenderer;
    [SerializeField] private int m_MineSortingOrder = 1;
    
    private float m_CellSize = 1f;
    private float m_MineScale = 0.8f;

    public void Initialize(float cellSize, float mineScale)
    {
        m_CellSize = cellSize;
        m_MineScale = mineScale;
        SetupRenderer();
    }

    private void SetupRenderer()
    {
        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = false;
            m_MineRenderer.sortingOrder = m_MineSortingOrder;
        }
    }

    public void UpdateMineSprite(Sprite mineSprite, Vector3 offset)
    {
        if (m_MineRenderer == null || mineSprite == null) return;

        m_MineRenderer.sprite = mineSprite;
        float targetMineSize = m_CellSize * m_MineScale;
        SetSpriteScale(m_MineRenderer, targetMineSize);
        m_MineRenderer.transform.localPosition = offset;
    }

    public void SetVisible(bool visible)
    {
        if (m_MineRenderer != null)
        {
            m_MineRenderer.enabled = visible;
        }
    }

    private void SetSpriteScale(SpriteRenderer renderer, float targetWorldSize)
    {
        if (renderer.sprite != null)
        {
            float pixelsPerUnit = renderer.sprite.pixelsPerUnit;
            float spriteSize = renderer.sprite.rect.width / pixelsPerUnit;
            float scale = targetWorldSize / spriteSize;
            renderer.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
} 
using UnityEngine;

public class CellRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_BackgroundRenderer;
    [SerializeField] private Sprite m_HiddenSprite;
    [SerializeField] private Sprite m_RevealedEmptySprite;
    [SerializeField] private Sprite m_RevealedMineSprite;
    [SerializeField] private Sprite m_DefeatedMonsterSprite;
    [SerializeField] private int m_BackgroundSortingOrder = 0;
    
    private float m_CellSize = 1f;

    public void Initialize(float cellSize)
    {
        m_CellSize = cellSize;
        SetupRenderer();
    }

    private void SetupRenderer()
    {
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.enabled = true;
            m_BackgroundRenderer.sortingOrder = m_BackgroundSortingOrder;
            SetSpriteScale(m_BackgroundRenderer, m_CellSize);
        }
    }

    public void UpdateVisuals(bool isRevealed, bool hasMine, bool isDefeatedMonster)
    {
        if (m_BackgroundRenderer == null) return;

        if (!isRevealed)
        {
            m_BackgroundRenderer.sprite = m_HiddenSprite;
        }
        else if (hasMine)
        {
            m_BackgroundRenderer.sprite = isDefeatedMonster ? 
                m_DefeatedMonsterSprite : m_RevealedMineSprite;
        }
        else
        {
            m_BackgroundRenderer.sprite = m_RevealedEmptySprite;
        }
    }

    public void SetColor(Color color)
    {
        if (m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.color = color;
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
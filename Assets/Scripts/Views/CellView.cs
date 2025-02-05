using UnityEngine;

public class CellView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Color m_HiddenColor = Color.gray;
    [SerializeField] private Color m_RevealedColor = Color.white;
    
    private Vector2Int m_Position;

    public void Initialize(Vector2Int position)
    {
        m_Position = position;
        UpdateVisuals(false);
    }

    public void UpdateVisuals(bool revealed)
    {
        m_SpriteRenderer.color = revealed ? m_RevealedColor : m_HiddenColor;
    }

    public void ApplyEffect()
    {
        // Add visual effect (e.g., particle system)
        Debug.Log($"Effect applied at position {m_Position}");
    }

    private void OnMouseDown()
    {
        GameEvents.RaiseCellRevealed(m_Position);
    }
} 
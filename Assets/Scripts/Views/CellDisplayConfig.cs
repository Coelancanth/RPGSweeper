using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CellDisplayConfig", menuName = "RPGMinesweeper/Cell Display Config")]
public class CellDisplayConfig : ScriptableObject 
{
    [Header("Cell Appearance")]
    [SerializeField] private Sprite m_HiddenSprite;
    [SerializeField] private Sprite m_RevealedEmptySprite;
    [SerializeField] private Sprite m_RevealedMineSprite;
    [SerializeField] private Sprite m_DefeatedMonsterSprite;
    
    [Header("Cell Size")]
    [SerializeField] private float m_CellSize = 1f;
    
    [Header("Mine Display")]
    [SerializeField] private float m_MineScale = 0.8f;
    [SerializeField] private Vector3 m_MineOffset = new Vector3(0f, 0.2f, -0.1f);
    
    [Header("Sorting Layers")]
    [SerializeField] private int m_BackgroundSortingOrder = 0;
    [SerializeField] private int m_MineSortingOrder = 1;
    [SerializeField] private int m_ValueSortingOrder = 2;
    [SerializeField] private int m_MarkSortingOrder = 3;
    
    // Event for notifying changes
    public event Action OnConfigChanged;
    
    // Accessors
    public Sprite HiddenSprite => m_HiddenSprite;
    public Sprite RevealedEmptySprite => m_RevealedEmptySprite;
    public Sprite RevealedMineSprite => m_RevealedMineSprite;
    public Sprite DefeatedMonsterSprite => m_DefeatedMonsterSprite;
    
    public float CellSize => m_CellSize;
    public float MineScale => m_MineScale;
    public Vector3 MineOffset => m_MineOffset;
    
    public int BackgroundSortingOrder => m_BackgroundSortingOrder;
    public int MineSortingOrder => m_MineSortingOrder;
    public int ValueSortingOrder => m_ValueSortingOrder;
    public int MarkSortingOrder => m_MarkSortingOrder;
    
    // Notify when values change
    public void NotifyConfigChanged()
    {
        OnConfigChanged?.Invoke();
    }
    
    // Called when values change in the inspector
    private void OnValidate()
    {
        NotifyConfigChanged();
    }
} 
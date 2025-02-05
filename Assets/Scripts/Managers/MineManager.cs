using UnityEngine;
using System.Collections.Generic;

public class MineManager : MonoBehaviour
{
    [SerializeField] private List<MineData> m_MineDatas;
    [SerializeField] private int m_MineCount = 10;
    
    private Dictionary<Vector2Int, IMine> m_Mines = new Dictionary<Vector2Int, IMine>();

    private void Start()
    {
        PlaceMines();
    }

    private void PlaceMines()
    {
        for (int i = 0; i < m_MineCount; i++)
        {
            MineData randomMineData = m_MineDatas[Random.Range(0, m_MineDatas.Count)];
            Vector2Int position = GetRandomEmptyPosition();
            
            IMine mine = CreateMine(randomMineData, position);
            m_Mines.Add(position, mine);
        }
    }

    private Vector2Int GetRandomEmptyPosition()
    {
        Vector2Int position;
        do
        {
            position = new Vector2Int(
                Random.Range(0, 10),
                Random.Range(0, 10)
            );
        } while (m_Mines.ContainsKey(position));

        return position;
    }

    private IMine CreateMine(MineData mineData, Vector2Int position)
    {
        switch (mineData.Type)
        {
            case MineType.Healing:
                return new HealingMine(mineData, position);
            case MineType.Experience:
                return new ExperienceMine(mineData, position);
            case MineType.MultiTrigger:
                return new MultiTriggerMine(mineData, position);
            case MineType.AreaReveal:
                return new AreaRevealMine(mineData, position);
            default:
                return new StandardMine(mineData, position);
        }
    }
} 
using UnityEngine;
using TMPro;

public interface IMineDisplayStrategy
{
    void SetupDisplay(GameObject cellObject, TextMeshPro valueText);
    void UpdateDisplay(IMine mine, MineData mineData, bool isRevealed);
    void CleanupDisplay();
} 
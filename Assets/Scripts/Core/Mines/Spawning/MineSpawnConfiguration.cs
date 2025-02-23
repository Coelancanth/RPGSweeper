using UnityEngine;
using Sirenix.OdinInspector;
using RPGMinesweeper;
using RPGMinesweeper.Core.Mines.Spawning;

[CreateAssetMenu(fileName = "MineSpawnConfiguration", menuName = "RPGMinesweeper/MineSpawnConfiguration")]
public class MineSpawnConfiguration : SerializedScriptableObject
{
    [TitleGroup("Spawn Settings")]
    [HorizontalGroup("Spawn Settings/Split")]
    [VerticalGroup("Spawn Settings/Split/Left"), LabelWidth(120)]
    [EnumToggleButtons]
    [OnValueChanged("OnSpawnStrategyChanged")]
    public SpawnStrategyType SpawnStrategy = SpawnStrategyType.Random;

    [VerticalGroup("Spawn Settings/Split/Left")]
    [ShowIf("@SpawnStrategy == SpawnStrategyType.Surrounded")]
    [EnumToggleButtons]
    [OnValueChanged("OnTargetMineTypeChanged")]
    [Tooltip("The type of mine this mine should be surrounded by")]
    public MineType TargetMineType = MineType.Monster;

    [VerticalGroup("Spawn Settings/Split/Left")]
    [ShowIf("@SpawnStrategy == SpawnStrategyType.Surrounded && TargetMineType == MineType.Monster")]
    [EnumToggleButtons]
    [Tooltip("The specific type of monster to be surrounded by")]
    public MonsterType TargetMonsterType = MonsterType.None;

    [VerticalGroup("Spawn Settings/Split/Left")]
    [ShowIf("@SpawnStrategy == SpawnStrategyType.SymmetricHorizontal || SpawnStrategy == SpawnStrategyType.SymmetricVertical")]
    [Tooltip("If enabled, symmetric pairs will be placed adjacent to each other")]
    public bool PlaceSymmetricPairsAdjacent = false;

    [TitleGroup("Spawn Count")]
    [HorizontalGroup("Spawn Count/Split")]
    [VerticalGroup("Spawn Count/Split/Left"), LabelWidth(120)]
    [MinValue(1)]
    [Tooltip("Number of mines to spawn with this configuration")]
    public int SpawnCount = 1;

    [TitleGroup("Mine Reference")]
    [Required]
    [InlineEditor]
    [Tooltip("The mine data to spawn")]
    public MineData MineData;

    private void OnSpawnStrategyChanged()
    {
        if (SpawnStrategy != SpawnStrategyType.Surrounded)
        {
            TargetMineType = MineType.Monster;
            TargetMonsterType = MonsterType.None;
        }
    }

    private void OnTargetMineTypeChanged()
    {
        if (TargetMineType != MineType.Monster)
        {
            TargetMonsterType = MonsterType.None;
        }
    }
} 
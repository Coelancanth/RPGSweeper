# Diff Details

Date : 2025-02-25 13:04:32

Directory f:\\unity\\Minesweeper\\Assets\\Scripts

Total : 75 files,  2621 codes, 115 comments, 458 blanks, all 3194 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [Assets/Scripts/Config/CellConfigManager.cs](/Assets/Scripts/Config/CellConfigManager.cs) | C# | 40 | 0 | 10 | 50 |
| [Assets/Scripts/Core/Effects/ConfusionEffect.cs](/Assets/Scripts/Core/Effects/ConfusionEffect.cs) | C# | 1 | 0 | 0 | 1 |
| [Assets/Scripts/Core/Effects/MonsterTransformEffect.cs](/Assets/Scripts/Core/Effects/MonsterTransformEffect.cs) | C# | 2 | -2 | 0 | 0 |
| [Assets/Scripts/Core/Effects/TeleportEffect.cs](/Assets/Scripts/Core/Effects/TeleportEffect.cs) | C# | 73 | 1 | 9 | 83 |
| [Assets/Scripts/Core/Effects/TeleportEffectData.cs](/Assets/Scripts/Core/Effects/TeleportEffectData.cs) | C# | 15 | 0 | 1 | 16 |
| [Assets/Scripts/Core/Events/GameEvents.cs](/Assets/Scripts/Core/Events/GameEvents.cs) | C# | 5 | 0 | 1 | 6 |
| [Assets/Scripts/Core/Mines/CompositeSpawnStrategy.cs](/Assets/Scripts/Core/Mines/CompositeSpawnStrategy.cs) | C# | -43 | -5 | -7 | -55 |
| [Assets/Scripts/Core/Mines/GridShape.cs](/Assets/Scripts/Core/Mines/GridShape.cs) | C# | -14 | 0 | 0 | -14 |
| [Assets/Scripts/Core/Mines/GridShapeHelper.cs](/Assets/Scripts/Core/Mines/GridShapeHelper.cs) | C# | -131 | -7 | -19 | -157 |
| [Assets/Scripts/Core/Mines/IDamagingMine.cs](/Assets/Scripts/Core/Mines/IDamagingMine.cs) | C# | -5 | 0 | -1 | -6 |
| [Assets/Scripts/Core/Mines/IMine.cs](/Assets/Scripts/Core/Mines/IMine.cs) | C# | -8 | 0 | -1 | -9 |
| [Assets/Scripts/Core/Mines/Implementation/MineConfigurationProvider.cs](/Assets/Scripts/Core/Mines/Implementation/MineConfigurationProvider.cs) | C# | 50 | 2 | 10 | 62 |
| [Assets/Scripts/Core/Mines/Implementation/MineEventHandler.cs](/Assets/Scripts/Core/Mines/Implementation/MineEventHandler.cs) | C# | 65 | 0 | 9 | 74 |
| [Assets/Scripts/Core/Mines/Implementation/MineVisualManager.cs](/Assets/Scripts/Core/Mines/Implementation/MineVisualManager.cs) | C# | 115 | 12 | 19 | 146 |
| [Assets/Scripts/Core/Mines/Interfaces/IDamagingMine.cs](/Assets/Scripts/Core/Mines/Interfaces/IDamagingMine.cs) | C# | 5 | 0 | 1 | 6 |
| [Assets/Scripts/Core/Mines/Interfaces/IMine.cs](/Assets/Scripts/Core/Mines/Interfaces/IMine.cs) | C# | 16 | 0 | 3 | 19 |
| [Assets/Scripts/Core/Mines/Interfaces/IMineConfigurationProvider.cs](/Assets/Scripts/Core/Mines/Interfaces/IMineConfigurationProvider.cs) | C# | 20 | 0 | 3 | 23 |
| [Assets/Scripts/Core/Mines/Interfaces/IMineCopyStrategy.cs](/Assets/Scripts/Core/Mines/Interfaces/IMineCopyStrategy.cs) | C# | 8 | 8 | 1 | 17 |
| [Assets/Scripts/Core/Mines/Interfaces/IMineEventHandler.cs](/Assets/Scripts/Core/Mines/Interfaces/IMineEventHandler.cs) | C# | 12 | 0 | 1 | 13 |
| [Assets/Scripts/Core/Mines/Interfaces/IMineVisualManager.cs](/Assets/Scripts/Core/Mines/Interfaces/IMineVisualManager.cs) | C# | 10 | 0 | 1 | 11 |
| [Assets/Scripts/Core/Mines/MineData.cs](/Assets/Scripts/Core/Mines/MineData.cs) | C# | -205 | -2 | -31 | -238 |
| [Assets/Scripts/Core/Mines/MineSpawnStrategies.cs](/Assets/Scripts/Core/Mines/MineSpawnStrategies.cs) | C# | -89 | -11 | -17 | -117 |
| [Assets/Scripts/Core/Mines/MineSpawnStrategy.cs](/Assets/Scripts/Core/Mines/MineSpawnStrategy.cs) | C# | -19 | -1 | -2 | -22 |
| [Assets/Scripts/Core/Mines/MineType.cs](/Assets/Scripts/Core/Mines/MineType.cs) | C# | -11 | 0 | 0 | -11 |
| [Assets/Scripts/Core/Mines/MineValueModifier.cs](/Assets/Scripts/Core/Mines/MineValueModifier.cs) | C# | -68 | -2 | -12 | -82 |
| [Assets/Scripts/Core/Mines/MineValuePropagator.cs](/Assets/Scripts/Core/Mines/MineValuePropagator.cs) | C# | -68 | -4 | -7 | -79 |
| [Assets/Scripts/Core/Mines/Mines/DisguisedMonsterMine.cs](/Assets/Scripts/Core/Mines/Mines/DisguisedMonsterMine.cs) | C# | 40 | 6 | 9 | 55 |
| [Assets/Scripts/Core/Mines/Mines/DisguisedMonsterMineData.cs](/Assets/Scripts/Core/Mines/Mines/DisguisedMonsterMineData.cs) | C# | 46 | 2 | 7 | 55 |
| [Assets/Scripts/Core/Mines/Mines/MineData.cs](/Assets/Scripts/Core/Mines/Mines/MineData.cs) | C# | 252 | 4 | 41 | 297 |
| [Assets/Scripts/Core/Mines/Mines/MineType.cs](/Assets/Scripts/Core/Mines/Mines/MineType.cs) | C# | 13 | 0 | 0 | 13 |
| [Assets/Scripts/Core/Mines/Mines/MineTypeSpawnData.cs](/Assets/Scripts/Core/Mines/Mines/MineTypeSpawnData.cs) | C# | 224 | 7 | 38 | 269 |
| [Assets/Scripts/Core/Mines/Mines/MineValueModifier.cs](/Assets/Scripts/Core/Mines/Mines/MineValueModifier.cs) | C# | 68 | 2 | 12 | 82 |
| [Assets/Scripts/Core/Mines/Mines/MineValuePropagator.cs](/Assets/Scripts/Core/Mines/Mines/MineValuePropagator.cs) | C# | 69 | 4 | 6 | 79 |
| [Assets/Scripts/Core/Mines/Mines/MonsterMine.cs](/Assets/Scripts/Core/Mines/Mines/MonsterMine.cs) | C# | 206 | 17 | 30 | 253 |
| [Assets/Scripts/Core/Mines/Mines/MonsterMineData.cs](/Assets/Scripts/Core/Mines/Mines/MonsterMineData.cs) | C# | 61 | 0 | 10 | 71 |
| [Assets/Scripts/Core/Mines/Mines/MonsterType.cs](/Assets/Scripts/Core/Mines/Mines/MonsterType.cs) | C# | 30 | 0 | 1 | 31 |
| [Assets/Scripts/Core/Mines/Mines/StandardMine.cs](/Assets/Scripts/Core/Mines/Mines/StandardMine.cs) | C# | 97 | 4 | 11 | 112 |
| [Assets/Scripts/Core/Mines/MonsterMine.cs](/Assets/Scripts/Core/Mines/MonsterMine.cs) | C# | -190 | -16 | -27 | -233 |
| [Assets/Scripts/Core/Mines/MonsterMineData.cs](/Assets/Scripts/Core/Mines/MonsterMineData.cs) | C# | -61 | -12 | -12 | -85 |
| [Assets/Scripts/Core/Mines/MonsterType.cs](/Assets/Scripts/Core/Mines/MonsterType.cs) | C# | -25 | 0 | 0 | -25 |
| [Assets/Scripts/Core/Mines/Spawning/AdjacentDirection.cs](/Assets/Scripts/Core/Mines/Spawning/AdjacentDirection.cs) | C# | 9 | 0 | 0 | 9 |
| [Assets/Scripts/Core/Mines/Spawning/IMineSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/IMineSpawnStrategy.cs) | C# | 79 | 3 | 17 | 99 |
| [Assets/Scripts/Core/Mines/Spawning/IMineSpawner.cs](/Assets/Scripts/Core/Mines/Spawning/IMineSpawner.cs) | C# | 11 | 1 | 1 | 13 |
| [Assets/Scripts/Core/Mines/Spawning/MineSpawnConfiguration.cs](/Assets/Scripts/Core/Mines/Spawning/MineSpawnConfiguration.cs) | C# | 55 | 0 | 8 | 63 |
| [Assets/Scripts/Core/Mines/Spawning/MineSpawner.cs](/Assets/Scripts/Core/Mines/Spawning/MineSpawner.cs) | C# | 133 | 10 | 21 | 164 |
| [Assets/Scripts/Core/Mines/Spawning/MonsterSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/MonsterSpawnStrategy.cs) | C# | 37 | 4 | 8 | 49 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/AdjacentSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/AdjacentSpawnStrategy.cs) | C# | 209 | 8 | 37 | 254 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/CenterSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/CenterSpawnStrategy.cs) | C# | 45 | 6 | 11 | 62 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/CornerSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/CornerSpawnStrategy.cs) | C# | 54 | 9 | 11 | 74 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/EdgeSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/EdgeSpawnStrategy.cs) | C# | 52 | 4 | 11 | 67 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/NeighborSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/NeighborSpawnStrategy.cs) | C# | 170 | 8 | 31 | 209 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/RandomSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/RandomSpawnStrategy.cs) | C# | 30 | 0 | 6 | 36 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/SurrondedSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/SurrondedSpawnStrategy.cs) | C# | 115 | 2 | 24 | 141 |
| [Assets/Scripts/Core/Mines/Spawning/Strategies/SymmetricSpawnStrategy.cs](/Assets/Scripts/Core/Mines/Spawning/Strategies/SymmetricSpawnStrategy.cs) | C# | 172 | 3 | 30 | 205 |
| [Assets/Scripts/Core/Mines/Spawning/Structures/SpawnResult.cs](/Assets/Scripts/Core/Mines/Spawning/Structures/SpawnResult.cs) | C# | 32 | 0 | 4 | 36 |
| [Assets/Scripts/Core/Mines/Spawning/Structures/SpawneContext.cs](/Assets/Scripts/Core/Mines/Spawning/Structures/SpawneContext.cs) | C# | 60 | 0 | 10 | 70 |
| [Assets/Scripts/Core/Mines/Spawning/Structures/SpawnedMine.cs](/Assets/Scripts/Core/Mines/Spawning/Structures/SpawnedMine.cs) | C# | 35 | 1 | 4 | 40 |
| [Assets/Scripts/Core/Mines/StandardMine.cs](/Assets/Scripts/Core/Mines/StandardMine.cs) | C# | -97 | -4 | -11 | -112 |
| [Assets/Scripts/Core/RPGMinesweeperTypes.cs](/Assets/Scripts/Core/RPGMinesweeperTypes.cs) | C# | 13 | 3 | 1 | 17 |
| [Assets/Scripts/Core/States/StateManager.cs](/Assets/Scripts/Core/States/StateManager.cs) | C# | 31 | 3 | 2 | 36 |
| [Assets/Scripts/Core/States/TeleportState.cs](/Assets/Scripts/Core/States/TeleportState.cs) | C# | 111 | 9 | 19 | 139 |
| [Assets/Scripts/Core/Utils/GridShape.cs](/Assets/Scripts/Core/Utils/GridShape.cs) | C# | 14 | 0 | 0 | 14 |
| [Assets/Scripts/Core/Utils/GridShapeHelper.cs](/Assets/Scripts/Core/Utils/GridShapeHelper.cs) | C# | 131 | 7 | 19 | 157 |
| [Assets/Scripts/Debug/MineDebugger.cs](/Assets/Scripts/Debug/MineDebugger.cs) | C# | 8 | -1 | 0 | 7 |
| [Assets/Scripts/Editor/MineEditorWindow.cs](/Assets/Scripts/Editor/MineEditorWindow.cs) | C# | 43 | 6 | 11 | 60 |
| [Assets/Scripts/Managers/GridManager.cs](/Assets/Scripts/Managers/GridManager.cs) | C# | 1 | 0 | -1 | 0 |
| [Assets/Scripts/Managers/MineCopyStrategies.cs](/Assets/Scripts/Managers/MineCopyStrategies.cs) | C# | 45 | 4 | 7 | 56 |
| [Assets/Scripts/Managers/MineFactory.cs](/Assets/Scripts/Managers/MineFactory.cs) | C# | 47 | 1 | 9 | 57 |
| [Assets/Scripts/Managers/MineManager.cs](/Assets/Scripts/Managers/MineManager.cs) | C# | -61 | -10 | 3 | -68 |
| [Assets/Scripts/Views/CellRenderer.cs](/Assets/Scripts/Views/CellRenderer.cs) | C# | 59 | 0 | 8 | 67 |
| [Assets/Scripts/Views/CellView.cs](/Assets/Scripts/Views/CellView.cs) | C# | 38 | 4 | 4 | 46 |
| [Assets/Scripts/Views/DisguisedMonsterMineDisplayStrategy.cs](/Assets/Scripts/Views/DisguisedMonsterMineDisplayStrategy.cs) | C# | 244 | 26 | 40 | 310 |
| [Assets/Scripts/Views/MineDisplayConfig.cs](/Assets/Scripts/Views/MineDisplayConfig.cs) | C# | 15 | 1 | 2 | 18 |
| [Assets/Scripts/Views/MineRenderer.cs](/Assets/Scripts/Views/MineRenderer.cs) | C# | 47 | 0 | 8 | 55 |
| [Assets/Scripts/Views/ValueRenderer.cs](/Assets/Scripts/Views/ValueRenderer.cs) | C# | 28 | 0 | 5 | 33 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details
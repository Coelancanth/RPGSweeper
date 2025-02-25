# Disguised Monsters - User Guide

## Overview

Disguised monsters are a special type of monster mine that initially appear to be collectible items, but reveal their true form when the player interacts with them. They're perfect for creating "mimic chest" type experiences where an apparent reward turns out to be a threat.

## How It Works

1. A disguised monster initially appears as a collectible (with a value displayed)
2. When a player clicks on the revealed disguised monster, it transforms into its true monster form
3. Upon transformation, the monster shows its stats (HP, damage) and behaves like a regular monster
4. The player must defeat the monster as normal before being able to collect it

## Creating a Disguised Monster

### Using the Editor

1. Go to **Tools > RPG Minesweeper > Mine Editor**
2. Click the **Create Disguised Monster Mine** button
3. Save the asset in the `ScriptableObjects/DisguisedMonsters` folder
4. Configure the monster properties and disguise properties

### Properties

The disguised monster has all the properties of a regular monster, plus:

- **Disguise Sprite**: The sprite shown when the monster is disguised
- **Disguised Value**: The apparent value shown to the player (to make it seem collectible)
- **Disguised Value Color**: The color of the displayed value
- **Is Disguised**: Whether the monster starts disguised (should usually be true)

## Recommended Settings

For a "Mimic Chest" type monster:
- Set the **Monster Type** to `MimicChest`
- Use a treasure chest sprite for the **Disguise Sprite**
- Set a high **Disguised Value** (e.g., 8-10) to make it tempting
- Use golden/yellow color for the **Disguised Value Color**
- Set moderate-high **HP** (80-100)
- Set significant **Base Damage** (25-30)
- Enable **Enrage State** for an added surprise

## Technical Details

- The `DisguisedMonsterMine` class extends `MonsterMine` with disguise functionality
- The `DisguisedMonsterMineData` scriptable object defines disguise appearance
- When disguised, the monster does not deal damage on first click (it just reveals)
- After revealing, it behaves exactly like a normal monster

## Example: MimicChest

A sample MimicChest is included in the `ScriptableObjects/DisguisedMonsters` folder. You can use this as a template for creating other disguised monster types.

## Spawning Disguised Monsters

To spawn disguised monsters in your dungeon:
1. Set up a mine spawner that targets the `MineType.DisguisedMonster` type
2. Configure spawn rules as needed (rarity, location, etc.)
3. Consider placing them near other collectibles to maximize the surprise factor

## Customization Ideas

- Create disguised monsters that look like healing items but deal damage
- Make "fake" experience orbs that are actually dangerous monsters
- Design special mimics for each dungeon theme with appropriate disguises 
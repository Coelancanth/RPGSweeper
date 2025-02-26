# Mark Panel UI Configuration Guide

## Overview
The Mark Panel is a popup UI element that appears when the player right-clicks on an unrevealed cell, allowing them to mark cells with flags, question marks, numbers, or custom inputs as reminders.

## Configuration Files

### 1. MarkPanelDisplayConfig
This ScriptableObject allows you to customize all aspects of the mark panel:
- Create one by right-clicking in the Project window → Create → RPGMinesweeper → Mark Panel Display Config
- Configure panel size, offsets, colors, and animation durations
- Add mark sprites (flag, question mark, numbers, custom input)
- Set button colors and sizes

### 2. CellDisplayConfig
This ScriptableObject handles the cell appearance configuration:
- Create one by right-clicking in the Project window → Create → RPGMinesweeper → Cell Display Config
- Configure cell sprites, sizes, and sorting orders
- Modify mine display scales and offsets

## UI Setup Instructions

1. **Create a Canvas for the Mark Panel**
   - Create a new Canvas in your scene if you don't already have one for UI elements
   - Set the Canvas Render Mode to "Screen Space - Camera" or "Screen Space - Overlay"
   - Add a Canvas Scaler component and set it to "Scale With Screen Size"
   - **Important**: Add a GraphicRaycaster component to the Canvas to handle UI interactions

2. **Create the Mark Panel GameObject**
   - Create a new empty GameObject as a child of the Canvas
   - Name it "MarkPanel"
   - Add a RectTransform component (should be added automatically)
   - Add a CanvasGroup component (used for fade in/out animations and blocking interactions)
   - Set the initial size to what's specified in your MarkPanelDisplayConfig

3. **Add Background Image**
   - Add an Image component to the MarkPanel
   - Assign a background sprite (preferably a 9-sliced sprite for better resizing)
   - Set the color to match your MarkPanelDisplayConfig setting

4. **Add Mark Buttons**
   - Create these Button GameObjects as children of the MarkPanel:
     - "FlagButton" - For marking cells with flags
     - "QuestionButton" - For marking cells with question marks
     - "NumbersButton" - For marking cells with numbers
     - "CustomInputButton" - For marking cells with custom inputs
     - "CloseButton" - For closing the panel without making a selection

   For each button:
   - Add a Button component
   - Add an Image component for the button background
   - Add a Text or TextMeshPro component as a child for the button label
   - Size and color will be controlled by the MarkPanelDisplayConfig

5. **Configure MarkPanel Script**
   - Add the `MarkPanel` script to the MarkPanel GameObject
   - Assign the following references:
     - Panel Rect: The RectTransform of the MarkPanel
     - Canvas Group: The CanvasGroup of the MarkPanel
     - Panel Background: The Image component on the MarkPanel
     - Flag Button: Reference to the FlagButton
     - Question Button: Reference to the QuestionButton
     - Numbers Button: Reference to the NumbersButton
     - Custom Input Button: Reference to the CustomInputButton
     - Close Button: Reference to the CloseButton
     - Raycaster: Reference to the GraphicRaycaster on the Canvas
     - Display Config: Reference to your MarkPanelDisplayConfig asset

6. **Configure CellView**
   - For each CellView prefab:
     - Add a new SpriteRenderer component for the mark indicator
     - Set its sorting order to be higher than the cell background but lower than any UI elements
     - Assign the MarkPanelDisplayConfig to the CellView's reference

7. **Connect to InteractionHandler**
   - Make sure the InteractionHandler has a reference to the MarkPanel
   - Ensure that it's subscribed to the OnCellRightClicked event

## Interaction Blocking

The MarkPanel prevents interactions with cells underneath it through two mechanisms:
1. The CanvasGroup component blocks raycasts when the panel is visible
2. The InputManager is temporarily disabled when the panel is shown
   - This prevents any cell clicks until the panel is closed
   - Ensures the player must select a mark or close the panel before continuing

## Mark Types

Four different mark types are supported:
1. **Flag**: Traditional flag mark, typically used to identify mines
2. **Question**: Question mark, used for uncertain cells
3. **Numbers**: Numeric indicators, for custom counting
4. **Custom Input**: For other custom markings

## Usage Flow

1. Player right-clicks on an unrevealed cell
2. The MarkPanel appears near the clicked cell
3. Player selects a marking option or closes the panel
4. The selected mark appears on the cell
5. Player can right-click the same cell again to change or remove the mark

## Required Assets

- Flag sprite: A visual indicator for cells the player thinks contain mines
- Question mark sprite: A visual indicator for cells the player is unsure about
- Numbers sprite: A visual indicator for cells the player wants to mark with a number
- Custom input sprite: A visual indicator for cells with custom markings
- Background panel sprite: A UI element that provides the backdrop for the mark panel
- Button sprites: Visual elements for the mark panel buttons

# Mark Panel UI Configuration Guide

## Overview
The Mark Panel is a popup UI element that appears when the player right-clicks on an unrevealed cell, allowing them to mark cells with flags or question marks as reminders.

## UI Setup Instructions

1. **Create a Canvas for the Mark Panel**
   - Create a new Canvas in your scene if you don't already have one for UI elements
   - Set the Canvas Render Mode to "Screen Space - Camera" or "Screen Space - Overlay"
   - Add a Canvas Scaler component and set it to "Scale With Screen Size"

2. **Create the Mark Panel GameObject**
   - Create a new empty GameObject as a child of the Canvas
   - Name it "MarkPanel"
   - Add a RectTransform component (should be added automatically)
   - Add a CanvasGroup component (used for fade in/out animations)
   - Set the initial size to approximately 120x150 pixels

3. **Add Background Image**
   - Add an Image component to the MarkPanel
   - Assign a background sprite (preferably a 9-sliced sprite for better resizing)
   - Set the color to match your game's UI theme

4. **Add Mark Buttons**
   - Create three Button GameObjects as children of the MarkPanel:
     - "FlagButton" - For marking cells with flags
     - "QuestionButton" - For marking cells with question marks
     - "CloseButton" - For closing the panel without making a selection

   For each button:
   - Add a Button component
   - Add an Image component for the button background
   - Add a Text or TextMeshPro component as a child for the button label
   - Set appropriate sprites for each button that indicate their function

5. **Configure MarkPanel Script**
   - Add the `MarkPanel` script to the MarkPanel GameObject
   - Assign the following references:
     - Panel Rect: The RectTransform of the MarkPanel
     - Canvas Group: The CanvasGroup of the MarkPanel
     - Flag Button: Reference to the FlagButton
     - Question Button: Reference to the QuestionButton
     - Close Button: Reference to the CloseButton

6. **Configure CellView**
   - For each CellView prefab:
     - Add a new SpriteRenderer component for the mark indicator
     - Set its sorting order to be higher than the cell background but lower than any UI elements
     - Assign flag and question mark sprites to the CellView component

7. **Connect to InteractionHandler**
   - Make sure the InteractionHandler has a reference to the MarkPanel
   - Ensure that it's subscribed to the OnCellRightClicked event

## Usage Flow

1. Player right-clicks on an unrevealed cell
2. The MarkPanel appears near the clicked cell
3. Player selects a marking option:
   - Flag: Places a flag marker on the cell (often used to mark suspected mines)
   - Question Mark: Places a question mark (often used to mark uncertain cells)
   - Close: Dismisses the panel without marking
4. The selected mark appears on the cell
5. Player can right-click the same cell again to change or remove the mark

## Required Assets

- Flag sprite: A visual indicator for cells the player thinks contain mines
- Question mark sprite: A visual indicator for cells the player is unsure about
- Background panel sprite: A UI element that provides the backdrop for the mark panel
- Button sprites: Visual elements for the mark panel buttons

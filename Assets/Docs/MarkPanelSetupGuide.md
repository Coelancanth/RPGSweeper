# Mark Panel Setup Guide

This guide will help you set up the Mark Panel feature in your Minesweeper game, including creating the necessary configuration assets.

## Creating the Mark Panel Display Config

1. In Unity's Project window, right-click and select **Create → RPGMinesweeper → Mark Panel Display Config**
2. Name the asset `MarkPanelDisplayConfig`
3. Configure the following settings in the inspector:

### Panel Settings
- **Panel Size**: Set the size of the mark panel (recommended: 120x150)
- **Offset**: Position offset from the cell (recommended: 20x20)
- **Animation Duration**: Time for show/hide animations (recommended: 0.25)
- **Panel Background Color**: Color with alpha for the panel background

### Mark Sprites
- **Flag Mark Sprite**: Assign your flag sprite asset
- **Question Mark Sprite**: Assign your question mark sprite
- **Numbers Mark Sprite**: Assign your numbers mark sprite (if any)
- **Custom Input Mark Sprite**: Assign your custom mark sprite (if any)

### Mark Size Settings
- **Mark Scale**: Size of the mark relative to the cell (recommended: 0.7)
- **Mark Offset**: Position offset of the mark within the cell

### Button Settings
- **Button Normal Color**: Default color of the buttons
- **Button Hover Color**: Color when mouse hovers over buttons
- **Button Size**: Size of each button in the panel
- **Button Spacing**: Space between buttons

## Assigning the Config

1. Select your `MarkPanel` GameObject in the hierarchy
2. In the Inspector, find the "Mark Panel (Script)" component
3. Drag your `MarkPanelDisplayConfig` asset into the "Display Config" field
4. Make sure the "Canvas Group" reference is also set

## Connecting to Cell Views

1. Select your Cell prefab or instance
2. In the Inspector, find the "Cell View (Script)" component
3. Drag your `MarkPanelDisplayConfig` asset into the "Mark Display Config" field

## Testing

1. Enter Play mode
2. Right-click on an unrevealed cell
3. Verify that the Mark Panel appears correctly with the configuration you set
4. Test clicking the buttons to apply different mark types to the cell

## Troubleshooting

If the Mark Panel doesn't appear:
- Check Console for errors
- Verify that the Input Manager is properly set up to handle right-click events
- Confirm that the MarkPanel GameObject is active in the hierarchy
- Ensure all references are properly assigned in the inspector 
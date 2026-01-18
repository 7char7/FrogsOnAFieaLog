# Crystal Alert System

## Overview
When a player mines a crystal, nearby enemies of the corresponding type are alerted and attracted to the player's location.

## Crystal to Enemy Type Mapping

| Crystal Rarity | Enemy Type Alerted | Alert Radius |
|----------------|-------------------|--------------|
| Common         | Shallow           | 15 units     |
| Uncommon       | Shallow           | 15 units     |
| Rare           | Medium            | 25 units     |
| Legendary      | Deep              | 35 units     |

## How It Works

1. **Mining Detection**: When `Crystal.MineCrystal()` reduces the crystal's health to zero, `OnCrystalFullyMined()` is called.

2. **Alert Trigger**: The `CrystalAlertManager` is notified with the crystal type and mining position.

3. **Enemy Search**: The manager finds all enemies within the alert radius that match the target enemy type.

4. **Enemy Response**: Matching enemies transition to the Chase state and move toward the player's last known position.

## Components

### CrystalAlertManager
- **Location**: Scene root (created in MainScene)
- **Settings**:
  - `commonUncommonAlertRadius`: 15 units
  - `rareAlertRadius`: 25 units
  - `legendaryAlertRadius`: 35 units

### Crystal (Modified)
- Added `OnCrystalFullyMined()` method that triggers the alert system
- Calls manager when crystal health reaches zero

### EnemyAI (Existing)
- Already has `AlertToPlayer()` method that transitions enemy to Chase state
- No modifications needed

## Customization

To adjust alert ranges, modify the serialized fields on the `CrystalAlertManager` component in the scene:
- Increase values for larger alert zones
- Decrease values for more localized alerts

To change which enemy types respond to which crystals, edit the `GetTargetEnemyTypesForCrystalType()` method in `CrystalAlertManager.cs`.

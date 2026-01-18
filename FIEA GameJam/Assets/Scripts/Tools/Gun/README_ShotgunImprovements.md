# Shotgun Improvements

## Changes Made

### 1. Improved Spread System
**Problem**: The old system used world-space position offsets (±30 units), causing wildly inconsistent and unpredictable spread patterns.

**Solution**: Changed to angle-based spread using `Quaternion.Euler()`. Now spread is measured in degrees, creating a consistent cone pattern.

**New Spread Value**: `3.5°` (changed from `30` units)
- Creates a tight, predictable spread pattern
- Maintains video game shotgun feel
- More effective at close to medium range

### 2. Better Pellet Distribution
**Changes**:
- Increased pellets from `5` to `8` per shot
- Each pellet now deals proportional damage (`total damage / pellet count`)
- More consistent damage application

### 3. Enhanced Visual Effects

#### Bullet Trails - Grey with Back-to-Front Fade
- **Color**: Changed from red to grey (0.7, 0.7, 0.7)
- **Fade Effect**: Animates from back to front
  - The start position moves toward the end position over time
  - Creates a "traveling fade" effect as if the bullet is disappearing
  - Both position and alpha animate simultaneously
- Added configurable `trailWidth` parameter (default: `0.02`)
- Trails now taper from start to end (start width → 50% at end)
- Faster, cleaner fade (0.1 seconds default)

#### Muzzle Flash (Optional)
New optional fields in `Shotgun.cs`:
- `muzzleFlash` (ParticleSystem): Visual flash effect
- `muzzleFlashLight` (Light): Brief light flash
- `muzzleFlashDuration`: How long the light stays on (default: 0.05s)

### 4. Code Quality Improvements
- Proper damage calculation per pellet
- Better raycast direction handling
- Null safety checks for trail creation
- More efficient fade coroutine with smooth interpolation

## How to Use

### Adjusting Spread
Edit the `bulletSpread` stat in `/Assets/Scriptable Objects/Stats/Shotgun.asset`:
- **Tight spread**: 2-3 degrees (CQB shotgun)
- **Medium spread**: 3.5-5 degrees (balanced, current setting)
- **Wide spread**: 6-10 degrees (sawed-off style)

### Customizing Trail Appearance

**Color**: Edit `/Assets/TEMP MATERIAL ASSETS UNTIL REMY IS GOATED/Temp Bullet Trail.prefab`
- Select the prefab
- Modify LineRenderer > Color Gradient
- Current: Grey gradient (light to dark)

**Fade Speed**: In Gun.cs Inspector:
- Increase `Fade Duration` for slower fade
- Decrease for faster fade
- Current default: `0.1` seconds

**Trail Thickness**: In Gun.cs Inspector:
- Increase `Trail Width` for thicker, more visible tracers
- Decrease for subtle, thin tracers
- Current default: `0.02`

### Adding Muzzle Flash VFX

1. **Create/Assign Particle System**:
   - Add a Particle System as child of the gun
   - Set `Duration` to ~0.1s
   - Set `Start Lifetime` to ~0.05-0.1s
   - Use a bright yellow/orange color
   - Enable `Emission` burst (5-10 particles)
   - Set `Shape` to Cone with small angle
   - Drag to `Muzzle Flash` field in Inspector

2. **Create Muzzle Flash Light**:
   - Add a Light component as child of gun
   - Set `Type` to Point Light
   - Set `Color` to orange/yellow
   - Set `Range` to 3-5 units
   - Set `Intensity` to 2-4
   - **Disable** the Light by default
   - Drag to `Muzzle Flash Light` field in Inspector

## Visual Effect Breakdown

### Back-to-Front Fade Mechanics
The trail creates a "disappearing bullet" effect:
1. Trail spawns from gun barrel to hit point
2. Over `fadeDuration`:
   - Start point moves toward end point
   - Both start and end positions fade to transparent
   - Creates impression of bullet "catching up" to impact point
3. Trail is destroyed when fully transparent

This creates a more dynamic, modern look compared to simple alpha fade.

## Performance Notes
- Each shot now creates 8 LineRenderer instances (up from 5)
- All trails fade and destroy within 0.1 seconds
- Minimal performance impact due to quick cleanup
- Muzzle flash light only active for 0.05s per shot
- Back-to-front fade uses Vector3.Lerp for smooth animation

## Upgrade Path
The spread system now works better with upgrades:
- **Tighter Spread**: Reduce `bulletSpread` stat
- **More Pellets**: Increase `bulletsPerShot` stat
- **Better Range**: Increase `range` stat
- Damage is automatically distributed across pellets

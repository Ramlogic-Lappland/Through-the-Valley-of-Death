# Changelog

## [0.10.1] - 2026-11-10

### New

- Image logo splash screen.

### Changes

- Product and company names corrected in settings.

## [0.10.0] - 2026-11-10

### New

- Fiend animations.
- Door and exit point animations.
- Weapon shoot sounds.
- UI button sounds.
- Credits placeholder in main menu.

### Changes

- Win condition now includes Charger enemies.
- Player footstep audio fixed.
- Audio events integrated across animations.

### Known Issues

- Enemy idle and footstep sounds need fixing.
- Ambient audio is not integrated yet.

## [0.9.1] - 2026-11-03

### Changes

- Charger and player collision fixed.

## [0.9.0] - 2026-11-03

### New

- Fiend charging enemy.
- Win and lose screens.
- Foot animations for the player and enemies.

### Changes

- Player no longer moves after dying.
- First level walls changed to tilemap walls.
- First level ground changed from sand to wood.
- Doors added to the first level.
- Level transitions fixed.
- Game over screen scaling fixed.
- Werewolf animation loop fixed.
- Fiend and bullet logic fixed.

## [0.8.7] - 2026-10-29

### Changes

- Version number shown in the pause menu updated.

## [0.8.6] - 2026-10-29

### New

- Player dash.
- Basic game over layout.

## [0.8.4] - 2026-10-27

### Changes

- Main menu button in the pause menu now works.

## [0.8.3] - 2026-10-27

### Changes

- Player no longer fires a bullet right after a collision.

## [0.8.2] - 2026-10-27

### Changes

- Enemies no longer share the player's collision layer.

## [0.8.1] - 2026-10-27

### Changes

- Build scene list updated to the 0.8.0 scenes.
- Windows sound bank added to the build.

## [0.8.0] - 2026-10-27

### New

- Enemy animations.
- Player animations.
- Dead enemy prefabs for Dhampir and Werewolf.

### Changes

- Pause menu exit button now returns to the main menu.
- Weapon retracts when it would clip into a wall.
- Main menu adjustments.

## [0.7.4] - 2026-10-22

### Changes

- Enemy HP bar bug fixed.

## [0.7.3] - 2026-10-22

### Changes

- Version number shown in the pause menu updated.

## [0.7.2] - 2026-10-20

### Changes

- Main menu fixed.

## [0.7.1] - 2026-10-20

### Changes

- Version number shown in the pause menu updated.

## [0.7.0] - 2026-10-20

### New

- Main menu.
- Exit arrow that points to the exit.
- Reload button.
- Scene 4 design and set dressing.
- Wall tile palette.

### Changes

- Scene arrows and level progression fixed.
- Wall tilemap and player hitbox adjusted.
- Player null dependency errors fixed.

## [0.6.2] - 2026-10-15

### Changes

- Version number shown in the pause menu updated.

## [0.6.1] - 2026-10-15

### New

- Wwise audio integration with a test sound.

### Changes

- Static field-of-view bug fixed.

## [0.6.0] - 2026-10-13

### New

- Exit points and a three-scene flow.
- Dhampir and Werewolf enemy sprites and prefabs.
- Restart button.
- Ammo UI animation.
- Custom cursor swapping.
- Tavern level layout.

### Changes

- Enemies turn around when shot from outside their vision.
- Enemies no longer need a patrol point.
- Scene difficulty progression fixed.
- Player light shadow fixed.
- Scene 2 and Scene 3 layouts updated.

## [0.5.0] - 2026-10-06

### New

- Custom crosshair.
- Visual ammo HUD replacing the ammo counter.
- Exit button in the pause menu.
- Wall lamps with field of view.
- Prefab lights in the scene.
- Walls tile palette.

### Changes

- Enemy knockback and player controller updated.
- Player speed increased.
- Reload speed increased.
- Bullet spawn offset adjusted.
- New 0.5.0 test scene.

## [0.4.0] - 2026-09-29

### New

- Pause menu.
- Version number shown in the pause menu.
- Reload timer slider.
- Ammo counter showing current and maximum bullets.
- Stage Clear text when the win condition is met.

### Changes

- Player rotation and shooting no longer stay active while the pause menu is open.
- Door hinge motor fixed.
- Field-of-view radius increased.
- Window visuals improved.
- Enemy behaviour and HP bar improved.

### Known Issues

- Door collision is not fully fixed yet.

## [0.3.0] - 2026-09-22

### New

- Win condition manager.
- Lose condition.
- Weapon reload.
- Static field-of-view mesh for environment light.
- Melee enemy with knockback.
- Door prefabs.
- New test scene and tile palette.

### Changes

- Environment light radius matches the field of view.
- Bullet speed increased.
- Door motor reworked.
- Level layout and scene order updated.
- Enemy and bullet behaviour fixes.

### Known Issues

- Win and lose screens are not implemented yet.
- Player death does not disable movement or play a death animation.

## [0.2.0] - 2026-09-15

### New

- Melee enemy.
- Enemy health system and HP bar.
- Working door with a hinge joint.
- Tavern props and set dressing.

### Changes

- Field-of-view mesh reacts when the player takes damage or heals.
- Camera view distance increased.
- Player visual rotation fixed.
- Tile placement alignment fixed.
- Scene fixes and tweaks.

### Known Issues

- Melee enemies do not damage the player yet.
- Door does not return to its original position yet.

## [0.1.1] - 2026-09-08

### New

- Player sprite.

### Changes

- Bullets now move with physics and disappear after a short time or on impact.

## [0.1.0] - 2026-09-08

### New

- Player movement and mouse-based rotation.
- Camera controller.
- Weapon system with a test pistol.
- Field-of-view mesh where enemies only render inside it.
- Enemy logic ported from the prototype with NavMeshPlus.
- Basic tiled test scene.

### Changes

- Enemy sprite rotates to its facing direction.

### Known Issues

- Enemies do not attack yet.

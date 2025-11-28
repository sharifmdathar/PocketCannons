# Pocket Cannons

A 2D turn-based artillery game built with Unity, inspired by classic games like Worms and Scorched Earth. Two players take turns firing cannons at each other on procedurally generated terrain.

![Unity Version](https://img.shields.io/badge/Unity-6000.2.13f1-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## 🎮 Gameplay

- **Turn-based combat**: Players alternate turns firing projectiles at each other
- **Procedural terrain**: Each game features a unique, randomly generated landscape using Perlin noise
- **Aim and power**: Adjust your cannon's angle and power to hit your opponent
- **Health system**: Reduce your opponent's health to zero to win
- **Terrain interaction**: Cannons automatically align to the terrain slope and can move along the ground
- **Smart camera**: Automatically follows both players and zooms to keep both cannons in view

## 🎯 Features

- **Procedural Terrain Generation**: Random terrain with configurable width, height, and noise parameters
- **Physics-based Projectiles**: Realistic projectile physics with collision detection
- **Turn Management**: Clear turn indicators and UI feedback
- **Health System**: Track both players' health with visual sliders
- **Camera System**: Intelligent camera that follows players and constrains view to terrain bounds
- **Terrain Bounds**: Players and camera are constrained to stay within terrain boundaries
- **WebGL Support**: Playable in web browsers via GitHub Pages
- **Linux Support**: Native Linux builds available

## 🚀 Getting Started

### Prerequisites

- Unity 6000.2.13f1 or compatible version
- Git LFS (for handling large binary assets)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/sharifmdathar/PocketCannons.git
cd PocketCannons
```

2. Open the project in Unity:
   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Ensure Unity 6000.2.13f1 is installed
   - Open the project

3. Open the game scene:
   - Navigate to `Assets/Scenes/GameScene.unity`
   - Press Play to start the game

## 🎮 Controls

- **Angle Adjustment**: Use the increment/decrement buttons or radial slider to adjust cannon angle
- **Power Adjustment**: Use the power slider to set projectile launch power (0-100)
- **Movement**: Use left/right arrow buttons to move your cannon along the terrain
- **Fire**: Press the fire button to launch your projectile
- **Regenerate Map**: Use the regenerate button to create a new random terrain

## 🏗️ Project Structure

```
Assets/
├── Scripts/
│   ├── GameManager.cs          # Manages game state, turns, and player data
│   ├── CannonController.cs     # Controls cannon behavior, movement, and firing
│   ├── Projectile.cs           # Handles projectile physics and collision
│   ├── TerrainGenerator.cs     # Generates procedural terrain using Perlin noise
│   ├── CameraFollow.cs         # Smart camera system that follows both players
│   ├── GameUi.cs               # UI management and event handling
│   ├── RepeatButton.cs         # Button that repeats while held
│   └── RadialSlider.cs         # Radial angle selector
├── Prefabs/
│   ├── Cannon.prefab           # Player cannon prefab
│   ├── CannonBall.prefab       # Projectile prefab
│   ├── Crosshair.prefab        # Aiming indicator
│   └── HealthSlider.prefab     # Health display
└── Scenes/
    └── GameScene.unity         # Main game scene
```

## 🔧 Configuration

### Terrain Settings

Edit `TerrainGenerator.cs` to customize terrain generation:
- `width`: Terrain width in units
- `heightMultiplier`: Maximum terrain height variation
- `noiseFrequency`: Controls terrain smoothness (lower = smoother)
- `resolution`: Terrain detail level (higher = more vertices)
- `groundDepth`: Depth of terrain below surface

### Game Settings

Edit `GameManager.cs` to adjust:
- Starting health for both players
- Starting angle and power values

### Camera Settings

Edit `CameraFollow.cs` to customize:
- `minOrthographicSize`: Minimum zoom level
- `maxOrthographicSize`: Maximum zoom level
- `cameraYOffset`: Vertical camera offset
- `cameraZOffset`: Camera depth

## 🌐 WebGL Build

The project includes GitHub Actions workflows for automated WebGL builds and deployment to GitHub Pages.

### Manual Build

1. In Unity, go to `File > Build Settings`
2. Select `WebGL` platform
3. Click `Build` and choose an output directory

### Automated Deployment

The project uses GitHub Actions to automatically build and deploy WebGL builds:
- Workflow: `.github/workflows/build-webgl.yml`
- Triggers on manual workflow dispatch
- Deploys to GitHub Pages automatically

## 🐧 Linux Build

A Linux build is available in the repository. To build your own:

1. In Unity, go to `File > Build Settings`
2. Select `Linux` platform
3. Click `Build` and choose an output directory

## 🛠️ Development

### Key Systems

- **GameManager**: Singleton that manages game state, turn order, and player statistics
- **TerrainGenerator**: Generates procedural terrain mesh and collider at runtime
- **CameraFollow**: Dynamically adjusts camera position and zoom to keep both players visible
- **CannonController**: Handles player input, movement, aiming, and firing

### Adding Features

- **New Projectile Types**: Extend `Projectile.cs` or create new projectile prefabs
- **Power-ups**: Add components to `CannonController` or create new game objects
- **Different Terrain Types**: Modify `TerrainGenerator.cs` to use different noise algorithms
- **Multiplayer**: Extend `GameManager` to support more than 2 players

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Contact

For questions or issues, please open an issue on the GitHub repository.

---

Made with Unity ❤️


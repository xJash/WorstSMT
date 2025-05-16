Modular Configuration System
Fully configurable via BepInEx/config/WorstsSMT.cfg. Toggle each performance tweak independently.

Aggressive Quality Reduction
- Force Unity’s lowest quality preset
- Disable real-time shadows, soft particles, vegetation, and reflection probes
- Strip fog and ambient lighting for ultra-clean visuals

Texture Optimization
- Global texture resolution scaling (mipmap bias)
- Disable anisotropic filtering and anti-aliasing to free up GPU memory and bandwidth

Performance Tweaks
- Disable VSync for uncapped FPS
- Adjustable Level-of-Detail bias (LOD) to switch to low-poly models sooner
- Lower physics solver iteration counts for CPU savings
- Adjust Unity’s fixed update timestep for better simulation performance

Lighting Simplification
- Disable all dynamic lights, baked light probes, and lens flares
- Use flat ambient lighting to eliminate lighting computations

Effect Removal
- Disable all particle systems (smoke, sparks, etc.)
- Strip lens flares and reflection effects

Terrain Simplification
- Remove terrain-related features
- Aggressively simplify terrain mesh with adjustable pixel error

Camera Optimization
- Disable occlusion culling to reduce CPU overhead
- Reduce camera draw distance (far clip plane)
- Disable HDR and MSAA per-camera for reduced render overhead

Audio Safety
- Limit to a single active AudioListener to avoid conflicts and CPU waste

Resolution Override (Optional)
- Optional screen resolution forcing (e.g., 800x600 windowed)

Default-Safe Behavior
- All settings are off by default
- Activate only what you need for your system

Ideal For:
- Low-spec laptops and office machines
- Users who want max framerate regardless of visuals
- Mod developers or testers who want to isolate performance bottlenecks
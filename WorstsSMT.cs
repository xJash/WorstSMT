using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace WorstsSMT {
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class WorstsSMT : BaseUnityPlugin {
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        public static WorstsSMT Instance;
        internal static new ManualLogSource Logger { get; private set; } = null!;

        // Config entries
        internal static ConfigEntry<bool> EnableMod;
        internal static ConfigEntry<bool> ApplyBaseQualityLevel;
        internal static ConfigEntry<bool> DisableShadows;
        internal static ConfigEntry<bool> DisableSoftEffects;
        internal static ConfigEntry<bool> DisableReflectionProbes;
        internal static ConfigEntry<bool> DisableFogAndAmbient;
        internal static ConfigEntry<int> TextureMipmapLimit;
        internal static ConfigEntry<bool> DisableAntialiasing;
        internal static ConfigEntry<bool> DisableAnisotropic;
        internal static ConfigEntry<bool> DisableVSync;
        internal static ConfigEntry<float> LODBias;
        internal static ConfigEntry<bool> DisableSceneLights;
        internal static ConfigEntry<bool> DisableLightProbes;
        internal static ConfigEntry<bool> DisableLensFlares;
        internal static ConfigEntry<bool> DisableParticles;
        internal static ConfigEntry<bool> DisableTerrainDetails;
        internal static ConfigEntry<int> TerrainPixelError;
        internal static ConfigEntry<bool> DisableOcclusionCulling;
        internal static ConfigEntry<float> CameraFarClipPlane;
        internal static ConfigEntry<bool> DisableHDR;
        internal static ConfigEntry<bool> DisableMSAA;
        internal static ConfigEntry<bool> LimitAudioListeners;
        internal static ConfigEntry<float> FixedDeltaTime;
        internal static ConfigEntry<int> SolverIterations;
        internal static ConfigEntry<int> SolverVelocityIterations;

        internal static ConfigEntry<float> RenderScale;
        internal static ConfigEntry<bool> DisablePostProcessing;
        internal static ConfigEntry<bool> DisableSkybox;
        internal static ConfigEntry<bool> LowerAudioSampleRate;
        internal static ConfigEntry<int> MaxQueuedFrames;
        internal static ConfigEntry<bool> AggressiveGC;
        internal static ConfigEntry<float> TimeScale;

        internal static ConfigEntry<bool> DisableRendererShadows;
        internal static ConfigEntry<bool> DisableLightCookies;
        internal static ConfigEntry<bool> DisableProjectors;
        internal static ConfigEntry<bool> DisableWindZones;
        internal static ConfigEntry<bool> DisableCloth;
        internal static ConfigEntry<bool> DisableNavMeshAgents;
        internal static ConfigEntry<int> ShaderLOD;
        private void Awake() {
            // === General ===
            EnableMod = Config.Bind("General", "enabled", false, new ConfigDescription("Master toggle for the graphics mod.\ntrue = apply all enabled graphics-lowering settings.\nfalse = mod does nothing, game runs as normal."));

            // === Quality ===
            ApplyBaseQualityLevel = Config.Bind("Quality", "apply_base_quality_level", false, new ConfigDescription("Sets Unity's internal quality preset to the lowest available.\nRecommended: true for ultra-low graphics. Default: false (retain game setting)."));
            DisableShadows = Config.Bind("Quality", "disable_shadows", false, new ConfigDescription("Disables all real-time shadows.\ntrue = removes all shadow rendering.\nfalse = keep shadows as the game configures them."));
            DisableSoftEffects = Config.Bind("Quality", "disable_soft_effects", false, new ConfigDescription("Disables soft particles and vegetation blending.\nDefault: false = preserve visual effects."));
            DisableReflectionProbes = Config.Bind("Quality", "disable_reflection_probes", false, new ConfigDescription("Disables real-time reflection probes used for glossy materials.\nDefault: false = keep dynamic reflections."));
            DisableFogAndAmbient = Config.Bind("Quality", "disable_fog_and_flat_ambient", false, new ConfigDescription("Disables fog and switches ambient lighting to flat color.\nDefault: false = retain original lighting atmosphere."));

            // === Textures ===
            TextureMipmapLimit = Config.Bind("Textures", "texture_mipmap_limit", 0, new ConfigDescription("Controls global texture resolution (mipmap bias).\n0 = full res (default), 1 = half, 2 = quarter, up to 4 = lowest.\nRecommended: 2–4 on low-memory systems.", new AcceptableValueRange<int>(0, 4)));
            DisableAntialiasing = Config.Bind("Textures", "disable_antialiasing", false, new ConfigDescription("Disables multi-sample anti-aliasing (MSAA).\nDefault: false = keep smoother edges."));
            DisableAnisotropic = Config.Bind("Textures", "disable_anisotropic", false, new ConfigDescription("Disables anisotropic texture filtering.\nDefault: false = preserve sharp textures at glancing angles."));

            // === Performance ===
            DisableVSync = Config.Bind("Performance", "disable_vsync", false, new ConfigDescription("Disables vertical sync.\nDefault: false = use game’s framerate and tearing policy."));
            LODBias = Config.Bind("Performance", "lod_bias", 1.0f, new ConfigDescription("Level of Detail (LOD) bias.\n1.0 = Unity default, 0.1 = most aggressive.\nLower values reduce model complexity sooner.", new AcceptableValueRange<float>(0.1f, 2.0f)));

            // === Lighting ===
            DisableSceneLights = Config.Bind("Lighting", "disable_scene_lights", false, new ConfigDescription("Disables all Light components in the scene.\nDefault: false = preserve lighting setup."));
            DisableLightProbes = Config.Bind("Lighting", "disable_light_probes", false, new ConfigDescription("Disables baked LightProbeGroups.\nDefault: false = preserve baked lighting."));
            DisableLensFlares = Config.Bind("Lighting", "disable_lens_flares", false, new ConfigDescription("Disables lens flare effects.\nDefault: false = preserve light effects."));

            // === Particles ===
            DisableParticles = Config.Bind("Particles", "disable_particles", false, new ConfigDescription("Disables all ParticleSystems.\nDefault: false = preserve visual effects."));

            // === Terrain ===
            DisableTerrainDetails = Config.Bind("Terrain", "disable_terrain_details", false, new ConfigDescription("Disables terrain grass, trees, and mesh details.\nDefault: false = preserve outdoor detail."));
            TerrainPixelError = Config.Bind("Terrain", "terrain_pixel_error", 5, new ConfigDescription("Controls how blocky terrain appears.\n5 = default (very detailed), 100+ = faster.", new AcceptableValueRange<int>(5, 500)));

            // === Cameras ===
            DisableOcclusionCulling = Config.Bind("Cameras", "disable_occlusion_culling", false, new ConfigDescription("Disables occlusion culling (CPU-side visibility optimization).\nDefault: false = preserve Unity's occlusion logic."));
            CameraFarClipPlane = Config.Bind("Cameras", "camera_far_clip_plane", 1000f, new ConfigDescription("Camera far clipping distance.\nDefault: 1000. Lower = faster, but less visible range.", new AcceptableValueRange<float>(1f, 2000f)));
            DisableHDR = Config.Bind("Cameras", "disable_hdr", false, new ConfigDescription("Disables High Dynamic Range rendering.\nDefault: false = retain bloom, glow, tone mapping."));
            DisableMSAA = Config.Bind("Cameras", "disable_msaa", false, new ConfigDescription("Disables multi-sample anti-aliasing (MSAA).\nDefault: false = smoother visuals."));

            // === Audio ===
            LimitAudioListeners = Config.Bind("Audio", "limit_audio_listeners", false, new ConfigDescription("Ensures only one AudioListener is active.\nPrevents audio conflicts in some Unity games."));
            LowerAudioSampleRate = Config.Bind("Audio", "lower_audio_sample_rate", false, new ConfigDescription("Reduces audio sample rate to 22050 Hz.\nCan improve CPU performance on weak systems."));

            // === Physics ===
            FixedDeltaTime = Config.Bind("Physics", "fixed_delta_time", 0.02f, new ConfigDescription("Physics update interval.\n0.02 = Unity default. Higher = fewer updates, less CPU.", new AcceptableValueRange<float>(0.01f, 0.1f)));
            SolverIterations = Config.Bind("Physics", "solver_iterations", 6, new ConfigDescription("Number of physics solver passes per update.\nLower = faster but less accurate.", new AcceptableValueRange<int>(1, 10)));
            SolverVelocityIterations = Config.Bind("Physics", "solver_velocity_iterations", 2, new ConfigDescription("Number of solver passes for velocity resolution.\nLower = better performance, rougher physics.", new AcceptableValueRange<int>(1, 5)));

            // === Advanced ===
            RenderScale = Config.Bind("Advanced", "render_scale", 1.0f, new ConfigDescription("Scale internal resolution (0.5 = half res, 1.0 = full).\nLowering this greatly boosts GPU performance.", new AcceptableValueRange<float>(0.1f, 1.0f)));
            DisablePostProcessing = Config.Bind("Advanced", "disable_post_processing", false, new ConfigDescription("Disables all post-processing volumes.\nIncludes bloom, motion blur, depth of field, AO."));
            DisableSkybox = Config.Bind("Advanced", "disable_skybox", false, new ConfigDescription("Removes skybox and forces solid background color."));
            MaxQueuedFrames = Config.Bind("Advanced", "max_queued_frames", -1, new ConfigDescription("Controls number of max queued GPU frames.\n-1 = default. 0 = lowest latency. 1+ = smoother, but delayed.", new AcceptableValueRange<int>(-1, 5)));
            AggressiveGC = Config.Bind("Advanced", "aggressive_gc", false, new ConfigDescription("Forces garbage collection more aggressively.\nCan reduce memory spikes, but may cause stutter."));
            TimeScale = Config.Bind("Advanced", "time_scale", 1.0f, new ConfigDescription("Time scale multiplier.\n1.0 = normal speed. Lower = slower game logic, less CPU.", new AcceptableValueRange<float>(0.1f, 1.0f)));

            // === Extra Optimizations ===
            DisableRendererShadows = Config.Bind("Lighting", "disable_renderer_shadows", false, new ConfigDescription("Disables shadows on all mesh renderers (receive + cast). Helpful for older GPUs."));
            DisableLightCookies = Config.Bind("Lighting", "disable_light_cookies", false, new ConfigDescription("Removes light cookies from all Light components."));
            DisableProjectors = Config.Bind("Effects", "disable_projectors", false, new ConfigDescription("Disables all Projector components, used in some visual effects and fake shadows."));
            DisableWindZones = Config.Bind("Environment", "disable_wind_zones", false, new ConfigDescription("Disables WindZone components that simulate wind in vegetation."));
            DisableCloth = Config.Bind("Characters", "disable_cloth_simulation", false, new ConfigDescription("Disables cloth physics simulation (e.g. capes, clothes)."));
            DisableNavMeshAgents = Config.Bind("AI", "disable_navmesh_agents", false, new ConfigDescription("Disables Unity NavMeshAgent and NavMeshObstacle components (used for AI pathfinding)."));
            ShaderLOD = Config.Bind("Graphics", "shader_global_lod", 600, new ConfigDescription("Overrides global shader LOD level. Default: 600.\nLower = simpler shaders. Try 200–300 for max performance.", new AcceptableValueRange<int>(200, 1000)));

            Instance = this;
            Logger = base.Logger;
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} is loaded!");
        }
    }
}

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;

namespace WorstsSMT {
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class WorstsSMT : BaseUnityPlugin {
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        public static WorstsSMT Instance;
        internal static new ManualLogSource Logger { get; private set; } = null!;

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

        private void Awake() {

            EnableMod = Config.Bind("General", "enabled", false,
                "Master toggle for the graphics mod.\n" +
                "true = apply all enabled graphics-lowering settings.\n" +
                "false = mod does nothing, game runs as normal.");

            // === QUALITY ===

            ApplyBaseQualityLevel = Config.Bind("Quality", "apply_base_quality_level", false,
                "Sets Unity's internal quality preset to the lowest available.\n" +
                "Recommended: true for ultra-low graphics. Default: false (retain game setting).");

            DisableShadows = Config.Bind("Quality", "disable_shadows", false,
                "Disables all real-time shadows.\n" +
                "true = removes all shadow rendering.\n" +
                "false = keep shadows as the game configures them.");

            DisableSoftEffects = Config.Bind("Quality", "disable_soft_effects", false,
                "Disables soft particles and soft vegetation blending.\n" +
                "Default: false (preserve visual effects).");

            DisableReflectionProbes = Config.Bind("Quality", "disable_reflection_probes", false,
                "Disables real-time reflection probes used for glossy materials.\n" +
                "Default: false = keep dynamic reflections.");

            DisableFogAndAmbient = Config.Bind("Quality", "disable_fog_and_flat_ambient", false,
                "Disables fog and switches ambient lighting to flat color.\n" +
                "Default: false = retain original lighting atmosphere.");

            // === TEXTURES ===

            TextureMipmapLimit = Config.Bind("Textures", "texture_mipmap_limit", 0,
                "Controls global texture resolution (mipmap bias).\n" +
                "0 = full res (default), 1 = half, 2 = quarter, up to 4 = lowest.\n" +
                "Recommended: 2–4 on low-memory systems.");

            DisableAntialiasing = Config.Bind("Textures", "disable_antialiasing", false,
                "Disables MSAA (multi-sample anti-aliasing).\n" +
                "Default: false (keep anti-aliasing enabled if present).");

            DisableAnisotropic = Config.Bind("Textures", "disable_anisotropic", false,
                "Disables anisotropic texture filtering.\n" +
                "Default: false = preserve sharp distant textures.");

            // === PERFORMANCE ===

            DisableVSync = Config.Bind("Performance", "disable_vsync", false,
                "Disables vertical sync.\n" +
                "Default: false = use game’s framerate and tearing policy.");

            LODBias = Config.Bind("Performance", "lod_bias", 1.0f,
                "LOD bias factor. 1.0 = Unity default.\n" +
                "Range: 0.1 (most aggressive) to 2.0+ (high fidelity).");

            // === LIGHTING ===

            DisableSceneLights = Config.Bind("Lighting", "disable_scene_lights", false,
                "Disables all Light components in the scene.\n" +
                "Default: false = preserve lighting setup.");

            DisableLightProbes = Config.Bind("Lighting", "disable_light_probes", false,
                "Disables baked LightProbeGroups.\n" +
                "Default: false = dynamic lighting works normally.");

            DisableLensFlares = Config.Bind("Lighting", "disable_lens_flares", false,
                "Disables lens flare effects from lights.\n" +
                "Default: false = preserve visual flares.");

            // === PARTICLES ===

            DisableParticles = Config.Bind("Particles", "disable_particles", false,
                "Disables all ParticleSystems.\n" +
                "Default: false = preserve all visual effects.");

            // === TERRAIN ===

            DisableTerrainDetails = Config.Bind("Terrain", "disable_terrain_details", false,
                "Disables terrain grass, trees, and mesh details.\n" +
                "Default: false = preserve outdoor detail.");

            TerrainPixelError = Config.Bind("Terrain", "terrain_pixel_error", 5,
                "Controls how blocky terrain appears.\n" +
                "5 = default (very detailed), up to 500 = extremely simplified.");

            // === CAMERAS ===

            DisableOcclusionCulling = Config.Bind("Cameras", "disable_occlusion_culling", false,
                "Disables occlusion culling (CPU-side visibility optimization).\n" +
                "Default: false = preserve Unity's visibility logic.");

            CameraFarClipPlane = Config.Bind("Cameras", "camera_far_clip_plane", 1000f,
                "Far clipping distance for all cameras.\n" +
                "Default: 1000 (typical). Lower = less rendering at distance.");

            DisableHDR = Config.Bind("Cameras", "disable_hdr", false,
                "Disables High Dynamic Range rendering.\n" +
                "Default: false = keep bloom/glow effects.");

            DisableMSAA = Config.Bind("Cameras", "disable_msaa", false,
                "Disables multi-sample anti-aliasing.\n" +
                "Default: false = preserve smoother edges.");

            // === AUDIO ===

            LimitAudioListeners = Config.Bind("Audio", "limit_audio_listeners", false,
                "Ensures only one active AudioListener exists.\n" +
                "Default: false = do not interfere with game’s audio setup.");

            // === PHYSICS ===

            FixedDeltaTime = Config.Bind("Physics", "fixed_delta_time", 0.02f,
                "Physics update interval.\n" +
                "0.02 = Unity default (50 updates/sec). Higher = fewer updates.");

            SolverIterations = Config.Bind("Physics", "solver_iterations", 6,
                "Physics constraint solver passes.\n" +
                "Default: 6 (Unity standard). Lower = faster, less accurate.");

            SolverVelocityIterations = Config.Bind("Physics", "solver_velocity_iterations", 2,
                "Physics velocity solver passes.\n" +
                "Default: 2. Lower = better performance, lower simulation quality.");

            Instance = this;
            Logger = base.Logger;
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} is loaded!");
        }
    }
}

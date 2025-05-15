using HarmonyLib;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering;

namespace WorstSMT.Patches {
    [HarmonyPatch(typeof(FirstPersonController), "Start")]
    public class LateUpdateRaycastPatch {
        static void Postfix() {
            // base quality level
            QualitySettings.SetQualityLevel(0, true);

            // player-pov shadows
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 0f;

            // antialiasing/texture
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.globalTextureMipmapLimit = 2; // 0 full res, 1 is half, 2 is quarter

            // performance
            QualitySettings.vSyncCount = 0;                // vsync = 0 = disabled
            QualitySettings.lodBias = 0.3f;                // lower the LOD lower the detail meaning lower poly

            // reflection/effects
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.softParticles = false;
            QualitySettings.softVegetation = false;

            // fps limit
            //Application.targetFrameRate = 30;

            // lighting
            foreach (var light in GameObject.FindObjectsOfType<Light>()) {
                light.enabled = false;
            }

            // particles
            foreach (var ps in GameObject.FindObjectsOfType<ParticleSystem>()) {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.gameObject.SetActive(false);
            }

            // ambient lighting, fog
            RenderSettings.fog = false;
            RenderSettings.ambientIntensity = 0.5f;
            RenderSettings.ambientMode = AmbientMode.Flat;

            // map shadows (optional, commented out)
            //foreach (var rend in GameObject.FindObjectsOfType<Renderer>())
            //{
            //    rend.shadowCastingMode = ShadowCastingMode.Off;
            //    rend.receiveShadows = false;
            //}

            // reflection
            foreach (var probe in GameObject.FindObjectsOfType<ReflectionProbe>()) {
                probe.enabled = false;
            }

            // light probes
            foreach (var lp in GameObject.FindObjectsOfType<LightProbeGroup>()) {
                lp.enabled = false;
            }

            // lens flare
            foreach (var lf in GameObject.FindObjectsOfType<LensFlare>()) {
                lf.enabled = false;
            }

            // resolution
            // Screen.SetResolution(800, 600, FullScreenMode.Windowed);
        }
    }
}

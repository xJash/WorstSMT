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

            //Application.targetFrameRate = 30;

            // lighting
            foreach (var light in GameObject.FindObjectsOfType<Light>())
                light.enabled = false;

            RenderSettings.fog = false;
            RenderSettings.ambientIntensity = 0.5f;
            RenderSettings.ambientMode = AmbientMode.Flat;

            // particles
            foreach (var ps in GameObject.FindObjectsOfType<ParticleSystem>()) {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.gameObject.SetActive(false);
            }

            // reflection
            foreach (var probe in GameObject.FindObjectsOfType<ReflectionProbe>()) {
                probe.enabled = false;
                probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
            }

            // lights
            foreach (var lp in GameObject.FindObjectsOfType<LightProbeGroup>())
                lp.enabled = false;

            // lens flare 
            foreach (var lf in GameObject.FindObjectsOfType<LensFlare>())
                lf.enabled = false;

            // terrain tweak
            foreach (var terrain in GameObject.FindObjectsOfType<Terrain>()) {
                terrain.detailObjectDistance = 0f;
                terrain.treeDistance = 0f;
                terrain.heightmapPixelError = 200; // makes terrain blocky but fast, lower=faster
                terrain.basemapDistance = 0f;
            }

            // skin mesh tweak
            foreach (var skinned in GameObject.FindObjectsOfType<SkinnedMeshRenderer>())
                skinned.updateWhenOffscreen = false; // lower CPU usage

            // camera tweaks // broken
            foreach (var cam in GameObject.FindObjectsOfType<Camera>()) {
                cam.useOcclusionCulling = false;    
                cam.farClipPlane = 0.01f;           
                cam.allowHDR = false;               
                cam.allowMSAA = false;              
            }

            // audio channel limiter
            var allListeners = GameObject.FindObjectsOfType<AudioListener>();
            for (int i = 1; i < allListeners.Length; i++)
                allListeners[i].enabled = false; // keep only one

            // physics shit
            Time.fixedDeltaTime = 0.05f; // reduce physics update frequency default was 0.02
            Physics.defaultSolverIterations = 4; // default was 6, lower = better
            Physics.defaultSolverVelocityIterations = 1; // default is 2

            // map shadow renderer
            //foreach (var rend in GameObject.FindObjectsOfType<Renderer>())
            //{
            //    rend.shadowCastingMode = ShadowCastingMode.Off;
            //    rend.receiveShadows = false;
            //}

            // force screen reso
            //Screen.SetResolution(800, 600, FullScreenMode.Windowed);
        }
    }
}

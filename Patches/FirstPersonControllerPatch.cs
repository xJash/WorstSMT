using HarmonyLib;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering;

namespace WorstsSMT.Patches {
    [HarmonyPatch(typeof(FirstPersonController), "Start")]
    public class LateUpdateRaycastPatch {
        static void Postfix() {
            if (!WorstsSMT.EnableMod.Value) return;

            if (WorstsSMT.ApplyBaseQualityLevel.Value)
                QualitySettings.SetQualityLevel(0, true);

            if (WorstsSMT.DisableShadows.Value) {
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowCascades = 0;
                QualitySettings.shadowDistance = 0f;
            }

            if (WorstsSMT.DisableAntialiasing.Value)
                QualitySettings.antiAliasing = 0;

            if (WorstsSMT.DisableAnisotropic.Value)
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;

            QualitySettings.globalTextureMipmapLimit = WorstsSMT.TextureMipmapLimit.Value;

            if (WorstsSMT.DisableVSync.Value)
                QualitySettings.vSyncCount = 0;

            QualitySettings.lodBias = WorstsSMT.LODBias.Value;

            if (WorstsSMT.DisableReflectionProbes.Value)
                QualitySettings.realtimeReflectionProbes = false;

            if (WorstsSMT.DisableSoftEffects.Value) {
                QualitySettings.softParticles = false;
                QualitySettings.softVegetation = false;
            }

            if (WorstsSMT.DisableSceneLights.Value) {
                foreach (var light in GameObject.FindObjectsOfType<Light>())
                    light.enabled = false;
            }

            if (WorstsSMT.DisableFogAndAmbient.Value) {
                RenderSettings.fog = false;
                RenderSettings.ambientIntensity = 0.5f;
                RenderSettings.ambientMode = AmbientMode.Flat;
            }

            if (WorstsSMT.DisableParticles.Value) {
                foreach (var ps in GameObject.FindObjectsOfType<ParticleSystem>()) {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ps.gameObject.SetActive(false);
                }
            }

            if (WorstsSMT.DisableReflectionProbes.Value) {
                foreach (var probe in GameObject.FindObjectsOfType<ReflectionProbe>()) {
                    probe.enabled = false;
                    probe.mode = ReflectionProbeMode.Custom;
                }
            }

            if (WorstsSMT.DisableLightProbes.Value) {
                foreach (var lp in GameObject.FindObjectsOfType<LightProbeGroup>())
                    lp.enabled = false;
            }

            if (WorstsSMT.DisableLensFlares.Value) {
                foreach (var lf in GameObject.FindObjectsOfType<LensFlare>())
                    lf.enabled = false;
            }

            if (WorstsSMT.DisableTerrainDetails.Value) {
                foreach (var terrain in GameObject.FindObjectsOfType<Terrain>()) {
                    terrain.detailObjectDistance = 0f;
                    terrain.treeDistance = 0f;
                    terrain.heightmapPixelError = WorstsSMT.TerrainPixelError.Value;
                    terrain.basemapDistance = 0f;
                }
            }

            foreach (var skinned in GameObject.FindObjectsOfType<SkinnedMeshRenderer>())
                skinned.updateWhenOffscreen = false;

            foreach (var cam in GameObject.FindObjectsOfType<Camera>()) {
                if (WorstsSMT.DisableOcclusionCulling.Value)
                    cam.useOcclusionCulling = false;

                cam.farClipPlane = WorstsSMT.CameraFarClipPlane.Value;

                if (WorstsSMT.DisableHDR.Value)
                    cam.allowHDR = false;

                if (WorstsSMT.DisableMSAA.Value)
                    cam.allowMSAA = false;
            }

            if (WorstsSMT.LimitAudioListeners.Value) {
                var listeners = GameObject.FindObjectsOfType<AudioListener>();
                for (int i = 1; i < listeners.Length; i++)
                    listeners[i].enabled = false;
            }

            Time.fixedDeltaTime = WorstsSMT.FixedDeltaTime.Value;
            Physics.defaultSolverIterations = WorstsSMT.SolverIterations.Value;
            Physics.defaultSolverVelocityIterations = WorstsSMT.SolverVelocityIterations.Value;

            Debug.Log("[WorstsSMT] Graphics settings applied from config.");
        }
    }
}

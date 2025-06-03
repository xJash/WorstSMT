using HarmonyLib;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.AI;

namespace WorstsSMT.Patches {
    [HarmonyPatch(typeof(FirstPersonController), "Start")]
    public class LateUpdateRaycastPatch {
        static void Postfix() {
            if (!WorstsSMT.EnableMod.Value)
                return;

            if (WorstsSMT.ApplyBaseQualityLevel.Value) QualitySettings.SetQualityLevel(0, true);

            if (WorstsSMT.DisableShadows.Value) {
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowCascades = 0;
                QualitySettings.shadowDistance = 0f;
            }

            if (WorstsSMT.DisableAntialiasing.Value) QualitySettings.antiAliasing = 0;

            if (WorstsSMT.DisableAnisotropic.Value) QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;

            QualitySettings.globalTextureMipmapLimit = WorstsSMT.TextureMipmapLimit.Value;

            if (WorstsSMT.DisableVSync.Value) QualitySettings.vSyncCount = 0;

            QualitySettings.lodBias = WorstsSMT.LODBias.Value;

            if (WorstsSMT.DisableReflectionProbes.Value) QualitySettings.realtimeReflectionProbes = false;

            if (WorstsSMT.DisableSoftEffects.Value) {
                QualitySettings.softParticles = false;
                QualitySettings.softVegetation = false;
            }

            if (WorstsSMT.DisableSceneLights.Value) {
                foreach (var light in GameObject.FindObjectsOfType<Light>()) light.enabled = false;
            }

            if (WorstsSMT.DisableLightCookies.Value) {
                foreach (var light in GameObject.FindObjectsOfType<Light>()) light.cookie = null;
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
                foreach (var lp in GameObject.FindObjectsOfType<LightProbeGroup>()) lp.enabled = false;
            }

            if (WorstsSMT.DisableLensFlares.Value) {
                foreach (var lf in GameObject.FindObjectsOfType<LensFlare>()) lf.enabled = false;
            }

            if (WorstsSMT.DisableTerrainDetails.Value) {
                foreach (var terrain in GameObject.FindObjectsOfType<Terrain>()) {
                    terrain.detailObjectDistance = 0f;
                    terrain.treeDistance = 0f;
                    terrain.heightmapPixelError = WorstsSMT.TerrainPixelError.Value;
                    terrain.basemapDistance = 0f;
                }
            }

            if (WorstsSMT.DisableRendererShadows.Value) {
                foreach (var renderer in GameObject.FindObjectsOfType<Renderer>()) {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                }
            }

            foreach (var skinned in GameObject.FindObjectsOfType<SkinnedMeshRenderer>()) skinned.updateWhenOffscreen = false;

            if (WorstsSMT.DisableWindZones.Value) {
                foreach (var wind in GameObject.FindObjectsOfType<WindZone>()) wind.gameObject.SetActive(false);
            }

            if (WorstsSMT.DisableCloth.Value) {
                foreach (var cloth in GameObject.FindObjectsOfType<Cloth>()) cloth.enabled = false;
            }

            if (WorstsSMT.DisableNavMeshAgents.Value) {
                foreach (var agent in GameObject.FindObjectsOfType<NavMeshAgent>()) agent.enabled = false;
                foreach (var obstacle in GameObject.FindObjectsOfType<NavMeshObstacle>()) obstacle.enabled = false;
            }

            if (WorstsSMT.DisableProjectors.Value) {
                foreach (var projector in GameObject.FindObjectsOfType<Projector>()) projector.enabled = false;
            }

            Shader.globalMaximumLOD = WorstsSMT.ShaderLOD.Value;

            if (WorstsSMT.RenderScale.Value < 1f)
                ScalableBufferManager.ResizeBuffers(WorstsSMT.RenderScale.Value, WorstsSMT.RenderScale.Value);

            if (WorstsSMT.DisablePostProcessing.Value) {
                foreach (var volume in GameObject.FindObjectsOfType<Volume>()) volume.enabled = false;
            }

            if (WorstsSMT.DisableSkybox.Value) {
                RenderSettings.skybox = null;
                if (Camera.main != null) {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                    Camera.main.backgroundColor = Color.black;
                }
            }

            if (WorstsSMT.LowerAudioSampleRate.Value) AudioSettings.outputSampleRate = 22050;

            if (WorstsSMT.MaxQueuedFrames.Value >= 0) QualitySettings.maxQueuedFrames = WorstsSMT.MaxQueuedFrames.Value;

            if (WorstsSMT.AggressiveGC.Value) System.GC.Collect();

            if (WorstsSMT.TimeScale.Value < 1f) Time.timeScale = WorstsSMT.TimeScale.Value;

            foreach (var cam in GameObject.FindObjectsOfType<Camera>()) {
                if (cam.gameObject.GetComponent<CameraFixer>() == null) {
                    cam.gameObject.AddComponent<CameraFixer>();
                }
            }

            if (WorstsSMT.LimitAudioListeners.Value) {
                var listeners = GameObject.FindObjectsOfType<AudioListener>();
                for (int i = 1; i < listeners.Length; i++) listeners[i].enabled = false;
            }

            Time.fixedDeltaTime = WorstsSMT.FixedDeltaTime.Value;
            Physics.defaultSolverIterations = WorstsSMT.SolverIterations.Value;
            Physics.defaultSolverVelocityIterations = WorstsSMT.SolverVelocityIterations.Value;

            Debug.Log("[WorstsSMT] Graphics settings applied from config.");
        }
    }

    public class CameraFixer : MonoBehaviour {
        private Camera _cam;

        void Start() {
            _cam = GetComponent<Camera>();
            if (_cam != null && _cam.enabled) Debug.Log($"[WorstsSMT] CameraFixer attached to: {_cam.name}");
        }

        void OnPreRender() {
            if (!WorstsSMT.EnableMod.Value || _cam == null || !_cam.enabled) return;
            if (_cam != Camera.main) return;

            if (WorstsSMT.DisableOcclusionCulling.Value)  _cam.useOcclusionCulling = false;
            if (WorstsSMT.DisableHDR.Value) _cam.allowHDR = false;
            if (WorstsSMT.DisableMSAA.Value) _cam.allowMSAA = false;
            if (Mathf.Abs(_cam.farClipPlane - WorstsSMT.CameraFarClipPlane.Value) > 0.01f) _cam.farClipPlane = WorstsSMT.CameraFarClipPlane.Value;
            if (_cam.nearClipPlane > 0.1f) _cam.nearClipPlane = 0.01f;

            Debug.Log($"Camera: {_cam.name}, Active: {_cam.enabled}, FarClip: {_cam.farClipPlane}, Main: {_cam == Camera.main}");
        }

        void LateUpdate() {
            if (!WorstsSMT.EnableMod.Value || _cam == null || !_cam.enabled) return;
            if (_cam != Camera.main) return;

            if (WorstsSMT.DisableOcclusionCulling.Value) _cam.useOcclusionCulling = false;
            if (WorstsSMT.DisableHDR.Value)  _cam.allowHDR = false;
            if (WorstsSMT.DisableMSAA.Value) _cam.allowMSAA = false;
            if (Mathf.Abs(_cam.farClipPlane - WorstsSMT.CameraFarClipPlane.Value) > 0.01f) _cam.farClipPlane = WorstsSMT.CameraFarClipPlane.Value;
            if (_cam.nearClipPlane > 0.1f) _cam.nearClipPlane = 0.01f;

            Debug.Log($"Camera: {_cam.name}, Active: {_cam.enabled}, FarClip: {_cam.farClipPlane}, Main: {_cam == Camera.main}");
        }
    }
}

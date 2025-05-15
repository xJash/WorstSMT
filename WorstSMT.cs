using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace WorstSMT {
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class WorstSMT : BaseUnityPlugin {
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        public static WorstSMT Instance;
        internal static new ManualLogSource Logger { get; private set; } = null!;

        private void Awake() {
            Instance = this;
            Logger = base.Logger;
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} is loaded!");
        }
    }
}

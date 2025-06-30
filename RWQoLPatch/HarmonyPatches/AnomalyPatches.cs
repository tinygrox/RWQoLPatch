using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLPatch.HarmonyPatches
{
    public class AnomalyPatches:AbstractPatchBase
    {
        protected override void LoadAllPatchInfo()
        {
            
        }

        protected override string ModDisplayName => "Anomaly";
        protected override string ModId => "Ludeon.RimWorld.Anomaly";
        public override bool IsModLoaded(IModChecker modChecker) => ModsConfig.AnomalyActive;
    }
}
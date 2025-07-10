using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class OdysseyPatches: AbstractPatchBase
    {
        protected override void LoadAllPatchInfo()
        {
            
        }

        protected override string ModDisplayName => "Odyssey";
        protected override string ModId => "Ludeon.RimWorld.Odyssey";

        public override bool IsModLoaded(IModChecker modChecker)
        {
#if RIMWORLD_1_6 || RELEASE
            return ModsConfig.OdysseyActive;
#else
                  return false;
#endif
        }
    }
}
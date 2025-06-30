using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLPatch
{
    public class RWModChecker: IModChecker
    {
        public bool IsModLoaded(string modPackageId)
        {
            return ModsConfig.IsActive(modPackageId);
        }
    }
}
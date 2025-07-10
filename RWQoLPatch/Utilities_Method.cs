using RimWorld;
using Verse;

namespace RWQoLPatch
{
    public static class Utilities_Method
    {
        public static string GetQualityLabel(QualityCategory cat)
        {
            return TheSettings.VEFLocPatch ? cat.GetLabel().CapitalizeFirst() : cat.ToString();
        }
    }
}

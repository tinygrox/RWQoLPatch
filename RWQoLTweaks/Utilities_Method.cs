using RimWorld;
using Verse;

namespace RWQoLTweaks
{
    public static class Utilities_Method
    {
        public static string GetQualityLabel(QualityCategory cat)
        {
            return TheSettings.VEFLocPatch ? cat.GetLabel().CapitalizeFirst() : cat.ToString();
        }
        
        public static string FormatResource(string reslabel, int current, int required)
        {
            string colored = current < required
                ? $"{current}/{required}".Colorize(ColorLibrary.RedReadable)
                : $"{current}/{required}".Colorize(ColorLibrary.Green);
            return $"{reslabel}: {colored}";
        }

    }
}

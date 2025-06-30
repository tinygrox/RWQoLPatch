using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLPatch
{
    public class TheSettings: ModSettings
    {
        public static int PreceptRoleNum = 2;
        public static bool TogglePowerInstantly = true;
        public static bool SubCoreScannerShowPawnType = true;
        public static bool DownedOverseerControlMechs = true;
        public static bool HoldOpenDoorInstantly = true;
        public static bool TransporterAutoload = true;
        public static bool MortarsAutoCool = true;
        public static bool NoPrisonBreak = true;
        public static bool MechChargeRatePatch = true;
        public static float MechChargeRate = 0.05f;
        public static bool NoMoreRelationGen = true;
        public static bool RimefellerUnmannedPatch = true;
        public static bool RimefellerLocPatch = true;
        public static bool RimatomicUnmannedPatch = true;
        public static bool RimatomicLocPatch = true;
        public static bool RimatomicFixPatch = true;
        public static bool NoBreakDownPatch = true;
        public static bool DBHLocPatch = true;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref PreceptRoleNum, "tinygrox.ModSettings.PreceptRoleNum", 2);
            Scribe_Values.Look(ref TogglePowerInstantly, "tinygrox.ModSettings.TogglePowerInstantly", true);
            Scribe_Values.Look(ref SubCoreScannerShowPawnType, "tinygrox.ModSettings.SubCorePawnType", true);
            Scribe_Values.Look(ref DownedOverseerControlMechs, "tinygrox.ModSettings.DownedOverseerControlMechs", true);
            Scribe_Values.Look(ref HoldOpenDoorInstantly, "tinygrox.ModSettings.HoldOpenDoorInstantly", true);
            Scribe_Values.Look(ref TransporterAutoload, "tinygrox.ModSettings.TransporterAutoload", true);
            Scribe_Values.Look(ref MortarsAutoCool, "tinygrox.ModSettings.MortarsAutoCool", true);
            Scribe_Values.Look(ref NoPrisonBreak, "tinygrox.ModSettings.NoPrisonBreak", true);
            Scribe_Values.Look(ref MechChargeRatePatch, "tinygrox.ModSettings.MechChargeRatePatch", true);
            Scribe_Values.Look(ref MechChargeRate, "tinygrox.ModSettings.MechChargeRate", 0.05f);
            Scribe_Values.Look(ref RimefellerUnmannedPatch, "tinygrox.ModSettings.RimefellerUnmannedPatch", true);
            Scribe_Values.Look(ref RimefellerLocPatch, "tinygrox.ModSettings.RimefellerLocPatch", true);
            Scribe_Values.Look(ref RimatomicUnmannedPatch, "tinygrox.ModSettings.RimatomicUnmannedPatch", true);
            Scribe_Values.Look(ref RimatomicLocPatch, "tinygrox.ModSettings.RimatomicLocPatch", true);
            Scribe_Values.Look(ref RimatomicFixPatch, "tinygrox.ModSettings.RimatomicFixPatch", true);
            Scribe_Values.Look(ref NoMoreRelationGen, "tinygrox.ModSettings.NoMoreRelationGen", true);
            Scribe_Values.Look(ref NoBreakDownPatch, "tinygrox.ModSettings.NoBreakDownPatch", true);
            Scribe_Values.Look(ref DBHLocPatch, "tinygrox.ModSettings.DBHLocPatch", true);
        }
    }
}
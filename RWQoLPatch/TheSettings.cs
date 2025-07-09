using System.Collections.Generic;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLPatch
{
    public class TheSettings: ModSettings
    {
        public static int PreceptRoleNum = 2;
        public static bool TogglePowerInstantly;
        public static bool SubCoreScannerShowPawnType;
        public static bool DownedOverseerControlMechs;
        public static bool HoldOpenDoorInstantly;
        public static bool TransporterAutoload;
        public static bool MortarsAutoCool;
        public static bool NoPrisonBreak;
        public static bool MechChargeRatePatch;
        public static float MechChargeRate = 0.05f;
        public static bool NoMoreRelationGen;
        public static bool RimefellerUnmannedPatch;
        public static bool RimefellerLocPatch;
        public static bool RimatomicUnmannedPatch;
        public static bool RimatomicLocPatch;
        public static bool RimatomicFixPatch;
        public static bool NoBreakDownPatch;
        public static bool DBHLocPatch;
        public static bool PlanetCoveragesModify;
        public static bool FloorNotOverrideFloor;
        public static bool LWMDeepStorageLocPatch;
        public static FloatRange CaravanNightRestTime;
        public static bool NoShortCircuit;
        public static bool NoSolarFlare;
        public static bool NoCenterDrop;
        public static bool NoEnityEscape;
        public static bool NoPsyfocusDown;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref PreceptRoleNum, "tinygrox.ModSettings.PreceptRoleNum", 2);
            Scribe_Values.Look(ref TogglePowerInstantly, "tinygrox.ModSettings.TogglePowerInstantly", true);
            Scribe_Values.Look(ref SubCoreScannerShowPawnType, "tinygrox.ModSettings.SubCorePawnType", true);
            Scribe_Values.Look(ref DownedOverseerControlMechs, "tinygrox.ModSettings.DownedOverseerControlMechs", false);
            Scribe_Values.Look(ref HoldOpenDoorInstantly, "tinygrox.ModSettings.HoldOpenDoorInstantly", true);
            Scribe_Values.Look(ref TransporterAutoload, "tinygrox.ModSettings.TransporterAutoload", true);
            Scribe_Values.Look(ref MortarsAutoCool, "tinygrox.ModSettings.MortarsAutoCool", true);
            Scribe_Values.Look(ref NoPrisonBreak, "tinygrox.ModSettings.NoPrisonBreak", false);
            Scribe_Values.Look(ref MechChargeRatePatch, "tinygrox.ModSettings.MechChargeRatePatch", false);
            Scribe_Values.Look(ref MechChargeRate, "tinygrox.ModSettings.MechChargeRate", 0.05f);
            Scribe_Values.Look(ref RimefellerUnmannedPatch, "tinygrox.ModSettings.RimefellerUnmannedPatch", true);
            Scribe_Values.Look(ref RimefellerLocPatch, "tinygrox.ModSettings.RimefellerLocPatch", true);
            Scribe_Values.Look(ref RimatomicUnmannedPatch, "tinygrox.ModSettings.RimatomicUnmannedPatch", true);
            Scribe_Values.Look(ref RimatomicLocPatch, "tinygrox.ModSettings.RimatomicLocPatch", true);
            Scribe_Values.Look(ref RimatomicFixPatch, "tinygrox.ModSettings.RimatomicFixPatch", false);
            Scribe_Values.Look(ref NoMoreRelationGen, "tinygrox.ModSettings.NoMoreRelationGen", false);
            Scribe_Values.Look(ref NoBreakDownPatch, "tinygrox.ModSettings.NoBreakDownPatch", false);
            Scribe_Values.Look(ref DBHLocPatch, "tinygrox.ModSettings.DBHLocPatch", false);
            Scribe_Values.Look(ref FloorNotOverrideFloor, "tinygrox.ModSettings.NoOverrideTerrain", false);
            Scribe_Values.Look(ref PlanetCoveragesModify, "tinygrox.ModSettings.PlanetCoveragesModify", false);
            Scribe_Values.Look(ref LWMDeepStorageLocPatch, "tinygrox.ModSettings.LWMDeepStorageLocPatch", true);
            Scribe_Values.Look(ref CaravanNightRestTime.min, "tinygrox.ModSettings.CaravanNightRestTime.Min", 6f);
            Scribe_Values.Look(ref CaravanNightRestTime.max, "tinygrox.ModSettings.CaravanNightRestTime.Max", 22f);
            Scribe_Values.Look(ref NoShortCircuit, "tinygrox.ModSettings.NoShortCircuit", false);
            Scribe_Values.Look(ref NoSolarFlare, "tinygrox.ModSettings.NoSolarFlare", false);
            Scribe_Values.Look(ref NoCenterDrop, "tinygrox.ModSettings.NoCenterDrop", false);
            Scribe_Values.Look(ref NoEnityEscape, "tinygrox.ModSettings.NoEnityEscape", false);
            Scribe_Values.Look(ref NoPsyfocusDown, "tinygrox.ModSettings.NoPsyfocusDown", false);
        }
    }
}
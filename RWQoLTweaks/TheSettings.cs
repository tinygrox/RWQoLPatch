using Verse;

namespace RWQoLTweaks
{
    public class TheSettings: ModSettings
    {
        public static int PreceptRoleNum = 2;
        public static bool TogglePowerInstantly = true;
        public static bool SubCoreScannerShowPawnType = true;
        public static bool DownedOverseerControlMechs;
        public static bool HoldOpenDoorInstantly = true;
        public static bool HoldOpenDoorInstantlyOnlyPoweredAutoDoor;
        public static bool TransporterAutoload = true;
        public static bool MortarsAutoCool = true;
        public static bool NoPrisonBreak;
        public static bool MechChargeRatePatch = true;
        public static float MechChargeRate = 0.05f;
        public static bool NoMoreRelationGen;
        public static bool NoBreakDownPatch;
        public static bool PlanetCoveragesModify;
        public static bool FloorNotOverrideFloor;
        public static bool LWMDeepStorageLocPatch;
        public static FloatRange CaravanNightRestTime = new FloatRange(6f, 22f);
        public static bool NoShortCircuit;
        public static bool NoSolarFlare;
        public static bool NoCenterDrop;
        public static bool NoEnityEscape;
        public static bool NoPsyfocusDown;
        public static bool HackingNeverLockOut;
        public static bool NoApparelDamagedThoughtPatch;
        public static bool OrbitalHabitatCheck;
        public static bool ShuttleNoCoolDownPatch;
        public static bool NoMechCommandRadius;
        public static bool IgnorePyrophobiaAtZeroFlammability;
        
        // Mods
        public static bool VEFLocPatch = true;
        public static bool DBHLocPatch;
        public static bool RimefellerUnmannedPatch;
        public static bool RimefellerLocPatch;
        public static bool RimatomicUnmannedPatch;
        public static bool RimatomicLocPatch;
        // public static bool RimatomicFixPatch;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref PreceptRoleNum, "tinygrox.ModSettings.PreceptRoleNum", 2);
            Scribe_Values.Look(ref TogglePowerInstantly, "tinygrox.ModSettings.TogglePowerInstantly", true);
            Scribe_Values.Look(ref HoldOpenDoorInstantlyOnlyPoweredAutoDoor, "tinygrox.ModSettings.HoldOpenDoorInstantlyOnlyPoweredAutoDoor", false);
            Scribe_Values.Look(ref SubCoreScannerShowPawnType, "tinygrox.ModSettings.SubCorePawnType", true);
            Scribe_Values.Look(ref DownedOverseerControlMechs, "tinygrox.ModSettings.DownedOverseerControlMechs");
            Scribe_Values.Look(ref HoldOpenDoorInstantly, "tinygrox.ModSettings.HoldOpenDoorInstantly", true);
            Scribe_Values.Look(ref TransporterAutoload, "tinygrox.ModSettings.TransporterAutoload", true);
            Scribe_Values.Look(ref MortarsAutoCool, "tinygrox.ModSettings.MortarsAutoCool", true);
            Scribe_Values.Look(ref NoPrisonBreak, "tinygrox.ModSettings.NoPrisonBreak");
            Scribe_Values.Look(ref MechChargeRatePatch, "tinygrox.ModSettings.MechChargeRatePatch");
            Scribe_Values.Look(ref MechChargeRate, "tinygrox.ModSettings.MechChargeRate", 0.05f);
            Scribe_Values.Look(ref RimefellerUnmannedPatch, "tinygrox.ModSettings.RimefellerUnmannedPatch", true);
            Scribe_Values.Look(ref RimefellerLocPatch, "tinygrox.ModSettings.RimefellerLocPatch", true);
            Scribe_Values.Look(ref RimatomicUnmannedPatch, "tinygrox.ModSettings.RimatomicUnmannedPatch", true);
            Scribe_Values.Look(ref RimatomicLocPatch, "tinygrox.ModSettings.RimatomicLocPatch", true);
            Scribe_Values.Look(ref NoMoreRelationGen, "tinygrox.ModSettings.NoMoreRelationGen");
            Scribe_Values.Look(ref NoBreakDownPatch, "tinygrox.ModSettings.NoBreakDownPatch");
            Scribe_Values.Look(ref DBHLocPatch, "tinygrox.ModSettings.DBHLocPatch");
            Scribe_Values.Look(ref FloorNotOverrideFloor, "tinygrox.ModSettings.NoOverrideTerrain");
            Scribe_Values.Look(ref PlanetCoveragesModify, "tinygrox.ModSettings.PlanetCoveragesModify");
            Scribe_Values.Look(ref LWMDeepStorageLocPatch, "tinygrox.ModSettings.LWMDeepStorageLocPatch", true);
            Scribe_Values.Look(ref CaravanNightRestTime.min, "tinygrox.ModSettings.CaravanNightRestTime.Min", 6f);
            Scribe_Values.Look(ref CaravanNightRestTime.max, "tinygrox.ModSettings.CaravanNightRestTime.Max", 22f);
            Scribe_Values.Look(ref NoShortCircuit, "tinygrox.ModSettings.NoShortCircuit");
            Scribe_Values.Look(ref NoSolarFlare, "tinygrox.ModSettings.NoSolarFlare");
            Scribe_Values.Look(ref NoCenterDrop, "tinygrox.ModSettings.NoCenterDrop");
            Scribe_Values.Look(ref NoEnityEscape, "tinygrox.ModSettings.NoEnityEscape");
            Scribe_Values.Look(ref NoPsyfocusDown, "tinygrox.ModSettings.NoPsyfocusDown");
            Scribe_Values.Look(ref VEFLocPatch, "tinygrox.ModSettings.VEFLocPatch");
            Scribe_Values.Look(ref HackingNeverLockOut, "tinygrox.ModSettings.HackingNeverLockOut");
            Scribe_Values.Look(ref NoApparelDamagedThoughtPatch, "tinygrox.ModSettings.ApparelDamagedThoughtPatch");
            Scribe_Values.Look(ref OrbitalHabitatCheck, "tinygrox.ModSettings.OrbitalHabitatCheck");
            Scribe_Values.Look(ref ShuttleNoCoolDownPatch, "tinygrox.ModSettings.ShuttleNoCoolDownPatch");
            Scribe_Values.Look(ref NoMechCommandRadius, "tinygrox.ModSettings.NoMechCommandRadius");
            Scribe_Values.Look(ref IgnorePyrophobiaAtZeroFlammability, "tinygrox.ModSettings.IgnorePyrophobiaAtZeroFlammability");
        }
    }
}
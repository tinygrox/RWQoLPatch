using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RWQoLTweaks
{
    public static class LocalizationCache
    {
        public static class Core
        {
            public static readonly string HoldOpenDoorInstantly ="RWQoLTweaks_HoldOpenDoorInstantly".Translate();
            public static readonly string HoldOpenDoorInstantlyOnlyPoweredAutoDoor ="RWQoLTweaks_HoldOpenDoorInstantlyOnlyPoweredAutoDoor".Translate();
            public static readonly string MortarsAutoCool = "RWQoLTweaks_MortarsAutoCool".Translate();
            public static readonly string TogglePowerInstantly = "RWQoLTweaks_TogglePowerInstantly".Translate();
            public static readonly string NoPrisonBreak = "RWQoLTweaks_NoPrisonBreak".Translate();
            public static readonly string NoMoreRelation = "RWQoLTweaks_NoMoreRelation".Translate();
            public static readonly string NoBreakDownPatch = "RWQoLTweaks_NoBreakDownPatch".Translate();
            public static readonly string FloorNotOverrideFloor = "RWQoLTweaks_FloorNotOverrideFloor".Translate();
            public static readonly string FloorNotOverrideFloor_Tips = "RWQoLTweaks_FloorNotOverrideFloor_Tips".Translate();
            public static readonly string PlanetCoveragesModify = "RWQoLTweaks_PlanetCoveragesModify".Translate();
            public static readonly string CaravanNightRestTimeTooltips = "RWQoLTweaks_CaravanNightRestTime_tooltips".Translate();
            public static readonly string NoShortCircuit = "RWQoLTweaks_NoShortCircuit".Translate();
            public static readonly string NoSolarFlare = "RWQoLTweaks_NoSolarFlare".Translate();
            public static readonly string NoCenterDrop = "RWQoLTweaks_NoCenterDrop".Translate();
            public static readonly string NoApparelDamagedThought = "RWQoLTweaks_NoApparelDamagedThought".Translate();
            
            private static string cacheCaravanNightRestTime = string.Empty;
            private static FloatRange cacheTime = new FloatRange(0,0);

            public static string CaravanNightRestTime(FloatRange range)
            {
                if (range != cacheTime)
                {
                    cacheTime = range;
                    cacheCaravanNightRestTime = "RWQoLTweaks_CaravanNightRestTime".Translate(range.min, range.max);
                }

                return cacheCaravanNightRestTime;
            }
        }
        public static class Royalty
        { 
            public static readonly string SetTransporterAutoLoad = "RWQoLTweaks_SetTransporterAutoLoad".Translate();
            public static readonly string NoPsyfocusDown = "RWQoLTweaks_NoPsyfocusDown".Translate();
        }

        public static class Ideology
        {
            private static int lastCount = -1;
            private static string cachePreceptRoleNum = string.Empty;

            public static string PreceptRoleNum(int count)
            {
                if (count != lastCount)
                {
                    lastCount = count;
                    cachePreceptRoleNum = "RWQoLTweaks_PreceptRoleNum".Translate(count);
                }

                return cachePreceptRoleNum;
            }
        }
        
        public static class Biotech
        {
            public static readonly string SubCoreScannerShowPawnType =
                "RWQoLTweaks_SubCoreScannerShowPawnType".Translate();

            public static readonly string DownedOverseerControlMechs =
                "RWQoLTweaks_DownedOverseerControlMechs".Translate();

            public static readonly string MechChargeRatePatch = "RWQoLTweaks_MechChargeRatePatch".Translate();
            public static readonly string NoMechCommandRadius = "RWQoLTweaks_NoMechCommandRadius".Translate();
            public static readonly string IgnorePyrophobiaAtZeroFlammability = "RWQoLTweaks_IgnorePyrophobiaAtZeroFlammability".Translate();

            private static float lastChargeRate = -1f;
            private static string cacheTips = string.Empty;

            public static string MechChargeRatePatchTips(float mechChargeRate)
            {
                if (!Mathf.Approximately(mechChargeRate, lastChargeRate))
                {
                    lastChargeRate = mechChargeRate;
                    cacheTips = "RWQoLTweaks_MechChargeRatePatchTips".Translate
                    (
                        mechChargeRate.ToString("F2"),
                        Mathf.RoundToInt(100f / mechChargeRate),
                        100f / mechChargeRate * 60f
                    );
                }

                return cacheTips;
            }
        }

        public static class Anomaly
        {
            public static readonly string NoEnityEscape = "RWQoLTweaks_NoEnityEscape".Translate();
        }

        public static class Odyssey
        {
            public static readonly string HackingNeverLockOut = "RWQoLTweaks_HackingNeverLockOut".Translate();
            public static readonly string OrbitalHabitatCheck = "RWQoLTweaks_OrbitalHabitatCheck".Translate();
            public static readonly string ShuttleNoCoolDown = "RWQoLTweaks_ShuttleNoCoolDown".Translate();
        }
        public static class Rimefeller
        {
            public static readonly string RimefellerUnmanPatch = "RWQoLTweaks_RimefellerUnmanPatch".Translate();
            public static readonly string RimefellerLocPatch = "RWQoLTweaks_RimefellerLocPatch".Translate();
            public static readonly string RimefellerLocPatch_Buffer = "RWQoLTweaks_RimefellerLocPatch_buffer".Translate();
            public static readonly string RimefellerLocPatch_Products = "RWQoLTweaks_RimefellerLocPatch_Products".Translate();
            public static readonly string RimefellerLocPatch_TPB = "RWQoLTweaks_RimefellerLocPatch_TPB".Translate();
        }
        
        public static class Rimatomic
        {
            public static readonly string RimatomicUnmannedPatch = "RWQoLTweaks_RimatomicUnmannedPatch".Translate();
            public static readonly string RimatomicLocPatch = "RWQoLTweaks_RimatomicLocPatch".Translate();
            public static readonly string RimatomicLocPatch_Tracking = "RWQoLTweaks_RimatomicLocPatch_Tracking".Translate();
        }

        public static class DBH
        {
            public static readonly string DBHLocPatch = "RWQoLTweaks_DBHLocPatch".Translate();
            public static readonly string DBHLocPatch_Water = "RWQoLTweaks_DBHLocPatch_CompWaterFillable".Translate();
            public static readonly string DBHLocPatch_Cap = "RWQoLTweaks_DBHLocPatch_PlaceWorker_UserGrid".Translate();
        }

        public static class LWMDS
        {
            public static readonly string LWMDeepStorageLocPatch = "RWQoLTweaks_LWMDeepStorageLocPatch".Translate();
        }

        public static class VEF
        {
            public static readonly string VEFLocPatch = "RWQoLTweaks_VEFLocPatch".Translate();
        }
    }
}

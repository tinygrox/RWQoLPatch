using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWQoLPatch
{
    // [StaticConstructorOnStartup]
    public static class LocalizationCache
    {
        public static class Core
        {
            public static readonly string HoldOpenDoorInstantly ="RWQoLPatch_HoldOpenDoorInstantly".Translate();
            public static readonly string MortarsAutoCool = "RWQoLPatch_MortarsAutoCool".Translate();
            public static readonly string TogglePowerInstantly = "RWQoLPatch_TogglePowerInstantly".Translate();
            public static readonly string NoPrisonBreak = "RWQoLPatch_NoPrisonBreak".Translate();
            public static readonly string NoMoreRelation = "RWQoLPatch_NoMoreRelation".Translate();
            public static readonly string NoBreakDownPatch = "RWQoLPatch_NoBreakDownPatch".Translate();
            public static readonly string FloorNotOverrideFloor = "RWQoLPatch_FloorNotOverrideFloor".Translate();
            public static readonly string FloorNotOverrideFloor_Tips = "RWQoLPatch_FloorNotOverrideFloor_Tips".Translate();
            public static readonly string PlanetCoveragesModify = "RWQoLPatch_PlanetCoveragesModify".Translate();
            public static readonly string CaravanNightRestTimeTooltips = "RWQoLPatch_CaravanNightRestTime_tooltips".Translate();
            public static readonly string NoShortCircuit = "RWQoLPatch_NoShortCircuit".Translate();
            public static readonly string NoSolarFlare = "RWQoLPatch_NoSolarFlare".Translate();
            public static readonly string NoCenterDrop = "RWQoLPatch_NoCenterDrop".Translate();
            private static string cacheCaravanNightRestTime = string.Empty;
            private static FloatRange cacheTime = new FloatRange(0,0);

            public static string CaravanNightRestTime(FloatRange range)
            {
                if (range != cacheTime)
                {
                    cacheTime = range;
                    cacheCaravanNightRestTime = "RWQoLPatch_CaravanNightRestTime".Translate(range.min, range.max);
                }

                return cacheCaravanNightRestTime;
            }
        }
        public static class Royalty
        { 
            public static readonly string SetTransporterAutoLoad = "RWQoLPatch_SetTransporterAutoLoad".Translate();
            public static readonly string NoPsyfocusDown = "RWQoLPatch_NoPsyfocusDown".Translate();
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
                    cachePreceptRoleNum = "RWQoLPatch_PreceptRoleNum".Translate(count);
                }

                return cachePreceptRoleNum;
            }
        }
        
        public static class Biotech
        {
            public static readonly string SubCoreScannerShowPawnType =
                "RWQoLPatch_SubCoreScannerShowPawnType".Translate();

            public static readonly string DownedOverseerControlMechs =
                "RWQoLPatch_DownedOverseerControlMechs".Translate();

            public static readonly string MechChargeRatePatch = "RWQoLPatch_MechChargeRatePatch".Translate();

            private static float lastChargeRate = -1f;
            private static string cacheTips = string.Empty;

            public static string MechChargeRatePatchTips(float mechChargeRate)
            {
                if (!Mathf.Approximately(mechChargeRate, lastChargeRate))
                {
                    lastChargeRate = mechChargeRate;
                    cacheTips = "RWQoLPatch_MechChargeRatePatchTips".Translate
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
            public static readonly string NoEnityEscape = "RWQoLPatch_NoEnityEscape".Translate();
        }
        
    }
}

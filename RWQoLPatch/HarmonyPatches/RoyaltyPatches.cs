using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLPatch.HarmonyPatches
{
    public class RoyaltyPatches: AbstractPatchBase
    {
        private static readonly Action<CompShuttle> CheckAutoLoad = (Action<CompShuttle>)Delegate.CreateDelegate(
            typeof(Action<CompShuttle>),
            AccessTools.Method(typeof(CompShuttle), "CheckAutoload")
        );
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo(
                    "穿梭机自动装载",
                    AccessTools.Method(typeof(CompShuttle), nameof(CompShuttle.PostSpawnSetup), new[] { typeof(bool) }),
                    new HarmonyMethod(typeof(RoyaltyPatches), nameof(SetShuttleAutoload)),
                    HarmonyPatchType.Postfix
                ),
            };
        }
        public static void SetShuttleAutoload(ref bool ___autoload, CompShuttle __instance)
        {
            if(__instance.Autoload && !TheSettings.TransporterAutoload) return;
            
            ___autoload = true;
            if (!__instance.Transporter.LoadingInProgressOrReadyToLaunch)
                TransporterUtility.InitiateLoading(Gen.YieldSingle(__instance.Transporter));
            CheckAutoLoad(__instance);
        }

        public override bool IsModLoaded(IModChecker modChecker) => ModsConfig.RoyaltyActive;

        protected override string ModDisplayName => "Royalty";
        protected override string ModId => "Ludeon.RimWorld.Royalty";

    }
}
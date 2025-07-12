using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class RoyaltyPatches: AbstractPatchBase
    {
        private static readonly Action<CompShuttle> CheckAutoLoad = (Action<CompShuttle>)Delegate.CreateDelegate(
            typeof(Action<CompShuttle>),
            AccessTools.Method(typeof(CompShuttle), "CheckAutoload")
        );

        private static readonly Func<CompShuttle, bool> Autoloadable = (Func<CompShuttle, bool>)Delegate.CreateDelegate(
            typeof(Func<CompShuttle, bool>),
            null,
            AccessTools.PropertyGetter(typeof(CompShuttle), "Autoloadable")
        );
        public static void SetShuttleAutoload(ref bool ___autoload, CompShuttle __instance)
        {
            if(!TheSettings.TransporterAutoload || !Autoloadable(__instance)) return;
            
            ___autoload = true;
            if (!__instance.Transporter.LoadingInProgressOrReadyToLaunch)
                TransporterUtility.InitiateLoading(Gen.YieldSingle(__instance.Transporter));
            CheckAutoLoad(__instance);
        }

        public static IEnumerable<CodeInstruction> NoPsyfocusDownPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            
            var tMethod = AccessTools.PropertyGetter(typeof(Pawn_PsychicEntropyTracker), nameof(Pawn_PsychicEntropyTracker.IsCurrentlyMeditating));
            var settingField = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.NoPsyfocusDown));

            for (int i = 0; i < codeList.Count - 1; i++)
            {
                // 纳闷了 C# 8.0 不支持 not
                if (codeList[i].opcode != OpCodes.Call || !(codeList[i].operand is MethodInfo method) || method != tMethod)
                    continue;
                
                if (codeList[i + 1].opcode == OpCodes.Brtrue_S && codeList[i + 1].operand is Label label)
                {
                    // -> if (!this.IsCurrentlyMeditating && !TheSettings.NoPsyfocusDown)
                    codeList.InsertRange(i + 2, new[]
                    {
                        new CodeInstruction(OpCodes.Ldsfld, settingField),
                        new CodeInstruction(OpCodes.Brtrue, label)
                    });
                    break;
                }
            }
            
            return codeList.AsEnumerable();
        }
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
                new HarmonyPatchInfo(
                    "精神力不自动衰减",
#if RIMWORLD_1_5
                    AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), nameof(Pawn_PsychicEntropyTracker.PsychicEntropyTrackerTick)),
#elif RIMWORLD_1_6
                    AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), nameof(Pawn_PsychicEntropyTracker.PsychicEntropyTrackerTickInterval), new[] { typeof(int) }),
#endif
                    new HarmonyMethod(typeof(RoyaltyPatches), nameof(NoPsyfocusDownPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }


        public override bool IsModLoaded(IModChecker modChecker) => ModsConfig.RoyaltyActive;

        protected override string ModDisplayName => "Royalty DLC";
        protected override string ModId => "Ludeon.RimWorld.Royalty";

    }
}
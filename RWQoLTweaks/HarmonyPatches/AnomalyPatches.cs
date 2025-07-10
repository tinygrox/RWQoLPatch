using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class AnomalyPatches:AbstractPatchBase
    {
        public static IEnumerable<CodeInstruction> NoEnityEscape(IEnumerable<CodeInstruction> codeInstructions)
        {
            var tMethod = AccessTools.Method(typeof(Rand), nameof(Rand.MTBEventOccurs),
                new[] { typeof(float), typeof(float), typeof(float) });
            var codeList = codeInstructions.ToList();
            for (int i = 0; i < codeList.Count - 1; i++)
            {
                if (codeList[i].opcode == OpCodes.Call && codeList[i].operand is MethodInfo method && method == tMethod)
                {
                    if (codeList[i + 1].opcode == OpCodes.Brfalse_S && codeList[i + 1].operand is Label endRet)
                    { 
                        codeList.InsertRange(i + 2, new[]
                        {
                            new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(TheSettings), nameof(TheSettings.NoEnityEscape))),
                            new CodeInstruction(OpCodes.Brfalse_S, endRet)
                        });
                        break;
                    }
                }
            }
            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo
                (
                    "禁用实体随机收容失效",
                    AccessTools.Method(typeof(RimWorld.CompHoldingPlatformTarget), "CaptivityTick",
                        new[] { typeof(Pawn) }),
                    new HarmonyMethod(typeof(AnomalyPatches), nameof(NoEnityEscape)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "Anomaly";
        protected override string ModId => "Ludeon.RimWorld.Anomaly";
        public override bool IsModLoaded(IModChecker modChecker) => ModsConfig.AnomalyActive;
    }
}
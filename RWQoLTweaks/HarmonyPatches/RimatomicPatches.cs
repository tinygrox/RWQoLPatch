using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class RimatomicPatches:AbstractPatchBase
    {
        public static void RimatomicUnmannedPatch(ref bool __result)
        {
            __result = __result || TheSettings.RimatomicUnmannedPatch;
        }

        private static string TrackingLabelTranslate()
        {
            if (!TheSettings.RimatomicLocPatch)
                return "Tracking";
            return "RWQoLPatch_RimatomicLocPatch_Tracking".Translate().ToString();
        }

        public static IEnumerable<CodeInstruction> RimatomicTrackingLocPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();

            for (int i = 0; i < codeList.Count; i++)
            {
                if (codeList[i].opcode == OpCodes.Ldstr && (codeList[i].operand as string)!.Contains("Tracking"))
                {
                    codeList[i].opcode =  OpCodes.Call;
                    codeList[i].operand = AccessTools.Method(typeof(RimatomicPatches), nameof(TrackingLabelTranslate));
                    break;
                }
            }

            return codeList.AsEnumerable();
        }

        public static void RimatomicStoragePoolFix(ref string __result)
        {
            if (__result == null! && TheSettings.RimatomicFixPatch)
            {
                __result = "GroupPlural".Translate();
            }
        }
        
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo
                (
                    "Rimatomic 无人操作补丁",
                    AccessTools.PropertyGetter(AccessTools.TypeByName("Rimatomics.WeaponsConsole"), "Manned"),
                    new HarmonyMethod(typeof(RimatomicPatches), nameof(RimatomicUnmannedPatch)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo
                (
                    "Rimatomic 硬编码翻译补充",
                    AccessTools.Method(AccessTools.TypeByName("Rimatomics.WeaponsConsole+<GetGizmos>d__29"),
                        "MoveNext"),
                    new HarmonyMethod(typeof(RimatomicPatches), nameof(RimatomicTrackingLocPatch)),
                    HarmonyPatchType.Transpiler
                ),
#if RIMWORLD_1_5
                new HarmonyPatchInfo
                (
                    "Rimatomic 存储池修复",
                    AccessTools.PropertyGetter(AccessTools.TypeByName("Rimatomics.Building_storagePool"),
                        "GroupingLabel"),
                    new HarmonyMethod(typeof(RimatomicPatches), nameof(RimatomicStoragePoolFix)),
                    HarmonyPatchType.Postfix
                ),
#endif
            };
        }

        protected override string ModDisplayName => "Rimatomic";
        protected override string ModId => "dubwise.rimatomics";
    }
}
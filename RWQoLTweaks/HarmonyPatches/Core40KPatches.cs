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
    public class Core40KPatches: AbstractPatchBase
    {
        public static IEnumerable<CodeInstruction> LetterIDFixPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            var targetMethod = AccessTools.PropertyGetter(typeof(Find), nameof(Find.LetterStack));

            for (int i = 0; i < codeList.Count; i++)
            {
                if(codeList[i].opcode != OpCodes.Call || !(codeList[i].operand is MethodInfo methodInfo) || methodInfo != targetMethod) continue;
                
                codeList.InsertRange(i, new []
                {
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Find), nameof(Find.UniqueIDsManager))),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(UniqueIDsManager), nameof(UniqueIDsManager.GetNextLetterID))),
                    new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Letter), nameof(Letter.ID))),
                });
                break;
            }

            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo(
                    "星际战士手术信息 ID 缺失修复",
                    AccessTools.Method(AccessTools.TypeByName("Core40k.Hediff_SendLetterAtSeverity"), "CompPostTick", new[] { typeof(float).MakeByRefType() }),
                    new HarmonyMethod(typeof(Core40KPatches), nameof(LetterIDFixPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "RimDark 40k - Framework";

        protected override string ModId => "phonicmas.rimdark.framework";
    }
}
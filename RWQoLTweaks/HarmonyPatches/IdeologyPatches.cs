using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class IdeologyPatches: AbstractPatchBase
    {
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo(
                    "专家数量上限修改",
                    AccessTools.Method(typeof(IdeoFoundation), nameof(IdeoFoundation.CanAdd),
                        new[] { typeof(PreceptDef), typeof(bool) }),
                    new HarmonyMethod(typeof(IdeologyPatches), nameof(PreceptRolesCountUnlimit)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "Ideology";
        protected override string ModId => "Ludeon.RimWorld.Ideology";
        public override bool IsModLoaded(IModChecker modChecker) => ModsConfig.IdeologyActive;
        
        public static IEnumerable<CodeInstruction> PreceptRolesCountUnlimit(IEnumerable<CodeInstruction> codeinstructions)
        {
            var codeList = codeinstructions.ToList();
            int replaced = 0;
            for (int i = 0;i + 1 < codeList.Count; i++)
            {
                if (replaced >= 2)
                    break;
                
                if (codeList[i].opcode == OpCodes.Ldc_I4_2 && codeList[i + 1].opcode == OpCodes.Blt_S)
                {
                    codeList[i].opcode = OpCodes.Ldsfld;
                    codeList[i].operand = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.PreceptRoleNum));
                    replaced++;
                }

                if (codeList[i].opcode == OpCodes.Ldc_I4_2 && codeList[i - 1].opcode == OpCodes.Ldstr)
                {
                    codeList[i].opcode = OpCodes.Ldsfld;
                    codeList[i].operand = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.PreceptRoleNum));
                    replaced++;
                }
                
            }
            return codeList.AsEnumerable();
        }
    }
}
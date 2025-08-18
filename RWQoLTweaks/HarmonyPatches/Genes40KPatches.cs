using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class Genes40KPatches:AbstractPatchBase
    {
        public static IEnumerable<CodeInstruction> TryAbsorbNutritiousThingTranspiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();

            var targetMethod = AccessTools.Method(typeof(Entity), nameof(Entity.DeSpawn), new[] { typeof(DestroyMode) });
            foreach (var t in codeList)
            {
                if(t.opcode != OpCodes.Callvirt || !(t.operand is MethodInfo method) || method != targetMethod)
                    continue;

                t.operand = AccessTools.Method(typeof(Thing), nameof(Thing.Destroy), new[] { typeof(DestroyMode) });
                break;
            }

            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>
            {
                new HarmonyPatchInfo(
                    "原体培养修复",
                    AccessTools.Method(AccessTools.TypeByName("Genes40k.Building_PrimarchGrowthVat"), "TryAbsorbNutritiousThing"),
                    new HarmonyMethod(typeof(Genes40KPatches), nameof(Genes40KPatches.TryAbsorbNutritiousThingTranspiler)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "RimDark 40k - Mankinds Finest";

        protected override string ModId => "phonicmas.rimdark.mankindsfinest";
    }
}
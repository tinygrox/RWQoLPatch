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
    public class VanillaExpandedFrameworkPatches: AbstractPatchBase
    {
        private static List<FloatMenuOption> LocQualityFloatMenu;

        private static readonly Dictionary<string, string> QualityLabelReplacements = new Dictionary<string, string>()
        {
            { nameof(QualityCategory.Awful), QualityCategory.Awful.GetLabel().CapitalizeFirst() },
            { nameof(QualityCategory.Poor), QualityCategory.Poor.GetLabel().CapitalizeFirst() },
            { nameof(QualityCategory.Normal), QualityCategory.Normal.GetLabel().CapitalizeFirst() },
            { nameof(QualityCategory.Good), QualityCategory.Good.GetLabel().CapitalizeFirst() },
            { nameof(QualityCategory.Excellent), QualityCategory.Excellent.GetLabel().CapitalizeFirst() },
            { nameof(QualityCategory.Masterwork), QualityCategory.Masterwork.GetLabel().CapitalizeFirst() },
            { nameof(QualityCategory.Legendary), QualityCategory.Legendary.GetLabel().CapitalizeFirst() },
        };
        public static void VEFQualitySelectionsLocPatch (ref List<FloatMenuOption> __result)
        {
            if (!TheSettings.VEFLocPatch) return;

            if (LocQualityFloatMenu == null)
            {
                LocQualityFloatMenu = __result.ListFullCopy();
                foreach (var floatMenuOption in LocQualityFloatMenu)
                {
                    if (QualityLabelReplacements.TryGetValue(floatMenuOption.Label, out string localized))
                    {
                        floatMenuOption.Label = localized;
                    }
                }
            }

            __result = LocQualityFloatMenu;
        }
        public static IEnumerable<CodeInstruction> VEFProcessDoInterfaceLocPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            var tMethod = AccessTools.Method(typeof(object), nameof(ToString));
            var dMethod = AccessTools.Method(typeof(Utilities_Method), nameof(Utilities_Method.GetQualityLabel), new[] { typeof(QualityCategory) });
            for (int i = 1; i < codeList.Count; i++)
            {
                if (codeList[i].opcode != OpCodes.Callvirt || !(codeList[i].operand is MethodInfo method) || method != tMethod) continue;

                if (codeList[i - 1].opcode != OpCodes.Constrained || !(codeList[i - 1].operand is Type type) || type != typeof(QualityCategory)) continue;
                
                codeList[i - 1].opcode = OpCodes.Ldobj;
                codeList[i].opcode = OpCodes.Call;
                codeList[i].operand = dMethod;
                
                break;
            }
            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>
            {
                new HarmonyPatchInfo
                (
                    "VEF 翻译补充 - Process.QualitySelections",
                    AccessTools.PropertyGetter(AccessTools.TypeByName("PipeSystem.Process"), "QualitySelections"),
                    new HarmonyMethod(typeof(VanillaExpandedFrameworkPatches), nameof(VEFQualitySelectionsLocPatch)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo
                (
                    "VEF 翻译补充 - Process.DoInterface",
                    AccessTools.Method(AccessTools.TypeByName("PipeSystem.Process"), "DoInterface", new []{typeof(float), typeof(float), typeof(float), typeof(int)}),
                    new HarmonyMethod(typeof(VanillaExpandedFrameworkPatches), nameof(VEFProcessDoInterfaceLocPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "Vanilla Expanded Framework";

        protected override string ModId => "oskarpotocki.vanillafactionsexpanded.core";
    }
}
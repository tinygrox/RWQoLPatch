using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TinyGroxMods.HarmonyFramework;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RWQoLTweaks.HarmonyPatches
{
    public class RimefellerPatches:AbstractPatchBase
    {
        private static string BufferLocPatch()
        {
            return !TheSettings.RimefellerLocPatch ? "Buffer: " : LocalizationCache.Rimefeller.RimefellerLocPatch_Buffer;
        }
        private static string ProductsLocPatch()
        {
            return !TheSettings.RimefellerLocPatch ? "Products" : LocalizationCache.Rimefeller.RimefellerLocPatch_Products;
        }
        private static string TPBLocPatch()
        {
            return !TheSettings.RimefellerLocPatch ? "\n\nTime per batch: " : "\n\n" + LocalizationCache.Rimefeller.RimefellerLocPatch_TPB;
        }
        public static IEnumerable<CodeInstruction> RimefellerCrudeCrackerUnmanPatch (IEnumerable<CodeInstruction> codeInstructions, ILGenerator iLGenerator)
        {
            var codeList = new List<CodeInstruction>(codeInstructions);
            var method = AccessTools.PropertyGetter(AccessTools.TypeByName("Rimefeller.MapComponent_Rimefeller"),
                "MannedConsole");
            for (int i = 1; i < codeList.Count; i++)
            {
                if (codeList[i - 1].opcode == OpCodes.Callvirt &&
                    (MethodInfo)codeList[i - 1].operand == method)
                {
                    Label label = (Label)codeList[i].operand;
                    Label newlabel = iLGenerator.DefineLabel();

                    codeList[i + 1].labels.Add(newlabel);
                    
                    codeList.InsertRange(i + 1, new List<CodeInstruction>()
                    {
                        new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(TheSettings), nameof(TheSettings.RimefellerUnmannedPatch))),
                        new CodeInstruction(OpCodes.Brfalse_S, label)
                    });

                    codeList[i].opcode = OpCodes.Brtrue_S;
                    codeList[i].operand = newlabel;
                    
                    break;
                }
            }

            return codeList.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> RimefellerRefineryUnmanPatch (IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilGenerator)
        {
            var codeList = new List<CodeInstruction>(codeInstructions);
            var method = AccessTools.PropertyGetter(AccessTools.TypeByName("Rimefeller.Building_ResourceConsole"),
                "Manned");
            for (int i = 1; i < codeList.Count; i++)
            {
                // if (building_ResourceConsole.Manned && this.Buffer < this.Props.BufferSize)
                // ->> if (building_ResourceConsole.Manned || TheSettings.RimefellerUnmanPatch) && this.Buffer < this.Props.BufferSize)
                if (codeList[i - 1].opcode == OpCodes.Callvirt && (MethodInfo)codeList[i - 1].operand == method)
                {
                    Label label = (Label)codeList[i].operand;
                    Label newlabel = ilGenerator.DefineLabel();

                    codeList[i + 1].labels.Add(newlabel);

                    codeList.InsertRange(i + 1, new List<CodeInstruction>()
                    {
                        new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(TheSettings), nameof(TheSettings.RimefellerUnmannedPatch))),
                        new CodeInstruction(OpCodes.Brfalse_S, label)
                    });
                    codeList[i].opcode = OpCodes.Brtrue_S;
                    codeList[i].operand = newlabel;
                    break;
                }
            }

            return codeList.AsEnumerable();
        }

        public static IEnumerable<CodeInstruction> RimefellerRefineryLocPatch(
            IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = new List<CodeInstruction>(codeInstructions);

            foreach (var t in codeList.Where(t => t.opcode == OpCodes.Ldstr && (string)t.operand == "Buffer: "))
            {
                t.opcode = OpCodes.Call;
                t.operand = AccessTools.Method(typeof(RimefellerPatches), nameof(BufferLocPatch));
                break;
            }
            
            return codeList.AsEnumerable();
        }

        public static IEnumerable<CodeInstruction> RimefellerResourceConsoleLocPatch(
            IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = new List<CodeInstruction>(codeInstructions);
            foreach (var t in codeList.Where(t => t.opcode == OpCodes.Ldstr && (string)t.operand == "Products"))
            {
                t.opcode = OpCodes.Call;
                t.operand = AccessTools.Method(typeof(RimefellerPatches), nameof(ProductsLocPatch));
                break;
            }

            return codeList.AsEnumerable();
        }
        
        public static IEnumerable<CodeInstruction> RimefellerResourceConsoleLocPatch2(
            IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = new List<CodeInstruction>(codeInstructions);
            foreach (var t in codeList.Where(t => t.opcode == OpCodes.Ldstr && (string)t.operand == "\n\nTime per batch: "))
            {
                t.opcode = OpCodes.Call;
                t.operand = AccessTools.Method(typeof(RimefellerPatches), nameof(TPBLocPatch));
                break;
            }

            return codeList.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> CleanOilFixPatch(IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilGenerator)
        {
            var codeList = codeInstructions.ToList();
            MethodInfo methodRFNT = AccessTools.Method(typeof(JobDriver), nameof(JobDriver.ReadyForNextToil));
            MethodInfo methodDesignationDelete =  AccessTools.Method(typeof(Designation), nameof(Designation.Delete));
            
            int targetIndex = codeList.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo mi && mi == methodRFNT);
            
            if (targetIndex == -1)
                return codeList.AsEnumerable();
            
            var ldargLabel = ilGenerator.DefineLabel();
            codeList[targetIndex + 1].labels.Add(ldargLabel);
            codeList[targetIndex - 6].operand = ldargLabel;

            var blockToMove = codeList.GetRange(targetIndex - 2, 3);
            codeList.RemoveRange(targetIndex - 2, 3);
            
            int deleteIndex = codeList.FindIndex(ci => ci.opcode == OpCodes.Callvirt &&  ci.operand is MethodInfo mi && mi == methodDesignationDelete);
            
            if (deleteIndex == -1)
                return codeList.AsEnumerable();
            
            codeList.InsertRange(deleteIndex + 1, blockToMove);
            var label2 = ilGenerator.DefineLabel();
            codeList[deleteIndex + 1].labels.Add(label2);
            codeList[deleteIndex - 2].operand = label2;
            
            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            // 当然最简单的是直接修改 Rimefeller.Building_ResourceConsole::Manned() 永远为真即可
            // 但是怕被其他联动 Mod 调用后导致不可控效果，所以还是得从 Mod 内部下手
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo
                (
                    "Rimefeller - CrudeCracker 无人补丁",
                    AccessTools.TypeByName("Rimefeller.CompCrudeCracker").Method("CompTick"),
                    new HarmonyMethod(typeof(RimefellerPatches),
                        nameof(RimefellerPatches.RimefellerCrudeCrackerUnmanPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo
                (
                    "Rimefeller - Refinery 无人补丁",
                    AccessTools.TypeByName("Rimefeller.CompRefinery").Method("CompTick"),
                    new HarmonyMethod(typeof(RimefellerPatches), nameof(RimefellerRefineryUnmanPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo
                (
                    "Rimefeller - Refinery 硬编码翻译补全",
                    AccessTools.TypeByName("Rimefeller.CompRefinery").Method("CompInspectStringExtra"),
                    new HarmonyMethod(typeof(RimefellerPatches), nameof(RimefellerRefineryLocPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo
                (
                    "Rimefeller - ITab_ResourceConsole 硬编码翻译补全",
                    AccessTools.TypeByName("Rimefeller.ITab_ResourceConsole").Method("FillTab"),
                    new HarmonyMethod(typeof(RimefellerPatches), nameof(RimefellerResourceConsoleLocPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo
                (
                    "Rimefeller - ITab_ResourceConsole 硬编码翻译补全2",
                    AccessTools.TypeByName("Rimefeller.ITab_ResourceConsole").Method("DoProductList", new[] { typeof(Rect) }),
                    new HarmonyMethod(typeof(RimefellerPatches), nameof(RimefellerResourceConsoleLocPatch2)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo
                (
                    "Rimefeller - 油污清理修复",
                    AccessTools.TypeByName("Rimefeller.JobDriver_CleanOil+<>c__DisplayClass4_0").Method("<MakeNewToils>b__0"),
                    new HarmonyMethod(typeof(RimefellerPatches), nameof(CleanOilFixPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "Rimefeller";
        protected override string ModId => "dubwise.rimefeller";
    }
}
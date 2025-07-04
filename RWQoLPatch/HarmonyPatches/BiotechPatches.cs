using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TinyGroxMods.HarmonyFramework;
using UnityEngine;
using Verse;

namespace RWQoLPatch.HarmonyPatches
{
    public class BiotechPatches: AbstractPatchBase
    {
        protected override string ModDisplayName => "Biotech";
        protected override string ModId => "ludeon.rimworld.biotech";

        public override bool IsModLoaded(IModChecker modChecker) => ModsConfig.BiotechActive;
        
        public static string Utility_GetTypeString(Pawn pawn)
        {
            if (!TheSettings.SubCoreScannerShowPawnType) return string.Empty;

            if (pawn.IsPrisoner)
            {
                return " (" + "Prisoner".Translate().CapitalizeFirst() + ")";
            }
            if (pawn.IsSlave)
            {
                return " (" + "Slave".Translate().CapitalizeFirst() + ")";
            }
            if (pawn.Faction != null)
            {
                return " (" + pawn.Faction.def.pawnSingular.CapitalizeFirst() + ")";
            }
            return " (" + "Colonist".Translate().CapitalizeFirst() + ")";
        }

        public static float Utility_MechChargeRateModify()
        {
            if (!TheSettings.MechChargeRatePatch)
                return 0.05f / 60f;
            return TheSettings.MechChargeRate / 60f;
        }

        private static float MechChargeRateStringInspect()
        {
            if (!TheSettings.MechChargeRatePatch)
            {
                return 50f;
            }
            return TheSettings.MechChargeRate * 1000f;
        }

        public static IEnumerable<CodeInstruction> Building_MechChargerTickPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();

            for (int i = 0; i < codeList.Count; i++)
            {
                if (codeList[i].opcode == OpCodes.Ldc_R4 &&
                    Mathf.Abs((float)codeList[i].operand - 0.05f / 60f) < 1e-7 &&
                    codeList[i + 1].opcode == OpCodes.Add) 
                {
                    codeList[i].opcode = OpCodes.Call;
                    codeList[i].operand = AccessTools.Method(typeof(BiotechPatches), nameof(Utility_MechChargeRateModify));
                    break;
                }
            }
            
            return codeList.AsEnumerable();
        }

        public static IEnumerable<CodeInstruction> MechChargeRateInspectPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            for (int i = 0; i < codeList.Count; i++)
            {
                if (codeList[i].opcode == OpCodes.Ldc_R4 && Math.Abs((float)codeList[i].operand - 50f) < 1e-7)
                {
                    codeList[i].opcode = OpCodes.Call;
                    codeList[i].operand = AccessTools.Method(typeof(BiotechPatches), nameof(MechChargeRateStringInspect));
                    break;
                }
            }
            return codeList.AsEnumerable();
        }

        public static IEnumerable<CodeInstruction> SubCoreScannerPawnSelectPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            for (int i = 0; i + 1 < codeList.Count; i++)
            {
                if (!(codeList[i].opcode == OpCodes.Callvirt && codeList[i].operand as MethodInfo ==
                        AccessTools.PropertyGetter(typeof(Entity), nameof(Entity.LabelShortCap))))
                    continue;
                
                // pawn.LabelShortCap => pawn.LabelShortCap + RWQoLPatch.HarmonyPatches.BiotechPatches.Utility_GetTypeString(pawn)
                if (codeList[i + 1].opcode == OpCodes.Ldloc_3)
                {
                    codeList.InsertRange(i + 1, new List<CodeInstruction>()
                    {
                        new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(AccessTools.TypeByName("RimWorld.Building_SubcoreScanner+<>c__DisplayClass45_0"), "pawn")),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BiotechPatches), nameof(Utility_GetTypeString), new[]{typeof(Pawn)})),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new[]{typeof(string), typeof(string)}))
                    });
                    break;
                }
            }
            return codeList.AsEnumerable();
        }

        public static IEnumerable<CodeInstruction> DownedCanControlMechs(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            for (int i = 0; i < codeList.Count; i++)
            {
                if (codeList[i].opcode == OpCodes.Brfalse_S)
                {
                    var firstLabel = (Label)codeList[i].operand;
                    codeList.InsertRange(i + 1, new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(TheSettings), nameof(TheSettings.DownedOverseerControlMechs))),
                        new CodeInstruction(OpCodes.Brfalse_S, firstLabel)
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
                    "次核建筑置入人员列表显示类型",
                    AccessTools.Method(typeof(Building_SubcoreScanner), "<GetGizmos>b__45_2"),
                    new HarmonyMethod(typeof(BiotechPatches), nameof(SubCoreScannerPawnSelectPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "机械师倒下也能控制机械体",
                    AccessTools.PropertyGetter(typeof(Pawn_MechanitorTracker),
                        nameof(Pawn_MechanitorTracker.CanControlMechs)),
                    new HarmonyMethod(typeof(BiotechPatches), nameof(DownedCanControlMechs)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "机械体充电速率修改",
                    AccessTools.Method(typeof(Building_MechCharger), "Tick"),
                    new HarmonyMethod(typeof(BiotechPatches), nameof(Building_MechChargerTickPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "机械体充电速率修改 - 相应信息",
                    AccessTools.Method(typeof(Pawn),nameof(Pawn.GetInspectString)),
                    new HarmonyMethod(typeof(BiotechPatches), nameof(MechChargeRateInspectPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

    }
}
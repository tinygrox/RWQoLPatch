using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using TinyGroxMods.HarmonyFramework;
using UnityEngine;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class OdysseyPatches: AbstractPatchBase
    {
        #region 废案
        // public static IEnumerable<CodeInstruction> GravshipNoMaxRangePatch(IEnumerable<CodeInstruction> codeInstructions, ILGenerator iLGenerator)
        // {
        //     var tField = AccessTools.Field(typeof(DebugSettings), nameof(DebugSettings.ignoreGravshipRange));
        //     var settingField = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.GravshipNoMaxRange));
        //     var codeList = codeInstructions.ToList();
        //     var fLabel = iLGenerator.DefineLabel();
        //     for (int i = 0; i < codeList.Count - 2; i++)
        //     {
        //         if (codeList[i].opcode != OpCodes.Ldsfld || !(codeList[i].operand is FieldInfo field) || field != tField) continue;
        //         
        //         if (codeList[i + 1].opcode != OpCodes.Brtrue_S || !(codeList[i + 1].operand is Label l)) continue;
        //         
        //         if (codeList[i + 2].opcode != OpCodes.Ldstr || !(codeList[i + 2].operand is string str) || str != "TransportPodDestinationBeyondMaximumRange")
        //             continue;
        //         
        //         codeList[i + 2].labels.Add(fLabel);
        //
        //         
        //         codeList.InsertRange(i + 2, new []
        //         { 
        //             new CodeInstruction(OpCodes.Ldsfld, settingField), 
        //             new CodeInstruction(OpCodes.Brtrue_S, l), 
        //         });
        //         codeList[i + 1].operand = fLabel;
        //         break;
        //     }
        //     return codeList.AsEnumerable();
        // }
        // public static IEnumerable<CodeInstruction> GravshipNoMaxRangeTooltipsPatch(IEnumerable<CodeInstruction> codeInstructions,  ILGenerator iLGenerator)
        // {
        //     var tMethod = AccessTools.Method(typeof(CompPilotConsole), "GetMaxLaunchDistance", new[] { typeof(PlanetLayer) });
        //     var settingField = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.GravshipNoMaxRange));
        //     var codeList = codeInstructions.ToList();
        //     for (int i = 0; i < codeList.Count - 5; i++)
        //     {
        //         if (codeList[i].opcode == OpCodes.Call && codeList[i].operand is MethodInfo method && method == tMethod)
        //         {
        //             if (codeList[i + 1].opcode == OpCodes.Ble_S && codeList[i + 1].operand is Label l)
        //             {
        //                 codeList.InsertRange(i + 2, new[]
        //                 {
        //                     new CodeInstruction(OpCodes.Ldsfld, settingField),
        //                     new CodeInstruction(OpCodes.Brtrue_S, l)
        //                 });
        //             }
        //
        //             if (codeList[i + 1].opcode == OpCodes.Cgt && codeList[i + 2].opcode == OpCodes.Ldc_I4_0)
        //             {
        //                 var label_TRUE = iLGenerator.DefineLabel();
        //                 var label_FALSE = iLGenerator.DefineLabel();
        //                 codeList[i + 5].labels.Add(label_FALSE);
        //                 
        //                 codeList.InsertRange(i + 4, new []
        //                 {
        //                     new CodeInstruction(OpCodes.Brtrue_S, label_TRUE),
        //                     new CodeInstruction(OpCodes.Ldsfld, settingField),
        //                     new CodeInstruction(OpCodes.Brfalse_S, label_FALSE),
        //                     new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(label_TRUE)
        //                 });
        //             }
        //         }
        //     }
        //     
        //     return codeList.AsEnumerable();
        // }
        //
        // public static IEnumerable<CodeInstruction> GravshipNoMaxRangeDrawRadiuPatch(IEnumerable<CodeInstruction> codeInstructions, ILGenerator iLGenerator)
        // {
        //     var codeList =  codeInstructions.ToList();
        //     var settingField = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.GravshipNoMaxRange));
        //     var label = iLGenerator.DefineLabel();
        //     for (int i = 0; i < codeList.Count - 8; i++)
        //     {
        //         if (codeList[i].opcode == OpCodes.Ldloc_2 && codeList[i + 5].opcode == OpCodes.Ldloc_1)
        //         {
        //             codeList[i + 8].labels.Add(label);
        //             codeList.InsertRange(i, new []
        //             {
        //                 new  CodeInstruction(OpCodes.Ldsfld, settingField),
        //                 new CodeInstruction(OpCodes.Brtrue_S, label),
        //             });
        //             break;
        //         }
        //     }
        //     return codeList.AsEnumerable();
        // }
        #endregion

        public static IEnumerable<CodeInstruction> HackingNeverLockOut(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList =  codeInstructions.ToList();
            var tMethod = AccessTools.Method(typeof(Rand), nameof(Rand.MTBEventOccurs), new[] { typeof(float), typeof(float), typeof(float) });
            var settingField = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.HackingNeverLockOut));
            for (var i = 1; i < codeList.Count; i++)
            {
                if(codeList[i - 1].opcode != OpCodes.Call || !(codeList[i - 1].operand is MethodInfo method) || method != tMethod) continue;

                if (codeList[i].opcode == OpCodes.Brfalse_S && codeList[i].operand is Label label)
                {
                    codeList.InsertRange(i + 1, new []
                    {
                        new CodeInstruction(OpCodes.Ldsfld, settingField),
                        new CodeInstruction(OpCodes.Brtrue_S, label)
                    });
                    break;
                }
            }

            return codeList.AsEnumerable();
        }

        public static IEnumerable<Gizmo> OrbitalHabitatCheckGizmosPatch(IEnumerable<Gizmo> gizmos, Building_GravEngine __instance)
        {
            foreach (var gizmo in gizmos)
            {
                yield return gizmo;
            }
            
            if(!TheSettings.OrbitalHabitatCheck) yield break;

            yield return new Command_Action
            {
                defaultLabel = "RWQoLTweaks_OrbitalHabitatCheck_Label".Translate(),
                defaultDesc = "RWQoLTweaks_OrbitalHabitatCheck_Desc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchReport"),
                action = () => OrbitalHabitatCheckDialog(__instance)
            };
        }

        private static void OrbitalHabitatCheckDialog(Building_GravEngine engine)
        {
            StringBuilder sb = new StringBuilder();
            int validSubstructure = engine.ValidSubstructure.Count;
            int heaterReq = Mathf.CeilToInt(validSubstructure / 250f);
            int oxygenPumpReq = Mathf.CeilToInt(validSubstructure / 400f);
            int heaterCount = 0;
            int oxygenPumpCount = 0;
            
            Map map = engine.Map;
            ThingDef oxygenPumpDef = DefDatabase<ThingDef>.GetNamed("OxygenPump");
            ThingDef heaterDef = DefDatabase<ThingDef>.GetNamed("Heater");

            foreach (IntVec3 cell in engine.ValidSubstructure)
            {
                var things = cell.GetThingList(map);
                foreach (var thing in things)
                {
                    if (thing.def == oxygenPumpDef || thing.HasComp<CompOxygenPusher>())
                    {
                        ++oxygenPumpCount;
                    }
                    else if (thing is Building_Heater || (thing.TryGetComp<CompHeatPusher>(out var compHeatPusher) && compHeatPusher.Props.heatPerSecond > 20f))
                    {
                        ++heaterCount;
                    }
                }
            }

            sb.AppendLine("RWQoLTweaks_OrbitalHabitatCheck_Orbitalstatus".Translate());


            sb.AppendLine(Utilities_Method.FormatResource(oxygenPumpDef.LabelCap, oxygenPumpCount, oxygenPumpReq));
            sb.AppendLine(Utilities_Method.FormatResource(heaterDef.LabelCap, heaterCount, heaterReq));

            if (oxygenPumpCount >= oxygenPumpReq && heaterCount >= heaterReq)
                sb.AppendLine("RWQoLTweaks_OrbitalHabitatCheck_Met".Translate());
            else
                sb.AppendLine("RWQoLTweaks_OrbitalHabitatCheck_UnMet".Translate());

            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(sb.ToString(), null, null,false, "RWQoLTweaks_OrbitalHabitatCheck_Label".Translate(), WindowLayer.Dialog));
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool ShuttleNoCoolDownPatch(CompLaunchable __instance)
        {
            if (!TheSettings.ShuttleNoCoolDownPatch)
            {
                return true;
            }
            
            if (__instance.parent.def != ThingDefOf.PassengerShuttle) return true;
            
            if (__instance.parent.Faction != Faction.OfPlayer) return true;

            return false;
        }

        public static IEnumerable<CodeInstruction> ShuttleNoCoolDownTranspilerPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            for (int i = 2; i < codeList.Count; i++)
            {
                if(codeList[i].opcode != OpCodes.Ble_S || codeList[i - 1].opcode != OpCodes.Ldc_I4_0 || codeList[i - 2].opcode != OpCodes.Ldloc_0) continue;
                
                var label = (Label)codeList[i].operand;
                
                codeList.InsertRange(i + 1, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(OdysseyPatches), nameof(ShuttleNoCoolDownPatch))),
                    new CodeInstruction(OpCodes.Brfalse_S, label)
                });
            }
            
            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>
            {
                new HarmonyPatchInfo(
                    "骇入永不锁定",
                    // RimWorld.CompPilotConsole+<>c__DisplayClass12_0.<StartChoosingDestination>b__3
                    AccessTools.Method(typeof(CompHackable), nameof(CompHackable.Hack), new[] { typeof(float), typeof(Pawn), typeof(bool) }),
                    new HarmonyMethod(typeof(OdysseyPatches), nameof(HackingNeverLockOut)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "逆重引擎显示轨道生存需求",
                    AccessTools.Method(typeof(Building_GravEngine), nameof(Building_GravEngine.GetGizmos)),
                    new HarmonyMethod(typeof(OdysseyPatches), nameof(OrbitalHabitatCheckGizmosPatch)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    "禁用载客穿梭机冷却",
                    AccessTools.Method(typeof(CompLaunchable), nameof(CompLaunchable.CanLaunch), new[] { typeof(float?) }),
                    new HarmonyMethod(typeof(OdysseyPatches), nameof(ShuttleNoCoolDownTranspilerPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "禁用载客穿梭机冷却 - 禁用提示冷却文本",
                    AccessTools.Method(typeof(CompLaunchable), nameof(CompLaunchable.CompInspectStringExtra)),
                    new HarmonyMethod(typeof(OdysseyPatches), nameof(ShuttleNoCoolDownTranspilerPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "Odyssey DLC";
        protected override string ModId => "ludeon.rimworld.odyssey";

        public override bool IsModLoaded(IModChecker modChecker)
        {
#if RIMWORLD_1_6 || RELEASE
            return ModsConfig.OdysseyActive;
#else
                  return false;
#endif
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using TinyGroxMods.HarmonyFramework;
using UnityEngine;
using Verse;

namespace RWQoLTweaks.HarmonyPatches
{
    public class CorePatches: AbstractPatchBase
    {
        protected override string ModDisplayName => "Core";
        protected override string ModId => "ludeon.rimworld";

        public static bool NoApparelDamagedThoughtPatch(ref ThoughtState __result)
        {
            if (!TheSettings.NoApparelDamagedThoughtPatch) return true;
            
            __result = ThoughtState.Inactive;
            
            return false;
        }
        public static void NoCenterDropPatch(ref IncidentParms parms)
        {
            if (!TheSettings.NoCenterDrop ) return;

            if (parms.raidArrivalMode != PawnsArrivalModeDefOf.CenterDrop || parms.faction.PlayerRelationKind != FactionRelationKind.Hostile) return;

            if (Prefs.DevMode)
                Messages.Message("CenterDropDisable", MessageTypeDefOf.NeutralEvent);

            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeDrop;
        }
        public static bool NoSolarFlarePatch(IncidentWorker_MakeGameCondition __instance, ref bool __result)
        {
            if (!TheSettings.NoSolarFlare) return true;

            if (__instance.def.gameCondition.defName != "SolarFlare") return true;

            if(Prefs.DevMode) 
                Messages.Message("SolarFlareDisable", MessageTypeDefOf.NeutralEvent);
            
            __result = false;

            return false;
        }
        public static bool NoPrisonBreakPatch()
        {
            return !TheSettings.NoPrisonBreak;
        }

        public static bool NoShortCircuitPatch(ref bool __result)
        {
            if(!TheSettings.NoShortCircuit) return true;

            if (Prefs.DevMode)
                Messages.Message("ShortCircuitDisable", MessageTypeDefOf.NeutralEvent);
            __result = false;
            return false;
        }

        public static void BurstCooldownTickPatch(ref int ___burstCooldownTicksLeft, ref Effecter ___progressBarEffecter, CompMannable ___mannableComp, Building_TurretGun __instance)
        {
            if(!TheSettings.MortarsAutoCool) return;

            if (__instance.Faction != Faction.OfPlayer)
                return;

            if (___burstCooldownTicksLeft <= 0)
                return;

            // if (___mannableComp != null && ___mannableComp.MannedNow)
            // 试试 C# 8.0 的新语法，效果同于上(IL 都是一样的)
            if (___mannableComp is { MannedNow: true })
                return;

            ___burstCooldownTicksLeft--;

            // 下面代码为了冷却黄条能正常显示，不然不动的
            if (___progressBarEffecter == null! || !__instance.def.building.IsMortar)
                return;

            float cooldownTime = __instance.def.building.turretBurstCooldownTime >= 0f ? __instance.def.building.turretBurstCooldownTime : __instance.AttackVerb.verbProps.defaultCooldownTime;

            int ticks = Math.Max(___burstCooldownTicksLeft, 0);

            int totalTicks = cooldownTime.SecondsToTicks();

            float progress = 1f - ticks / (float)totalTicks;

            MoteProgressBar ef = ((SubEffecter_ProgressBar)___progressBarEffecter.children[0]).mote;
            ef.progress = progress;

            if (progress >= 1f)
            {
                ___progressBarEffecter.Cleanup();
                ___progressBarEffecter = null!;
            }
        }

        public static void HoldOpenDoorInstantlyPatch(bool ___holdOpenInt, ref bool ___openInt, Building_Door __instance)
        {
            if(!TheSettings.HoldOpenDoorInstantly) return;
            
            if(TheSettings.HoldOpenDoorInstantlyOnlyPoweredAutoDoor && !__instance.DoorPowerOn) return;

            ___openInt = ___holdOpenInt;
        }

        public static bool TogglePowerInstantlyPatch(ref bool ___wantSwitchOn, CompFlickable __instance)
        {
            if (!TheSettings.TogglePowerInstantly)
                return true;

            ___wantSwitchOn = !___wantSwitchOn;
            __instance.DoFlick();

            return false;
        }

        public static void NoRelationGenerationChancePatch(ref float __result)
        {
            if (__result > 0f && TheSettings.NoMoreRelationGen)
            {
                __result = 0f;
            }
        }

        public static bool NoBreakDownPatch()
        {
            return !TheSettings.NoBreakDownPatch;
        }

        public static bool FloorNotOverrideFloorPatch(BuildableDef entDef, IntVec3 center, Rot4 rot, Map map, ref AcceptanceReport __result)
        {
            if(!TheSettings.FloorNotOverrideFloor) return true;
            
            TerrainDef terrainDef = (entDef as TerrainDef)!;

            if (terrainDef == null!) return true;

            if (terrainDef.IsSubstructure) return true;

            if (terrainDef.categoryType == map.terrainGrid.TerrainAt(center).categoryType && 
                !map.terrainGrid.TerrainAt(center).natural &&
                !map.terrainGrid.TerrainAt(center).IsSubstructure)
            {
                __result = LocalizationCache.Core.FloorNotOverrideFloor_Tips;
                return false;
            }

            return true;
        }

        private static float[] get_PlanetCoveragesModify()
        {
            return TheSettings.PlanetCoveragesModify ? Enumerable.Range(1, 20).Select(i => i * 0.05f).ToArray() : new [] { 0.3f, 0.5f, 1f };
        }

        public static IEnumerable<CodeInstruction> PlanetCoverageModifyPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            var PlanetCoverages = AccessTools.Field(typeof(Page_CreateWorldParams), "PlanetCoverages");
            // var PlanetCoveragesDev = AccessTools.Field(typeof(Page_CreateWorldParams), "PlanetCoveragesDev");
            for (int i = 0; i < codeList.Count(); i++)
            {
                if (codeList[i].opcode == OpCodes.Ldsfld && codeList[i].operand as FieldInfo == PlanetCoverages)
                {
                    codeList[i].opcode = OpCodes.Call;
                    codeList[i].operand = AccessTools.Method(typeof(CorePatches), nameof(get_PlanetCoveragesModify));
                    break;
                }
            }

            return codeList.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> CaravanNightRestTimeModifyPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            for (int i = 0; i < codeList.Count(); i++)
            {
                if (codeList[i].opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)codeList[i].operand, 22f))
                {
                    codeList[i].opcode = OpCodes.Ldsfld;
                    codeList[i].operand = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.CaravanNightRestTime));
                    codeList.Insert(i + 1, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(FloatRange), nameof(FloatRange.max))));
                    break;
                }
                if (codeList[i].opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)codeList[i].operand, 6f))
                {
                    codeList[i].opcode = OpCodes.Ldsfld;
                    codeList[i].operand = AccessTools.Field(typeof(TheSettings), nameof(TheSettings.CaravanNightRestTime));
                    codeList.Insert(i + 1, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(FloatRange), nameof(FloatRange.min))));
                }
            }

            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo(
                    "禁止囚犯越狱",
                    AccessTools.Method(typeof(PrisonBreakUtility),
                        nameof(PrisonBreakUtility.StartPrisonBreak),
                        new[]
                        {
                            typeof(Pawn), typeof(string).MakeByRefType(), typeof(string).MakeByRefType(),
                            typeof(LetterDef).MakeByRefType(), typeof(List<Pawn>).MakeByRefType()
                        }),
                    new HarmonyMethod(typeof(CorePatches), nameof(NoPrisonBreakPatch)),
                    HarmonyPatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    "迫击炮自动冷却",
                    AccessTools.Method(typeof(Building_TurretGun), "Tick"),
                    new HarmonyMethod(typeof(CorePatches), nameof(CorePatches.BurstCooldownTickPatch)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    "立即敞门",
#if RIMWORLD_1_6
                    AccessTools.Method(typeof(Building_Door), "<GetGizmos>b__79_1"),
#elif RIMWORLD_1_5
                    AccessTools.Method(typeof(Building_Door), "<GetGizmos>b__69_1"),
#endif
                    new HarmonyMethod(typeof(CorePatches), nameof(HoldOpenDoorInstantlyPatch)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    "立即开关电源",
                    AccessTools.Method(typeof(CompFlickable), "<CompGetGizmosExtra>b__20_1"),
                    new HarmonyMethod(typeof(CorePatches), nameof(TogglePowerInstantlyPatch)),
                    HarmonyPatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    "禁止亲戚关系生成",
                    AccessTools.Method(typeof(PawnRelationWorker),
                        nameof(PawnRelationWorker.BaseGenerationChanceFactor),
                        new[] { typeof(Pawn), typeof(Pawn), typeof(PawnGenerationRequest) }),
                    new HarmonyMethod(typeof(CorePatches), nameof(NoRelationGenerationChancePatch)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    "电器永不故障",
                    AccessTools.Method(typeof(CompBreakdownable), "CanBreakdownNow"),
                    new HarmonyMethod(typeof(CorePatches), nameof(NoBreakDownPatch)),
                    HarmonyPatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    "铺设地板禁止覆盖其他地板",
#if RIMWORLD_1_5
                    AccessTools.Method(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintAt_NewTemp),
                        new []
                        {
                            typeof(BuildableDef), typeof(IntVec3), typeof(Rot4), typeof(Map), typeof(bool),
                            typeof(Thing), typeof(Thing), typeof(ThingDef), typeof(bool), typeof(bool), typeof(bool)
                        }),
#elif RIMWORLD_1_6
                    AccessTools.Method(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintAt),
                        new[]
                        {
                            typeof(BuildableDef), typeof(IntVec3), typeof(Rot4), typeof(Map), typeof(bool),
                            typeof(Thing), typeof(Thing), typeof(ThingDef), typeof(bool), typeof(bool), typeof(bool)
                        }),
#endif
                    new HarmonyMethod(typeof(CorePatches), nameof(FloorNotOverrideFloorPatch)),
                    HarmonyPatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    "全球覆盖率扩展",
                    AccessTools.Method(typeof(Page_CreateWorldParams), nameof(Page_CreateWorldParams.DoWindowContents), new[] { typeof(Rect) }),
                    new HarmonyMethod(typeof(CorePatches), nameof(PlanetCoverageModifyPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "远行队夜晚休息时间调整",
#if RIMWORLD_1_5
                    AccessTools.Method(typeof(CaravanNightRestUtility), nameof(CaravanNightRestUtility.WouldBeRestingAt), new[] { typeof(int), typeof(long) }),
#elif RIMWORLD_1_6
                    AccessTools.Method(typeof(CaravanNightRestUtility), nameof(CaravanNightRestUtility.WouldBeRestingAt), new[] { typeof(PlanetTile), typeof(long) }),
#endif
                    new HarmonyMethod(typeof(CorePatches), nameof(CaravanNightRestTimeModifyPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "禁用短路事件",
                    AccessTools.Method(typeof(IncidentWorker_ShortCircuit), "TryExecuteWorker", new[] { typeof(IncidentParms) }),
                    new HarmonyMethod(typeof(CorePatches), nameof(NoShortCircuitPatch)),
                    HarmonyPatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    "禁用太阳耀斑",
                    AccessTools.Method(typeof(IncidentWorker_MakeGameCondition), "TryExecuteWorker", new[] { typeof(IncidentParms) }),
                    new HarmonyMethod(typeof(CorePatches), nameof(NoSolarFlarePatch)),
                    HarmonyPatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    "敌方中心空投变成边缘空投",
                    AccessTools.Method(typeof(IncidentWorker_Raid), nameof(IncidentWorker_Raid.ResolveRaidArriveMode), new[] { typeof(IncidentParms) }),
                    new HarmonyMethod(typeof(CorePatches), nameof(NoCenterDropPatch)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    "禁用衣衫褴褛",
                    AccessTools.Method(typeof(ThoughtWorker_ApparelDamaged), "CurrentStateInternal", new[] { typeof(Pawn) }),
                    new HarmonyMethod(typeof(CorePatches), nameof(NoApparelDamagedThoughtPatch)),
                    HarmonyPatchType.Prefix
                ),
            };
        }
    }
}
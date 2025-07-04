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

namespace RWQoLPatch.HarmonyPatches
{
    public class CorePatches: AbstractPatchBase
    {

        protected override string ModDisplayName => "Core";
        protected override string ModId => "ludeon.rimworld";
        public static bool NoPrisonBreak()
        {
            return !TheSettings.NoPrisonBreak;
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

        public static void HoldOpenDoorInstantly(ref bool ___holdOpenInt, ref bool ___openInt)
        {
            if(!TheSettings.HoldOpenDoorInstantly) return;

            ___openInt = ___holdOpenInt;
        }

        public static bool TogglePowerInstantly(ref bool ___wantSwitchOn, CompFlickable __instance)
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

        public static bool FloorNotOverrideFloor(BuildableDef entDef, IntVec3 center, Rot4 rot, Map map, ref AcceptanceReport __result)
        {
            if(!TheSettings.FloorNotOverrideFloor) return true;
            
            TerrainDef terrainDef = (entDef as TerrainDef)!;
            
            if (terrainDef != null! )
            {
                if (terrainDef.categoryType == map.terrainGrid.TerrainAt(center).categoryType)
                {
                    __result = "RWQoLPatch_FloorNotOverrideFloor_Tips".Translate();
                    return false;
                }
            }

            return true;
        }

        private static float[] get_PlanetCoveragesModify()
        {

            if (TheSettings.PlanetCoveragesModify)
                return Enumerable.Range(1, 20).Select(i => i * 0.05f).ToArray();
            return new [] { 0.3f, 0.5f, 1f };
        }

        public static IEnumerable<CodeInstruction> PlanetCoverageModify(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            var PlanetCoverages = AccessTools.Field(typeof(Page_CreateWorldParams), "PlanetCoverages");
            var PlanetCoveragesDev = AccessTools.Field(typeof(Page_CreateWorldParams), "PlanetCoveragesDev");
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
        public static IEnumerable<CodeInstruction> CaravanNightRestTimeModify(IEnumerable<CodeInstruction> codeInstructions)
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
                    new HarmonyMethod(typeof(CorePatches), nameof(NoPrisonBreak)),
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
                    new HarmonyMethod(typeof(CorePatches), nameof(HoldOpenDoorInstantly)),
                    HarmonyPatchType.Postfix
                ),
                new HarmonyPatchInfo(
                    "立即开关电源",
                    AccessTools.Method(typeof(CompFlickable), "<CompGetGizmosExtra>b__20_1"),
                    new HarmonyMethod(typeof(CorePatches), nameof(TogglePowerInstantly)),
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
                    AccessTools.Method(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintAt_NewTemp),
                        new []
                        {
                            typeof(BuildableDef), typeof(IntVec3), typeof(Rot4), typeof(Map), typeof(bool),
                            typeof(Thing), typeof(Thing), typeof(ThingDef), typeof(bool), typeof(bool), typeof(bool)
                        }),
                    new HarmonyMethod(typeof(CorePatches), nameof(FloorNotOverrideFloor)),
                    HarmonyPatchType.Prefix
                ),
                new HarmonyPatchInfo(
                    "全球覆盖率扩展",
                    AccessTools.Method(typeof(Page_CreateWorldParams), nameof(Page_CreateWorldParams.DoWindowContents), new[] {typeof(Rect)}),
                    new HarmonyMethod(typeof(CorePatches), nameof(PlanetCoverageModify)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo(
                    "远行队夜晚休息时间调整",
                    AccessTools.Method(typeof(CaravanNightRestUtility), nameof(CaravanNightRestUtility.WouldBeRestingAt), new[] {typeof(int), typeof(long)}),
                    new HarmonyMethod(typeof(CorePatches), nameof(CaravanNightRestTimeModify)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using TinyGroxMods.HarmonyFramework;
using UnityEngine;
using Verse;

namespace RWQoLPatch.HarmonyPatches
{
    public class DubsBadHygienePatches: AbstractPatchBase
    {
        private static string DBH_WaterString()
        {
            if (!TheSettings.DBHLocPatch || LanguageDatabase.activeLanguage.FriendlyNameEnglish == "English")
                return "Water: ";
            return "RWQoLPatch_DBHLocPatch_CompWaterFillable".Translate();
        }
        private static string DBH_CapString()
        {
            if (!TheSettings.DBHLocPatch)
                return "Cap: {0}L";
            return "RWQoLPatch_DBHLocPatch_PlaceWorker_UserGrid".Translate();
        }
        public static IEnumerable<CodeInstruction> DBH_WaterStringPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            for (int i = 0; i < codeList.Count; i++)
            {
                if (codeList[i].opcode == OpCodes.Ldstr && codeList[i].operand as string == "Water: ")
                {
                    codeList[i].opcode = OpCodes.Call;
                    codeList[i].operand = AccessTools.Method(typeof(DubsBadHygienePatches), nameof(DBH_WaterString));
                    break;
                }
            }
            return codeList.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> DBH_CapStringPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            var patched = 0;
            for (int i = 0; i < codeList.Count; i++)
            {
                if(patched == 2) break;

                if (codeList[i].opcode == OpCodes.Ldstr && codeList[i].operand as string == "Cap: {0}L")
                {
                    patched++;
                    codeList[i].opcode = OpCodes.Call;
                    codeList[i].operand = AccessTools.Method(typeof(DubsBadHygienePatches), nameof(DBH_CapString));
                }
            }
            return codeList.AsEnumerable();
        }
        protected override void LoadAllPatchInfo()
        {
            HarmonyPatches = new HashSet<HarmonyPatchInfo>()
            {
                new HarmonyPatchInfo
                (
                    "DBH 翻译补充",
                    AccessTools.Method(AccessTools.TypeByName("DubsBadHygiene.CompWaterFillable"),
                        "CompInspectStringExtra"),
                    new HarmonyMethod(typeof(DubsBadHygienePatches), nameof(DBH_WaterStringPatch)),
                    HarmonyPatchType.Transpiler
                ),
                new HarmonyPatchInfo
                (
                    "DBH 翻译补充2",
                    AccessTools.Method(AccessTools.TypeByName("DubsBadHygiene.PlaceWorker_UserGrid"),
                        "DrawGhost", new []{typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(Color), typeof(Thing)}),
                    new HarmonyMethod(typeof(DubsBadHygienePatches), nameof(DBH_CapStringPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "Dubs Bad Hygiene";
        protected override string ModId => "Dubwise.DubsBadHygiene";
    }
}
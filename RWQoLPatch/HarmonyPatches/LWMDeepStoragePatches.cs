using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TinyGroxMods.HarmonyFramework;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RWQoLPatch.HarmonyPatches
{
    public class LWMDeepStoragePatches: AbstractPatchBase
    {
        private static string getStatLabel(StatDef stat)
        {
            return TheSettings.LWMDeepStorageLocPatch ? stat.label : stat.ToString();
        }
        public static IEnumerable<CodeInstruction> LWMDeepStorageLocPatch(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codeList = codeInstructions.ToList();
            
            var toStringMethod = AccessTools.Method(typeof(object), "ToString");
            var statField = AccessTools.Field(AccessTools.TypeByName("LWM.DeepStorage.CompDeepStorage"), "stat");
            var getStatLabelMethod = AccessTools.Method(typeof(LWMDeepStoragePatches), nameof(getStatLabel));
            for (int i = 0; i < codeList.Count - 1; i++)
            {
                if (codeList[i].opcode == OpCodes.Ldfld && codeList[i].operand as FieldInfo == statField &&
                    codeList[i + 1].opcode == OpCodes.Callvirt && codeList[i + 1].operand as MethodInfo == toStringMethod)
                {
                    codeList[i + 1].opcode = OpCodes.Call;
                    codeList[i + 1].operand = getStatLabelMethod;
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
                    "LWM Deep Storage 翻译补充",
                    AccessTools.Method(AccessTools.TypeByName("LWM.DeepStorage.ITab_Inventory_HeaderUtil"),
                        "GetContentsHeader",
                        new[]
                        {
                            AccessTools.TypeByName("LWM.DeepStorage.CompDeepStorage"), typeof(string).MakeByRefType(),
                            typeof(string).MakeByRefType()
                        }),
                    new HarmonyMethod(typeof(LWMDeepStoragePatches), nameof(LWMDeepStorageLocPatch)),
                    HarmonyPatchType.Transpiler
                ),
            };
        }

        protected override string ModDisplayName => "LWM's Deep Storage";
        protected override string ModId => "lwm.deepstorage";
    }
}
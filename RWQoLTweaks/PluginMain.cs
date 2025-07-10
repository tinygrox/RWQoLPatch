using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLTweaks
{
    [StaticConstructorOnStartup]
    public class PluginMain
    {
        static PluginMain()
        {
            RWLogger logger = new RWLogger();
            Stopwatch clock = Stopwatch.StartNew();
            Harmony harmonyInstance = new Harmony("tinygrox.mods.RWQoLPatch");
            // Harmony.DEBUG = true;
            var modchecker = new RWModChecker();
            IEnumerable<Type> patchTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(AbstractPatchBase).IsAssignableFrom(t) && !t.IsAbstract);
            
            int patches = 0;
            
            AbstractPatchBase.ModName = "RWQoLPatch";
            
            foreach (var patchType in patchTypes)
            {
                if (!(Activator.CreateInstance(patchType) is AbstractPatchBase patchInstance)) continue;
                patches++;
                patchInstance.ApplyPatches(harmonyInstance, logger, modchecker);
            }
            
            clock.Stop();
            
            logger.LogMessage($"[{AbstractPatchBase.ModName}] Patch 全部运行完毕！共针对 {AbstractPatchBase.GetCount} 个 Mod 进行了 Patch，跳过了 {patches - AbstractPatchBase.GetCount} 个 Mod。总耗时：{clock.Elapsed.TotalSeconds:F2}秒");
            logger.Flush();
        }
    }
}
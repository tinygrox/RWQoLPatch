using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TinyGroxMods.HarmonyFramework;
using UnityEngine;
using Verse;

namespace RWQoLTweaks
{
    public class ModMain: Mod
    {
        private Vector2 scrollPosition = new Vector2(0,0);
        private readonly Listing_Standard Options_Content = new Listing_Standard();
        private float cacheContentHeight = -1f;

        private readonly List<(bool active, Action<Listing_Standard> draw)> sections =
            new List<(bool active, Action<Listing_Standard> draw)>
            {
                (true, ls =>
                {
                    var core = ModLister.GetExpansionWithIdentifier("ludeon.rimworld").LabelCap;
                    Utilities_UI.MediumLabel(ls, core);
                    ls.GapLine();
                    ls.CheckboxLabeled(LocalizationCache.Core.HoldOpenDoorInstantly, ref TheSettings.HoldOpenDoorInstantly);
                    ls.CheckboxLabeled(LocalizationCache.Core.MortarsAutoCool, ref TheSettings.MortarsAutoCool);
                    ls.CheckboxLabeled(LocalizationCache.Core.NoPrisonBreak, ref TheSettings.NoPrisonBreak);
                    ls.CheckboxLabeled(LocalizationCache.Core.TogglePowerInstantly, ref TheSettings.TogglePowerInstantly);
                    ls.CheckboxLabeled(LocalizationCache.Core.NoMoreRelation, ref TheSettings.NoMoreRelationGen);
                    ls.CheckboxLabeled(LocalizationCache.Core.NoBreakDownPatch, ref TheSettings.NoBreakDownPatch);
                    ls.CheckboxLabeled(LocalizationCache.Core.FloorNotOverrideFloor, ref TheSettings.FloorNotOverrideFloor);
                    ls.CheckboxLabeled(LocalizationCache.Core.PlanetCoveragesModify, ref TheSettings.PlanetCoveragesModify);
                    ls.CheckboxLabeled(LocalizationCache.Core.NoShortCircuit, ref TheSettings.NoShortCircuit);
                    ls.CheckboxLabeled(LocalizationCache.Core.NoSolarFlare, ref TheSettings.NoSolarFlare);
                    ls.CheckboxLabeled(LocalizationCache.Core.NoCenterDrop, ref TheSettings.NoCenterDrop);

                    ls.Label(LocalizationCache.Core.CaravanNightRestTime(TheSettings.CaravanNightRestTime), tooltip: LocalizationCache.Core.CaravanNightRestTimeTooltips);
                    Utilities_UI.FloatRangeSliderWithStep(ls.GetRect(24f), ref TheSettings.CaravanNightRestTime, 0f,
                        24f,
                        1f);
                    ls.Gap();
                }),
                (ModsConfig.RoyaltyActive, ls =>
                {
                    var royaltyname = ModLister.GetExpansionWithIdentifier("ludeon.rimworld.royalty").LabelCap;
                    // Options_Content.Label(royaltyname);
                    Utilities_UI.MediumLabel(ls, royaltyname);
                    ls.GapLine();
                    ls.CheckboxLabeled(LocalizationCache.Royalty.SetTransporterAutoLoad, ref TheSettings.TransporterAutoload);
                    ls.CheckboxLabeled(LocalizationCache.Royalty.NoPsyfocusDown, ref TheSettings.NoPsyfocusDown);
                    ls.Gap();
                }),
                (ModsConfig.IdeologyActive, ls =>
                {
                    Utilities_UI.MediumLabel(ls, ModLister.GetExpansionWithIdentifier("ludeon.rimworld.ideology").LabelCap);
                    ls.GapLine();
                    ls.Label(LocalizationCache.Ideology.PreceptRoleNum(TheSettings.PreceptRoleNum));
                    // Options_Content.IntAdjuster(ref TheSettings.PreceptRoleNum, 1,2); // IntAdjuster 太傻逼不想用
                    TheSettings.PreceptRoleNum =
                        Mathf.RoundToInt(ls.Slider(TheSettings.PreceptRoleNum, 2, 20));
                    ls.Gap();
                }),
                (ModsConfig.BiotechActive, ls =>
                {
                    Utilities_UI.MediumLabel(ls, ModLister.GetExpansionWithIdentifier("ludeon.rimworld.biotech").LabelCap);
                    ls.GapLine();

                    ls.CheckboxLabeled(LocalizationCache.Biotech.SubCoreScannerShowPawnType, ref TheSettings.SubCoreScannerShowPawnType);
                    ls.CheckboxLabeled(LocalizationCache.Biotech.DownedOverseerControlMechs, ref TheSettings.DownedOverseerControlMechs);
                    ls.CheckboxLabeled(LocalizationCache.Biotech.MechChargeRatePatch, ref TheSettings.MechChargeRatePatch);
                    if (TheSettings.MechChargeRatePatch)
                    {
                        ls.Indent(24f);
                        ls.Label(LocalizationCache.Biotech.MechChargeRatePatchTips(TheSettings.MechChargeRate));
                        ls.Outdent(24f);
                        TheSettings.MechChargeRate = Mathf.Round(ls.Slider(TheSettings.MechChargeRate, 0.05f, 10f) * 20f) / 20f;
                    }

                    ls.Gap();
                }),
                (ModsConfig.AnomalyActive, ls =>
                {
                    Utilities_UI.MediumLabel(ls, ModLister.GetExpansionWithIdentifier("ludeon.rimworld.anomaly").LabelCap);
                    ls.GapLine();

                    ls.CheckboxLabeled(LocalizationCache.Anomaly.NoEnityEscape, ref TheSettings.NoEnityEscape);

                    ls.Gap();
                }),
#if RIMWORLD_1_6
                (ModsConfig.OdysseyActive, ls =>
                {
                    Utilities_UI.MediumLabel(ls, ModLister.GetExpansionWithIdentifier("ludeon.rimworld.odyssey").LabelCap);
                    ls.GapLine();
                }),
#endif
                (ModsConfig.IsActive("Dubwise.Rimefeller"), ls =>
                {
                    Utilities_UI.MediumLabel(ls, "Rimefeller");
                    ls.GapLine();
                    ls.CheckboxLabeled(LocalizationCache.Rimefeller.RimefellerUnmanPatch, ref TheSettings.RimefellerUnmannedPatch, "");
                    ls.CheckboxLabeled(LocalizationCache.Rimefeller.RimefellerLocPatch, ref TheSettings.RimefellerLocPatch);
                    ls.Gap();
                }),
                (ModsConfig.IsActive("dubwise.rimatomics"), ls =>
                {
                    Utilities_UI.MediumLabel(ls, "Rimatomics");
                    ls.GapLine();
                    ls.CheckboxLabeled(LocalizationCache.Rimatomic.RimatomicUnmannedPatch, ref TheSettings.RimatomicUnmannedPatch, "");
                    ls.CheckboxLabeled(LocalizationCache.Rimatomic.RimatomicLocPatch, ref TheSettings.RimatomicLocPatch, "");
#if RIMWORLD_1_5
                    ls.CheckboxLabeled("RWQoLTweaks_RimatomicFixPatch".Translate(), ref TheSettings.RimatomicFixPatch, "");
#endif

                    ls.Gap();
                }),
                (ModsConfig.IsActive("Dubwise.DubsBadHygiene"), ls =>
                {
                    Utilities_UI.MediumLabel(ls, "Dubs Bad Hygiene");
                    ls.GapLine();
                    ls.CheckboxLabeled(LocalizationCache.DBH.DBHLocPatch, ref TheSettings.DBHLocPatch, "");
                    ls.Gap();
                }),
                (ModsConfig.IsActive("lwm.deepstorage"), ls =>
                {
                    Utilities_UI.MediumLabel(ls, "LWM's Deep Storage");
                    ls.GapLine();
                    ls.CheckboxLabeled(LocalizationCache.LWMDS.LWMDeepStorageLocPatch, ref TheSettings.LWMDeepStorageLocPatch, "");
                    ls.Gap();
                }),
                (ModsConfig.IsActive("oskarpotocki.vanillafactionsexpanded.core"), ls =>
                {
                    Utilities_UI.MediumLabel(ls, "Vanilla Expanded Framework");
                    ls.GapLine();
                    ls.CheckboxLabeled(LocalizationCache.VEF.VEFLocPatch, ref TheSettings.VEFLocPatch, "");
                    ls.Gap();
                })
            };
        public ModMain(ModContentPack content) : base(content)
        {
            GetSettings<TheSettings>();
        }
        
        // 傻逼 IMGUI
        private float getHeight()
        {
            var measuringLS = new Listing_Standard();
            Rect measuringRect = new Rect(0, 0, 0f, 9999f);
            measuringLS.Begin(measuringRect);
            int rows = 0;
            float colMaxHeight = 0f;
            var MaxRow = Mathf.Max(4, sections.Count / 2);
            foreach (var section in sections)
            {
                if(!section.active) continue;

                section.draw(measuringLS);

                if (++rows <= MaxRow)
                    colMaxHeight = measuringLS.CurHeight;
                else
                {
                    colMaxHeight = Mathf.Max(colMaxHeight, measuringLS.CurHeight - colMaxHeight);
                }
            }
            // var contentHeight = measuringLS.CurHeight;
            measuringRect.Set(0,0,0,0);
            measuringLS.End();
            return colMaxHeight;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout) // 只计算不绘制，现在 IMGUI 不再傻逼了
                cacheContentHeight = Mathf.Max(cacheContentHeight, getHeight());

            Rect scrollViewRect = new Rect(0f, 20f, inRect.width - 16f, cacheContentHeight);
            Widgets.BeginScrollView(inRect, ref scrollPosition, scrollViewRect);
            Options_Content.Begin(scrollViewRect);
            int row = 0;
            bool newcol = false;
            var MaxRow = Mathf.Max(5, sections.Count / 2);
            foreach (var section in sections)
            {
                if (!section.active) continue;

                if (++row > MaxRow && !newcol)
                {
                    Options_Content.ColumnWidth = (scrollViewRect.width -20f) / 2f;
                    newcol = true;
                    Options_Content.NewColumn();
                }
                section.draw(Options_Content);
            }

            Options_Content.End();
            Widgets.EndScrollView();
        }

        public override string SettingsCategory()
        {
            return $"RWQoLTweaks".Translate();
        }
    }
}
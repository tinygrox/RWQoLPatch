using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Verse;

namespace RWQoLTweaks
{
    public static class Utilities_UI
    {
        // 中号 Label
        public static void MediumLabel(Listing_Standard UI, string label)
        {
            GameFont originalFont = Text.Font;
            Text.Font = GameFont.Medium;
            float height = Text.CalcHeight(label, UI.ColumnWidth);
            Rect labelRect = UI.GetRect(height);
            Widgets.Label(labelRect, label);
            Text.Font = originalFont;
        }

        public static void FloatRangeSliderWithStep(Rect rect ,ref FloatRange range, float min, float max,float step)
        {
            Widgets.FloatRange(rect, rect.GetHashCode(), ref range, min, max, valueStyle: ToStringStyle.Integer);
            range.min = Mathf.Clamp(Mathf.Round(range.min / step) * step, min, max);
            range.max = Mathf.Clamp(Mathf.Round(range.max / step) * step, min, max);
        }
        // TODO
        // public static void FloatAdjuster(Listing_Standard UI, List<float> list, ref float buffer)
        // {
        //     
        //     Rect rect = UI.GetRect(24f);
        //     rect.width = 42f;
        //     if (Widgets.ButtonText(rect, "-"))
        //     {
        //
        //     }
        //     rect.x += rect.width + 2f;
        //     rect.width = 84f;
        //     Widgets.TextField(rect, buffer.ToString(CultureInfo.CurrentCulture));
        //     rect.x += rect.width + 30f;
        //     rect.width = 42f;
        //     if (Widgets.ButtonText(rect, "+"))
        //     {
        //         list.Add(buffer);
        //     }
        // }
    }
}
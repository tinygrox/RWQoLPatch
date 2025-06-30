using UnityEngine;
using Verse;

namespace RWQoLPatch
{
    public static class Utilities_UI
    {
        public static void MediumLabel(Listing_Standard UI, string label)
        {
            GameFont originalFont = Text.Font;
            Text.Font = GameFont.Medium;
            float height = Text.CalcHeight(label, UI.ColumnWidth);
            Rect labelRect = UI.GetRect(height);
            Widgets.Label(labelRect, label);
            Text.Font = originalFont;
        }
    }
}
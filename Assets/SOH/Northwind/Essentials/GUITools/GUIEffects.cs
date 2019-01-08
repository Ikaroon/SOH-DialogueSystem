using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public static class GUIEffects
    {
        public static void DrawOutline(Rect rect, string text, GUIStyle style, Color color, float thickness = 1f)
        {
            Rect offsetRect = rect;
            string outlineColor = "#" + ColorUtility.ToHtmlStringRGBA(color);
            string finalText = "<color=" + outlineColor + ">" + text + "</color>";

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    offsetRect.position = rect.position + new Vector2(x, y) * thickness;

                    GUI.Label(offsetRect, new GUIContent(finalText), style);
                }
            }
        }
    }
}
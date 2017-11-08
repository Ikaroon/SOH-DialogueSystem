using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public static class NodeDrawer
    {
        #region Style Data

        private static GUIStyle nodeBGStyle, headerStyle;

        #endregion

        #region Style Methods

        private static void InitiateStyles()
        {
            if (nodeBGStyle != null)
            {
                return;
            }

            nodeBGStyle = GUI.skin.GetStyle("ChannelStripBg");

            headerStyle = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            headerStyle.alignment = TextAnchor.MiddleCenter;
        }

        #endregion

        #region Basic Node Drawer

        public static void DrawRawNode(Rect rect, string title)
        {
            InitiateStyles();

            GUILayout.BeginArea(rect, nodeBGStyle);

            GUI.Label(new Rect(0f, 0f, rect.width, 22f), new GUIContent(title), headerStyle);

            GUILayout.EndArea();
        }

        #endregion
    }
}
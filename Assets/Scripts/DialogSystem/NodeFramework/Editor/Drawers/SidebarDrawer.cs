using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public static class SidebarDrawer
    {

        #region Style Data

        private static GUIStyle groupBoxStyle;

        #endregion

        #region Style Methods

        private static void InitializeStyles()
        {
            if (groupBoxStyle != null)
            {
                return;
            }

            groupBoxStyle = GUI.skin.GetStyle("GroupBox");
        }

        #endregion

        #region Sidebar Data

        private static float anchorPos;

        #endregion

        #region Main GUI

        public static void DrawSidebar(Rect rect, DialogEditor editor, ref float width, float minWidth = 100f, float maxWidth = 400f)
        {
            //Resize
            rect = new Rect(rect.x, rect.y, width, rect.height);

            //Initialize Styles
            InitializeStyles();

            //-----------------------------------------------------------------------------------------

            //Sidebar
            GUILayout.BeginArea(rect, groupBoxStyle);

            //Draw Content
            DatabaseDrawer.DrawDatabase(new Rect(0f, 0f, rect.width - 4f, rect.height), false);


            //-----------------------------------------------------------------------------------------

            //Mouse Zone and Calculation

            Rect resizeRect = new Rect(width - 4f, 0f, 4f, rect.height);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);

            if (editor.currentTask == DialogEditor.NodeEditorTask.None && Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 0 && resizeRect.Contains(Event.current.mousePosition))
            {
                anchorPos = Event.current.mousePosition.x - width;
                editor.currentTask = DialogEditor.NodeEditorTask.ResizeSidebar;
            }

            if (editor.currentTask == DialogEditor.NodeEditorTask.ResizeSidebar && Event.current.isMouse && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                width = Mathf.Clamp(Event.current.mousePosition.x - anchorPos, minWidth, maxWidth);
                editor.Repaint();
            }
            
            width = Mathf.Clamp(width, minWidth, maxWidth);

            GUILayout.EndArea();
        }

        #endregion
    }
}
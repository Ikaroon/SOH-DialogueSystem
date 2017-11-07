using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.NodeFramework
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

        public static void DrawSidebar(Rect rect, NodeEditor editor, ref float width, float minWidth = 100f, float maxWidth = 400f)
        {
            //Resize
            rect = new Rect(rect.x, rect.y, width, rect.height);

            //Initialize Styles
            InitializeStyles();

            //-----------------------------------------------------------------------------------------

            //Sidebar
            GUILayout.BeginArea(rect, groupBoxStyle);

            //TODO: Content


            //-----------------------------------------------------------------------------------------

            //Mouse Zone and Calculation

            Rect resizeRect = new Rect(width - 8f, 0f, 8f, rect.height);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);

            if (editor.currentTask == NodeEditor.NodeEditorTask.None && Event.current.isMouse && Event.current.type == EventType.MouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                anchorPos = Event.current.mousePosition.x - width;
                editor.currentTask = NodeEditor.NodeEditorTask.ResizeSidebar;
            }

            if (editor.currentTask == NodeEditor.NodeEditorTask.ResizeSidebar && Event.current.isMouse && Event.current.type == EventType.MouseDrag)
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
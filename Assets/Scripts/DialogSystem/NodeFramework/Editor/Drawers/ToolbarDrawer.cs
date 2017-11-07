using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.NodeFramework
{
    public static class ToolbarDrawer
    {
        #region Style Data

        private static GUIStyle toolbarStyle, toolbarButtonStyle, toolbarDropdownStyle;

        #endregion

        #region Style Methods

        private static void InitializeStyles()
        {
            if (toolbarStyle != null)
            {
                return;
            }

            toolbarStyle = GUI.skin.GetStyle("Toolbar");

            toolbarButtonStyle = GUI.skin.GetStyle("toolbarbutton");

            toolbarDropdownStyle = GUI.skin.GetStyle("ToolbarDropDown");
        }

        #endregion

        #region Main GUI

        public static void DrawToolbar(Rect rect, NodeEditor editor)
        {
            //Initialize Styles
            InitializeStyles();

            //-----------------------------------------------------------------------------------------

            //Menus Toolbar
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width - 128f, 18f), toolbarStyle);
            GUILayout.BeginHorizontal();

            //Data Menu Button
            if (GUILayout.Button(new GUIContent("Data"), toolbarButtonStyle, GUILayout.Width(100f))) {
                DataMenu(new Rect(7f, 18f, 0f, 0f), editor);
            }

            //Edit Menu Button
            if (GUILayout.Button(new GUIContent("Edit"), toolbarButtonStyle, GUILayout.Width(100f)))
            {
                EditMenu(new Rect(107f, 18f, 0f, 0f), editor);
            }

            //Add new Menus when needed

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //-----------------------------------------------------------------------------------------

            //Language Toolbar
            GUILayout.BeginArea(new Rect(rect.x + rect.width - 128f, rect.y, 128f, 18f), toolbarStyle);

            if (GUI.Button(new Rect(0f, 0f, 120f, 18f), new GUIContent("Language"), toolbarDropdownStyle))
            {
                //TODO: Language Drop Down
            }

            GUILayout.EndArea();
        }

        #endregion

        #region Data Menu

        private static void DataMenu(Rect rect, NodeEditor editor)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("New"), false, New, editor);
            menu.AddItem(new GUIContent("Load"), false, Load, editor);

            if (editor.canvas)
            {
                menu.AddItem(new GUIContent("Save"), false, Save, editor);
                menu.AddItem(new GUIContent("Save As"), false, SaveAs, editor);
            } else
            {
                menu.AddDisabledItem(new GUIContent("Save"));
                menu.AddDisabledItem(new GUIContent("Save As"));
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Settings"), false, Settings);
            menu.DropDown(rect);
        }

        #endregion

        #region Data Menu Methods

        private static void New(object obj)
        {
            (obj as NodeEditor).canvas = NodeCanvas.CreateCanvas<NodeCanvas>();
            (obj as NodeEditor).oldPath = "";
        }

        private static void Load(object obj)
        {
            (obj as NodeEditor).oldPath = NodeSaveOperator.Load(ref (obj as NodeEditor).canvas);
        }

        private static void Save(object obj)
        {
            (obj as NodeEditor).oldPath = NodeSaveOperator.Save(ref (obj as NodeEditor).canvas, (obj as NodeEditor).oldPath);
        }

        private static void SaveAs(object obj)
        {
            (obj as NodeEditor).oldPath = NodeSaveOperator.Save(ref (obj as NodeEditor).canvas, (obj as NodeEditor).oldPath);
        }

        private static void Settings()
        {
            //TODO: Open Settings Window
        }

        #endregion

        #region Edit Menu

        private static void EditMenu(Rect rect, NodeEditor editor)
        {
            GenericMenu menu = new GenericMenu();

            if (editor.canvas)
            {
                menu.AddItem(new GUIContent("Smart Align"), false, SmartAlign, editor);
                menu.AddItem(new GUIContent("Improve"), false, Improve, editor);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Smart Align"));
                menu.AddDisabledItem(new GUIContent("Improve"));
            }

            menu.DropDown(rect);
        }

        #endregion

        #region Edit Menu Methods
        
        private static void SmartAlign(object obj)
        {
            //TODO: Smart Align Tool
        }

        private static void Improve(object obj)
        {
            //TODO: Improve Tool
        }

        #endregion
    }
}
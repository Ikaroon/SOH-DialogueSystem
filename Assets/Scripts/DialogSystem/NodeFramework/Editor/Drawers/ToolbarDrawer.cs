using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public static class ToolbarDrawer
    {
        #region Style Data

        private static GUIStyle toolbarStyle, toolbarButtonStyle, toolbarDropdownStyle, toolbarTextfieldStyle;

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

            toolbarTextfieldStyle = GUI.skin.GetStyle("ToolbarTextField");
        }

        #endregion

        #region Main GUI

        public static void DrawToolbar(Rect rect, DialogEditor editor)
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

            EditorGUI.BeginDisabledGroup(!editor.canvas);

            EditorGUILayout.Space();

            if (editor.canvas)
            {
                EditorGUILayout.LabelField(new GUIContent("Name"), GUILayout.Width(50f));
                editor.canvas.canvasName = EditorGUILayout.TextField(editor.canvas.canvasName, toolbarTextfieldStyle, GUILayout.MaxWidth(200f));

                EditorGUILayout.LabelField(new GUIContent("Description"), GUILayout.Width(80f));
                editor.canvas.canvasDescription = EditorGUILayout.TextField(editor.canvas.canvasDescription, toolbarTextfieldStyle, GUILayout.MaxWidth(300f));
            } else
            {
                EditorGUILayout.LabelField(new GUIContent("Name"), GUILayout.Width(50f));
                EditorGUILayout.TextField("", toolbarTextfieldStyle, GUILayout.MaxWidth(200f));

                EditorGUILayout.LabelField(new GUIContent("Description"), GUILayout.Width(80f));
                EditorGUILayout.TextField("", toolbarTextfieldStyle, GUILayout.MaxWidth(300f));
            }

            EditorGUILayout.Space();

            EditorGUI.EndDisabledGroup();

            //Add new Menus when needed

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //-----------------------------------------------------------------------------------------

            //Language Toolbar
            GUILayout.BeginArea(new Rect(rect.x + rect.width - 128f, rect.y, 128f, 18f), toolbarStyle);

            if (GUI.Button(new Rect(0f, 0f, 120f, 18f), new GUIContent("[" + LangSys.activeLang + "] " + LangSys.DATA[LangSys.activeLang].fullname), toolbarDropdownStyle))
            {
                LanguageMenu(new Rect(0f, 18f, 0f, 0f), editor);
            }

            GUILayout.EndArea();
        }

        #endregion

        #region Data Menu

        private static void DataMenu(Rect rect, DialogEditor editor)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("New"), false, New, editor);
            menu.AddItem(new GUIContent("Load"), false, Load, editor);

            if (editor.canvas)
            {
                menu.AddItem(new GUIContent("Save"), false, Save, editor);
                menu.AddItem(new GUIContent("Save As"), false, SaveAs, editor);
                menu.AddItem(new GUIContent("Export JSON"), false, ExportJSON, editor);
            } else
            {
                menu.AddDisabledItem(new GUIContent("Save"));
                menu.AddDisabledItem(new GUIContent("Save As"));
                menu.AddDisabledItem(new GUIContent("Export JSON"));
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Settings"), false, Settings);
            menu.DropDown(rect);
        }

        #endregion

        #region Data Menu Methods

        private static void New(object obj)
        {
            (obj as DialogEditor).canvas = DialogCanvas.CreateCanvas<DialogCanvas>();
            (obj as DialogEditor).oldPath = "";
        }

        private static void Load(object obj)
        {
            (obj as DialogEditor).oldPath = NodeSaveOperator.Load(ref (obj as DialogEditor).canvas);
        }

        private static void Save(object obj)
        {
            (obj as DialogEditor).oldPath = NodeSaveOperator.Save(ref (obj as DialogEditor).canvas, (obj as DialogEditor).oldPath);
        }

        private static void SaveAs(object obj)
        {
            (obj as DialogEditor).oldPath = NodeSaveOperator.Save(ref (obj as DialogEditor).canvas, (obj as DialogEditor).oldPath);
        }

        private static void ExportJSON(object obj)
        {
            NodeSaveOperator.Export(ref (obj as DialogEditor).canvas, "");
        }

        private static void Settings()
        {
            //TODO: Open Settings Window
        }

        #endregion

        #region Edit Menu

        private static void EditMenu(Rect rect, DialogEditor editor)
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

        #region Language Menu

        private static void LanguageMenu(Rect rect, DialogEditor editor)
        {
            GenericMenu menu = new GenericMenu();

            foreach (KeyValuePair<string, Language> pair in LangSys.DATA.languages)
            {
                menu.AddItem(new GUIContent("[" + pair.Key + "] " + pair.Value.fullname), false, ChangeLanguage, pair.Key);
            }

            menu.DropDown(rect);
        }

        #endregion

        #region Language Menu Methods

        private static void ChangeLanguage(object obj)
        {
            LangSys.activeLang = obj as string;
        }

        #endregion
    }
}
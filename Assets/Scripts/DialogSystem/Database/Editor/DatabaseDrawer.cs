using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.CharacterSystem;
using SpyOnHuman.DialogSystem.LanguageSystem;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    public static class DatabaseDrawer
    {

        #region Style Data

        private static GUIStyle groupBoxStyle, capsuleStyle, minusStyle, plusStyle;

        #endregion

        #region Style Methods

        private static void InitializeStyles()
        {
            if (groupBoxStyle != null)
            {
                return;
            }

            groupBoxStyle = GUI.skin.GetStyle("GroupBox");

            capsuleStyle = GUI.skin.GetStyle("GroupBox");

            minusStyle = GUI.skin.GetStyle("OL Minus");

            plusStyle = GUI.skin.GetStyle("OL Plus");
        }

        #endregion

        #region Task Data

        private enum Tasks { None, AddCharacter, AddDialog};
        private static Tasks task = Tasks.None;
        private static Character addChar;
        private static DialogCanvas addDialog;

        #endregion

        #region Task Methods

        private static void AddCharacter(Character character)
        {
            task = Tasks.AddCharacter;
            addChar = character;
        }

        private static void AddDialog(DialogCanvas dialog)
        {
            task = Tasks.AddDialog;
            addDialog = dialog;
        }

        private static void ProceedTask()
        {
            if (task != Tasks.None)
            {
                switch (task)
                {
                    case Tasks.AddCharacter:
                        if (addChar && !DialogSystemDatabase.DATA.characters.Contains(addChar))
                        {
                            DialogSystemDatabase.DATA.characters.Add(addChar);
                            EditorUtility.SetDirty(DialogSystemDatabase.DATA);
                        }
                        break;
                    case Tasks.AddDialog:
                        if (addDialog && !DialogSystemDatabase.DATA.dialogs.Contains(addDialog))
                        {
                            DialogSystemDatabase.DATA.dialogs.Add(addDialog);
                            EditorUtility.SetDirty(DialogSystemDatabase.DATA);
                        }
                        break;
                }
                task = Tasks.None;
            }
        }

        #endregion

        private static Vector2 scroll;
        private static bool characters = true;
        private static bool dialogs = false;
        private static bool languages = true;

        public static void UpdateDrawer()
        {
            ProceedTask();
        }

        public static void DrawDatabase(Rect rect, bool editable)
        {
            //Initialize Styles
            InitializeStyles();

            //Scroll view
            float height = CharactersHeight(24f, editable) + 4f
                + DialogsHeight(24f, editable) + 4f
                + LanguagesHeight(24f, editable) + 4f + 8f;
            scroll = GUI.BeginScrollView(rect, scroll, new Rect(0f, -8f, rect.width - 16f, height), false, true);

            height = 0f;

            DrawCharacters(new Rect(0f, height, rect.width - 14f, 24f), ref height, editable);

            DrawDialogs(new Rect(0f, height, rect.width - 14f, 24f), ref height, editable);

            DrawLanguages(new Rect(0f, height, rect.width - 14f, 24f), ref height, editable);

            GUI.EndScrollView();

            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == 50)
                {
                    AddCharacter((Character)EditorGUIUtility.GetObjectPickerObject());
                }
                if (EditorGUIUtility.GetObjectPickerControlID() == 60)
                {
                    AddDialog((DialogCanvas)EditorGUIUtility.GetObjectPickerObject());
                }
            }
        }

        #region Character Drawers

        private static float CharactersHeight(float baseHeight, bool editable)
        {
            int charCount = DialogSystemDatabase.DATA.characters.Count;
            return baseHeight + (characters ? (charCount + (editable ? 1 : 0)) * (baseHeight + 2f) : 0f);
        }

        private static void DrawCharacters(Rect rect, ref float height, bool editable)
        {
            int charCount = DialogSystemDatabase.DATA.characters.Count;
            float finalHeight = rect.height + (characters ? (charCount + (editable ? 1 : 0)) * (rect.height + 2f) : 0f);
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width, finalHeight), groupBoxStyle);
            characters = EditorGUI.Foldout(new Rect(8f, 4f, rect.width, 16f), characters, new GUIContent("Characters"));

            if (characters)
            {
                for (int c = 0; c < charCount; c++)
                {
                    DrawCharacter(new Rect(0f, rect.height + c * (rect.height + 2f), rect.width - 0f, rect.height), c, editable);
                }
                if (editable)
                {
                    DrawAddCharacter(new Rect(0f, rect.height + (charCount) * (rect.height + 2f), rect.width - 0f, rect.height));
                }
            }

            GUILayout.EndArea();
            height += finalHeight + 4f;
        }

        private static void DrawCharacter(Rect rect, int characterID, bool editable)
        {
            GUILayout.BeginArea(rect, capsuleStyle);
            float mid = (rect.height - 16f) / 2f;
            Character character = DialogSystemDatabase.DATA.characters[characterID];
            string charName = character ? character.forename + " " + character.surname : "----";
            GUI.Label(new Rect(32f, mid, rect.width - 16f, 16f), new GUIContent(charName));
            if (editable && GUI.Button(new Rect(rect.width - 20f, mid, 16f, 16f), new GUIContent(""), minusStyle))
            {
                DialogSystemDatabase.DATA.characters.RemoveAt(characterID);
            }
            GUILayout.EndArea();
        }

        private static void DrawAddCharacter(Rect rect)
        {
            GUILayout.BeginArea(rect, capsuleStyle);
            float mid = (rect.height - 16f) / 2f;
            GUI.Label(new Rect(32f, mid, rect.width - 16f, 16f), new GUIContent(""));
            if ( GUI.Button(new Rect(rect.width - 20f, mid, 16f, 16f), new GUIContent(""), plusStyle))
            {
                EditorGUIUtility.ShowObjectPicker<Character>(addChar, false, "", 50);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Dialog Drawers

        private static float DialogsHeight(float baseHeight, bool editable)
        {
            int dialogCount = DialogSystemDatabase.DATA.dialogs.Count;
            return baseHeight + (dialogs ? (dialogCount + (editable ? 1 : 0)) * (baseHeight + 2f) : 0f);
        }

        private static void DrawDialogs(Rect rect, ref float height, bool editable)
        {
            int dialogCount = DialogSystemDatabase.DATA.dialogs.Count;
            float finalHeight = rect.height + (dialogs ? (dialogCount + (editable ? 1 : 0)) * (rect.height + 2f) : 0f);
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width, finalHeight), groupBoxStyle);
            dialogs = EditorGUI.Foldout(new Rect(8f, 4f, rect.width, 16f), dialogs, new GUIContent("Dialogs"));

            if (dialogs)
            {
                for (int c = 0; c < dialogCount; c++)
                {
                    DrawDialog(new Rect(0f, rect.height + c * (rect.height + 2f), rect.width - 0f, rect.height), c, editable);
                }
                if (editable)
                {
                    DrawAddDialog(new Rect(0f, rect.height + (dialogCount) * (rect.height + 2f), rect.width - 0f, rect.height));
                }
            }

            GUILayout.EndArea();
            height += finalHeight + 4f;
        }

        private static void DrawDialog(Rect rect, int dialogID, bool editable)
        {
            GUILayout.BeginArea(rect, capsuleStyle);
            float mid = (rect.height - 16f) / 2f;
            DialogCanvas dialog = DialogSystemDatabase.DATA.dialogs[dialogID];
            string charName = dialog ? dialog.canvasName + " " + dialog.canvasTimestamp : "----";
            GUI.Label(new Rect(32f, mid, rect.width - 16f, 16f), new GUIContent(charName));
            if (editable && GUI.Button(new Rect(rect.width - 20f, mid, 16f, 16f), new GUIContent(""), minusStyle))
            {
                DialogSystemDatabase.DATA.dialogs.RemoveAt(dialogID);
            }
            GUILayout.EndArea();
        }

        private static void DrawAddDialog(Rect rect)
        {
            GUILayout.BeginArea(rect, capsuleStyle);
            float mid = (rect.height - 16f) / 2f;
            GUI.Label(new Rect(32f, mid, rect.width - 16f, 16f), new GUIContent(""));
            if (GUI.Button(new Rect(rect.width - 20f, mid, 16f, 16f), new GUIContent(""), plusStyle))
            {
                EditorGUIUtility.ShowObjectPicker<DialogCanvas>(addDialog, false, "", 60);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Language Drawers

        private static float LanguagesHeight(float baseHeight, bool editable)
        {
            int langCount = LangSys.DATA.languages.Count;
            return baseHeight + (languages ? (langCount) * (baseHeight + 2f) : 0f);
        }

        private static void DrawLanguages(Rect rect, ref float height, bool editable)
        {
            int langCount = LangSys.DATA.languages.Count;
            float finalHeight = rect.height + (languages ? (langCount) * (rect.height + 2f) : 0f);
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width, finalHeight), groupBoxStyle);
            languages = EditorGUI.Foldout(new Rect(8f, 4f, rect.width, 16f), languages, new GUIContent("Languages"));

            if (languages)
            {
                for (int l = 0; l < langCount; l++)
                {
                    DrawLanguage(new Rect(0f, rect.height + l * (rect.height + 2f), rect.width - 0f, rect.height), LangSys.DATA.keys[l], editable);
                }
            }

            GUILayout.EndArea();
            height += finalHeight + 4f;
        }

        private static void DrawLanguage(Rect rect, string langID, bool editable)
        {
            GUILayout.BeginArea(rect, capsuleStyle);
            float mid = (rect.height - 16f) / 2f;
            Language language = LangSys.DATA[langID];
            string langName = language != null ? "[" + langID + "] " + language.fullname : "----";
            GUI.Label(new Rect(32f, mid, rect.width - 16f, 16f), new GUIContent(langName));
            GUILayout.EndArea();
        }

        #endregion
    }
}
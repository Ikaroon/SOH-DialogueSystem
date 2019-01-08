using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    public class LangSysWindow : EditorWindow
    {
        #region Editor Data

        private static LangPack langPack;

        private static Vector2 scrollPos = Vector2.zero;

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Task Data

        /// <summary>
        /// The type of task
        /// </summary>
        private enum TaskTypes { None, Add, Remove, ChangeFlag };

        /// <summary>
        /// The current task
        /// </summary>
        private static TaskTypes taskType = TaskTypes.None;

        /// <summary>
        /// The current task key to access the right language
        /// </summary>
        private static string taskKey = "";

        /// <summary>
        /// The add key for adding a new language
        /// </summary>
        private static string addKey = "";

        #endregion

        #region Task Methods
        
        private void StartTask(TaskTypes type, string task)
        {
            taskType = type;
            taskKey = task;
        }

        private void ProceedTask()
        {
            if (taskType != TaskTypes.None)
            {
                switch (taskType)
                {
                    case TaskTypes.Add:
                        if (!langPack.languages.ContainsKey(taskKey))
                        {
                            langPack.languages.Add(taskKey, new Language(""));
                            SaveChanges();
                        }
                        break;
                    case TaskTypes.Remove:
                        if (langPack.languages.ContainsKey(taskKey) && langPack.languages.Count > 1)
                        {
                            langPack.languages.Remove(taskKey);
                            SaveChanges();
                        }
                        break;
                }
                taskType = TaskTypes.None;
                taskKey = "";
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Stlyes Data

        private static GUIStyle styleBox, styleHeader, styleCenterLabel;

        #endregion

        #region Styles Methods

        private static void InitStyles()
        {
            styleBox = new GUIStyle(GUI.skin.GetStyle("GroupBox"));

            styleHeader = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleHeader.alignment = TextAnchor.MiddleLeft;

            styleCenterLabel = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleCenterLabel.alignment = TextAnchor.MiddleCenter;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Window Methods

        [MenuItem("SpyOnHuman/Language Settings", priority = 20)]
        static void Init()
        {
            LangSysWindow window = CreateInstance<LangSysWindow>();
            window.ShowUtility();
        }

        void OnEnable()
        {
            minSize = new Vector2(400f, 200f);
            titleContent = new GUIContent("Language Settings");
            langPack = LangSys.DATA;
        }

        private void OnDisable()
        {
            LangSys.Save();
        }

        void OnGUI()
        {
            if (langPack == null)
            {
                langPack = LangSys.DATA;
            }

            if (styleBox == null)
            {
                InitStyles();
            }

            //Languages
            scrollPos = GUI.BeginScrollView(new Rect(0f, 0f, position.width, position.height), scrollPos, new Rect(8f, 8f, position.width - 32f, (langPack.keys.Count + 1) * 100f - 20f), false, true);
            Languages(new Rect(16f, 16f, position.width - 32f, position.height - 80f));
            GUI.EndScrollView();
        }

        void Update()
        {
            if (langPack == null)
            {
                langPack = LangSys.DATA;
            }

            ProceedTask();

            if (!langPack.languages.ContainsKey(langPack.mainLang) && langPack.keys.Count > 0)
            {
                langPack.mainLang = langPack.keys[0];
                SaveChanges();
            }

            if (!langPack.languages.ContainsKey(LangSys.activeLang) && langPack.keys.Count > 0)
            {
                LangSys.activeLang = langPack.keys[0];
            }
        }

        #endregion

        #region Drawer Methods

        void Languages(Rect rect)
        {
            int l = 0;
            for (l = 0; l < langPack.keys.Count; l++)
            {
                Language(new Rect(rect.x, rect.y + l * 100f, rect.width, 92f), langPack.keys[l]);
            }
            AddField(new Rect(rect.x, rect.y + l * 100f, rect.width, 64f));
        }

        void Language(Rect rect, string key)
        {
            float offset = (rect.height - 48f) / 2f;
            Rect innerRect = new Rect(offset, offset, rect.width - offset * 2f, rect.height - offset * 2f);

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginArea(rect, styleBox);

            if (GUI.Button(new Rect(rect.width - 20f, 4f, 16f, 16f), "x", styleCenterLabel))
            {
                StartTask(TaskTypes.Remove, key);
            }

            GUI.Label(new Rect(innerRect.x, innerRect.y, 50f, 20f), key);
            langPack[key].fullname = GUI.TextField(new Rect(innerRect.x + 58f, innerRect.y, innerRect.width - 58f, 20f), langPack[key].fullname);
            
            if (GUI.Toggle(new Rect(innerRect.x + 58f, innerRect.y + 28f, (innerRect.width - 58f) / 2f - 4f, 20f), langPack.mainLang == key, " Main Language"))
            {
                if (langPack.mainLang != key)
                {
                    SaveChanges();
                }
                langPack.mainLang = key;
            }

            if (GUI.Toggle(new Rect(innerRect.x + 58f + (innerRect.width - 58f) / 2f, innerRect.y + 28f, (innerRect.width - 58f) / 2f - 4f, 20f), LangSys.activeLang == key, " Active Language"))
            {
                LangSys.activeLang = key;
            }

            GUILayout.EndArea();
            if (EditorGUI.EndChangeCheck())
            {
                SaveChanges();
            }
        }

        void AddField(Rect rect)
        {
            float offset = (rect.height - 20f) / 2f;
            Rect innerRect = new Rect(offset, offset, rect.width - offset * 2f, rect.height - offset * 2f);
            GUILayout.BeginArea(rect, styleBox);
            addKey = GUI.TextField(new Rect(innerRect.x, innerRect.y, innerRect.width - (100f + offset), innerRect.height), addKey);
            if (GUI.Button(new Rect(innerRect.x + innerRect.width - (100f), innerRect.y - 1f, 100f, innerRect.height), "ADD"))
            {
                if (!langPack.languages.ContainsKey(taskKey) && addKey != "")
                {
                    StartTask(TaskTypes.Add, addKey.ToUpper());
                    addKey = "";
                }
            }
            GUILayout.EndArea();
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Tool Methods

        void SaveChanges()
        {
            EditorUtility.SetDirty(langPack);
        }

        #endregion
    }
}
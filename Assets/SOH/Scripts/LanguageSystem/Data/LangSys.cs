using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    /// <summary>
    /// The fast access tool to get the LangPack valid for this project
    /// </summary>
    public static class LangSys
    {

        #region Constant Data

        //The path of the language data in the editor
        public const string RESOURCE_PATH = "Assets/SOH/Resources/";
        public const string DATA_PATH = "LANGUAGE_SETTINGS";
        public const string ABS_PATH = RESOURCE_PATH + DATA_PATH + ".asset";

        #endregion

        #region Language Data

        /// <summary>
        /// The key of the active Language
        /// </summary>
        public static string activeLang;

        //The cached LangPack
        private static LangPack dataCache;

        #endregion

        #region Language GetterSetter
                
        /// <summary>
        /// Returns the LangPack
        /// </summary>
        public static LangPack DATA
        {
            get
            {
                LangPack pack;

                //If LangPack is already cached in this session:
                if (dataCache)
                {
                    //Return the cached LangPack
                    return dataCache;
                }
                else
                {
                    pack = Resources.Load<LangPack>(DATA_PATH);
                }
                
                //Check if the Pack is given
                if (pack == null)
                {
                    #if UNITY_EDITOR
                    //Create a new LangPack
                    pack = ScriptableObject.CreateInstance<LangPack>();
                    UnityEditor.AssetDatabase.CreateFolder("", RESOURCE_PATH);
                    UnityEditor.AssetDatabase.CreateAsset(pack, ABS_PATH);
                    #else
                    //No LangPack given: No Language Support!
                    Debug.Log("No LangPack found!");
                    #endif

                }

                //If pack is given:
                if (pack != null)
                {
                    //Check if any language is contained
                    if (pack.languages.Count == 0)
                    {
                        //Add default language: [EN] English
                        pack.languages.Add("EN", new Language("English"));
                        pack.mainLang = "EN";
                        LangSys.activeLang = "EN";
                    }

                    //Check if the main language is still available
                    if (!pack.languages.ContainsKey(pack.mainLang) && pack.keys.Count > 0)
                    {
                        //Set the main language to the first entry
                        pack.mainLang = pack.keys[0];
                    }

                    //Check if the active language is still available
                    if (!pack.languages.ContainsKey(LangSys.activeLang) && pack.keys.Count > 0)
                    {
                        //Set the active language to the first entry
                        LangSys.activeLang = pack.keys[0];
                    }

                    //Store the LangPack in the cache for less performance peak in the future
                    dataCache = pack;
                }

                //Return the evaluated LangPack
                return pack;
            }
        }

        #endregion

        #region Methods

        public static void Save()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(DATA);
            #endif
        }

        public static void Clear()
        {
            DATA.Delete();
        }

        public static void Preload()
        {
            dataCache = DATA;
        }

        #endregion

    }
}
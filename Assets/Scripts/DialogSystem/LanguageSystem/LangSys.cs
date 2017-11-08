using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    /// <summary>
    /// The fast access tool to get the LangPack valid for this project
    /// </summary>
    public static class LangSys
    {

        #region Constant Data

        //The relative path inside the Resources folder
        public const string PATH = "LangSys/DATA";
        
        //The absolute path starting with the Assets folder
        public const string ABS_PATH = ORI_PATH + "/Resources/" + PATH + ".asset";

        //The absolute path to the Resources folder
        public const string ORI_PATH = "Assets/_DATA";

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
                //If LangPack is already cached in this session:
                if (dataCache)
                {
                    //Return the cached LangPack
                    return dataCache;
                }

                //Load LangPack from the Resources
                LangPack pack = Resources.Load<LangPack>(PATH);

                //Check if the Pack is given
                if (pack == null)
                {
                    #if UNITY_EDITOR
                        //Create a new LangPack
                        pack = ScriptableObject.CreateInstance<LangPack>();
                        UnityEditor.AssetDatabase.CreateAsset(pack, ABS_PATH);
                    #else
                        //No LangPack given: No Language Support!
                        Debug.Warning("No LangPack found!", Color.yellow);
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

    }
}
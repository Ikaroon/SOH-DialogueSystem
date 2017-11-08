using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.CharacterSystem;
using SpyOnHuman.DialogSystem.LanguageSystem;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    public class DialogSystemDatabase : ScriptableObject
    {
        #region Constant Data

        //The relative path inside the Resources folder
        public const string PATH = "DialogSystem/DATA";

        //The absolute path starting with the Assets folder
        public const string ABS_PATH = ORI_PATH + "/Resources/" + PATH + ".asset";

        //The absolute path to the Resources folder
        public const string ORI_PATH = "Assets/_DATA";

        #endregion

        #region Database Static Data

        //The cached Database
        private static DialogSystemDatabase dataCache;

        #endregion

        #region Database GetterSetter

        /// <summary>
        /// Returns the Database
        /// </summary>
        public static DialogSystemDatabase DATA
        {
            get
            {
                //If Database is already cached in this session:
                if (dataCache)
                {
                    //Return the cached Database
                    return dataCache;
                }

                //Load Database from the Resources
                DialogSystemDatabase pack = Resources.Load<DialogSystemDatabase>(PATH);

                //Check if the Database is given
                if (pack == null)
                {
                    #if UNITY_EDITOR
                    //Create a new Database
                    pack = ScriptableObject.CreateInstance<DialogSystemDatabase>();
                    UnityEditor.AssetDatabase.CreateAsset(pack, ABS_PATH);
                    #else
                    //No Database given
                    Debug.Warning("No LangPack found!", Color.yellow);
                    #endif

                }

                //If pack is given:
                if (pack != null)
                {
                    //Store the Database in the cache for less performance peak in the future
                    dataCache = pack;
                }

                //Return the evaluated Database
                return pack;
            }
        }

        #endregion

        #region Database Data

        public List<Character> characters = new List<Character>();
        public List<DialogCanvas> dialogs = new List<DialogCanvas>();

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    /// <summary>
    /// A package of all Languages available in this project: Should only exist once
    /// </summary>
    [System.Serializable]
    public class LangPack : ScriptableObject
    {
        #region LangPack Content

        /// <summary>
        /// The language packing dictionary
        /// </summary>
        [SerializeField]
        public LangDictionary languages = new LangDictionary();
        
        /// <summary>
        /// The key for the main language
        /// </summary>
        public string mainLang;

        #endregion

        #region LangPack GetterSetter

        /// <summary>
        /// Get the language with the given key
        /// </summary>
        /// <param name="i">The lang Key</param>
        /// <returns>The requested Language from the LangPack</returns>
        public Language this[string i] {
            get
            {
                return languages[i];
            }
        }

        /// <summary>
        /// Returns a list of all keys stored in the LangPack
        /// </summary>
        public List<string> keys
        {
            get
            {
                return new List<string>(languages.Keys);
            }
        }

        #endregion

        #region Extra Methods

        public void Delete()
        {
            DestroyImmediate(this);
        }

        #endregion

    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// A Dictionary which is sorted and serializable
    /// </summary>
    [System.Serializable]
    public class LangDictionary : SerializableSortedDictionary<string, Language>
    {
        //A Language packing dictionary
    }
}
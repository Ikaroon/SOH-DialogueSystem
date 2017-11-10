using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    /// <summary>
    /// A text container which changes it's content with the active language
    /// </summary>
    [System.Serializable]
    public class LangText
    {
        //--------------------------------\\
        //-----------< EDITOR >-----------\\
        //--------------------------------\\

        #if UNITY_EDITOR

        #region LangText Editor Methods

        /// <summary>
        /// Removes all obsolete keys and adds all missing keys
        /// </summary>
        public void CheckKeys()
        {
            for (int k = 0; k < keys.Count; k++)
            {
                if (!LangSys.DATA.languages.ContainsKey(keys[k]))
                {
                    langTexts.Remove(keys[k]);
                }
            }
            for (int k = 0; k < LangSys.DATA.keys.Count; k++)
            {
                if (!langTexts.ContainsKey(LangSys.DATA.keys[k]))
                {
                    langTexts.Add(LangSys.DATA.keys[k], "");
                }
            }
        }

        #endregion

        #endif

        //--------------------------------\\
        //-----------< PLAYER >-----------\\
        //--------------------------------\\

        #region LangText Data

        //The language packing dictionary
        [SerializeField]
        private TextDictionary langTexts = new TextDictionary();

        #endregion

        #region LangText GetterSetter

        /// <summary>
        /// Get all keys of the container
        /// </summary>
        private List<string> keys
        {
            get
            {
                return new List<string>(langTexts.Keys);
            }
        }

        /// <summary>
        /// Get or set the text of the given key
        /// </summary>
        /// <param name="key">The language key which should be accessed</param>
        /// <returns>The text stored in the container under the given key</returns>
        public string this[string key]
        {
            get
            {
                if (!langTexts.ContainsKey(key))
                {
                    return "";
                }
                return langTexts[key];
            }
            set
            {
                if (langTexts.ContainsKey(key))
                {
                    langTexts[key] = value;
                } else
                {
                    langTexts.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Returns the text of the active language
        /// </summary>
        public string text
        {
            get
            {
                return this[LangSys.activeLang];
            }
            set
            {
                this[LangSys.activeLang] = value;
            }
        }

        #endregion
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// A Dictionary which is sorted and serializable
    /// </summary>
    [System.Serializable]
    public class TextDictionary : SerializableSortedDictionary<string, string>
    {
        //A Language packing dictionary
    }
}
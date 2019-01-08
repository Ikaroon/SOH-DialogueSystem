using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    /// <summary>
    /// Language Data Package
    /// </summary>
    [System.Serializable]
    public class Language
    {
        #region Language Content

        //The fullname of the language
        public string fullname;

        #endregion

        #region Language Methods

        /// <summary>
        /// Creates a Language with an optional flag
        /// </summary>
        /// <param name="langName">The fullname of the language</param>
        /// <param name="langFlag">The flag of the language</param>
        public Language(string langName)
        {
            fullname = langName;
        }

        #endregion
        
    }
}
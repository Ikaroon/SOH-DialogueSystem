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
    public class LangAudio
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
                    langAudios.Remove(keys[k]);
                }
            }
            for (int k = 0; k < LangSys.DATA.keys.Count; k++)
            {
                if (!langAudios.ContainsKey(LangSys.DATA.keys[k]))
                {
                    langAudios.Add(LangSys.DATA.keys[k], null);
                }
            }
        }

        #endregion

        #endif

        //--------------------------------\\
        //-----------< PLAYER >-----------\\
        //--------------------------------\\

        #region LangAudio Data

        //The language packing dictionary
        [SerializeField]
        private AudioDictionary langAudios = new AudioDictionary();

        #endregion

        #region LangAudio GetterSetter

        /// <summary>
        /// Get all keys of the container
        /// </summary>
        private List<string> keys
        {
            get
            {
                return new List<string>(langAudios.Keys);
            }
        }

        /// <summary>
        /// Get or set the audio of the given key
        /// </summary>
        /// <param name="key">The language key which should be accessed</param>
        /// <returns>The audio stored in the container under the given key</returns>
        public AudioClip this[string key]
        {
            get
            {
                if (!langAudios.ContainsKey(key))
                {
                    return null;
                }
                return langAudios[key];
            }
            set
            {
                if (langAudios.ContainsKey(key))
                {
                    langAudios[key] = value;
                }
                else
                {
                    langAudios.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Returns the audio of the active language
        /// </summary>
        public AudioClip audio
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
    public class AudioDictionary : SerializableSortedDictionary<string, AudioClip>
    {
        //A Language packing dictionary
    }
}
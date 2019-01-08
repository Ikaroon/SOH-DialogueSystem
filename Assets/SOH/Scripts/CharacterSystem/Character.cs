using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.CharacterSystem
{
    /// <summary>
    /// A character sheet with all needed information
    /// </summary>
    public class Character : ScriptableObject
    {
        //--------------------------------\\
        //-----------< EDITOR >-----------\\
        //--------------------------------\\

        #region Constant Data

        public const string PATH = "Assets/SOH/Resources/Characters/";

        #endregion

        #if UNITY_EDITOR

        #region Editor Data

        //The timestamp used for identification
        public string timestamp;

        #endregion

        #endif

        //--------------------------------\\
        //-----------< PLAYER >-----------\\
        //--------------------------------\\

        #region Profile Data
            
        /// <summary>
        /// The profile image for this Character
        /// </summary>
        public Texture2D profile;
        
        /// <summary>
        /// The foreame for this Character
        /// </summary>
        public string forename = "";

        /// <summary>
        /// The surname for this Character
        /// </summary>
        public string surname = "";

        /// <summary>
        /// The age of this Character
        /// </summary>
        public int age = 0;

        //The Gender enum
        public enum Genders { Male = 0, Female = 1 };
        private Dictionary<string, string[]> genderTranslation;

        /// <summary>
        /// The gender of this Character
        /// </summary>
        public Genders gender = Genders.Male;

        public string profession = "";

        #endregion

        #region System Data

        /// <summary>
        /// The basic color for this character
        /// </summary>
        public Color textColor = Color.white;

        public bool isPlayer = false;

        #endregion

        #region Methods

        private void OnEnable()
        {
            genderTranslation = new Dictionary<string, string[]>();
            genderTranslation.Add("EN", new string[] { "Male", "Female" });
            genderTranslation.Add("DE", new string[] { "Männlich", "Weiblich" });
        }

        public string GenderTranslation()
        {
            if (genderTranslation.ContainsKey(LangSys.activeLang))
            {
                return genderTranslation[LangSys.activeLang][(int)gender];
            }
            return gender.ToString();
        }

        #endregion

    }
}
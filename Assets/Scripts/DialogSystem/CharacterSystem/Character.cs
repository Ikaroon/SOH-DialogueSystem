using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem.CharacterSystem
{
    /// <summary>
    /// A character sheet with all needed information
    /// </summary>
    public class Character : ScriptableObject
    {
        //--------------------------------\\
        //-----------< EDITOR >-----------\\
        //--------------------------------\\

        #if UNITY_EDITOR

        #region Editor Data

        public string timestamp;

        #endregion

        #endif

        //--------------------------------\\
        //-----------< PLAYER >-----------\\
        //--------------------------------\\

        #region Profile Data

        public Texture2D profile;

        public string forename = "", surname = "";
        public int age = 0;

        public enum Genders { Male = 0, Female = 1 };
        public Genders gender = Genders.Male;

        #endregion

        #region Gameplay Data

        [Range(0f, 1f)]
        public float trustStability = 0.75f;

        [Range(0.5f, 2f)]
        public float heartModifier = 1f;

        #endregion

        #region System Data

        public Color textColor = Color.white;

        public GameObject playerPrefab;

        public InfoSheet infoSheet;

        public List<DialogCanvas> storySheets = new List<DialogCanvas>();

        #endregion

    }
}
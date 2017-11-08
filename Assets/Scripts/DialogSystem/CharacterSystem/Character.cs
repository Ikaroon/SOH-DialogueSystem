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

        /// <summary>
        /// The gender of this Character
        /// </summary>
        public Genders gender = Genders.Male;

        #endregion

        #region Gameplay Data

        /// <summary>
        /// The stability of the trust level
        /// </summary>
        [Range(0f, 1f)]
        public float trustStability = 0.75f;

        /// <summary>
        /// The modification of the relationship level
        /// </summary>
        [Range(0.5f, 2f)]
        public float heartModifier = 1f;

        #endregion

        #region System Data

        /// <summary>
        /// The basic color for this character
        /// </summary>
        public Color textColor = Color.white;

        /// <summary>
        /// The prefab for the player Instance
        /// </summary>
        public GameObject playerPrefab;

        /// <summary>
        /// The sheet with all individual Informations
        /// </summary>
        public InfoSheet infoSheet;

        /// <summary>
        /// All Dialog Canvas Entities which are individual Dialogs
        /// </summary>
        public List<DialogCanvas> storySheets = new List<DialogCanvas>();

        #endregion

    }
}
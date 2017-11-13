using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    public class LanguageStarter : MonoBehaviour
    {
        public string languageKey = "DE";

        // Use this for initialization
        void Start()
        {
            LangSys.activeLang = languageKey;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
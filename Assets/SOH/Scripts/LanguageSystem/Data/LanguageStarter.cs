using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.LanguageSystem
{
    public class LanguageStarter : MonoBehaviour
    {

        public string langKey = "EN";
        
        void Start()
        {
            LangSys.activeLang = langKey;
        }
    }
}
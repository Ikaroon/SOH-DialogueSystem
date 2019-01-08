using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public class FPSDisplay : MonoBehaviour
    {

        void OnGUI()
        {
            GUI.Label(new Rect(16f, 16f, 256f, 256f), "FPS: " + Mathf.RoundToInt(1f / Time.deltaTime));
        }
    }
}
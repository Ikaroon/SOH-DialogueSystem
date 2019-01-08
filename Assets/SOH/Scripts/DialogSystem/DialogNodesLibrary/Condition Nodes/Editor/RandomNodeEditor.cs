using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

using SpyOnHuman.CharacterSystem;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(RandomNode))]
    public class RandomNodeEditor : NodeInspector
    {
        RandomNode node;

        void OnEnable()
        {
            node = (RandomNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {

        }
    }
}
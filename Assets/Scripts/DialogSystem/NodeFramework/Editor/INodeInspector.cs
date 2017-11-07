using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.NodeFramework
{
    public interface INodeInspector
    {

        void OnDrawNodeGUI(Rect rect);

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public abstract class NodeInspector : Editor
    {

        public virtual void OnDrawNodeGUI(Rect rect, DialogCanvas canvas)
        {
            OnDrawNodeGUI(rect);
        }

        public virtual void OnDrawNodeGUI(Rect rect)
        {

        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Define Point", "Defines a point in the canvas you can jump to.", 128f, 80f)]
    public class DefinePointNode : Node
    {
        [NodeHandle(0, ConnectionType.Output, 0f)]
        public NodeConnection output;

        public string pointKey = "";
        public bool isValid = false;

        public override void OnDelete(DialogCanvas canvas)
        {
            if (canvas.definedPoints.ContainsKey(pointKey))
            {
                canvas.definedPoints.Remove(pointKey);
            }
        }
    }
}
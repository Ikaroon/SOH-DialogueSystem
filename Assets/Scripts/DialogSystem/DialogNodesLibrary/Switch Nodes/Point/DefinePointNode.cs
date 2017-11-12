using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Define Point", "Defines a point in the canvas you can jump to.", 128f, 64f)]
    public class DefinePointNode : Node
    {
        [NodeHandle(0, ConnectionType.Output, 0f)]
        public NodeConnection output;

        public string pointKey;
    }
}
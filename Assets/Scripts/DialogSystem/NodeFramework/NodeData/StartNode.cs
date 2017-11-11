using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [NodeData("Start", "The Start Node", 128f, 64f)]
    public class StartNode : Node
    {
        [NodeHandle(0, ConnectionType.Output, y: 32f)]
        public NodeConnection output;
    }
}
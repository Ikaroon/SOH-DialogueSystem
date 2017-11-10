﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [NodeData("Start", "The Start Node", 320f, 140f)]
    public class StartNode : Node
    {
        [NodeHandle(0, ConnectionType.Output, y: 16f)]
        public NodeConnection output;
    }
}
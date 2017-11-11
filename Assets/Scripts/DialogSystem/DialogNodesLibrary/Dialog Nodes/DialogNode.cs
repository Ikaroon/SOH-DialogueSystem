﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog", "A Node which refers to another Dialog Canvas", 144f, 64f, false, 0.1f, 0.5f, 0.9f)]
    public class DialogNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, y: 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 0f)]
        public NodeConnection output;

        public DialogCanvas dialog;
    }
}
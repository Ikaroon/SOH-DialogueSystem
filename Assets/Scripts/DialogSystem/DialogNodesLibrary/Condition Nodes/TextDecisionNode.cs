﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Player Decision", "A Dialog Node", 256f, 256f, true, 1f, 0.8f, 0.1f)]
    public class TextDecisionNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, 1f/6f, fixedPosition: false)]
        public NodeConnection decisionA;

        [NodeHandle(1, ConnectionType.Output, 3f / 6f, fixedPosition: false)]
        public NodeConnection decisionB;

        [NodeHandle(2, ConnectionType.Output, 5f / 6f, fixedPosition: false)]
        public NodeConnection decisionC;

        public LangText decisionAText = new LangText();
        public LangText decisionBText = new LangText();
        public LangText decisionCText = new LangText();

    }
}
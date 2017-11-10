using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Player Decision", "A Dialog Node", 264f, 140f)]
    public class TextDecisionNode : Node
    {
        [NodeHandle(ConnectionType.Input, y: 36f)]
        public NodeConnection input;

        [NodeHandle(ConnectionType.Output, y: 36f)]
        public NodeConnection decisionA;

        [NodeHandle(ConnectionType.Output, y: 72f)]
        public NodeConnection decisionB;

        [NodeHandle(ConnectionType.Output, y: 108f)]
        public NodeConnection decisionC;

        public LangText decisionAText = new LangText();
        public LangText decisionBText = new LangText();
        public LangText decisionCText = new LangText();

    }
}
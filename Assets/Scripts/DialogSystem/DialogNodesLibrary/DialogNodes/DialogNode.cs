using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog", "A Dialog Node", 264f, 140f)]
    public class DialogNode : Node
    {
        [NodeHandle(ConnectionType.Input, y: 16f)]
        public NodeConnection input;

        [NodeHandle(ConnectionType.Output, y: 16f)]
        public NodeConnection output;

        [NodeHandle(ConnectionType.Output, y: 48f)]
        public NodeConnection outputA;

        public LangText text = new LangText();

    }
}
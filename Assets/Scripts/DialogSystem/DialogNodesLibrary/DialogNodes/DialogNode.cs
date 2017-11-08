using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog", "A Dialog Node", 320f, 140f)]
    public class DialogNode : Node
    {
        [NodeHandle(ConnectionType.Input, 0f, 16f)]
        public NodeConnection input;

        [NodeHandle(ConnectionType.Output, 0f, 16f)]
        public NodeConnection output;

        public LangText text = new LangText();

    }
}
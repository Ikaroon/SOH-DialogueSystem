using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog", "A Dialog Node", 256f, 128f)]
    public class DialogNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, y: 32f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 32f)]
        public NodeConnection output;

        public LangText text = new LangText();

    }
}
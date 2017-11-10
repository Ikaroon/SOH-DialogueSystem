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
        [NodeHandle(0, ConnectionType.Input, y: 36f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 36f)]
        public NodeConnection output;

        public LangText text = new LangText();

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class MouseEventPackage
    {

        public EventType eventType;
        public MouseButton mouseButton;

        public MouseEventPackage(EventType type, MouseButton button)
        {
            eventType = type;
            mouseButton = button;
        }

    }
}

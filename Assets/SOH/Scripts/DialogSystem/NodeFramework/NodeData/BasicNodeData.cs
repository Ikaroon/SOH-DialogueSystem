using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    //------------------------------\\
    //-----------< DATA >-----------\\
    //------------------------------\\

    //All connection types which can be used by a NodeConnection
    public enum ConnectionType {
        Input = 0,
        Output = 1
    };

    //The type of Mouse Button
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        None = 3,
    }
}

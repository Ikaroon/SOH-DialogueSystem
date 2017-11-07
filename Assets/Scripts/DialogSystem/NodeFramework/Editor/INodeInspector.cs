using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public interface INodeInspector {

    void OnDrawNodeGUI(Rect rect);

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

[CustomEditor(typeof(TestOperator))]
public class TestOperatorEditor : Editor {

    TestOperator test;

    private void OnEnable()
    {
        test = (TestOperator)target;
    }

    public override void OnInspectorGUI()
    {
        test.testString = GUILayout.TextField(test.testString);

        if (GUILayout.Button("Test Operator"))
        {
            Dictionary<string, System.Type> types = NodeOperator.CollectNodeTypes(test.testString);
            foreach (KeyValuePair<string, System.Type> type in types)
            {
                //Node node = Node.CreateNode(type.Value, Vector2.zero);
                Debug.Log(type.Key + " -> " + type.Value.Name);
            }
        }
    }
}

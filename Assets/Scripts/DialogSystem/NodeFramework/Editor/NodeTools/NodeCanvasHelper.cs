using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SpyOnHuman.DialogSystem.NodeFramework {
    public class NodeCanvasHelper {

        [OnOpenAsset(0)]
        public static bool OpenCanvas(int instanceID, int line)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj.GetType() == typeof(DialogCanvas))
            {
                DialogEditor window = (DialogEditor)EditorWindow.GetWindow(typeof(DialogEditor));
                window.oldPath = NodeSaveOperator.LoadCanvas(ref window.canvas, AssetDatabase.GetAssetPath(instanceID));
                window.Show();
                return true;
            }
            return false;
        }

    }
}
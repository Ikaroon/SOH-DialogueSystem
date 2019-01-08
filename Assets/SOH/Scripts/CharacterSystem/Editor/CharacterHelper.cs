using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SpyOnHuman.CharacterSystem
{
    public class CharacterHelper
    {
        [OnOpenAsset(0)]
        public static bool OpenCanvas(int instanceID, int line)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj.GetType() == typeof(Character))
            {
                CharacterEditWindow window = (CharacterEditWindow)EditorWindow.GetWindow(typeof(CharacterEditWindow));
                window.Load(AssetDatabase.GetAssetPath(instanceID));
                window.Show();
                return true;
            }
            return false;
        }
    }
}
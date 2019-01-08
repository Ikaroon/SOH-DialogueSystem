using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using SpyOnHuman.CharacterSystem;

public static class CharacterGUI {

    /// <summary>
    /// Collects all Characters  under a specific root folder and returns them in a Dictionary
    /// </summary>
    /// <param name="root">The root folder of the search query</param>
    /// <returns>The relative path and the Node Type</returns>
    private static Dictionary<string, Character> CollectCharacters(string root)
    {
        Dictionary<string, Character> characters = new Dictionary<string, Character>();

        string[] assets = AssetDatabase.FindAssets("t:Character", new string[] { root });
        for (int a = 0; a < assets.Length; a++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[a]);
            if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(Character))
            {
                Character character = AssetDatabase.LoadAssetAtPath<Character>(path);

                if (character != null)
                {
                    characters.Add(character.forename + " " + character.surname, character);
                }
            }
        }

        return characters;
    }

    public static Character CharacterDropDown(Rect rect, Character character)
    {
        Dictionary<string, Character> characters = CollectCharacters(Character.PATH.Substring(0, Character.PATH.Length - 1));

        List<string> keys = new List<string>(characters.Keys);

        keys.Insert(0, "None");
        int index = 0;
        if (character != null && characters.ContainsValue(character))
        {
            index = keys.IndexOf(character.forename + " " + character.surname);
        }

        GUIContent[] contents = new GUIContent[keys.Count];
        for (int k = 0; k < keys.Count; k++)
        {
            contents[k] = new GUIContent(keys[k]);
        }
        int newIndex = EditorGUI.Popup(rect, index, contents);

        if (newIndex == 0)
        {
            return null;
        }

        return characters[keys[newIndex]];
    }

}

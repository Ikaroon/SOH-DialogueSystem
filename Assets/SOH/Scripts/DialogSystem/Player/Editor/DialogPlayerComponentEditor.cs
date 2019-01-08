using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DialogPlayerComponent))]
    public class DialogPlayerComponentEditor : Editor
    {

        #region Style Data

        private GUIStyle playbarHeader, playbarRewind, playbarPlay, playbarPause, playbarStop, playbarFastForward;
        private bool init = false;

        private string symbolPause = '\u258C' + "" + '\u258C';
        private string symbolPlay = ((char)9654) + "";
        private string symbolStop = ((char)9632) + "";
        private string symbolSkip = ((char)9654) + "" + ((char)9654);
        //private string symbolReturn = ((char)9664) + "" + ((char)9664);

        #endregion

        #region Style Methods

        private void InitializeStyles()
        {
            if (init)
            {
                return;
            }

            playbarHeader = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            playbarHeader.alignment = TextAnchor.MiddleCenter;

            playbarRewind = new GUIStyle(GUI.skin.GetStyle("LargeButtonLeft"));
            playbarRewind.fixedWidth = 48f;
            playbarRewind.fixedHeight = 24f;

            playbarPlay = new GUIStyle(GUI.skin.GetStyle("LargeButtonLeft"));
            playbarPlay.fixedWidth = 48f;
            playbarPlay.fixedHeight = 24f;

            playbarPause = new GUIStyle(GUI.skin.GetStyle("LargeButtonMid"));
            playbarPause.fixedWidth = 48f;
            playbarPause.fixedHeight = 24f;
            playbarPause.fontSize = 8;
            playbarPause.alignment = TextAnchor.MiddleCenter;
            playbarPause.padding = new RectOffset(4, 0, 0, 0);

            playbarStop = new GUIStyle(GUI.skin.GetStyle("LargeButtonMid"));
            playbarStop.fixedWidth = 48f;
            playbarStop.fixedHeight = 24f;
            playbarStop.fontSize = 20;
            playbarStop.alignment = TextAnchor.MiddleCenter;
            playbarStop.padding = new RectOffset(0, 0, -4, 0);

            playbarFastForward = new GUIStyle(GUI.skin.GetStyle("LargeButtonRight"));
            playbarFastForward.fixedWidth = 48f;
            playbarFastForward.fixedHeight = 24f;
        }

        #endregion

        DialogPlayerComponent player;

        private void OnEnable()
        {
            player = (DialogPlayerComponent)target;
        }

        public override void OnInspectorGUI()
        {
            InitializeStyles();

            DialogPlayer playerInstance = player.player;

            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("GroupBox"));

            GUILayout.Label(new GUIContent("Dialog Player Controls"), playbarHeader);

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            playerInstance.playerState = GUILayout.Toggle(playerInstance.playerState == DialogPlayer.PlayerState.Playing,
                new GUIContent(symbolPlay, "Plays the Canvas"), playbarPlay)
                ? DialogPlayer.PlayerState.Playing : playerInstance.playerState;
            playerInstance.playerState = GUILayout.Toggle(playerInstance.playerState == DialogPlayer.PlayerState.Paused,
                new GUIContent(symbolPause, "Pauses the Player"), playbarPause)
                ? DialogPlayer.PlayerState.Paused : playerInstance.playerState;
            playerInstance.playerState = GUILayout.Toggle(playerInstance.playerState == DialogPlayer.PlayerState.Stopped,
                new GUIContent(symbolStop, "Returns to the Canvas' start"), playbarStop)
                ? DialogPlayer.PlayerState.Stopped : playerInstance.playerState;
            playerInstance.playerState = GUILayout.Toggle(playerInstance.playerState == DialogPlayer.PlayerState.Skip,
                new GUIContent(symbolSkip, "Forces a new Update of the current Node"), playbarFastForward)
                ? DialogPlayer.PlayerState.Skip : playerInstance.playerState;

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            player.isAutoplay = GUILayout.Toggle(player.isAutoplay, "Autoplay", EditorStyles.miniButtonLeft, GUILayout.Width(96f));
            playerInstance.pauseGame = GUILayout.Toggle(playerInstance.pauseGame, "Pause Game", EditorStyles.miniButtonRight, GUILayout.Width(96f));

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);
            
            Node lastHub = playerInstance.GetCurrentHub();
            EditorGUI.BeginDisabledGroup(lastHub == null);

            int count = playerInstance.registeredHubs.Count;
            if (GUILayout.Button("Return to Hub: " + count + "/" + count, EditorStyles.miniButton, GUILayout.Width(192f)))
            {
                playerInstance.CheckHubs();
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUI.BeginDisabledGroup(lastHub == null);

            if (GUILayout.Button("Discard Hub: " + count + "/" + count, EditorStyles.miniButton, GUILayout.Width(192f)))
            {
                playerInstance.DiscardHub();
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUI.BeginDisabledGroup(lastHub == null);

            if (GUILayout.Button("Discard all Hubs", EditorStyles.miniButton, GUILayout.Width(192f)))
            {
                playerInstance.registeredHubs.Clear();
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(((Screen.width - 40f) - 192f) / 2f);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Dialog Name"));
            playerInstance.canvas = (DialogCanvas)EditorGUILayout.ObjectField(playerInstance.canvas, typeof(DialogCanvas), false);// DialogSettings.DialogsDropDown(playerInstance.dialogName);
            GUILayout.Space(5f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        private Node lastHub;
        private DialogPlayer.PlayerState lastPlayerState;
        public override bool RequiresConstantRepaint()
        {
            if (player.player.GetCurrentHub() != lastHub)
            {
                lastHub = player.player.GetCurrentHub();
                return true;
            }
            if (player.player.playerState != lastPlayerState)
            {
                lastPlayerState = player.player.playerState;
                return true;
            }
            return false;
        }

        string DecodeHtmlChars(string aText)
        {
            string[] parts = aText.Split(new string[] { "&#" }, System.StringSplitOptions.None);
            for (int i = 1; i < parts.Length; i++)
            {
                int n = parts[i].IndexOf(';');
                string number = parts[i].Substring(0, n);
                try
                {
                    int unicode = System.Convert.ToInt32(number, 16);
                    parts[i] = ((char)unicode) + parts[i].Substring(n + 1);
                }
                catch { }
            }
            return System.String.Join("", parts);
        }

    }
}
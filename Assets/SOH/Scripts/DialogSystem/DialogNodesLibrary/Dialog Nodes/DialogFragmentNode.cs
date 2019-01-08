using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;
using SpyOnHuman.CharacterSystem;

using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog Fragment", "A Dialog Fragment Node", 256f, 176f, true, 0.1f, 0.5f, 0.9f)]
    public class DialogFragmentNode : Node
    {

        #region Handles

        [NodeHandle(0, ConnectionType.Input, y: 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 0f)]
        public NodeConnection output;

        #endregion

        #region Content

        public Character speaker;
        
        [SerializeField]
        public LangText textSource = new LangText();
        [SerializeField]
        public LangAudio audioSource = new LangAudio();

        public bool isAuto = false;
        public float showTime = 3f;

        #endregion

        #region Getter

        public string text
        {
            get
            {
                return textSource.text;
            }
        }

        public AudioClip audio
        {
            get
            {
                return audioSource.audio;
            }
        }

        #endregion

        #region Display Data

        private string[] words;
        private Vector2Int wordRange = new Vector2Int(0,1);
        private float directionATime, directionBTime = 0f;
        private float remainingTime = 0f;

        #endregion

        #region Update Methods

        public override bool IsAuto()
        {
            return isAuto;
        }

        public override Node PrepareNode()
        {
            if (audio)
            {
                remainingTime = audio.length;
                Audio.PlayAtPosition(audio, Vector3.zero, 1f, 10f);
            } else
            {
                remainingTime = showTime;
            }

            words = text.Split(' ');
            wordRange = new Vector2Int(0, words.Length);
            return this;
        }

        public override Node UpdateNode()
        {
            if (isAuto)
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime <= 0f)
                {
                    if (output)
                    {
                        return output.to.PrepareNode();
                    }
                    return null;
                }
                return this;
            }
            else if (output)
            {
                return output.to.PrepareNode();
            }
            return null;
        }

        #endregion

        #region Display Methods

        public override void DisplayNode(Rect rect)
        {
            float sizeMod = (rect.width / 1920);

            GUIStyle label = new GUIStyle(GUI.skin.label);
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = (int)(30 * sizeMod);
            label.richText = true;

            GUIStyle smallLabel = new GUIStyle(GUI.skin.label);
            smallLabel.alignment = TextAnchor.MiddleCenter;
            smallLabel.fontSize = (int)(30 * sizeMod);
            smallLabel.richText = true;

            Rect textRect = new Rect(0f, rect.height - 128f * sizeMod, rect.width, 128f * sizeMod);
            string displayText = "";
            string whiteText = "";

            if (speaker != null)
            {
                string speakerColor = "#" + ColorUtility.ToHtmlStringRGBA(speaker.textColor);
                displayText = "<b><color=" + speakerColor + ">" + speaker.forename + ": </color></b>" + text;
                whiteText = "<b>" + speaker.forename + ": </b>" + text;
            }
            else
            {
                displayText = text;
                whiteText = text;
            }


            Northwind.Essentials.GUIEffects.DrawOutline(textRect, whiteText, label, Color.black, 1f);

            GUI.Label(textRect, new GUIContent(displayText), label);

        }

        #endregion

    }
}
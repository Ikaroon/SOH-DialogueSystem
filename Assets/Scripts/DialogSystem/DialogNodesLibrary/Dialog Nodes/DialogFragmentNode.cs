using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;
using SpyOnHuman.DialogSystem.CharacterSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog Fragment", "A Dialog Node", 256f, 160f, true, 0.1f, 0.5f, 0.9f)]
    public class DialogFragmentNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, y: 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 0f)]
        public NodeConnection output;

        public Character speaker;

        public LangText text = new LangText();
        public LangAudio audio = new LangAudio();

        private string[] words;
        private Vector2Int wordRange = new Vector2Int(0,1);
        private float directionATime, directionBTime = 0f;

        public override Node PrepareNode()
        {
            words = text.text.Split(' ');
            wordRange = new Vector2Int(0, words.Length);
            return this;
        }

        public override Node UpdateNode()
        {
            if (output)
            {
                return output.to.PrepareNode();
            }
            return null;
        }

        public override void DisplayNode(Rect rect)
        {
            float direction = Input.GetAxis("ChangeRightMark");// Input.GetAxis("ChangeMark");
            if (direction != 0)
            {
                directionBTime += Time.deltaTime;
                if (directionBTime > 0.5f)
                {
                    directionBTime = 0f;
                } else
                {
                    direction = Mathf.Sign(direction);
                    direction *= 0f;
                }
            } else
            {
                directionBTime = 0f;
                direction *= 0f;
            }
            
            if (direction != 0f)
            {
                wordRange.y = (int)Mathf.Clamp(wordRange.y + Mathf.Round(direction), Mathf.Max(0, wordRange.x + 1), words.Length);
            }

            direction = Input.GetAxis("ChangeLeftMark");// Input.GetAxis("ChangeMark");
            if (direction != 0)
            {
                directionATime += Time.deltaTime;
                if (directionATime > 0.5f)
                {
                    directionATime = 0f;
                }
                else
                {
                    direction = Mathf.Sign(direction);
                    direction *= 0f;
                }
            }
            else
            {
                directionATime = 0f;
                direction *= 0f;
            }

            if (direction != 0f)
            {
                wordRange.x = (int)Mathf.Clamp(wordRange.x + Mathf.Round(direction), 0, Mathf.Min(words.Length - 1, wordRange.y - 1));
            }

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
                whiteText = "<b>" + speaker.forename + ": </b>" + text.text;
                displayText = "<b><color=" + speakerColor + ">" + speaker.forename + ": </color></b>";// + text.text;
            } else
            {
                displayText = "<b>Du: </b>";// + text.text;
                whiteText = "<b>Du: </b>" + text.text;//displayText;
            }

            string prefixText = "";
            string markedText = "";
            string suffixText = "";
            for (int w = 0; w < words.Length; w++)
            {
                if (w < wordRange.x)
                {
                    prefixText += words[w] + " ";
                } else if (w < wordRange.y)
                {
                    markedText += words[w] + " ";
                } else
                {
                    suffixText += words[w] + " ";
                }
            }

            DrawOutline(textRect, whiteText, label, Color.black);
            GUI.Label(textRect, new GUIContent(displayText + prefixText + "<color=red>" + (suffixText.Length > 0 ? markedText : markedText.Substring(0, Mathf.Max(0, markedText.Length - 1))) + 
                "</color>" + suffixText.Substring(0, Mathf.Max(0, suffixText.Length - 1))), label);// displayText), label);

            /*GUI.Label(new Rect(0f, rect.height - 256f * sizeMod, rect.width, 128f * sizeMod), new GUIContent(
                prefixText + "<color=red>" + markedText + "</color>" + suffixText), smallLabel);*/
        }

        private void DrawOutline(Rect rect, string text, GUIStyle style, Color color, float thickness = 1f)
        {
            Rect offsetRect = rect;
            string outlineColor = "#" + ColorUtility.ToHtmlStringRGBA(color);
            string finalText = "<color=" + outlineColor + ">" + text + "</color>";

            for (int x = -1; x <= 1; x+= 2)
            {
                for (int y = -1; y <= 1; y+= 2)
                {
                    offsetRect.position = rect.position + new Vector2(x, y) * thickness;

                    GUI.Label(offsetRect, new GUIContent(finalText), style);
                }
            }
        }

    }
}
using I2.Loc;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ConveyorX.Editor.Utils
{
    public class TextConvertorEditor : MonoBehaviour
    {
        [MenuItem("ConveyorX/Convert Selected Texts To TMPro")]
        private static void ConvertAllText()
        {
            GameObject[] selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                Debug.LogError("Error! No active objects selected!");
                return;
            }

            Text[] texts = GetTexts(selected);

            if (texts.Length <= 0)
                return;

            for (int i = 0; i < texts.Length; i++)
            {
                string content = texts[i].text;
                Color textColor = texts[i].color;
                int fontSize = texts[i].fontSize;
                TextAnchor alignment = texts[i].alignment;
                bool raycastTarget = texts[i].raycastTarget;
                FontStyles fs = GetTMPFontStyle(texts[i].fontStyle);

                GameObject g = texts[i].gameObject;
                DestroyImmediate(texts[i]);

                TextMeshProUGUI tmp = g.AddComponent<TextMeshProUGUI>();
                tmp.text = content;
                tmp.fontSize = fontSize;
                tmp.color = textColor;
                tmp.alignment = GetTMPAligment(alignment);
                tmp.raycastTarget = raycastTarget;

                //tmp.enableAutoSizing = true;
                tmp.fontStyle = fs;
                //tmp.outlineWidth = 0.2f;

                EditorUtility.SetDirty(g);
            }

            Debug.Log("Converted all Text Components to TMPro Texts.");
        }

        private static Text[] GetTexts(GameObject[] selected)
        {
            List<Text> Texts = new();

            for (int i = 0; i < selected.Length; i++)
            {
                Texts.Add(selected[i].GetComponent<Text>());
            }

            return Texts.ToArray();
        }

        private static TextMeshProUGUI[] GetTMPros(GameObject[] selected)
        {
            List<TextMeshProUGUI> Texts = new();

            for (int i = 0; i < selected.Length; i++)
            {
                Texts.Add(selected[i].GetComponent<TextMeshProUGUI>());
            }

            return Texts.ToArray();
        }

        private static FontStyles GetTMPFontStyle(FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case FontStyle.Bold: return FontStyles.Bold;
                case FontStyle.Italic: return FontStyles.Italic;
                case FontStyle.BoldAndItalic: return FontStyles.Bold;
                default: return FontStyles.Normal;
            }
        }

        private static TextAlignmentOptions GetTMPAligment(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
                case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;

                case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;

                case TextAnchor.MiddleCenter: return TextAlignmentOptions.MidlineLeft;
                case TextAnchor.MiddleLeft: return TextAlignmentOptions.Midline;
                case TextAnchor.MiddleRight: return TextAlignmentOptions.MidlineRight;

                default: return TextAlignmentOptions.Center;
            }
        }
    }
}
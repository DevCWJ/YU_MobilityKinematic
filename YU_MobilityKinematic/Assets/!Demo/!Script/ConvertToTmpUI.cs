
using UnityEngine;
using UnityEngine.UI;

namespace CWJ.YU.Mobility.Demo
{
    public class ConvertToTmpUI : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Reset()
        {
            Text text = GetComponent<Text>();
            if (text == null)
            {
                return;
            }

            var textAnchor = text.alignment;
            int fontSize = text.fontSize;
            Color txtColor = text.color;
            bool autoSize = text.resizeTextForBestFit;
            int minSize, maxSize;
            minSize = text.resizeTextMinSize; maxSize = text.resizeTextMaxSize;
            bool richText = text.supportRichText;
            string txtContent = text.text;

            DestroyImmediate(text);
            var tmp3DText = gameObject.AddComponent<TMPro.TextMeshProUGUI>();

            tmp3DText.alignment = ConvertToTmpAlign(textAnchor);
            tmp3DText.fontSize = ConvertToTmpSize(fontSize);
            tmp3DText.color = txtColor;
            if (autoSize)
            {
                tmp3DText.fontSizeMin = ConvertToTmpSize(minSize);
                tmp3DText.fontSizeMax = ConvertToTmpSize(maxSize);
            }
            tmp3DText.richText = richText;
            tmp3DText.SetText(txtContent);
            tmp3DText.enableAutoSizing = autoSize;
            tmp3DText.autoSizeTextContainer = autoSize;

            DestroyImmediate(this);
            CWJ.AccessibleEditor.EditorSetDirty.SetObjectDirty(tmp3DText);
        }
#endif
        TMPro.TextAlignmentOptions ConvertToTmpAlign(TextAnchor textAnchor)
        {
            switch (textAnchor)
            {
                case TextAnchor.UpperLeft:
                    return TMPro.TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter:
                    return TMPro.TextAlignmentOptions.Top;
                case TextAnchor.UpperRight:
                    return TMPro.TextAlignmentOptions.TopRight;
                case TextAnchor.MiddleLeft:
                    return TMPro.TextAlignmentOptions.Left;
                case TextAnchor.MiddleCenter:
                    return TMPro.TextAlignmentOptions.Center;
                case TextAnchor.MiddleRight:
                    return TMPro.TextAlignmentOptions.Right;
                case TextAnchor.LowerLeft:
                    return TMPro.TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerCenter:
                    return TMPro.TextAlignmentOptions.Bottom;
                case TextAnchor.LowerRight:
                    return TMPro.TextAlignmentOptions.BottomRight;
            }
            return TMPro.TextAlignmentOptions.Midline;
        }

        float ConvertToTmpSize(int fontSize) => fontSize;
    }
}

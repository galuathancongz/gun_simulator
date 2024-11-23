#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ScaleToOneAndKeepSize : MonoBehaviour
{
    // Tạo một mục trong Menu Unity
    [MenuItem("Luzart/Reset Parent Scale & Keep Child Size")]
    public static void ResetScaleForSelected()
    {
        if (Selection.activeTransform is RectTransform parentTransform)
        {
            // Lưu lại kích thước và tỷ lệ ban đầu của thằng cha
            Vector2 originalParentSize = parentTransform.rect.size;
            Vector3 originalParentScale = parentTransform.localScale;

            // Lưu lại kích thước và vị trí ban đầu của các phần tử con
            RectTransform[] childTransforms = parentTransform.GetComponentsInChildren<RectTransform>(true);
            Vector3[] originalChildSizeDeltas = new Vector3[childTransforms.Length];
            Vector2[] originalChildPositions = new Vector2[childTransforms.Length];
            TMP_Text [] txts = parentTransform.GetComponentsInChildren<TMP_Text>();

            for (int i = 0; i < childTransforms.Length; i++)
            {
                var child = childTransforms[i];
                if (child != null)
                {
                    originalChildSizeDeltas[i] = child.sizeDelta; // Lưu kích thước ban đầu
                    originalChildPositions[i] = child.anchoredPosition; // Lưu vị trí ban đầu
                }
            }

            // Đặt lại localScale của thằng cha về 1
            parentTransform.localScale = Vector3.one;

            // Điều chỉnh lại sizeDelta và anchoredPosition của các phần tử con
            for (int i = 0; i < childTransforms.Length; i++)
            {
                if (childTransforms[i] != null)
                {
                    var child = childTransforms[i];

                    // Điều chỉnh sizeDelta của các phần tử con
                    child.sizeDelta = new Vector2(
                        originalChildSizeDeltas[i].x * originalParentScale.x,
                        originalChildSizeDeltas[i].y * originalParentScale.y
                    );

                    // Điều chỉnh vị trí của các phần tử con (anchoredPosition)
                    child.anchoredPosition = new Vector2(
                        originalChildPositions[i].x * originalParentScale.x,
                        originalChildPositions[i].y * originalParentScale.y
                    );

                    EditorUtility.SetDirty(childTransforms[i]);
                }
            }

            for (int i = 0; i < txts.Length; i++)
            {
                float size = txts[i].fontSize;
                size = size * originalParentScale.x;
                txts[i].fontSize = size;
            }

            // Đánh dấu đối tượng đã có thay đổi để Unity lưu lại
            EditorUtility.SetDirty(parentTransform);
        }
        else
        {
            Debug.LogWarning("Please select a RectTransform in the hierarchy.");
        }
    }
}
#endif
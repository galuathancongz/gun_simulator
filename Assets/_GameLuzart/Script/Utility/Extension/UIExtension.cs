using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System.Reflection;

public static class UIExtension
{
    public static void OnClickAnim(this Button btn, UnityAction action)
    {
        if (btn == null)
        {
            return;
        }
        var effect = btn.GetComponent<EffectButton>();
        if (effect == null)
        {
            effect = btn.gameObject.AddComponent<EffectButton>();
        }
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            action?.Invoke();
        });
    }
    public static Vector2 GetRandomPositionInRect(this RectTransform targetRectTransform)
    {
        if (targetRectTransform == null)
        {
            Debug.LogError("targetRectTransform is null!");
            return Vector2.zero;
        }

        // Lấy kích thước của RectTransform
        Vector2 size = targetRectTransform.rect.size;

        // Lấy tọa độ ngẫu nhiên bên trong kích thước
        float randomX = UnityEngine.Random.Range(-size.x / 2, size.x / 2);
        float randomY = UnityEngine.Random.Range(-size.y / 2, size.y / 2);

        // Trả về vị trí ngẫu nhiên trong RectTransform (trong không gian cục bộ)
        return new Vector2(randomX, randomY);
    }
    public static void CallOnEnable(this GameObject obj)
    {
        if(obj == null)
        {
            return;
        }
        obj.SetActive(false);
        obj.SetActive(true);
    }
    public static void ChangeParentAndKeepSize(this RectTransform childRectTransform, RectTransform newParent)
    {
        // Lưu kích thước trong không gian thế giới trước khi đổi cha
        Vector2 worldSizeBefore = GetWorldSize(childRectTransform);

        // Đổi cha cho đối tượng
        childRectTransform.SetParent(newParent, false); // 'false' để không thay đổi vị trí trong không gian thế giới ngay lập tức

        // Tính tỉ lệ của cha mới trong không gian thế giới
        Vector3 newParentWorldScale = newParent.lossyScale;

        // Cập nhật lại sizeDelta dựa trên kích thước thế giới và tỉ lệ cha mới
        childRectTransform.sizeDelta = new Vector2(worldSizeBefore.x / newParentWorldScale.x, worldSizeBefore.y / newParentWorldScale.y);

        Vector2 GetWorldSize(RectTransform rectTransform)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            Vector3 worldScale = rectTransform.lossyScale;

            // Kích thước trong không gian thế giới
            return new Vector2(sizeDelta.x * worldScale.x, sizeDelta.y * worldScale.y);
        }
    }
    public static void ClaimedRectTransformScrollView(this ScrollRect scrollRect, RectTransform itemRectTransform)
    {
        Canvas.ForceUpdateCanvases();

        Vector3[] itemCorners = new Vector3[4];
        itemRectTransform.GetWorldCorners(itemCorners);
        Vector3[] viewCorners = new Vector3[4];
        scrollRect.viewport.GetWorldCorners(viewCorners);

        float difference = 0;

        if (scrollRect.horizontal)
        {
            if (itemCorners[2].x > viewCorners[2].x)
            {
                difference = itemCorners[2].x - viewCorners[2].x;
            }
            else if (itemCorners[0].x < viewCorners[0].x)
            {
                difference = itemCorners[0].x - viewCorners[0].x;
            }

            float width = viewCorners[2].x - viewCorners[0].x;
            float normalizedDifference = difference / width;

            Vector2 posCurrent = scrollRect.content.anchoredPosition;
            Vector2 size = scrollRect.content.sizeDelta;

            float newX = posCurrent.x - normalizedDifference * size.x;
            float minX = 0;
            float maxX = scrollRect.content.sizeDelta.x - scrollRect.viewport.rect.width;

            scrollRect.content.anchoredPosition = new Vector2(Mathf.Clamp(newX, minX, maxX), posCurrent.y);
        }
        else
        {
            if (itemCorners[1].y > viewCorners[1].y)
            {
                difference = itemCorners[1].y - viewCorners[1].y;
            }
            else if (itemCorners[0].y < viewCorners[0].y)
            {
                difference = itemCorners[0].y - viewCorners[0].y;
            }

            float height = viewCorners[1].y - viewCorners[0].y;
            float normalizedDifference = difference / height;

            Vector2 posCurrent = scrollRect.content.anchoredPosition;
            Vector2 size = scrollRect.content.sizeDelta;

            float newY = posCurrent.y - normalizedDifference * size.y;
            float minY = 0;
            float maxY = scrollRect.content.sizeDelta.y - scrollRect.viewport.rect.height;

            scrollRect.content.anchoredPosition = new Vector2(posCurrent.x, Mathf.Clamp(newY, minY, maxY));
        }
    }

    public static void ClaimedRectTransformScrollView(this ScrollRect scrollRect, RectTransform itemRectTransform, float elasticity)
    {
        scrollRect.elasticity = 0.5f;
        ClaimedRectTransformScrollView(scrollRect, itemRectTransform);
    }

    public static void FocusOnRectTransform(this ScrollRect scrollRect, RectTransform itemRectTransform)
    {
        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float targetPositionY = 0f;

        // Tính vị trí phần tử dựa trên offset của VerticalLayoutGroup
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            RectTransform child = scrollRect.content.GetChild(i) as RectTransform;
            if (child == itemRectTransform)
            {
                break; // Dừng lại khi đến phần tử cần cuộn tới
            }
            targetPositionY += child.rect.height;
        }

        // Điều chỉnh vị trí mục tiêu để phần tử hiển thị không sát mép trên
        float elementOffset = viewportHeight / 2 - itemRectTransform.rect.height / 2;
        targetPositionY -= elementOffset;

        // Tính giá trị cuộn
        float normalizedPosition = Mathf.Clamp01(1 - (targetPositionY / (contentHeight - viewportHeight)));

        // Cuộn đến phần tử
        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }
    public static void FocusOnRectTransformFromBottom(this ScrollRect scrollRect, RectTransform itemRectTransform)
    {
        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float targetPositionY = 0f;

        // Tính vị trí phần tử dựa trên offset của VerticalLayoutGroup, nhưng từ dưới lên trên
        for (int i = scrollRect.content.childCount - 1; i >= 0; i--)
        {
            RectTransform child = scrollRect.content.GetChild(i) as RectTransform;
            if (child == itemRectTransform)
            {
                break; // Dừng lại khi đến phần tử cần cuộn tới
            }
            targetPositionY += child.rect.height;
        }

        // Điều chỉnh vị trí mục tiêu để phần tử hiển thị không sát mép dưới
        float elementOffset = viewportHeight / 2 - itemRectTransform.rect.height / 2;
        targetPositionY -= elementOffset;

        // Tính giá trị cuộn từ dưới lên
        float normalizedPosition = Mathf.Clamp01(targetPositionY / (contentHeight - viewportHeight));

        // Cuộn đến phần tử
        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }

    public static float CalculatePositionInHorizontalScroll(this ScrollRect scrollRect, RectTransform itemRectTransform)
    {
        // Lấy kích thước và vị trí của content trong tọa độ viewport
        var contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.viewport, scrollRect.content);
        var itemBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.viewport, itemRectTransform);

        // Tính vị trí mục tiêu của phần tử trong content
        float contentWidth = contentBounds.size.x;
        float viewportWidth = scrollRect.viewport.rect.width;

        // Xác định vị trí mục tiêu
        float targetPositionX = itemBounds.center.x - contentBounds.min.x;
        float elementOffset = viewportWidth / 2 - itemRectTransform.rect.width / 2;
        targetPositionX -= elementOffset;

        // Tính toán giá trị cuộn chuẩn hóa
        float normalizedPosition = Mathf.Clamp01(targetPositionX / (contentWidth - viewportWidth));

        return normalizedPosition;
    }
    public static void SetInteractable(this Button bt, bool interactable)
    {
        var graphics = bt.GetComponentsInChildren<MaskableGraphic>();
        bt.interactable = interactable;
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].color = interactable ? bt.colors.normalColor : bt.colors.disabledColor;
        }
    }

    public static int SumRange(this IList<int> collection, int min, int max)
    {
        int num = 0;
        for (var i = min; i <= max && i < collection.Count; i++)
        {
            int obj = collection[i];
            num += obj;
        }

        return num;
    }

    public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        int num = 0;
        foreach (T obj in collection)
        {
            if (predicate(obj))
                return num;
            ++num;
        }
        return -1;
    }
}

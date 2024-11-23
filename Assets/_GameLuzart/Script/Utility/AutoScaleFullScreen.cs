using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaleFullScreen : MonoBehaviour
{
    private void Awake()
    {
        ScaleToFitScreen();
    }

    private void ScaleToFitScreen()
    {
        // Lấy đối tượng RectTransform của object hiện tại
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Lấy tỉ lệ màn hình
        float screenAspect = (float)Screen.width / (float)Screen.height;

        // Kiểm tra xem màn hình là dọc hay ngang
        if (screenAspect < 1) // màn hình ngang
        {
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.width / screenAspect);
        }
        else // màn hình dọc
        {
            rectTransform.sizeDelta = new Vector2(Screen.height * screenAspect, Screen.height);
        }

        // Đặt kích thước của RectTransform thành match với kích thước màn hình
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = Vector2.zero;
    }
}

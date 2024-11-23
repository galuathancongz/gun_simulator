using System;
using UnityEngine;

public class ProgressBarUISize : ProgressBarUI
{
    [SerializeField]
    private RectTransform rtContain;
    private RectTransform rtFill
    {
        get
        {
            return imFill.rectTransform;
        }
    }
    public float width;
    public float height;
    public bool isGetWidth = false;
    private void Awake()
    {
        width = rtContain.sizeDelta.x;
        height = rtFill.sizeDelta.y;
    }
    public override void SetSlider(float prePercent, float targetPercent, float time, Action onDone, Action<float> actionUpdate = null)
    {
        if (isGetWidth)
        {
            width = rtContain.sizeDelta.x;
            height = rtFill.sizeDelta.y;
        }
        prePercent = Mathf.Clamp01(prePercent);
        targetPercent = Mathf.Clamp01(targetPercent);
        float preWidth = width * prePercent;
        float targetWidth = targetPercent * width;
        if (prePercent == targetPercent || time <= 0)
        {
            rtFill.sizeDelta = new Vector2(targetWidth, rtFill.sizeDelta.y);
            onDone?.Invoke();
            return;
        }
        GameUtil.Instance.StartLerpValue(this, preWidth, targetWidth, time, (x) =>
        {
            rtFill.sizeDelta = new Vector2(x, rtFill.sizeDelta.y);
            actionUpdate?.Invoke(x);
        }, onDone);
    }
}
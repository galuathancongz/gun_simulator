using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    public Image imFill;
    public virtual void SetSlider(float prePercent, float targetPercent, float time, Action onDone, Action<float> actionUpdate = null)
    {
        prePercent = Mathf.Clamp01(prePercent);
        targetPercent = Mathf.Clamp01(targetPercent);
        if (prePercent == targetPercent || time <= 0)
        {
            imFill.fillAmount = targetPercent;
            onDone?.Invoke();
            return;
        }
        GameUtil.Instance.StartLerpValue(this, prePercent, targetPercent, time, (x) =>
        {
            imFill.fillAmount = x;
            actionUpdate?.Invoke(x);
        }, onDone);
    }
}

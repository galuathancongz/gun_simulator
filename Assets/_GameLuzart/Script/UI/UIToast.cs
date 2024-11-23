#if DOTWEEN
using DG.Tweening;
#endif
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIToast : UIBase
{
    public CanvasGroup canvasGroup;
    public TMP_Text txtNoti;
#if DOTWEEN
    private Sequence sq;
#endif
    public void Init(string str)
    {
        txtNoti.text = str;
#if DOTWEEN
        sq?.Kill();
        sq = DOTween.Sequence();
        sq.AppendInterval(1f);
        sq.Append(DOVirtual.Float(1, 0, 0.5f, (x) =>
        {
            canvasGroup.alpha = x;
        }));
        sq.AppendCallback(Hide);
#else
        Hide();
#endif

    }
}
public static class KeyToast
{
    public const string NoInternetLoadAds = "No Internet to Load Ads. \n Try Again";
}

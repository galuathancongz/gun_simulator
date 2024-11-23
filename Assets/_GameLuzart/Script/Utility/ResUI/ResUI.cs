using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResUI : MonoBehaviour
{
    public Image imIcon;
    public Image imBg;
    public TMP_Text txt;
    public string preStr = "";
    public string endStr = "";

    public void InitData(DataResource dataRes)
    {
        if (imIcon != null)
        {
            imIcon.sprite = ResourceManager.Instance.spriteResourcesSO.GetSpriteIcon(dataRes);
        }
        if (imBg != null)
        {
            imBg.sprite = ResourceManager.Instance.spriteResourcesSO.GetSpriteIcon(dataRes);
        }
        if (txt != null)
        {
            txt.text = $"{preStr}{dataRes.amount}{endStr}";
        }

    }
}

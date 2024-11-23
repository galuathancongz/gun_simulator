using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinObserver : MonoBehaviour
{
    public Image imCoin;
    public TMP_Text txtGold;
    public Button btnAddCoin;
    private Action actionAddCoin;
    private bool IsAutoSetText = false;

    private void Awake()
    {
        GameUtil.ButtonOnClick(btnAddCoin, ClickAddCoin, true);
    }
    private void OnEnable()
    {
        Observer.Instance.AddObserver(ObserverKey.CoinObserverNormal, SetText);
        Observer.Instance.AddObserver(ObserverKey.CoinObserverTextRun, SetTextTimeRun);
        Observer.Instance.AddObserver(ObserverKey.CoinObserverDontAuto, AutoSetText);
        SetText(null);
    }
    private void OnDisable()
    {
        if (Observer.Instance == null)
        {
            return;
        }
        Observer.Instance.RemoveObserver(ObserverKey.CoinObserverNormal, SetText);
        Observer.Instance.RemoveObserver(ObserverKey.CoinObserverTextRun, SetTextTimeRun);
        Observer.Instance.RemoveObserver(ObserverKey.CoinObserverDontAuto, AutoSetText);
    }
    public void InitAddCoin(Action onAddCoin)
    {
        actionAddCoin = onAddCoin;
    }
    private void ClickAddCoin()
    {
        actionAddCoin?.Invoke();
    }
    private void AutoSetText(object data)
    {
        if (data == null)
        {
            return;
        }
        bool isTrue = (bool)data;
        IsAutoSetText = isTrue;
        if (!isTrue)
        {
            SetText(null);
        }
    }
    private void SetText(object data)
    {
        if (IsAutoSetText)
        {
            return;
        }
        txtGold.text = $"{DataManager.Instance.Gold}";
    }
    private void SetTextTimeRun(object data)
    {
        if (data == null)
        {
            return;
        }
        SetTextTimeRun dataRun = (SetTextTimeRun)data;
        GameUtil.Instance.StartLerpValue(this, dataRun.preGold, dataRun.targetGold, dataRun.timeRun, (x) =>
        {
            txtGold.text = $"{x}";
        },
        () => AutoSetText(false)
        );
    }
}
public struct SetTextTimeRun
{
    public int preGold;
    public int targetGold;
    public int timeRun;
}

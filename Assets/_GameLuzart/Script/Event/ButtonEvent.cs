using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour
{
    public EEventName eEventName;
    public Button btn;
    private void Start()
    {
        GameUtil.ButtonOnClick(btn, ClickBtnEvent, true);
    }

    public Action actionClick;
    public bool IsActiveEvent
    {
        get
        {
            DB_Event db = EventManager.Instance.GetEvent(eEventName);
            return db != null && db.eventStatus != EEventStatus.Finish;
        }

    }
    public virtual void InitEvent(Action action)
    {
        this.actionClick = action;
        if(IsActiveEvent)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void ClickBtnEvent()
    {
        actionClick?.Invoke();
    }
}

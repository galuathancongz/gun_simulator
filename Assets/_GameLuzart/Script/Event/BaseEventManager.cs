using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventManager : MonoBehaviour
{
    public abstract EEventName eEvent { get; }
    public abstract TimeEvent GetTimeEvent { get;}

    public abstract void LoadData();
    public abstract void SaveData();

}

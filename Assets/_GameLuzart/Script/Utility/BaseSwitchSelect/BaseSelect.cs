using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSelect : MonoBehaviour
{
    public virtual void Select(bool isSelect) { }
    public virtual void Select(int index)
    {

    }
}
public interface ISelect
{
    public abstract void Select();
    public abstract void UnSelect();
}
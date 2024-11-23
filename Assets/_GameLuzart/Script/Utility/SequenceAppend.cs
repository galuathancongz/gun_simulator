using UnityEngine;
using UnityEngine.Events;
#if DOTWEEN
using DG.Tweening;
#endif

public class SequenceAppend : MonoBehaviour
{
#if DOTWEEN
    public DB_ActionTween[] listAction;
    public Sequence sequence;
    private void OnEnable()
    {
        AddTweenFrom();
    }
    private void AddTweenFrom(int value = 0)
    {
        if (listAction == null || listAction.Length == 0) return;
        ResetTween();
        sequence = DOTween.Sequence();
        for (int i = value; i < listAction.Length; i++)
        {
            var idx = i;
            var a = listAction[idx];
            Tween tw = DOVirtual.DelayedCall(a.delay, () =>
            {
                a.isCallBack = true;
                a.action.Invoke();
            });
            sequence.Append(tw);
        }
    }
    private void OnDisable()
    {
        if (listAction == null || listAction.Length == 0) return;
        ResetTween();
    }
    private void ResetTween()
    {
        if (sequence != null)
        {
            sequence.Kill();
        }
    }
    public void NextStepDelay()
    {
        for (int i = 0; i < listAction.Length; i++)
        {
            if (!listAction[i].isCallBack)
            {
                var a = listAction[i];
                a.isCallBack = true;
                a.action.Invoke();
                ResetTween();
                AddTweenFrom(i + 1);
                break;
            }
        }
    }
#endif
}
[System.Serializable]
public class DB_ActionTween
{
#if UNITY_EDITOR
    public string nameAction;
#endif
    public UnityEvent action;
    public float delay;
    public bool isCallBack { get; set; }
}

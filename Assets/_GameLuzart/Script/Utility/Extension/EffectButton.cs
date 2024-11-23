using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool isAutoButton = false;
    private Button btn;

    private Vector3 m_localScale = Vector3.one;
    public float valueScale = 1.1f;
    public float timeScale = 0.1f;
    private void Awake()
    {
        if (!isAutoButton)
        {
            btn = GetComponent<Button>();
        }
    }
    private Coroutine corIEScale = null;
    private IEnumerator IEScale()
    {
        float time = 0;
        float initialScale = transform.localScale.x;
        float targetScale = m_localScale.x * valueScale;
        WaitForSecondsRealtime waitRealTime = new WaitForSecondsRealtime(0);
        while (time < timeScale)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(initialScale, targetScale, time / timeScale);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return waitRealTime;
        }
        transform.localScale = m_localScale * valueScale;
    }

    private Coroutine corIEDeScale = null;
    private IEnumerator IEDeScale()
    {
        float time = 0;
        float initialScale = transform.localScale.x;
        WaitForSecondsRealtime waitRealTime = new WaitForSecondsRealtime(0);
        while (time < timeScale)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(initialScale, m_localScale.x, time / timeScale);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return waitRealTime;
        }
        transform.localScale = m_localScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!btn.interactable)
        {
            return;
        }
        if (corIEScale != null)
        {
            StopCoroutine(corIEScale);
        }
        if (corIEDeScale != null)
        {
            StopCoroutine(corIEDeScale);
            transform.localScale = m_localScale;
        }
        corIEScale = StartCoroutine(IEScale());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!btn.interactable)
        {
            return;
        }
        if (corIEDeScale != null)
        {
            StopCoroutine(corIEDeScale);
        }
        if (corIEScale != null)
        {
            StopCoroutine(corIEScale);
        }
        corIEDeScale = StartCoroutine(IEDeScale());

    }
    private void OnDisable()
    {
        if (corIEDeScale != null)
        {
            StopCoroutine(corIEDeScale);
        }
        if (corIEScale != null)
        {
            StopCoroutine(corIEScale);
        }
        transform.localScale = m_localScale;
    }
}

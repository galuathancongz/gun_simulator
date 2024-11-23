using System;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN
using DG.Tweening;
#endif

public class CoinsSpawn : MonoBehaviour
{

    public RectTransform coinPf;
    public Transform target;

    public int coinAmount = 18;
    //private int _coinAmount;

    public void SpawnCoins(Action onFirstTimeMoveTo = null, Action onCompleteMoveTo = null)
    {
        int _coinAmount = coinAmount;
        bool isShowPtc = false;
        isShowPtc = false;
#if DOTWEEN
        //AudioManager.Instance.PlaySFXRewardCoin();
        for (int i = 0; i < _coinAmount; i++)
        {
            DOVirtual.DelayedCall(UnityEngine.Random.Range(0, .4f), () => CallBack(onFirstTimeMoveTo, onCompleteMoveTo));
        }

        void CallBack(Action onFirstTimeMoveTo, Action onCompleteMoveTo)
        {
            Sequence sequence = DOTween.Sequence();
            var coin = Instantiate<RectTransform>(coinPf, transform);
            coin.gameObject.SetActive(true);
            coin.transform.localScale = new Vector2(0f, 0f);
            sequence.Append(coin.transform.DOScale(UnityEngine.Random.Range(1.05f, 1.3f), UnityEngine.Random.Range(.1f, .2f)));
            sequence.Append(coin.transform.DOScale(UnityEngine.Random.Range(.6f, .9f), UnityEngine.Random.Range(.1f, .2f)));
            sequence.Append(coin.transform.DOScale(1f, UnityEngine.Random.Range(.1f, .2f)));
            sequence.AppendInterval(UnityEngine.Random.Range(.1f, .3f));
            sequence.Append(coin.DOMove(target.position, UnityEngine.Random.Range(.4f, .6f)).SetEase(Ease.InBack).OnComplete(() =>
            {
                _coinAmount--;
                if (_coinAmount == 0)
                {
                    onCompleteMoveTo?.Invoke();
                }
                if (!isShowPtc)
                {
                    onFirstTimeMoveTo?.Invoke();
                    isShowPtc = true;
                }
                AudioManager.Instance.PlaySFXCoin();
                Destroy(coin.gameObject);
            }));
            sequence.Insert(0, coin.DOAnchorPos(new Vector2(UnityEngine.Random.Range(-110, 110), UnityEngine.Random.Range(-110, 110)), UnityEngine.Random.Range(.1f, .3f)));
            sequence.AppendCallback(() => Observer.Instance.Notify(ObserverKey.CoinObserverDontAuto, false));
            sequence.AppendCallback(() => Observer.Instance.Notify(ObserverKey.CoinObserverNormal));
            sequence.SetTarget(this);

        }
#else
        onFirstTimeMoveTo?.Invoke();
        onCompleteMoveTo?.Invoke();
#endif
    }
    private void OnDestroy()
    {
#if DOTWEEN
        this.DOKill();
#endif
    }
}

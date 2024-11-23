using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UICoinSpawn : UIBase
{
    public CoinsSpawn coinSpawn;
    public void InitCoinSpawn(Action onFirstTimeMoveTo = null, Action onCompleteMoveTo = null, Transform target = null)
    {
        if(target == null)
        {
            var coinObserver = FindObjectOfType<CoinObserver>();
            if(coinObserver != null)
            {
                target = coinObserver.imCoin.transform;
            }
            if(target != null)
            {
                coinSpawn.target = target;  
            }
        }
        Action onDoneAll = () =>
        {
            onCompleteMoveTo?.Invoke();
            Hide();
        };
        coinSpawn.SpawnCoins(onFirstTimeMoveTo, onDoneAll);
    }
}

//using BG_Library.NET;
using System;

public class AdsWrapperManager 
{
    public static void ShowReward(string where, Action onDone, Action onFail)
    {
        GameUtil.Log(where);
        //if (GameManager.IS_REMOVE_ADS_REWARD)
        {
            onDone?.Invoke();
            return;
        }
        //if (!AdsManager.IsRewardedReady())
        {
            onFail?.Invoke();
            return;
        }
        where = $"{where}_level_{DataManager.Instance.CurrentLevel}";
        //AdsManager.ShowRewardVideo(where, onDone);
    }
    public static void ShowInter(string where, Action onDone)
    {
        if (GameManager.IS_REMOVE_ADS_INTER)
        {
            onDone?.Invoke();
            return;
        }
        //if (DataManager.Instance.CurrentLevel >= GameCustom.Ins.RemoteConfigCustom.LevelShowAds)
        //{
        //    GameUtil.Log(where);
        //    AdsManager.ShowInterstitial(where, onDone);
        //}
        else
        {
            onDone?.Invoke();
        }

    }
}
public static class KeyAds
{
    // Reward
    public const string AdsGetBooster0 = "ads_get_booster_0";
    public const string AdsGetBooster1 = "ads_get_booster_1";
    public const string AdsGetBooster2 = "ads_get_booster_2";

    public const string AdsUIAddCoin = "ads_ui_add_coin";
    public const string AdsUIAddHeart = "ads_ui_add_heart";
    public const string AdsUITimeOut = "ads_ui_time_out";

    public const string AdsClaimStarterPack = "ads_claim_starterpack";
    public const string AdsRewardCoin = "reward_adsCoin";


    // Inter
    public const string AdsWinLevel = "ads_win_level";

}
public enum PositionAds
{
    continue_win_1 = 0,
    continue_win_2 = 1,
    QuitHome = 1,


}
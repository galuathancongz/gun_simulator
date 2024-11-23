using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardManager : MonoBehaviour
{
    private const string PATH_DAILY_REWARD = "daily_reward";
    public GroupDataResources[] dataResourceDaily = new GroupDataResources[7];
    public DBProcessReward[] dataResourceProcess = new DBProcessReward[4];
    public int Today;
    public int MaxDay
    {
        get
        {
            int length = dataResourceProcess.Length;
            return dataResourceProcess[length - 1].day;
        }
    }
    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.OnNewDay, OnNewDay);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.OnNewDay, OnNewDay);
    }
    public DataDailyReward dataDailyReward;
    private void OnNewDay(object data)
    {
        int lastTimeLogin = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        dataDailyReward.lastTimeLogin = lastTimeLogin;
        dataDailyReward.count++;
        SaveData();
        Today = GetRewardNewDay();
    }
    public void InitData()
    {
        GetData();
        if (!dataDailyReward.isFirstTimeLogin)
        {
            int lastTimeLogin = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            dataDailyReward.firstTimeLogin = lastTimeLogin;
            dataDailyReward.isFirstTimeLogin = true;
            SaveData();
        }
        Today = GetRewardNewDay();
    }
    private void GetData()
    {
        dataDailyReward = SaveLoadUtil.LoadDataPrefs<DataDailyReward>(PATH_DAILY_REWARD);
        if(dataDailyReward == null)
        {
            dataDailyReward =new DataDailyReward();
        }
    }
    private void SaveData()
    {
        SaveLoadUtil.SaveDataPrefs<DataDailyReward>(dataDailyReward, PATH_DAILY_REWARD);
    }
    public int GetTimeFromFirstTimeLogin()
    {
        DateTimeOffset dtNow = DateTime.UtcNow;
        DateTimeOffset dtFist = DateTimeOffset.FromUnixTimeSeconds((long)dataDailyReward.firstTimeLogin);
        return dtFist.Day - dtNow.Day;
    }
    public int GetTimeFromLastTimeLogin()
    {
        DateTimeOffset dtNow = DateTime.UtcNow;
        DateTimeOffset dtFist = DateTimeOffset.FromUnixTimeSeconds((long)dataDailyReward.lastTimeLogin);
        return dtFist.Day - dtNow.Day;
    }
    public bool IsClaimDay(int day)
    {
        return dataDailyReward.isClaim[day];
    }
    public void ClaimReward(int day = -1)
    {
        if(day == -1)
        {
            day = GetTimeFromFirstTimeLogin();
        }
        if (day > dataDailyReward.isClaim.Length - 1 )
        {
            GameUtil.LogError("DailyReward False");
        }
        dataDailyReward.isClaim[day] = true;
        SaveData();
    }
    public bool IsClaimProcess(int index)
    {
        return dataDailyReward.process.isClaim[index];
    }
    public void ClaimProcess(int index)
    {
        dataDailyReward.process.isClaim[index] = true;
        SaveData();
    }

    public int GetRewardNewDay()
    {
        return dataDailyReward.count - 1;
    }
    public int ReturnIndexDayProcess(int day)
    {
        int length = dataResourceProcess.Length;
        for (int i = 0; i < length; i++)
        {
            int index = i;
            var process = dataResourceProcess[index];
            if(index == length -1 && day == process.day)
            {
                return index;
            }
            if(day < process.day)
            {
                return index -1;
            }
        }
        return -1;
    }
    public float ReturnProcessPercent(int day)
    {

        float percent = (float)(day+1) / (float)(MaxDay+1);
        return percent;
    }

    public bool IsFullProces()
    {
        int length = dataDailyReward.process.isClaim.Length;
        for (int i = 0; i < length; i++)
        {
            if (!dataDailyReward.process.isClaim[i])
            {
                return false;
            }
        }
        for (int i = 0; i < dataDailyReward.isClaim.Length; i++)
        {
            if (!dataDailyReward.isClaim[i])
            {
                return false;
            }
        }
        return true;
    }
}
[System.Serializable]
public class DataDailyReward
{
    public int firstTimeLogin = 0;
    public int lastTimeLogin = 0;
    public bool isFirstTimeLogin = false;
    public bool[] isClaim = new bool[7];
    public int count = 0;
    public DataProcessReward process = new DataProcessReward();
}
[System.Serializable]
public class DataProcessReward
{
    public bool[] isClaim = new bool[4];
}
[System.Serializable]
public class DBProcessReward
{
    public int day;
    public GroupDataResources groupDataResources;
}

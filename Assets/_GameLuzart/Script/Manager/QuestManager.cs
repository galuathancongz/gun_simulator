using System.Collections.Generic;

public class QuestManager : Singleton<QuestManager>
{
    public DB_DailyQuestSO dbDailyQuestSO;
    private const string PATH_DAILY_QUEST = "daily_quest";
    private DataDailyQuestJson dataDailyQuestJson;
    public List<DataDailyQuest> dailyQuest = new List<DataDailyQuest>();
    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.QuestKey, AddQuestKey);
        Observer.Instance.AddObserver(ObserverKey.OnNewDay, OnNewDay);
        InitData();
    }
    private void InitData()
    {
        dataDailyQuestJson = SaveLoadUtil.LoadDataPrefs<DataDailyQuestJson>(PATH_DAILY_QUEST);
        if (dataDailyQuestJson == null)
        {
            dataDailyQuestJson = new DataDailyQuestJson();
            dataDailyQuestJson.data = new List<DataDailyQuest>();
            dailyQuest = new List<DataDailyQuest>();
        }
        else
        {
            dailyQuest = dataDailyQuestJson.data;
        }
    }
    private void SaveData()
    {
        dataDailyQuestJson = new DataDailyQuestJson();
        dataDailyQuestJson.data = dailyQuest;
        SaveLoadUtil.SaveDataPrefs<DataDailyQuestJson>(dataDailyQuestJson, PATH_DAILY_QUEST);
    }
    private void OnNewDay(object data)
    {
        dailyQuest = new List<DataDailyQuest>();
        SaveData();
    }
    private void AddQuestKey(object oj)
    {
        if (oj == null)
        {
            return;
        }
        DataDailyQuest data = (DataDailyQuest)oj;
        List<DataDailyQuest> list = new List<DataDailyQuest> ();
        DataDailyQuest dataCur = new DataDailyQuest();
        bool isHasData = false;
        if (dailyQuest != null && dailyQuest.Count > 0)
        {
            list = dailyQuest;
            for (int i = 0; i < dailyQuest.Count; i++)
            {
                if (dailyQuest[i].idQuest == data.idQuest)
                {
                    dataCur = dailyQuest[i];
                    isHasData = true;
                }
            }
        }
        dataCur.idQuest = data.idQuest;
        dataCur.curCount = dataCur.curCount + data.curCount;
        if (!isHasData)
        {
            list.Add (dataCur);
        }
        dailyQuest = list;
        DataManager.Instance.SaveGameData();
    }
    public int GetQuestCurent(int idQuest)
    {
        var data = GetDataDailyQuestCurrent(idQuest);
        if(data == null)
        {
            return 0;
        }
        else
        {
            return data.curCount;
        }
    }
    public DataDailyQuest GetDataDailyQuestCurrent(int idQuest)
    {
        if (dailyQuest != null && dailyQuest.Count > 0)
        {
            for (int i = 0; i < dailyQuest.Count; i++)
            {
                if (dailyQuest[i].idQuest == idQuest)
                {
                    return dailyQuest[i];
                }
            }
        }
        return null;
    }
    public DB_DailyQuest GetDB_DailyQuest(int idQuest)
    {
        var data = dbDailyQuestSO.dbDailyQuest;
        int length  = data.Length;
        for (int i = 0; i < length; i++)
        {
            if (dailyQuest[i].idQuest == idQuest)
            {
                return data[i];
            }
        }
        return null;
    }
    public bool IsCompleteQuest(int idQuest)
    {
        var data = GetDataDailyQuestCurrent(idQuest);
        if (data == null)
        {
            return false;
        }
        else
        {
            var db = GetDB_DailyQuest(idQuest);
            if(data.curCount >= db.totalCount)
            {
                return true;
            }
            return false;
        }
    }
    public bool IsClaimed(int idQuest)
    {
        var data = GetDataDailyQuestCurrent(idQuest);
        if (data == null)
        {
            return false;
        }
        else
        {
            return data.isComplete;
        }
    }
    public void ClaimQuest(int idQuest)
    {
        var data = GetDataDailyQuestCurrent(idQuest);
        if (data != null)
        {
            data.isComplete = true;
        }
        SaveData();
    }
    public override void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.QuestKey, AddQuestKey);
        Observer.Instance.RemoveObserver(ObserverKey.OnNewDay, OnNewDay);
    }
}
[System.Serializable]
public class DB_DailyQuest
{
    public int idQuest;
    public int totalCount;
    public string title;
    public GroupDataResources groupDataResources;
}
[System.Serializable]
public class DataDailyQuest
{
    public DataDailyQuest()
    {

    }
    public DataDailyQuest(int idQuest, int curCount = 1, bool isComplete = true)
    {
        this.idQuest = idQuest;
        this.curCount = curCount;
        this.isComplete = isComplete;
    }
    public int idQuest;
    public int curCount;
    public bool isComplete;
}
[System.Serializable]
public class DataDailyQuestJson
{
    public List<DataDailyQuest> data;
}

using System.Collections.Generic;
using UnityEngine;

public class GameRes
{
    private static string playerResourcesKey = "PlayerResources";
    private static PlayerResources cachedPlayerResources = null;

    public static bool isAddRes(DataResource resource)
    {
        PlayerResources playerResources = GetCachedPlayerResources();
        int amountCurrent = playerResources.GetResourceAmount(resource.type);
        return amountCurrent + resource.amount >= 0;
    }

    public static int GetRes(DataTypeResource dataTypeResource)
    {
        PlayerResources playerResources = GetCachedPlayerResources();
        return playerResources.GetResourceAmount(dataTypeResource);
    }

    public static void AddRes(DataTypeResource dataTypeResource, int amount)
    {
        PlayerResources playerResources = GetCachedPlayerResources();
        playerResources.AddResource(new DataResource(dataTypeResource, amount));
        SavePlayerResources(playerResources);
        GameUtil.Log($"To Add RES {dataTypeResource.type}_{dataTypeResource.id} _ currentvalue {amount}");

        if (dataTypeResource.type == RES_type.Gold)
        {
            Observer.Instance.Notify(ObserverKey.CoinObserverNormal);
        }
    }

    public static void SavePlayerResources()
    {
        SavePlayerResources(cachedPlayerResources);
    }

    public static void SavePlayerResources(PlayerResources playerResources)
    {
        string json = JsonUtility.ToJson(playerResources);
        PlayerPrefs.SetString(playerResourcesKey, json);
        PlayerPrefs.Save();
        cachedPlayerResources = playerResources; // Cập nhật bộ nhớ cache
    }

    public static PlayerResources GetCachedPlayerResources()
    {
        if (cachedPlayerResources == null)
        {
            cachedPlayerResources = LoadPlayerResources();
        }
        return cachedPlayerResources;
    }

    private static PlayerResources LoadPlayerResources()
    {
        if (PlayerPrefs.HasKey(playerResourcesKey))
        {
            string json = PlayerPrefs.GetString(playerResourcesKey);
            return JsonUtility.FromJson<PlayerResources>(json);
        }
        else
        {
            return new PlayerResources();
        }
    }
}
[System.Serializable]
public class PlayerResources
{
    public List<DataResource> resources;

    public PlayerResources()
    {
        resources = new List<DataResource>();
    }

    public void AddResource(DataResource resource)
    {
        DataResource existingResource = resources.Find(r => r.type.Compare(resource.type));
        if (existingResource != null)
        {
            existingResource.amount += resource.amount;
        }
        else
        {
            resources.Add(resource);
        }
    }

    public int GetResourceAmount(DataTypeResource dataTypeResource)
    {
        DataResource resource = resources.Find(r => r.type.Compare(dataTypeResource));
        return resource != null ? resource.amount : 0;
    }
}

[System.Serializable]
public class DataResource
{
    public DataResource()
    {

    }
    public DataResource(DataTypeResource type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
    public DataTypeResource type;
    public int amount;
    [System.NonSerialized]
    public int idIcon = 0;
    [System.NonSerialized]
    public Sprite spIcon;

    //[JsonIgnore]
    //public int idBg = 0;
    //[JsonIgnore]
    //[JsonIgnore]
    //public Sprite spBg;
    public DataResource Clone()
    {
        return new DataResource(this.type, this.amount);
    }
}
[System.Serializable]
public struct DataTypeResource
{
    public DataTypeResource(RES_type type, int id = 0)
    {
        this.type = type;
        this.id = id;
    }
    public RES_type type;
    public int id;
    public bool Compare(DataTypeResource dataOther)
    {
        if (type == dataOther.type && id == dataOther.id)
        {
            return true;
        }
        return false;
    }
    public string GetKeyString(RES_type _type, int _id)
    {
        return $"{_type}_{_id}";
    }
    public override string ToString()
    {
        return $"{type}_{id}";
    }
    // Định nghĩa toán tử == để so sánh các instance của struct
    public static bool operator ==(DataTypeResource left, DataTypeResource right)
    {
        return left.type == right.type && left.id == right.id;
    }
    public static bool operator !=(DataTypeResource left, DataTypeResource right)
    {
        return left.type != right.type || left.id != right.id;
    }


    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
public enum RES_type
{
    None = 0,
    Gold = 1,
    Heart = 2,
    HeartTime = 3,
    Booster = 4, // 0 : Free Time, 1: Jump booster , 2: Car
    Chest = 5, // 0: None, start for 1, 2 , 3,4
    NoAds = 6,
    TimeAdd = 7,
}

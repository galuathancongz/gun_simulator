using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public GameData GameData => _gameData;
    public GameData _gameData;
    private bool IsInit { get; set; } = false;

    private const string KEY_GAME_DATA = "key_gamedata";
    public void Initialize()
    {
        LoadGameData();
    }
    private void LoadGameData()
    {
        _gameData = SaveLoadUtil.LoadDataPrefs<GameData>(KEY_GAME_DATA);
        if(_gameData == null)
        {
            _gameData = new GameData();
        }
        IsInit = true;
    }
    public void SaveGameData()
    {
        if (!IsInit)
        {
            return;
        }
        SaveLoadUtil.SaveDataPrefs<GameData>(_gameData, KEY_GAME_DATA);
    }

    public bool IsAddRes(DataResource dataResource)
    {
        return GameRes.isAddRes(dataResource);
    }
    public static int GetRes(DataTypeResource type)
    {
        return GameRes.GetRes(type);
    }
    public static void AddRes(DataTypeResource type, int amount)
    {
        GameRes.AddRes(type, amount);
    }
    public void ReceiveRes(params DataResource[] dataResource)
    {
        int length = dataResource.Length;
        for (int i = 0; i < length; i++)
        {
            DataResource resource = dataResource[i];
            AddRes(resource.type, resource.amount);
        }
    }
    public int Gold
    {
        get
        {
            DataTypeResource dataTypeResource = new DataTypeResource();
            dataTypeResource.type = RES_type.Gold;
            dataTypeResource.id = 0;
            return DataManager.GetRes(dataTypeResource);
        }

    }
    public int CurrentLevel => _gameData.level;
}
[System.Serializable]
public class GameData
{
    public int level = 0;
}

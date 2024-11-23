using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUtil : Singleton<GameUtil>
{
    public static void ButtonOnClick(Button bt, UnityAction action, bool isAnim = false, string whereAds = null)
    {
        UnityAction _action = null;
        if (String.IsNullOrEmpty(whereAds))
        {
            _action = () =>
            {
                action?.Invoke();
            };
        }
        else
        {
            _action = () =>
            {
                Action onDone = () =>
                {
                    action?.Invoke();
                };
            };
        }
        if (isAnim)
        {
            bt.OnClickAnim(_action);
        }
        else
        {
            bt.onClick.RemoveAllListeners();
            bt.onClick.AddListener(_action);
        }

    }

    public static string LongTimeSecondToUnixTime(long unixTimeSeconds, bool isDoubleParam = false, string day = "d", string hour = "h", string minutes = "m", string second = "s")
    {
        TimeSpan dateTime = TimeSpan.FromSeconds(unixTimeSeconds);
        return TimeSpanToUnixTime(dateTime, isDoubleParam, day, hour, minutes, second);
    }
    public static string FloatTimeSecondToUnixTime(float unixTimeSeconds, bool isDoubleParam = false, string day = "d", string hour = "h", string minutes = "m", string second = "s")
    {
        TimeSpan dateTime = TimeSpan.FromSeconds(unixTimeSeconds);
        string strValue = TimeSpanToUnixTime(dateTime, isDoubleParam, day, hour, minutes, second);
        int milliseconds = dateTime.Milliseconds;
        if (milliseconds > 0)
        {
            strValue += $".{milliseconds:D3}";
        }
        return strValue;

    }
    public static string TimeSpanToUnixTime(TimeSpan dateTime, bool isDoubleParam = false, string day = "d", string hour = "h", string minutes = "m", string second = "s")
    {
        string strValue = "";
        if (dateTime.Days > 0)
        {
            if (dateTime.Hours == 0 && !isDoubleParam)
            {
                strValue = $"{dateTime.Days:D2}{day}";
            }
            else
            {
                strValue = $"{dateTime.Days:D2}{day}:{dateTime.Hours:D2}{hour}";
            }
        }

        else if (dateTime.Hours > 0)
        {
            if (dateTime.Minutes == 0 && !isDoubleParam)
            {
                strValue = $"{dateTime.Hours:D2}{hour}";
            }
            else
            {
                strValue = $"{dateTime.Hours:D2}{hour}:{dateTime.Minutes:D2}{minutes}";
            }
        }
        else
        {
            if (dateTime.Seconds == 0 && !isDoubleParam)
            {
                strValue = $"{dateTime.Minutes:D2}{minutes}";
            }
            else
            {
                strValue = $"{dateTime.Minutes:D2}{minutes}:{dateTime.Seconds:D2}{second}";
            }
        }
        return strValue;
    }
    private const string ColorRed = "#FF0000";
    private const string ColorGreen = "#00FF06";
    private const string ColorBlack = "#000000";
    private const string ColorWhite = "#FFFFFF";
    private const string ColorBlueLight = "#00FFF0";
    private const string ColorBlueDark = "#000FFF";
    private const string ColorYellow = "#FFFF00";

    public static void Log(object debug)
    {
#if DEBUG_DA
        Debug.Log($"<color={ColorBlueLight}>DEBUG_DA: {debug}</color>");
#endif
    }
    public static void LogError(object debug)
    {
#if DEBUG_DA
        Debug.LogError($"<color={ColorBlueLight}>DEBUG_DA: {debug}</color>");
#endif
    }
    [SerializeField]
    private bool isActionPerSecond = false;
    [SerializeField]
    private bool isOnNewDay = false;
    private void Start()
    {
        if (isActionPerSecond)
        {
            StartCount();
        }
        if (isOnNewDay)
        {
            StartCoroutine(IEWaitEndFrameNewDay());
        }
    }
    private void StartCount()
    {
        StartCoroutine(IECountTime());
    }
    private int timePing = 0;
    private int dayCache = int.MinValue;
    private IEnumerator IECountTime()
    {
        dayCache = DateTimeOffset.FromUnixTimeSeconds(TimeUtils.GetLongTimeCurrent).Day;
        WaitForSeconds wait = new WaitForSeconds(1);
        while (true)
        {
            long timeCb = TimeUtils.GetLongTimeCurrent;
            Observer.Instance.Notify(ObserverKey.TimeActionPerSecond, timeCb);
            timePing++;
            if (timePing >= 5)
            {
                StartDayCount(timeCb);
                timePing = 0;
            }
            yield return wait;
        }
    }
    private void StartDayCount(long timeCurrent)
    {
        int dayCacheLocal = DateTimeOffset.FromUnixTimeSeconds(TimeUtils.GetLongTimeCurrent).Day;
        if (dayCacheLocal != dayCache)
        {
            DataManager.Instance.SaveGameData();
            EventManager.Instance.SaveAllData();
            EventManager.Instance.Initialize();
            UIManager.Instance.RefreshUI();
            if (isOnNewDay)
            {
                StartCoroutine(IEWaitEndFrameNewDay());
            }
        }
        dayCache = dayCacheLocal;
    }
    private Dictionary<MonoBehaviour, List<Coroutine>> coroutineDictionary = new Dictionary<MonoBehaviour, List<Coroutine>>();

    public void WaitAndDo(float time, Action action)
    {
        StartCoroutine(ieWaitAndDo(time, action));
    }

    public void WaitAndDo(MonoBehaviour behaviour, float time, Action action)
    {
        if (!coroutineDictionary.TryGetValue(behaviour, out var coroutines))
        {
            coroutines = new List<Coroutine>();
            coroutineDictionary[behaviour] = coroutines;
        }

        // Dừng tất cả các coroutine hiện tại của MonoBehaviour (nếu cần thiết)
        StopAllCoroutinesForBehaviour(behaviour);

        Coroutine coroutine = behaviour.StartCoroutine(ieWaitAndDo(time, action));
        coroutines.Add(coroutine);
    }

    private IEnumerator ieWaitAndDo(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }

    public void StopAllCoroutinesForBehaviour(MonoBehaviour behaviour)
    {
        if (coroutineDictionary.TryGetValue(behaviour, out var coroutines))
        {
            foreach (var coroutine in coroutines)
            {
                behaviour.StopCoroutine(coroutine);
            }
            coroutines.Clear();
        }
    }

    public void StartLerpValue(MonoBehaviour behaviour, float preValue, float value, float timeLerp = 1f, Action<float> action = null, Action onComplete = null)
    {
        if (!coroutineDictionary.TryGetValue(behaviour, out var coroutines))
        {
            coroutines = new List<Coroutine>();
            coroutineDictionary[behaviour] = coroutines;
        }
        else
        {
            // Dừng tất cả các coroutine hiện tại của MonoBehaviour
            StopAllCoroutinesForBehaviour(behaviour);
        }

        Coroutine coroutine = behaviour.StartCoroutine(IEFloatLerp(behaviour, preValue, value, timeLerp, action, onComplete));
        coroutines.Add(coroutine);
    }

    private IEnumerator IEFloatLerp(MonoBehaviour behaviour, float preValue, float value, float timeLerp = 1f, Action<float> action = null, Action onComplete = null)
    {
        float timeDeltaTime = Mathf.Approximately(Time.deltaTime, 0) ? (1f / 50f) : Time.deltaTime;
        float sub = timeLerp / timeDeltaTime;
        float delta = (preValue - value) / sub;
        bool isNegativeValue = preValue < value;
        while (true)
        {
            preValue -= delta;
            if ((!isNegativeValue && (preValue <= value)) || (isNegativeValue && (preValue >= value)))
            {
                action?.Invoke(value);
                onComplete?.Invoke();
                coroutineDictionary.Remove(behaviour);
                yield break;
            }
            action?.Invoke(preValue);
            yield return null;
        }
    }
    public static float[] Vector2ToFloatArray(Vector2 value)
    {
        return new float[]
        {
            value.x,
            value.y,
        };
    }
    public static Vector2 FloatArrayToVector2(float[] value)
    {
        return new Vector2(value[0], value[1]);
    }

    public static bool IsLayerInLayerMask(int layer, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layer)) != 0);
    }
    public static List<GameObject> FindGameObjectsByName(string name)
    {
        // Tạo danh sách để lưu các GameObject tìm được
        List<GameObject> objectsWithName = new List<GameObject>();

        // Lấy tất cả các GameObject trong scene
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        // Kiểm tra từng GameObject xem có tên là "collidd" không
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                objectsWithName.Add(obj);
            }
        }

        return objectsWithName;
    }

    #region ONNewDay
    private const string LastCheckedDateKey = "LastCheckedDateSeconds";
    private IEnumerator IEWaitEndFrameNewDay()
    {
        yield return null;
        OnNewDay();
    }
    public void OnNewDay()
    {
        // Lấy thời gian hiện tại dưới dạng giây
        long currentSeconds = TimeUtils.GetLongTimeCurrent;

        // Lấy số giây lưu trữ trong PlayerPrefs
        long lastCheckedSeconds = PlayerPrefs.GetInt(LastCheckedDateKey, 0);

        if (lastCheckedSeconds == 0)
        {
            // Nếu không có thời gian lưu trữ trước đó, đây là lần đầu tiên chạy
            PerformNewDayActions(currentSeconds);
        }
        else
        {
            DateTimeOffset lastCheckedDate = DateTimeOffset.FromUnixTimeSeconds(lastCheckedSeconds);
            DateTimeOffset currentDate = DateTimeOffset.FromUnixTimeSeconds(currentSeconds);

            if (currentDate.Date > lastCheckedDate.Date)
            {
                // Nếu ngày hiện tại khác ngày lưu trữ, thực hiện hành động cho ngày mới
                PerformNewDayActions(currentSeconds);
            }
        }
    }

    void PerformNewDayActions(long currentSeconds)
    {
        // Cập nhật ngày mới vào PlayerPrefs
        PlayerPrefs.SetInt(LastCheckedDateKey, (int)currentSeconds);
        PlayerPrefs.Save();

        Observer.Instance.Notify(ObserverKey.OnNewDay);

    }
    #endregion

    public static string ToOrdinal(int number)
    {
        return $"{number}{GetOrdinalSuffix(number)}";
    }
    public static string GetOrdinalSuffix(int number)
    {
        if (number <= 0) return "";

        int lastTwoDigits = number % 100;
        int lastDigit = number % 10;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
        {
            return "th";
        }

        switch (lastDigit)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }

    public static void SetActiveCheckNull(GameObject ob, bool status)
    {
        if (ob == null) return;
        ob.SetActive(status);
    }
    public static void SetSpriteCheckNull(Image im, Sprite sp)
    {
        if (im == null) return;
        im.sprite = sp;
    }
    public static void SetTextCheckNull(TMPro.TMP_Text txt, string str)
    {
        if (txt == null) return;
        txt.text = str;
    }

    public static int GetCurrentLevel(int currentLevel)
    {
        int level = currentLevel;
        if (level >= 199)
        {
            level = currentLevel % 150;
            if (level <= 48)
            {
                level += 50;
            }
        }
        return level;
    }
    public static bool AreListsEqual<T>(List<T> list1, List<T> list2)
    {
        var groupedList1 = list1.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var groupedList2 = list2.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        return groupedList1.Count == groupedList2.Count &&
               groupedList1.All(pair => groupedList2.ContainsKey(pair.Key) && groupedList2[pair.Key] == pair.Value);
    }

    public static int GetRandomValueInCountStack(int total, int contain, int count, int minGive)
    {
        int valueSub = (total - contain);
        if (count <= 1)
        {
            return valueSub;
        }
        int canSub = valueSub - minGive * (count - 1);
        int valueSure = UnityEngine.Random.Range(minGive, canSub);
        return valueSure;
    }

    public static void StepToStep(Action<Action>[] step)
    {
        ExecuteStep(step, 0);

        void ExecuteStep(Action<Action>[] stepOnDone, int currentIndex)
        {
            if (currentIndex < stepOnDone.Length)
            {
                stepOnDone[currentIndex](() =>
                {
                    // Sau khi bước hoàn thành, tiếp tục bước tiếp theo
                    ExecuteStep(stepOnDone, currentIndex + 1);
                });
            }
        }
    }
    public static int GetIndexInArrayInt(int[] array, int a)
    {
        int length = array.Length;
        for (int i = 0; i < length - 1; i++)
        {
            if (a > array[i] && a <= array[i + 1])
            {
                return i;
            }
        }

        // Nếu a lớn hơn giá trị cuối cùng
        if (a > array[^1])
        {
            return array.Length - 1;
        }

        // Nếu không thỏa mãn điều kiện nào
        return 0; // Hoặc giá trị mặc định khác
    }


}

public static class HelperClass
{

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

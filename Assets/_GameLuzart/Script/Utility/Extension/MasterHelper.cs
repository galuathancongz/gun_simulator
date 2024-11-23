//using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MasterHelper
{
    #region Layer

    public static bool HasLayer(int idxLayerCheck, int valueAllLayer)
    {
        if ((valueAllLayer & 1 << idxLayerCheck) == 1 << idxLayerCheck)
        {
            return true;
        }
        return false;
    }

    public static bool HasLayer(LayerMask layerCheck, LayerMask allLayer)
    {
        return HasLayerValue(layerCheck.value, allLayer.value);
    }

    public static bool HasLayerValue(int valueLayerCheck, int valueAllLayer)
    {
        return valueLayerCheck == (valueAllLayer & valueLayerCheck);
    }

    #endregion Layer

    #region Json

    public static T JsonDeserialize<T>(string ob)
    {
        try
        {
            return JsonUtility.FromJson<T>(ob);
        }
        catch
        {
            return default;
        }
    }

    public static string JsonSerializeObject<T>(T t)
    {
        return JsonUtility.ToJson(t);
    }

    #endregion Json

    #region Enum

    public static TEnum CovertToEnum<TEnum>(string nameOfEnum, TEnum defaulEnum) where TEnum : struct, System.IConvertible
    {
        bool isEnum = System.Enum.TryParse(nameOfEnum, out TEnum _enum);
        if (!isEnum) return defaulEnum;
        return _enum;
    }

    public static TEnum CovertToEnum<TEnum>(string nameOfEnum) where TEnum : struct, System.IConvertible
    {
        System.Enum.TryParse(nameOfEnum, out TEnum _enum);
        return _enum;
    }

    public static TEnum CovertToEnum<TEnum>(int idEnum) where TEnum : struct, System.IConvertible
    {
        TEnum _enum = (TEnum)System.Enum.ToObject(typeof(TEnum), idEnum);
        return _enum;
    }

    public static TEnum CovertToEnum<TEnum>(int idEnum, TEnum defaulEnum) where TEnum : struct, System.IConvertible
    {
        TEnum _enum = (TEnum)System.Enum.ToObject(typeof(TEnum), idEnum);
        if (_enum.ToString().Equals(idEnum.ToString())) return defaulEnum;
        return _enum;
    }

    #endregion Enum

    #region Debug

    public static void Debug<T>(T t)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log($"CheckErr: {t}");
#endif
    }

    #endregion Debug

    #region Mathf

    public static float Round(float _value, int indexRound = 0)
    {
        return Mathf.Round(_value * Mathf.Pow(10, indexRound)) / Mathf.Pow(10, indexRound);
    }

    public static float Floor(float _value, int indexRound = 0)
    {
        return Mathf.Floor(_value * Mathf.Pow(10, indexRound)) / Mathf.Pow(10, indexRound);
    }

    public static float Ceil(float _value, int indexRound = 0)
    {
        return Mathf.Ceil(_value * Mathf.Pow(10, indexRound)) / Mathf.Pow(10, indexRound);
    }

    #endregion Mathf


    #region Spawn Obj
    public static void InitListObj<Tobj, Tdata>(IList<Tdata> data, Tobj objPf, IList<Tobj> objs, Transform holdObj, System.Action<Tobj, int> onSetup) where Tobj : MonoBehaviour
    {
        if (objs == null)
        {
            objs = new List<Tobj>();
        }
        objPf.gameObject.SetActive(false);
        if (data != null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                Tobj n;
                var idx = i;
                if (i < objs.Count)
                {
                    n = objs[idx];
                }
                else
                {
                    n = Object.Instantiate(objPf, holdObj);
                    objs.Add(n);
                }
                onSetup?.Invoke(n, idx);
            }
        }
        var c = data == null ? 0 : data.Count;
        if (c < objs.Count)
        {
            for (int i = c; i < objs.Count; i++)
            {
                objs[i].gameObject.SetActive(false);
            }
        }
    }
    public static void InitListObj<Tobj>(int num, Tobj objPf, IList<Tobj> objs, Transform holdObj, System.Action<Tobj, int> onSetup) where Tobj : MonoBehaviour
    {
        if (objs == null)
        {
            objs = new List<Tobj>();
        }
        objPf.gameObject.SetActive(false);
        for (int i = 0; i < num; i++)
        {
            Tobj n;
            var idx = i;
            if (i < objs.Count)
            {
                n = objs[idx];
            }
            else
            {
                n = Object.Instantiate(objPf, holdObj);
                objs.Add(n);
            }
            onSetup?.Invoke(n, idx);
        }
        if (num < objs.Count)
        {
            for (int i = num; i < objs.Count; i++)
            {
                objs[i].gameObject.SetActive(false);
            }
        }
    }
    //public static Sequence InitListObjTween<Tobj, Tdata>(IList<Tdata> data, Tobj objPf, IList<Tobj> objs, Transform holdObj, System.Action<Tobj, int> onSetup) where Tobj : MonoBehaviour
    //{
    //    Sequence sequence = DOTween.Sequence();
    //    if (objs == null)
    //    {
    //        objs = new List<Tobj>();
    //    }
    //    objPf.gameObject.SetActive(false);
    //    if (data != null)
    //    {
    //        for (int i = 0; i < data.Count; i++)
    //        {
    //            Tobj n;
    //            var idx = i;
    //            if (i < objs.Count)
    //            {
    //                n = objs[idx];
    //            }
    //            else
    //            {
    //                n = Object.Instantiate(objPf, holdObj);
    //                objs.Add(n);
    //            }
    //            onSetup?.Invoke(n, idx);
    //        }
    //    }
    //    var c = data == null ? 0 : data.Count;
    //    if (c < objs.Count)
    //    {
    //        for (int i = c; i < objs.Count; i++)
    //        {
    //            objs[i].gameObject.SetActive(false);
    //        }
    //    }
    //    return sequence;
    //}
    #endregion

    #region Validate Name
    public static bool ValidateName(string input)
    {
        if (input[0] == ' ') return false;
        return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[\p{L}\p{M}\p{N}' \.\-]+$");
    }
    #endregion

    public static IEnumerable<System.Type> GetAllTypesThatImplement<T>()
    {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
    }
}
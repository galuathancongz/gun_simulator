using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static TestManager Instance;
    private void Awake()
    {
        Instance = this;
    }
}

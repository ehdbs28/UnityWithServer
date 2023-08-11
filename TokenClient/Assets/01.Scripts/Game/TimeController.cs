using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    private int _time;
    public int Time => _time;

    public bool GameOver { get; set; }

    public event Action<int> TimeEvent = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private IEnumerator Start()
    {
        while (!GameOver)
        {
            yield return new WaitForSeconds(1f);
            _time++;
            TimeEvent?.Invoke(_time);
        }
    }
}

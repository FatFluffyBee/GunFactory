using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TickManager : MonoBehaviour
{
    public static TickManager instance;
    public const float TICK_TIMER_MAX = 0.1f;

    private int tickCount = 0;
    private float timer = 0;

    private void Awake()
    {
        instance = this;
        //OnTick.AddListener(DebugTest);
    }

    public UnityEvent OnTick = new UnityEvent();

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > TICK_TIMER_MAX)
        {
            timer -= TICK_TIMER_MAX;
            tickCount++;

            OnTick?.Invoke();
        }
    }

    private void DebugTest()
    {
        Debug.Log(tickCount);
    }

}

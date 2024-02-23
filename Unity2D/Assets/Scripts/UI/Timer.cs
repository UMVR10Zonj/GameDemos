using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text txtTime;
    [SerializeField] private float remainingTime;
    public float RemainTime { get => remainingTime; }
    [NonSerialized] public ReactiveProperty<float> NowTime;
    private bool isPause = false;

    private float secCounter = 0;
    private void Awake()
    {
        NowTime = new ReactiveProperty<float>(remainingTime);
    }
    private void Update()
    {
        if (isPause) return;
        if (NowTime.Value == 0) return;

        secCounter += Time.deltaTime;

        if (secCounter >= 1)
        {
            NowTime.Value -= secCounter;
            secCounter = 0;
        }

        if (NowTime.Value <= 0)
        {
            NowTime.Value = 0;
            return;
        }

        int minutes = Mathf.FloorToInt(NowTime.Value * 0.016f);
        int seconds = Mathf.FloorToInt(NowTime.Value % 60);
        txtTime.text = string.Format("{0}:{1:00}", minutes, seconds);
    }
    
    public void StartTimer()
    {
        isPause = false;
    }
    public void Pause()
    {
        isPause = true;
    }
    public void Continue()
    {
        isPause = false;
    }
    public void Reset()
    {
        isPause = true;
        NowTime.Value = remainingTime;
        txtTime.text = string.Format("{0}:{1:00}", NowTime.Value / 60, NowTime.Value % 60);
    }
}

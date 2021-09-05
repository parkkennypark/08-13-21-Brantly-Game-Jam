using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patience : MonoBehaviour
{
    public delegate void TimerAction();
    public event TimerAction OnTimerTimeout;

    public float maxTime = 15;
    public Transform progressTooltip;
    public Transform progressBar;
    public Vector2 tooltipOffset;
    public bool reverse;

    public bool isTruck;

    private bool timedOut;
    private float timeLeft;

    void Start()
    {
        if (isTruck)
        {
            maxTime = GameManager.instance.gameTime[GameManager.instance.currentDay - 1];
        }
    }

    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        progressTooltip.position = screenPos + tooltipOffset;


        float ratio = timeLeft / maxTime;
        if (reverse)
        {
            ratio = 1 - ratio;
        }
        progressBar.localScale = new Vector3(1, ratio, 1);

        if (timeLeft <= 0 && !timedOut)
        {
            timeLeft = 0;
            OnTimeout();
        }
        else if (!timedOut)
        {
            timeLeft -= Time.deltaTime;
        }
    }

    public void SetVisible(bool visible)
    {
        // Debug.Log("AAAA", gameObject);
        transform.GetChild(0).gameObject.SetActive(visible);
    }

    public void StartTimer()
    {
        timeLeft = maxTime;
        timedOut = false;
    }

    public void StopTimer()
    {
        timedOut = true;
    }

    public void OnTimeout()
    {
        if (OnTimerTimeout != null)
        {
            OnTimerTimeout();
        }
        StopTimer();
        SetVisible(false);
    }
}

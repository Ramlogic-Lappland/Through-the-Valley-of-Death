using UnityEngine;
using System;

public class GameTimerManager : MonoBehaviour
{
    public static GameTimerManager Instance { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        // Prevent duplicate managers when changing scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this object alive between scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        ElapsedTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        IsRunning = true;
    }

    public void StopTimer()
    {
        IsRunning = false;

        Debug.Log($"Final Time: {GetFormattedTime()}");
    }

    public void ResetTimer()
    {
        ElapsedTime = 0f;
        IsRunning = false;
    }

    public string GetFormattedTime()
    {
        int totalSeconds = Mathf.FloorToInt(ElapsedTime);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }
}
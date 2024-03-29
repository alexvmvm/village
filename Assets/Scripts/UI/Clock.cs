﻿using UnityEngine;
using UnityEngine.UI;
using Village;

public class Clock : MonoBehaviour
{
    public Transform ClockFace;
    public Text Season;
    public Color Day;
    public Color Night;

    private float _rotationalSpeed;
    private Game _game;

    void Start()
    {
        _game = FindObjectOfType<Game>();
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        Season.text = $"{_game.WorldTime.Season.ToString()} {_game.WorldTime.DayInMonth}";

        var normTime = _game.WorldTime.NormalizedTimeOfDay;

        // rotate clock
        var rotation = normTime * 360;
        ClockFace.transform.rotation = Quaternion.Euler(0, 0, -rotation);

        // update ambient color
        if(normTime < 0.5) 
        {
            RenderSettings.ambientLight = Color.Lerp(Night, Day, normTime * 2);
        }
        else 
        {
            RenderSettings.ambientLight = Color.Lerp(Day, Night, (normTime - 0.5f) * 2);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeOfDay
{
    Day,
    Night
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}

public class WorldTime 
{
    public float TimeSinceStart;
    public float NormalizedTimeOfDay { get { return ((TimeSinceStart % SecondsInADay) / SecondsInADay); } }
    public float SecondsInADay { get { return _secondsInADay; } }
    public TimeOfDay TimeOfDay;
    public Season Season;
    public int HourOfDay;
    private float _secondsInADay;
    private int _morningHour;
    private int _nightHour;

    public WorldTime(float secondsInADay, int morningHour, int nightHour)
    {
        _secondsInADay = secondsInADay;
        _nightHour = nightHour;
        _morningHour = morningHour;

        // start in day..
        TimeSinceStart = 160f;
    }

    public TimeOfDay GetTimeOfDay()
    {
        return TimeOfDay;
    }

    public void Update()
    {
        TimeSinceStart += Time.deltaTime;

        // hour of day
        HourOfDay = Mathf.FloorToInt(NormalizedTimeOfDay * 24); 
        
        // time of day
        if(HourOfDay >= _nightHour || HourOfDay < _morningHour)
            TimeOfDay = TimeOfDay.Night;
        else
            TimeOfDay = TimeOfDay.Day;
    }
}

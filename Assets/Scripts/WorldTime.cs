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
    public int DaysSinceStart { get { return Mathf.FloorToInt(TimeSinceStart / SecondsInADay); } }
    public int DayInMonth { get { return DaysSinceStart % DaysInAMonth + 1; } }
    public int DayInYear { get { return DaysSinceStart % (DaysInAMonth * 4) + 1; } }
    public int SeasonIndex { get { return Mathf.FloorToInt(DayInYear / ((float)DaysInAMonth * 4) * 4); } }
    public float SecondsInADay { get; }
    public int DaysInAMonth { get; }
    private int MorningHour { get; }
    private int NightHour { get; }
    public TimeOfDay TimeOfDay;
    public Season Season;
    public int HourOfDay;


    public WorldTime()
    {
        SecondsInADay = 360;
        DaysInAMonth = 30;

        NightHour = 5;
        MorningHour = 23;

        // start in day..
        TimeSinceStart = 110;
    }

    public TimeOfDay GetTimeOfDay()
    {
        return TimeOfDay;
    }

    void CalculateTimeOfDay()
    {
        if(HourOfDay >= NightHour || HourOfDay < MorningHour)
            TimeOfDay = TimeOfDay.Night;
        else
            TimeOfDay = TimeOfDay.Day;
    }

    void CalculateSeason()
    {
        switch(SeasonIndex)
        {
            case 0:
                Season = Season.Spring;
            break;
            case 1:
                Season = Season.Summer;
            break;
            case 2:
                Season = Season.Autumn;
            break;
            case 3:
                Season = Season.Winter;
            break;
        }
    }

    public void Update()
    {
        TimeSinceStart += Time.deltaTime;

        // hour of day
        HourOfDay = Mathf.FloorToInt(NormalizedTimeOfDay * 24); 
        
        // time of day
        CalculateTimeOfDay();
        CalculateSeason();
    }
}

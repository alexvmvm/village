using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimeOfDay
{
    Day,
    Night
}

public class Clock : MonoBehaviour
{
    
    public Transform ClockFace;
    public float WorldTime;
    public float SecondsInADay = 60;
    public int HourOfDay;
    public int MorningHour = 7;
    public int NightHour = 23;
    public TimeOfDay TimeOfDay;
    public Color Day;
    public Color Night;

    private float _rotationalSpeed;
    private float _normalizedTime { get { return ((WorldTime % SecondsInADay) / SecondsInADay); } } 

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    
    public int GetHourOfDay()
    {
        return Mathf.FloorToInt(WorldTime) % (int)SecondsInADay;
    }

    // Update is called once per frame
    void Update()
    {
        WorldTime += Time.deltaTime;

        // rotate clock
        var rotation = _normalizedTime * 360;
        ClockFace.transform.rotation = Quaternion.Euler(0, 0, -rotation);

        // hour of day
        HourOfDay = Mathf.FloorToInt(_normalizedTime * 24); 
        
        // time of day
        if(HourOfDay >= NightHour || HourOfDay < MorningHour)
            TimeOfDay = TimeOfDay.Night;
        else
            TimeOfDay = TimeOfDay.Day;

        // update ambient color
        if(_normalizedTime < 0.5) 
        {
            RenderSettings.ambientLight = Color.Lerp(Night, Day, _normalizedTime * 2);
        }
        else 
        {
            RenderSettings.ambientLight = Color.Lerp(Day, Night, (_normalizedTime - 0.5f) * 2);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public Transform ClockFace;
    public float WorldTime;
    public float SecondsInADay = 60;
    public Color Day;
    public Color Night;

    private float _rotationalSpeed;

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        WorldTime += Time.deltaTime;
        var timeUnit = ((WorldTime % SecondsInADay) / SecondsInADay);

        // rotate clock
        var rotation = timeUnit * 360;
        ClockFace.transform.rotation = Quaternion.Euler(0, 0, -rotation);
        

        // update ambient color
        if(timeUnit < 0.5) 
        {
            RenderSettings.ambientLight = Color.Lerp(Day, Night, timeUnit * 2);
        }
        else 
        {
            RenderSettings.ambientLight = Color.Lerp(Night, Day, (timeUnit - 0.5f) * 2);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NeedType
{
    Hunger,
    Thirst,
    Warmth,
    Rest    
}

public class Needs
{
    private Game _game;

    private Dictionary<NeedType, float> _status;

    public Needs(Game game)
    {
        _game = game;
        _status = new Dictionary<NeedType, float>();
    }

    public bool IsNeedCritical(NeedType need)
    {
        return _status[need] <= -2;
    }

    public bool IsNeedPending(NeedType need)
    {
        return _status[need] <= -1;
    }

    public void IncreaseNeed(NeedType need, float amount)
    {
        if(!_status.ContainsKey(need))
            _status[need] = 0f;
        _status[need] += amount;
    }

    public void DecreaseNeed(NeedType need, float amount)
    {
        if(!_status.ContainsKey(need))
            _status[need] = 0f;
        _status[need] -= amount;
    }

}

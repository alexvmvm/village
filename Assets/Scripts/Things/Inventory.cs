﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    public Thing Holding { get { return _holding; } }
    private Thing _parent;

    private Thing _holding;

    public Inventory(Thing parent)
    {
        _parent = parent;
    }

    public void HoldThing(Thing thing)
    {
        _holding = thing;

        if(_holding.agent != null)
            _holding.agent.PauseAgent();

        _holding.transform.SetParent(_parent.transform);
        _holding.transform.localPosition = Vector3.up;
    }

    public void Drop()
    {
        if(_holding != null)
        {
            _holding.transform.SetParent(null);
            _holding = null;
        }
    }

    public bool IsHoldingSomething()
    {
        return _holding != null;
    }

    public bool IsHoldingSomethingToEat()
    {
        return _holding != null && _holding.edible;
    }

    public bool IsHolding(TypeOfThing type)
    {
        return _holding != null && _holding.type == type;
    }

    public bool IsHoldingTool()
    {
        return IsHoldingSomething() && (IsHolding(TypeOfThing.Hoe) || IsHolding(TypeOfThing.Axe));
    }

}

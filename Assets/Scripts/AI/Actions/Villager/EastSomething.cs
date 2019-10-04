﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EastSomething : GOAPAction
{
    private Inventory _inventory;

    public EastSomething(Game game, Inventory inventory) : base(game)
    {
        _inventory = inventory;
    }

    public override bool IsDone()
    {
        return !_inventory.IsHoldingSomething();
    }

    public override bool IsPossibleToPerform()
    {
        return true;
    }

    public override bool Perform()
    {
        _inventory.Holding.Destroy();
        _inventory.Drop();
        
        return true;
    }

    public override void Reset()
    {

    }

    public override string ToString()
    {
        if(_inventory.Holding == null)
            return base.ToString();
        return $"Eating {_inventory.Holding.type.ToString()}";
    }
}
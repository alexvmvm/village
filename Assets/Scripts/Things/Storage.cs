using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage 
{
    public Dictionary<TypeOfThing, bool> Allowed { get { return _allowed; } }
    private Dictionary<TypeOfThing, bool> _allowed;

    public Storage(Game game)
    {
        _allowed = new Dictionary<TypeOfThing, bool>();
    }
    
    public void Allow(TypeOfThing type)
    {
        _allowed[type] = true;
    }

    public void Disallow(TypeOfThing type)
    {
        _allowed[type] = false;
    }
    
}

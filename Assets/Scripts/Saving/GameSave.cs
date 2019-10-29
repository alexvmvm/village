using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class GameSave
{
    public Vector2Int Size;
    public ThingSave[] Things;
}
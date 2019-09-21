using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileRule
{
    Sprites GetSprite(Position position);
}

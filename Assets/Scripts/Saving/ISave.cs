using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISave<T> where T : new()
{
    T ToSaveObj();
    void FromSaveObj(T obj);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop 
{
    private Thing _thing;
    private Game _game;
    private string[] _sprites;
    private float _age;
    private float _timeToGrow;
    private int _index;
    
    public Crop(Game game, Thing thing, float timeToGrow, string[] sprites)
    {
        _game = game;
        _thing = thing; 
        _sprites = sprites;
        _timeToGrow = timeToGrow;
    }

    public void Setup()
    {
        
    }

    public string GetSprite()
    {
        return _sprites[_index];
    }

    public void Update()
    {
        _age += Time.deltaTime;
        var index = Mathf.FloorToInt(Mathf.Min(_age / _timeToGrow, 1) * (_sprites.Length - 1));
        if(index != _index)
        {
            _index = index;
            _thing.SetSprite();
        }
    }

    public void DrawGizmos()
    {
    #if UNITY_EDITOR
               
        var style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.white;

        var label = $"age: {_age}/{_timeToGrow}\nindex: {_index}";

        // current actions
        var position = _thing.transform.position + Vector3.up;
        UnityEditor.Handles.Label(position, label, style);
                  
#endif
    }
}

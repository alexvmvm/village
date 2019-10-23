using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PositionalAudio
{
    private string _group;
    private Game _game;
    private AudioClip _audioClip;
    private Vector3 _playerWorldPosition 
    {
        get 
        {
            return Camera.main.ViewportToWorldPoint(Vector3.one * 0.5f);
        }
    }
    private AudioSource _audioSource;

    public PositionalAudio(Game game, string group, string audio)
    {
        _group = group;
        _game = game;
        _audioClip = Assets.GetAudioClip(audio);
        
        var obj = _game.InstantiateObj();
        obj.name = "positionalAudio_" + group;
        _audioSource = obj.AddComponent<AudioSource>();
        _audioSource.clip = _audioClip;
        _audioSource.loop = true;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.spatialBlend = 1f;
        _audioSource.minDistance = 1;
        _audioSource.maxDistance = 20;
        _audioSource.Play();
    }

    public void Update()
    {
        var closestToPlayer = _game.Things
            .Where(t => t.positionalAudioGroup == _group)
            .OrderBy(t => Vector3.Distance(t.transform.position, _playerWorldPosition))
            .FirstOrDefault();

        if(closestToPlayer == null)
            return;
        
        _audioSource.transform.position = closestToPlayer.transform.position;
        
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_playerWorldPosition, 1f);

        Gizmos.color = Color.green;
        var things = _game.Things
            .Where(t => t.positionalAudioGroup == _group)
            .OrderBy(t => Vector3.Distance(t.transform.position, _playerWorldPosition))
            .ToArray();

        for(var i = 0; i < things.Length; i++)
        {
            Gizmos.color = i == 0 ? Color.green : Color.red;
            Gizmos.DrawWireCube(things[i].transform.position, Vector3.one);
        }
    }
}

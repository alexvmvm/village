using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using Cinemachine;

public class ZoomCamera : MonoBehaviour
{
    public float MinZoom = 10f;
    public float MaxZoom = 20f;
    public float StartZoom = 15f;
	public float ScrollSpeed = 1f;
    public float ZoomShortcutIncrement = 2f;
    public float Damping;
	public CinemachineVirtualCamera CinemachineCamera;
    public AudioMixer AudioMixer;
    public string AudioMixerGroup;
    private float _targetZoom;

    private static float _t;

    void Start()
    {
        _targetZoom = StartZoom;
    }

    // Update is called once per frame
    void LateUpdate () 
    {

        var scroll = Input.GetAxis("Mouse ScrollWheel");

        // if scrolling
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                _targetZoom -= 2;
                _t = 0f;
            }
            else if (scroll < 0)
            {
                _targetZoom += 2;
                _t = 0f;
            }


        }
      

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            _targetZoom -= ZoomShortcutIncrement;
            _t = 0;
        }
          

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            _targetZoom += ZoomShortcutIncrement;
            _t = 0;
        }
         
        _targetZoom = Mathf.Clamp(_targetZoom, MinZoom, MaxZoom);

        // set zoom level
        //_currentZoom = Mathf.Clamp (_currentZoom, MinZoom, MaxZoom);

        var current = CinemachineCamera.m_Lens.OrthographicSize;
        if (_targetZoom != current)
        {
            _t += Time.unscaledDeltaTime * ScrollSpeed;

            CinemachineCamera.m_Lens.OrthographicSize = Mathf.SmoothStep(current, _targetZoom, _t);

            // set audio 
            // var normalized = 1 - CinemachineCamera.m_Lens.OrthographicSize / (MaxZoom - MinZoom);
            // AudioMixer.SetFloat("worldVolume", Mathf.Log(normalized) * 20);

        }

    }
}

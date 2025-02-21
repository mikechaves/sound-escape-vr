using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnAmplitude : MonoBehaviour
{
    public float _startScale, _maxScale;
    public bool _useBuffer;
    Material _material;
    public float _red, _green, _blue;

    void Start()
    {
        _material = GetComponent<MeshRenderer>().materials[0];
    }

    void Update()
    {
        if (!_useBuffer && SampleManager._Amplitude > 0)
        {
            transform.localScale = new Vector3((SampleManager._Amplitude * _maxScale) + _startScale, (SampleManager._Amplitude * _maxScale) + _startScale, (SampleManager._Amplitude * _maxScale) + _startScale);
            Color _color = new Color(_red * SampleManager._Amplitude, _green * SampleManager._Amplitude, _blue * SampleManager._Amplitude);
            _material.SetColor("_EmissionColor", _color);
        }
        if (_useBuffer && SampleManager._Amplitude > 0)
        {
            transform.localScale = new Vector3((SampleManager._AmplitudeBuffer * 30f) + _startScale , (SampleManager._AmplitudeBuffer * 30f) + _startScale, (SampleManager._AmplitudeBuffer * 100f) + _startScale);
            Color _color = new Color(_red * SampleManager._AmplitudeBuffer, _green * SampleManager._AmplitudeBuffer, _blue * SampleManager._AmplitudeBuffer);
            _material.SetColor("_EmissionColor", _color);
        }
    }
}

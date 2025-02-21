using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Light))]
public class LightOnAudio : MonoBehaviour
{
    public int _band;
    public float _minIntensity, _maxIntensity;
    Light _light;

    void Start()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        _light.intensity = (SampleManager._audioBandBuffer[_band] * (_maxIntensity - _minIntensity)) + _minIntensity;
    }
}

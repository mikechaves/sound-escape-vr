using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    public int _band;
    public float _startScale, _maxScale;
    public bool _useBuffer;
    Material _material;

    void Start()
    {
        _material = GetComponent<MeshRenderer>().materials[0];
    }

    void Update()
    {
        if (_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (SampleManager._audioBandBuffer[_band] * _maxScale) + _startScale, transform.localScale.z);
            Color _color = new Color(SampleManager._audioBandBuffer[_band], SampleManager._audioBandBuffer[_band], SampleManager._audioBandBuffer[_band]);
            _material.SetColor("_EmissionColor", _color);
        }
        if (!_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (SampleManager._audioBand[_band] * _maxScale) + _startScale, transform.localScale.z);
            Color _color = new Color(SampleManager._audioBand[_band], SampleManager._audioBand[_band], SampleManager._audioBand[_band]);
            _material.SetColor("_EmissionColor", _color);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCam : MonoBehaviour
{
    private SampleManager _sampleManager;
    public Vector3 _rotateAxis, _rotateSpeed;

    public SampleManager SampleManager { get => _sampleManager; set => _sampleManager = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).transform.LookAt(this.transform);

        this.transform.Rotate(_rotateAxis.x * _rotateSpeed.x * Time.deltaTime * SampleManager._AmplitudeBuffer,
            _rotateAxis.y * _rotateSpeed.y * Time.deltaTime * SampleManager._AmplitudeBuffer,
            _rotateAxis.z * _rotateSpeed.z * Time.deltaTime * SampleManager._AmplitudeBuffer
            );
    }
}

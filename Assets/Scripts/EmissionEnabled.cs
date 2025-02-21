using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionEnabled : MonoBehaviour
{
    //public int _band;
    //public bool _useBuffer;
    //Material _material;

    void Start()
    {
        //_material = GetComponent<MeshRenderer>().materials[0];
        GetComponentInChildren<ParticleSystem>().Stop();
    }


    void Update()
    {


    }

    public void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag == "Tile")
        //{
        //    //Color _color = new Color(SampleManager._audioBandBuffer[_band], SampleManager._audioBandBuffer[_band], SampleManager._audioBandBuffer[_band]);
        //    _material.SetColor("_EmissionColor", Color.yellow);
        //}
    }

    public void OnTriggerExit(Collider other) // I ADDED THIS
    {
        //if (other.gameObject.tag == "Tile")
        //{
        //    _material.SetColor("_EmissionColor", Color.blue);
        //}
    }
}

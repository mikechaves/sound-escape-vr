using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //public Transform orgPos;
    public GameObject cube;

    public void Spawn(Transform t)
    {
        //GameObject clone = Instantiate(cube);
        GameObject clone = Instantiate(cube, t.transform.position, t.transform.rotation);
        //clone.transform.position = t.transform.position;
        //clone.transform.rotation = t.transform.rotation;
    }

    void Start()
    {
       // orgPos.transform.position = transform.position;
    }

    void Update()
    {
        
    }
}

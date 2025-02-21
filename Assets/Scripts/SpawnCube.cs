using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCube : MonoBehaviour
{
    public GameObject cube;

    public void Spawn(Transform t)
    {
        //GameObject clone = Instantiate(cube);
        GameObject clone = Instantiate(cube, new Vector3(0, 0, 0), Quaternion.identity);
        clone.transform.position = t.transform.position;
        clone.transform.rotation = t.transform.rotation;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

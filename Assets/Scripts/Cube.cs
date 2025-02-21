using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 startRot;

    private bool hasSpawnedNewCube = false;

    void Start()
    {
        startPos = this.transform.position;
        startRot = this.transform.rotation.eulerAngles;
    }

    void Update()
    {
        
    }

    public void Spawn()
    {
        GameObject clone = Instantiate(gameObject);
        clone.transform.position = startPos;
        clone.transform.rotation = Quaternion.Euler(startRot);
        hasSpawnedNewCube = true;
    }

    public void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Shelf" && !hasSpawnedNewCube)
        {
            StartCoroutine(waitToSpawn());
        }
    }

    IEnumerator waitToSpawn()
    {
        yield return new WaitForSeconds(1.0f); 
        Spawn();
    }
}

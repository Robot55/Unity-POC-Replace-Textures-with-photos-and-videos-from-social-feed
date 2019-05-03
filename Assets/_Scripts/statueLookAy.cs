using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class statueLookAy : MonoBehaviour
{
    public Transform objectToLookAt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform temp = objectToLookAt;
        temp.position = new Vector3(objectToLookAt.position.x, transform.position.y, objectToLookAt.position.z);
       transform.LookAt(objectToLookAt);
    }
}

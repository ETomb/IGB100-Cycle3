using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMove : MonoBehaviour {

    Vector3 position;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        position = transform.position;
        position.z += 0.36f;
        transform.position = position;
	}
}

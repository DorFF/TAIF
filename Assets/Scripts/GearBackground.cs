using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBackground : MonoBehaviour {
    [Header("Speed")]
    public float speedRotation;
    public bool rightDirection;
    float direct;

    // Use this for initialization
    void Start () {
        if (rightDirection) direct = -1; else direct = 1;
    }
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(0, 0, direct * speedRotation);
    }
}

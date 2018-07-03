using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDistance : MonoBehaviour {

    public Transform A;
    public Transform B;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //var a = A.position;
        //var b = B.position;
        //a.y = 0;
        //b.y = 0;
        //Debug.Log((a - b).sqrMagnitude);
	}

    private void OnBecameVisible()
    {
        Debug.Log("TRUE");
    }

    private void OnBecameInvisible()
    {
        Debug.Log("FALSE");
    }

    
}

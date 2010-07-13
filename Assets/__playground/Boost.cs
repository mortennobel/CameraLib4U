using UnityEngine;
using System.Collections;

public class Boost : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Z)){
			rigidbody.velocity = new Vector3(1,0,0);
		} else {
			rigidbody.velocity = new Vector3(1,0,1);
		}
	}
}

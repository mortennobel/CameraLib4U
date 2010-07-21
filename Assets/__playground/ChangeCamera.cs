using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CompositeCamera))]
public class ChangeCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CompositeCamera cc = GetComponent<CompositeCamera>();
		if (Input.GetKeyUp(KeyCode.Alpha1)){
			cc.SetCamera(0,5.0f);
		} else if (Input.GetKeyUp(KeyCode.Alpha2)){
			cc.SetCamera(1,5.0f);
		} else if (Input.GetKeyUp(KeyCode.Alpha3)){
			cc.SetCamera(2,5.0f);
		} else if (Input.GetKeyUp(KeyCode.Alpha4)){
			cc.SetCamera(3,5.0f);
		}
	}
}

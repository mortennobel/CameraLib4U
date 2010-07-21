using UnityEngine;
using System.Collections;

public class MouseDragDetector : MonoBehaviour {
	
	private Vector3 mousePositionStart;
	private bool mouseDown = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(1)){
			mouseDown = true;
			mousePositionStart = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(1)){
			mouseDown = false;
		}
		if (mouseDown){
			Debug.Log("Mouse position is: "+(Input.mousePosition-mousePositionStart));
		}
	}
}

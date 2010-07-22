using UnityEngine;
using System.Collections;

public class MouseDragDetector : MonoBehaviour {
	
	private Vector3 mousePositionStart;
	private bool mouseDown = false;
	
	public float x;
	public float y;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)){
			Screen.lockCursor = true;
		}
		if (Input.GetMouseButtonDown(2)){
			Screen.lockCursor = false;
		}
		if (Input.GetMouseButtonDown(1)){
			mouseDown = true;
			mousePositionStart = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(1)){
			mouseDown = false;
			x = 0;
			y = 0;
			ChaseCamera cc = GetComponent<ChaseCamera>();
			cc.lookHorizontal = 0;
		}
		if (mouseDown){
			x += Input.GetAxis("Mouse X")*0.05f;
			x = Mathf.Clamp(x,-1,1);
			y += Input.GetAxis("Mouse Y")*0.05f;
			y = Mathf.Clamp(y,-1,1);
			ChaseCamera cc = GetComponent<ChaseCamera>();
			cc.lookHorizontal = x;
		}
	}
}

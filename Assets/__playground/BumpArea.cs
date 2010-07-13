using UnityEngine;
using System.Collections;


/**
 * 
 */
public class BumpArea : MonoBehaviour {
	public float bumpHeight = 1;
	private bool isOver =false;
	public Transform target;
	private float y; 
	
	// Use this for initialization
	void Start () {
		y =target.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (isOver){
			Vector3 p =target.position;
			p.y =y+bumpHeight+Mathf.Sin(Time.time*100f)*bumpHeight;
			target.position = p;
		}
	}
	
	void OnTriggerEnter (Collider collider){
		if (collider.transform.Equals(target)){
			isOver =true;
		}
	}
	
	void OnTriggerExit(Collider collider){
		if (collider.transform.Equals(target)){
			isOver = false;
		}
	}
}

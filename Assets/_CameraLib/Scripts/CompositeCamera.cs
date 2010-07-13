using UnityEngine;
using System.Collections;

public class CompositeCamera : MonoBehaviour, ICamera {

	public ICamera[] cameras;
	public int currentCamera = 0;
	public Transform target;
	
	public Vector3 GetCameraTargetPosition(){
		return Vector3.zero;
	}
	
	public void SetTarget(Transform target){
		this.target = target;
	}
	
	public Transform GetTarget(){
		return target;
	}
	
	public void InitCamera(){
		// unused
		Debug.Log("Init camera");
	}
	
	/**
	 * setCamera
	 */
	public void SetCamera(int i){
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
}

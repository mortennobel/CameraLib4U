using UnityEngine;
using System.Collections;

/// <summary>
/// Delegames most of the methods all cameras
/// </summary>
[AddComponentMenu("CameraLib/Composite Camera")]
[RequireComponent (typeof (Camera))]
public class CompositeCamera : ICamera {

	public ICamera[] cameras;
	public int currentCamera = 0;
	public int lastCamera = -1;
	
	public override Vector3 GetCameraTargetPosition(){
		// not used here
		return Vector3.zero;
	}
	
	public override void SetTarget(Transform target){
		this.target = target;
		cameras[currentCamera].SetTarget(target);
	}
	
	public override Transform GetTarget(){
		return null;
	}
	
	public override void InitCamera(){
		// do nothing
	}
	
	/**
	 * setCamera
	 */
	public void SetCamera(int i){
		cameras[currentCamera].enabled = false;
		currentCamera = i;
		cameras[currentCamera].SetTarget(target);
		cameras[currentCamera].enabled = true;
		cameras[currentCamera].InitCamera();
		
	}
	
	// Use this for initialization
	void Start () {
		for (int i=0;i<cameras.Length;i++){
			if (cameras[i]!=null){
				cameras[i].enabled = currentCamera==i;
			}
		}
	}
	
	// Update is called once per frame
	public override void UpdateCameraPosition () {
		
	}
}

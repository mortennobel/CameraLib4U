using UnityEngine;
using System.Collections;

/// <summary>
/// Change the camera when the target enters the collider (and change camera back, when target exists collider).
/// 
/// The collider should be a trigger.
/// </summary>
[RequireComponent (typeof (Collider))]
public class CameraChangeCollider : MonoBehaviour {
	public Transform cameraTarget;
	public CompositeCamera compositeCamera;
	public int newCamera = 0;
	private int oldCamera = 0;
	public float interpolationTime = 1;
	
	void Start(){
		if (!collider.isTrigger){
			Debug.LogWarning("CameraChangeCollider should be set to trigger");
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.transform==cameraTarget){
			oldCamera = compositeCamera.currentCamera;
			compositeCamera.SetCamera(newCamera, interpolationTime);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.transform==cameraTarget){
			compositeCamera.SetCamera(oldCamera, interpolationTime);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Change the camera when the target enters the collider (and change camera back, when target exists collider).
/// 
/// The collider should be a trigger.
/// </summary>
[RequireComponent (typeof (Collider))]
public class CameraChangeCollider : MonoBehaviour {
	public CompositeCamera compositeCamera;
	public int newCamera = 0;
	private static List<int> cameraStack = new  List<int>();
	public float interpolationTime = 1;
	
	void Start(){
		if (!collider.isTrigger){
			Debug.LogWarning("CameraChangeCollider should be set to trigger");
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.transform==compositeCamera.GetTarget()){
			int oldCamera = compositeCamera.currentCamera;
			cameraStack.Add(oldCamera);
			compositeCamera.SetCamera(newCamera, interpolationTime);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.transform==compositeCamera.GetTarget()){
			if (compositeCamera.currentCamera == newCamera){
				// if current camera, then pop
				compositeCamera.SetCamera(cameraStack[cameraStack.Count-1], interpolationTime);
				cameraStack.RemoveAt(cameraStack.Count-1);
			} else {
				int lastIndex = cameraStack.LastIndexOf(newCamera);
				if (lastIndex>-1){
					cameraStack.RemoveAt(lastIndex);	
				} else {
					Debug.LogWarning("Error in CameraChangeCollider. Cannot find camera "+newCamera+" in the camera stack");
				}
			}
		}
	}
}

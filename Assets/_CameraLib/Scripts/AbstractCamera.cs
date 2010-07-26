using UnityEngine;
using System.Collections;

/// <summary>
/// This abstract camera type mainly defines the default camera properties and behavior. The camera is associated with 
/// one target object, that can be changed runtime using the SetTarget method.
/// 
/// The default behavior is first update position (UpdateCameraPosition() method) and then update rotation 
/// (UpdateLookRotation() method).
/// </summary>
public abstract class AbstractCamera : MonoBehaviour{
	public float smoothLookAtDamping = 6.0f;
	public bool smoothLookAtEnabled = true;
	
	public Transform target;
	public float targetMinimumRenderingDistance = 0.5f;
	
	/// <summary>
	/// Updates the camera position - by calling the GetCameraDesiredPosition() and smooth the movement.
	/// </summary>
	public virtual void UpdateCameraPosition(float lookHorizontal, float lookVertical){
		// empty
	}
	
	
	private bool targetRenderingEnabled;
	private void TargetRendering(bool enabled){
		if (enabled==targetRenderingEnabled){
			return;
		}
		targetRenderingEnabled = enabled;
		foreach (Renderer r in target.GetComponentsInChildren<Renderer>()){
			r.enabled = enabled;
		}
		Renderer localR = target.GetComponent<Renderer>();
		if (localR != null){
			localR.enabled = enabled;
		}
	}
	
	/// <summary>
	/// Sets the target runtime.
	/// </summary>
	public virtual void SetTarget(Transform transform){
		this.target = transform;	
	}
	
	public virtual Transform GetTarget(){
		return target;
	}
	
	/// <summary>
	/// Is called before a camera becomes active
	/// </summary>
	public virtual void InitCamera(){
		// empty
	}
	
	/// <summary>
	/// This method is reponsible for applying lookat
	/// </summary>
	void LateUpdate () {
		float lookHorizontal = Input.GetAxis("Horizontal_Alt");
		float lookVertical = Input.GetAxis("Vertical_Alt");
		UpdateCameraPosition(lookHorizontal, lookVertical);
		UpdateLookOrientation();
		
		
		float targetDistanceSqr = (transform.position-target.position).magnitude;
		bool targetRenderingEnabled = (targetDistanceSqr>targetMinimumRenderingDistance);
		TargetRendering(targetRenderingEnabled);
		Debug.Log("targetDistanceSqr "+targetDistanceSqr+" enabled "+targetRenderingEnabled);
	}
	
	public virtual void UpdateLookOrientation(){
		if (smoothLookAtEnabled) {
			// Look at and dampen the rotation
			Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
			transform.rotation =  Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * smoothLookAtDamping);
		} else {
			// Just lookat
		    transform.rotation =  Quaternion.LookRotation(target.position - transform.position);
		}
	}
}

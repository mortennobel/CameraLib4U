using UnityEngine;
using System.Collections;

public abstract class AbstractCamera : MonoBehaviour{
	public float smoothLookAtDamping = 6.0f;
	public bool smoothLookAtEnabled = true;
	
	public Transform target;
	
	public virtual void UpdateCameraPosition(){
		// empty
	}
	
	public virtual Vector3 GetCameraTargetPosition(){
		return Vector3.zero;
	}
	
	public virtual void SetTarget(Transform transform){
		this.target = transform;	
	}
	
	public virtual Transform GetTarget(){
		return target;
	}
	
	public virtual void InitCamera(){
		// empty
	}
	
	/// <summary>
	/// This method is reponsible for applying lookat
	/// </summary>
	void LateUpdate () {
		UpdateCameraPosition();
		UpdateLookRotation();
	}
	
	public virtual void UpdateLookRotation(){
		if (smoothLookAtEnabled)
		{
			// Look at and dampen the rotation
			Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
			transform.rotation =  Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * smoothLookAtDamping);
		}
		else
		{
			// Just lookat
		    transform.rotation =  Quaternion.LookRotation(target.position - transform.position);
		}
	}
}

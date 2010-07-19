using UnityEngine;
using System.Collections;

public class ICamera :MonoBehaviour{
	public float lookatDamping = 6.0f;
	public bool smooth = true;
	
	public Transform target;
	
	public virtual void UpdateCameraPosition(){
		Debug.Log("Foo");
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
		
	}
	
	/// <summary>
	/// This method is reponsible for applying lookat
	/// </summary>
	void LateUpdate () {
		UpdateCameraPosition();
		if (smooth)
		{
			// Look at and dampen the rotation
			Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookatDamping);
		}
		else
		{
			// Just lookat
		    transform.LookAt(target);
		}
	}
}

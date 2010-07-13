using UnityEngine;
using System.Collections;

public class ControlPointComponent : MonoBehaviour {
	public float ptime = 0.0f;
	private Vector3 pTangent = Vector3.zero;
	private Vector3 oldPos = Vector3.zero;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnDrawGizmosSelected () {
		Transform t = transform.parent;
		if (t==null){
			return;
		} 
		
		SplineComponent splineComponent = t.gameObject.GetComponent<SplineComponent>();
		if (splineComponent==null){
			return;
		}
		splineComponent.OnDrawGizmosSelected();
		Gizmos.color = splineComponent.controlPointColorSelected;
		Gizmos.DrawSphere(transform.position, splineComponent.sphereRadius*1.2f);
		if (!position.Equals(oldPos)){
			oldPos =position;
			splineComponent.UpdateSpline();
		}
	}
	
	public Vector3 position{
		get{
			return this.transform.position;
		}
	}
	
	public Vector3 tangent{
		get{
			return pTangent;
		}
	}
	
	public float time{
		get{
			return ptime;
		}
		set{
			ptime = value;
		}
	}
	
}

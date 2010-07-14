using UnityEngine;
using System.Collections;

/**
 * The camera will track the object, but it's 
 * movement is bound to the spline curve
 */
public class PathBoundCamera : ICamera {
	public SplineComponent cameraSpline;
	// public SplineComponent targetSpline;
	private SplineCurve cameraSplineObject;
	
	/// <summary>
	/// If the preferred distance between camera and is exceeded, then the camera needs to move
	/// </summary>
	public float maxDistanceToTarget = 2;
	public float maxDistanceToJumpCut =4;
	public float currentPositionOnPath = 0;
	public float cameraVelocity = 0;
	public float currentDistanceToTargetDebug=0;
	
	/// Physics coefficient which controls the influence of the camera's position
    /// over the spring force. The stiffer the spring, the closer it will stay to
    /// the chased object.
    /// Also known as SpringConstant
    private float springStiffness = 36.0f;

    /// Physics coefficient which approximates internal friction of the spring.
    /// Sufficient damping will prevent the spring from oscillating infinitely.
    private float springDamping = 12.0f;
	
	// Use this for initialization
	void Start () {
		this.cameraSplineObject = cameraSpline.GetSplineObject();
		InitCamera();	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary> 
	void Update () {
		//GetCameraTargetPosition();
		transform.position = GetCameraTargetPosition();
	}
	
	/// <summary>
	/// Set the camera on on the controlpoint closest to the player
	/// </summary>
	public override void InitCamera(){
		Vector3[] controlpoints =cameraSplineObject.controlPoints;
		float minDistance = float.MaxValue;
		
		int closestControlPoint= 0;
		for (int i=0;i<controlpoints.Length;i++){
			float controlpointLength =(controlpoints[i]-target.position).sqrMagnitude;
			if (minDistance > controlpointLength){
				minDistance =controlpointLength;
				closestControlPoint = i;
			}
		}
		transform.position = controlpoints[closestControlPoint];
		currentPositionOnPath =cameraSplineObject.GetRenderPoints()[closestControlPoint];
	}
	
	public void SetPreferredDistanceToCamera(float d){
		this.maxDistanceToTarget = d;
	}
	
	public override Vector3 GetCameraTargetPosition(){
		Vector3 splineVelocity = cameraSplineObject.GetVelocity(currentPositionOnPath);
		Vector3 distance = transform.position-target.position;
		if (currentPositionOnPath==0 && Vector3.Dot(distance, splineVelocity)>=0){
			//  trying to navigate away from endpoint
			return transform.position;
		}
		Debug.DrawLine(transform.position, target.position, Color.red);
		currentDistanceToTargetDebug =distance.magnitude;
		if (distance.sqrMagnitude < maxDistanceToTarget*maxDistanceToTarget){
			// don't move camera, since target is closer than preferredDistanceToCamera
			return transform.position;
		} 	
		
		// determine search direction
		distance = distance-(distance.normalized*maxDistanceToTarget);
		
		Debug.DrawLine(transform.position, transform.position+splineVelocity.normalized*5, Color.blue);
		Vector3 estimatedDirection = Vector3.Project(distance, splineVelocity);
		
		//float estimatedDelta = estimatedDirection.magnitude;
		float estimatedDelta = distance.magnitude;
		float dotProduct = Vector3.Dot(distance,splineVelocity);
		
		if (dotProduct>0){
			estimatedDelta =-estimatedDelta;
			Debug.DrawLine(transform.position, transform.position+estimatedDirection, Color.white);  
		} else {
			Debug.DrawLine(transform.position, transform.position+estimatedDirection, Color.red);  
		}
		
		float desiredPosition = currentPositionOnPath+estimatedDelta;
		
		// make sure if the desired position will not pull the camera backwards
		Vector3 newSplineVelocity = cameraSplineObject.GetVelocity(desiredPosition);
		Vector3 newDistance = cameraSplineObject.GetPosition(desiredPosition)-target.position;
		float newDotProduct = Vector3.Dot(newSplineVelocity, newDistance);
		if (newDotProduct>0==dotProduct>0){
			currentPositionOnPath = desiredPosition;
			
			if (currentPositionOnPath<0){
				currentPositionOnPath = 0;
				cameraVelocity = 0;
			} else if (currentPositionOnPath>cameraSplineObject.totalLength){
				currentPositionOnPath=cameraSplineObject.totalLength;
				cameraVelocity = 0;
			}
		}
		return cameraSplineObject.GetPosition(currentPositionOnPath);
	}
	
	public override void SetTarget(Transform target){
		this.target = target;
	}
	
	public override Transform GetTarget(){
		return transform;
	}
	
	public float GetDampingRadio(){
		return springDamping/(2*Mathf.Sqrt(springStiffness));
	}
	
	public void OnDrawGizmosSelected(){
		Gizmos.color = Color.white;
    	Gizmos.DrawWireSphere (transform.position, 0.2f);
	}
}

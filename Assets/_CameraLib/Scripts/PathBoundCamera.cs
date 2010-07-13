using UnityEngine;
using System.Collections;

/**
 * The camera will track the object, but it's 
 * movement is bound to the spline curve
 */
public class PathBoundCamera : MonoBehaviour, ICamera {
	public SplineComponent cameraSpline;
	// public SplineComponent targetSpline;
	public Transform target;
	private SplineCurve cameraSplineObject;
	
	/// <summary>
	/// If the preferred distance between camera and is exceeded, then the camera needs to move
	/// </summary>
	public float preferredDistanceToCamera = 2;
	private float preferredDistanceToCameraSqr;
	public float currentPositionOnPath = 0;
	private float cameraVelocity = 0;
	
	
	/// Physics coefficient which controls the influence of the camera's position
    /// over the spring force. The stiffer the spring, the closer it will stay to
    /// the chased object.
    /// Also known as SpringConstant
    public float springStiffness = 36.0f;

    /// Physics coefficient which approximates internal friction of the spring.
    /// Sufficient damping will prevent the spring from oscillating infinitely.
    public float springDamping = 12.0f;
	
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
	public void InitCamera(){
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
		this.preferredDistanceToCamera = d;
		this.preferredDistanceToCameraSqr = d*d;
	}
	
	public Vector3 GetCameraTargetPosition(){
		Vector3 distance = transform.position-target.position;
		Debug.DrawLine(transform.position, target.position, Color.red);
		if (distance.magnitude < preferredDistanceToCamera){
			// don't move camera, since target is closer than preferredDistanceToCamera
			return transform.position;
		} 	
		
		// determine search direction
		distance = distance-(distance.normalized*preferredDistanceToCamera);
		Vector3 splineVelocity = cameraSplineObject.GetVelocity(currentPositionOnPath);
		Debug.DrawLine(transform.position, transform.position+splineVelocity.normalized*5, Color.blue);
		Vector3 estimatedDirection = Vector3.Project(distance.normalized, splineVelocity);
		
		float estimatedDelta = estimatedDirection.magnitude;
		float dotProduct = Vector3.Dot(distance,splineVelocity);
		if (dotProduct>0){
			estimatedDelta =-estimatedDelta;
			Debug.DrawLine(transform.position, transform.position+estimatedDirection, Color.white);  
		} else {
			Debug.DrawLine(transform.position, transform.position+estimatedDirection, Color.red);  
		}
		
		float desiredPosition = currentPositionOnPath+estimatedDelta;
		    
		float displace = currentPositionOnPath - desiredPosition;
		float springAccel =(-springStiffness*displace)-(springDamping*cameraVelocity);
		
		cameraVelocity = cameraVelocity+springAccel*Time.deltaTime;
		
		currentPositionOnPath =currentPositionOnPath+cameraVelocity*Time.deltaTime;
		
		return cameraSplineObject.GetPosition(currentPositionOnPath+cameraVelocity*Time.deltaTime);
	}
	
	public void SetTarget(Transform target){
		this.target = target;
	}
	
	public Transform GetTarget(){
		return transform;
	}
	
	public void OnDrawGizmosSelected(){
		Gizmos.color = Color.white;
    	Gizmos.DrawWireSphere (transform.position, 0.2f);
	}
}

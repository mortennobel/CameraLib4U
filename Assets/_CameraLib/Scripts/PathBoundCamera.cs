using UnityEngine;
using System.Collections;

/// <summary>
/// The camera will track the object, but it's movement is bound to the spline curve.
///  
/// Assumptions:
/// I assume that the camera can see the player from the spline given the maxDistance-parameter. This means that I 
/// do not do any visibility check.
/// I also assumes that the spline does not collide with the environment (or other parts of the game world).
/// Note that neither of these assumption may hold in a game with moving objects or dynamic environment.
/// </summary>
[AddComponentMenu("CameraLib/Path Bound Camera")]
[RequireComponent (typeof (Camera))]
public class PathBoundCamera : AbstractCamera {
	public SplineComponent cameraSpline;
	
	private SplineCurve cameraSplineObject;
	
	/// <summary>
	/// If the preferred distance between camera and is exceeded, then the camera needs to move
	/// </summary>
	public float maxDistanceToTarget = 2;
	/// <summary>
	/// if the distance between camera and target is 'too large' the camera will search the full path for the 
	/// closest control-point and position itself there
	/// </summary>
	public float maxDistanceToJumpCut =4;
	private float currentPositionOnPath = 0;
	 
	/// The minimum time between distancebased jumpcut.
	public float minTimeBetweenDistancebasedJumpcut =3;
	private float distanceBasedJumpcutTimer = 0;
	
	public bool movementSpringDampingEnabled = true; 
	
	/// Physics coefficient which controls the influence of the camera's position
    /// over the spring force. The stiffer the spring, the closer it will stay to
    /// the chased object.
    /// Also known as SpringConstant
    public float movementSpringStiffness = 36.0f;

    /// Physics coefficient which approximates internal friction of the spring.
    /// Sufficient damping will prevent the spring from oscillating infinitely.
    public float movementSpringDamping = 12.0f;
	
	private float movementDesiredPosition;
	private float movementVelocity = 0f;
	
	// Use this for initialization
	void Start () {
		this.cameraSplineObject = cameraSpline.GetSplineObject();
		InitCamera();	
		
		if (maxDistanceToJumpCut<=maxDistanceToTarget){
			Debug.LogWarning("maxDistanceToJumpCut should be larger than maxDistanceToTarget");
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary> 
	public override void UpdateCameraPosition (float lookHorizontal, float lookVertical) {
		distanceBasedJumpcutTimer += Time.deltaTime;
		transform.position = GetCameraDesiredPosition();
	}
	
	/// <summary>
	/// Set the camera on on the controlpoint closest to the player
	/// </summary>
	public override void InitCamera(){
		if (cameraSplineObject==null){
			this.cameraSplineObject = cameraSpline.GetSplineObject();
			Debug.LogWarning("cameraSplineObject is null");
			return;
		}
		JumpCutToClosestControlPoint();
		base.InitCamera();
	}
	
	/// <summary>
	/// Search for the control point closest to the player and jump cut to that position 
	/// </summary>
	private void JumpCutToClosestControlPoint(){
		
		Vector3[] controlpoints = cameraSplineObject.controlPoints;
		float minDistance = float.MaxValue;
		
		int closestControlPoint= 0;
		for (int i=0;i<controlpoints.Length;i++){
			float controlpointLength =(controlpoints[i]-target.position).sqrMagnitude;
			if (minDistance > controlpointLength){
				minDistance = controlpointLength;
				closestControlPoint = i;
			}
		}
		
		float lengthAtControlPoint = cameraSplineObject.GetLengthAtTime(cameraSplineObject.time[closestControlPoint]);
		transform.position = controlpoints[closestControlPoint];
		currentPositionOnPath = lengthAtControlPoint;
		movementDesiredPosition = lengthAtControlPoint;
		movementVelocity = 0f;
		distanceBasedJumpcutTimer = 0;
	}
	
	public void SetPreferredDistanceToCamera(float d){
		this.maxDistanceToTarget = d;
	}
	
	public Vector3 GetCameraDesiredPosition(){
		float timeAtLength = cameraSplineObject.GetTimeByLength(currentPositionOnPath);
		Vector3 splineVelocity = cameraSplineObject.GetVelocity(timeAtLength);
		Vector3 distance = transform.position-target.position;
		
		if (distance.sqrMagnitude <= maxDistanceToTarget*maxDistanceToTarget){
			// don't move camera, since target is closer than preferredDistanceToCamera
			movementDesiredPosition = currentPositionOnPath = Damping.SpringDamping(currentPositionOnPath, currentPositionOnPath, ref movementVelocity,movementSpringStiffness, movementSpringDamping);
			currentPositionOnPath = Mathf.Clamp(movementDesiredPosition,0,cameraSplineObject.totalLength);
			return cameraSplineObject.GetPositionByLength(currentPositionOnPath);
		} 	
		
		if (distance.sqrMagnitude > maxDistanceToJumpCut*maxDistanceToJumpCut && 
		    distanceBasedJumpcutTimer > minTimeBetweenDistancebasedJumpcut){
			// jump cut
			JumpCutToClosestControlPoint();
			return transform.position;
		}
		
		// determine search direction
		distance = distance-(distance.normalized*maxDistanceToTarget);
		
		float estimatedDelta = distance.magnitude;
		float dotProduct = Vector3.Dot(distance,splineVelocity);
		
		if (dotProduct>0){
			estimatedDelta =-estimatedDelta;
		}
		
		movementDesiredPosition = currentPositionOnPath+estimatedDelta;
		float velocityCopy = movementVelocity;
		movementDesiredPosition = Damping.SpringDamping(currentPositionOnPath, movementDesiredPosition, ref velocityCopy,movementSpringStiffness, movementSpringDamping);
		
		// make sure if the desired position will not pull the camera backwards
		float movementDesiredPositionLength = cameraSplineObject.GetTimeByLength(movementDesiredPosition);
		Vector3 newSplineVelocity = cameraSplineObject.GetVelocity(movementDesiredPositionLength);
		Vector3 newDistance = cameraSplineObject.GetPosition(movementDesiredPositionLength)-target.position;
		float newDotProduct = Vector3.Dot(newSplineVelocity, newDistance);
		if (newDotProduct>0==dotProduct>0){
			// only update velocity if movement
			movementVelocity = velocityCopy; 
		} else {
			// note that this update velocity directly
			movementDesiredPosition = Damping.SpringDamping(currentPositionOnPath, currentPositionOnPath, ref movementVelocity,movementSpringStiffness, movementSpringDamping);
		}
		currentPositionOnPath = Mathf.Clamp(movementDesiredPosition,0,cameraSplineObject.totalLength);
			
		return cameraSplineObject.GetPositionByLength(currentPositionOnPath);
	}
	
	public override void SetTarget(Transform target){
		this.target = target;
	}
	
	public override Transform GetTarget(){
		return transform;
	}
	
	public void OnDrawGizmosSelected(){
		if (cameraSplineObject!=null){
			Gizmos.color = Color.white;
    		Gizmos.DrawWireSphere (cameraSplineObject.GetPositionByLength(movementDesiredPosition), 0.2f);
		}
	}
}

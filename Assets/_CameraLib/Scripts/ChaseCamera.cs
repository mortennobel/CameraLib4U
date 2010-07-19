using UnityEngine;
using System.Collections;

[AddComponentMenu("CameraLib/Chase Camera")]
[RequireComponent (typeof (Camera))]
public class ChaseCamera : ICamera {
	public float cameraHeight = 2;
	public float distance = 3;
	
	private Vector3 lastCameraTargetPosition = Vector3.zero;
	
	// fields for calculating velocity
	private const int velocityAccuracy = 2; // default 1/6th of a second (assuming 60 ftp) - the higher the better
	private int current = 0;
	// idea use a circular buffer with last known positions
	// since velocities are not used often the actual velocity 
	// is not computed every frame, but only when required
	private Vector3[] lastCameraPosition = new Vector3[velocityAccuracy];
	private Vector3[] lastTargetPosition = new Vector3[velocityAccuracy];
	private float[] lastDeltaTime = new float[velocityAccuracy];
	
	private Vector3 velocity = Vector3.zero;
	
	public bool springSmoothingEnabled = true; 
	
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
		// distanceSqr = distance*distance;
		lastCameraTargetPosition = transform.position;
		
		// fills buffers 
		for (int i=0;i<velocityAccuracy;i++){
			lastCameraPosition[i] = transform.position;
			if (target!=null){
				lastTargetPosition[i] =target.position;
			}
			lastDeltaTime[i] = 0.001f; // just to avoid div by zero
		}
	} 
	
	// Update is called once per frame
	public override void UpdateCameraPosition () {
		
		Vector3 desiredPosition = GetCameraTargetPosition();
		if (springSmoothingEnabled){
			Vector3 position = transform.position;
	        
			Vector3 displace = position - desiredPosition;
			Vector3 springAccel =(-springStiffness*displace)-(springDamping*velocity);
			
			velocity = velocity+springAccel*Time.deltaTime;
			transform.position = transform.position+velocity*Time.deltaTime;
		} else {
			transform.position = desiredPosition;
		}
		UpdateVelocity();
	}
	
	public override void InitCamera(){
		// unused
	}
	
	// store the current positions into an array
	private void UpdateVelocity(){
		current++;
		if (current>=velocityAccuracy){ // wrap counter
			current = 0;
		}
		lastCameraPosition[current] = transform.position;
		lastTargetPosition[current] = target.position;
		lastDeltaTime[current] = Time.deltaTime;
	}
	
	public Vector3 GetCameraVelocity(){
		int first = (current+velocityAccuracy-1)%velocityAccuracy;
		int last = current;
		float sumDeltaTime = 0f;
		for (int i=0;i<velocityAccuracy;i++){
			if (i==first){ //skip first, since this we only have velocityAccuracy-1 segments
				continue; 
			}
			sumDeltaTime += lastDeltaTime[i];
		}
		return (lastCameraPosition[last]-lastCameraPosition[first])/sumDeltaTime;
	}
	
	public Vector3 GetTargetVelocity(){
		int first = (current+velocityAccuracy-1)%velocityAccuracy;
		int last = current;
		float sumDeltaTime = 0f;
		for (int i=0;i<velocityAccuracy;i++){
			if (i==first){ //skip first, since this we only have velocityAccuracy-1 segments
				continue; 
			}
			sumDeltaTime += lastDeltaTime[i];
		}
		return (lastTargetPosition[last]-lastTargetPosition[first])/sumDeltaTime;	
	}
	
	
	/**
	 * This is the optimal camera position that the camera moves towards
	 */
	public override Vector3 GetCameraTargetPosition(){
		if (target==null){
			return transform.position;
		}
		
		// ignore y when calculating distance
		Vector3 distanceVector = target.position-transform.position;
		distanceVector.y = 0; 
		/*
		if (distanceVector.sqrMagnitude<distanceSqr){
			return lastCameraTargetPosition;
		}
		*/
		
		lastCameraTargetPosition = target.position - (distanceVector.normalized*distance);
		lastCameraTargetPosition.y += cameraHeight;
		
		return lastCameraTargetPosition;
	}
	
	// the performance of this method is a bit on the heavy side, but is only used in the editor
	public void OnDrawGizmosSelected () {
		if (target!=null){
			Gizmos.DrawIcon(GetCameraTargetPosition(), "camera.png");
			
			// draw circle to illustrate distance to target
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (transform.position, distance);
		}
		// draw velocity of camera
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position+GetCameraVelocity());	
		// draw velocity of camera
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(target.position, target.position+GetTargetVelocity());	
		
	}
	
	public override void SetTarget(Transform transform){
		this.target = transform;
		for (int i=0;i<velocityAccuracy;i++){
			if (target!=null){
				lastTargetPosition[i] = target.position;
			}
		}
	}
	
	public override Transform GetTarget(){
		return target;
	}
	
	public float GetDampingRatio(){
		return springDamping/(2*Mathf.Sqrt(springStiffness));
	}
	
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("CameraLib/Chase Camera")]
[RequireComponent (typeof (Camera))]
public class ChaseCamera : AbstractCamera {
	public float cameraHeight = 2;
	public float distance = 3;
	
	public enum ChaseCameraType { Loose, LooseAllowMovementUnderCamra,StayBehind };
	
	public ChaseCameraType cameraType = ChaseCameraType.StayBehind;
	
	private Vector3 lastCameraTargetPosition = Vector3.zero;
	public Vector3 idealSpherical = new Vector3();
	private float initialPitch;
	
	private Vector3 desiredPosition = Vector3.zero;
	
	// target
	public float targetHeight = 0.5f;
	
	// movement spring
	public bool springSmoothingEnabled = true; 
	/// Physics coefficient which controls the influence of the camera's position
    /// over the spring force. The stiffer the spring, the closer it will stay to
    /// the chased object.
    /// Also known as SpringConstant
    public float springStiffness = 36.0f;
    /// Physics coefficient which approximates internal friction of the spring.
    /// Sufficient damping will prevent the spring from oscillating infinitely.
    public float springDamping = 12.0f;
	private Vector3 velocity = Vector3.zero;
	
	// horizontal spring damping
	public bool lookHorizontalSpringDamped = true;
	public float lookHorizontalSpringStiffness = 36;
	public float lookHorizontalSpringDamping = 12;
	private float lookHorizontalActual = 0;
	private float lookHorizontalVelocity = 0;
	
	// vertical spring damping
	public bool lookVerticalSpringDamped = true;
	public float lookVerticalSpringStiffness = 36;
	public float lookVerticalSpringDamping = 12;
	private float lookVerticalActual = 0;
	private float lookVerticalVelocity = 0;
	
	// virtual camera collisions test (See "Third-Person Camera Navigasion" Jonathan Stone, Game Programming Gems 4)
	public bool virtualCameraCollisionTest = true;
	public float virtualCameraCollisionRadius = 0.5f;
	public LayerMask virtualCameraCollisionLayerMask = new LayerMask();
		
	public bool[] raycastResult = new bool[3];
	private Vector3[] raycastOffset = new Vector3[3];
	private int raycastCounter = 0;
	
	// Use this for initialization
	void Start () {
		lastCameraTargetPosition = transform.position;
		UpdateIdealSpherical();
		raycastOffset[0] = Vector3.up*(targetHeight*0.5f);
		raycastOffset[1] = Vector3.up*(targetHeight*0.5f);
		raycastOffset[2] = Vector3.zero;
		
	} 
	
	private void UpdateIdealSpherical(){
		idealSpherical.x = distance;
		idealSpherical.z = Mathf.Asin(cameraHeight/distance);
		initialPitch = idealSpherical.z;
	}
	
	/// <summary>
	/// Returns the Pitch of the camera in degrees (rotation around the x axis) 
	/// </summary>
	public float GetCameraPitch(){
		return -Mathf.Asin(cameraHeight/distance)*Mathf.Rad2Deg;
	}
	
	public override void InitCamera(){
		base.InitCamera();
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary> 
	public override void UpdateCameraPosition (float lookHorizontal, float lookVertical) {
		desiredPosition = GetCameraDesiredPosition(lookHorizontal, lookVertical);
		if (springSmoothingEnabled){
			transform.position = Damping.SpringDamping(transform.position,desiredPosition,ref velocity,springStiffness,springDamping);
		} else {
			transform.position = desiredPosition;
		}
	}
	
	/**
	 * This is the optimal camera position that the camera moves towards
	 */
	public Vector3 GetCameraDesiredPosition(float lookHorizontal, float lookVertical){
		if (target==null){
			return lastCameraTargetPosition;
		}
		
		if (cameraType==ChaseCameraType.StayBehind){
			Vector3 rotation = target.transform.rotation.eulerAngles;
			idealSpherical.y = -rotation.y*Mathf.Deg2Rad-(Mathf.PI*0.5f);
			if (lookHorizontalSpringDamped){
				lookHorizontalActual = Damping.SpringDamping(lookHorizontalActual,lookHorizontal, ref lookHorizontalVelocity,
					lookHorizontalSpringStiffness,lookHorizontalSpringDamping);
				idealSpherical.y += lookHorizontalActual*(Mathf.PI*0.99f);
			}
			if (lookVerticalSpringDamped){
				lookVerticalActual = Damping.SpringDamping(lookVerticalActual,lookVertical, ref lookVerticalVelocity,
					lookVerticalSpringStiffness,lookVerticalSpringDamping);
				
				idealSpherical.z = Mathf.Clamp(initialPitch+lookVerticalActual*Mathf.PI, -Mathf.PI*0.45f, Mathf.PI*0.45f);
				
			}
		} else {
			if (cameraType==ChaseCameraType.LooseAllowMovementUnderCamra){
				Vector3 displacement = transform.position-target.position;
				if (displacement.sqrMagnitude < distance*distance){
					return lastCameraTargetPosition;
				}
			}
			idealSpherical.y = Mathf.Atan2(transform.position.z-target.position.z,
				transform.position.x-target.position.x);
		}
		Vector3 direction = Vector3Ext.SphericalToCartesian(idealSpherical);;
		lastCameraTargetPosition = target.position+direction;
		if (virtualCameraCollisionTest){
			raycastCounter++;
			if (raycastCounter>2){
				raycastCounter = 0;
			}
			raycastResult[raycastCounter] = Physics.Raycast(target.position+raycastOffset[raycastCounter],direction,distance, virtualCameraCollisionLayerMask.value);
			
			RaycastHit hitInfo = new RaycastHit(); 
			bool hit = Physics.SphereCast(target.position,virtualCameraCollisionRadius,direction, out hitInfo, distance/*,virtualCameraCollisionLayerMask.value*/);  
			if (hit){
				// todo search through hits
				/*if (hitInfo.distance<distance*0.25f && (!raycastResult[0] || !raycastResult[1]|| !raycastResult[2])){
					idealSpherical.x = distance;
				} else {
				*/	idealSpherical.x = hitInfo.distance;
				// }
			} else {
				idealSpherical.x = distance;
			}
		}
		return lastCameraTargetPosition;
	}
	
	// the performance of this method is a bit on the heavy side, but is only used in the editor
	public void OnDrawGizmosSelected () {
		if (target!=null){
			Gizmos.DrawIcon(desiredPosition, "camera.png");
			
			// draw circle to illustrate distance to target
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (transform.position, distance);
			if (virtualCameraCollisionTest){
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere (transform.position, virtualCameraCollisionRadius);
			}
			
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(target.position+Vector3.up*(targetHeight*0.5f), target.position+Vector3.up*(targetHeight*0.5f));
			
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(target.position, targetMinimumRenderingDistance);
		}	
	}
}

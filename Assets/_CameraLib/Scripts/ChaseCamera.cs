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
	
	/// <summary>
	/// The controller look horizontal. Value is between -1 and 1
	/// </summary>
	public float  lookHorizontal = 0; 
	private float lookHorizontalActual = 0;
	private float lookHorizontalVelocity = 0;
	public bool lookHorizontalSpringDamped = true;
	public float lookHorizontalSpringStiffness = 36;
	public float lookHorizontalSpringDamping = 12;
	
	// Use this for initialization
	void Start () {
		lastCameraTargetPosition = transform.position;
		
		UpdateIdealSpherical();
	} 
	
	private void UpdateIdealSpherical(){
		idealSpherical.x = distance;
		idealSpherical.z = Mathf.Asin(cameraHeight/distance);
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
	public override void UpdateCameraPosition () {
		Vector3 position = GetCameraTargetPosition();
		if (springSmoothingEnabled){
			transform.position = Damping.SpringDamping(transform.position,position,ref velocity,springStiffness,springDamping);
		} else {
			transform.position = position;
		}
	}
	
	/**
	 * This is the optimal camera position that the camera moves towards
	 */
	public override Vector3 GetCameraTargetPosition(){
		if (target==null){
			return lastCameraTargetPosition;
		}
		
		if (cameraType==ChaseCameraType.StayBehind){
			Vector3 rotation = target.transform.rotation.eulerAngles;
			idealSpherical.y = -rotation.y*Mathf.Deg2Rad-(Mathf.PI*0.5f);
			if (lookHorizontalSpringDamped){
				lookHorizontalActual = Damping.SpringDamping(lookHorizontalActual,lookHorizontal, ref lookHorizontalVelocity,
					lookHorizontalSpringStiffness,lookHorizontalSpringDamping);
				idealSpherical.y += lookHorizontalActual*(2*Mathf.PI);
				 
				// normalize
				// ???
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
		lastCameraTargetPosition = target.position+Vector3Ext.SphericalToCartesian(idealSpherical);
		
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
	}
}

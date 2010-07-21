using UnityEngine;
using System.Collections;

[AddComponentMenu("CameraLib/Chase Camera")]
[RequireComponent (typeof (Camera))]
public class ChaseCamera : ICamera {
	public float cameraHeight = 2;
	public float distance = 3;
	
	private Vector3 lastCameraTargetPosition = Vector3.zero;
	public Vector3 idealSpherical = new Vector3();
	
	public bool stayBehindTarget = true;
	public bool allowMovementUnderCamera = false;
	
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
	
	// Use this for initialization
	void Start () {
		lastCameraTargetPosition = transform.position;
		
		UpdateIdealSpherical();
	} 
	
	private void UpdateIdealSpherical(){
		idealSpherical.x = distance;
		idealSpherical.z = Mathf.Asin(cameraHeight/distance);
		
	}
	
	public override void InitCamera(){
		// unused
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
			return transform.position;
		}
		
		if (stayBehindTarget){
			Vector3 rotation = target.transform.rotation.eulerAngles;
			idealSpherical.y = -rotation.y*Mathf.Deg2Rad-(Mathf.PI*0.5f);
		} else {
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

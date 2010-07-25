using UnityEngine;
using System.Collections;

/// <summary>
/// A implementation of Unitys smooth follow script (Standard assets) implemented to fit this library.
/// 
/// Original documentation:
/// This camera smoothes out rotation around the y-axis and height.
/// Horizontal Distance to the target is always fixed.
/// 
/// There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.
/// 
/// For every of those smoothed values we calculate the wanted value and the current value.
/// Then we smooth it using the Lerp function.
/// Then we apply the smoothed values to the transform's position.
/// </summary>
[AddComponentMenu("CameraLib/Smooth follow")]
[RequireComponent (typeof (Camera))]
public class UnitySmoothFollow : AbstractCamera {
	/// <summary>
	/// The distance in the x-z plane to the target
	/// </summary> 
	public float distance = 10.0f;
	/// <summary>
	/// the height we want the camera to be above the target
	/// </summary> 
	public float height = 5.0f;
	public float  heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	
	// Use this for initialization
	void Start () {
		this.smoothLookAtEnabled = false; // disable smooth rotation
	}
	
	// Update is called once per frame
	public override void UpdateCameraPosition() {
		// Early out if we don't have a target
		if (target == null){
			return;
		}
		
		GetCameraDesiredPosition();
	}
	
	public override Vector3 GetCameraDesiredPosition(){
		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;
		
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;
	
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		Vector3 tmp = transform.position;
		tmp.y = currentHeight;
		transform.position = tmp;
		return transform.position;
	}
}
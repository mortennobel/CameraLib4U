using UnityEngine;
using System.Collections;

/// <summary>
/// Delegames most of the methods all cameras
/// </summary>
[AddComponentMenu("CameraLib/Composite Camera")]
[RequireComponent (typeof (Camera))]
public class CompositeCamera : ICamera {

	public ICamera[] cameras;
	public int currentCamera = 0;
	public int lastCamera = -1;
	private float interpolationTime = 0;
	private float maxInterpolationTime = 0;
	private float maxInterpolationTimeInv = 0;
	
	// An ease in, ease out animation curve (tangents are all flat)
	public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

	
	public override Vector3 GetCameraTargetPosition(){
		// not used here
		return Vector3.zero;
	}
	
	public override void SetTarget(Transform target){
		this.target = target;
		cameras[currentCamera].SetTarget(target);
	}
	
	public override Transform GetTarget(){
		return null;
	}
	
	public override void InitCamera(){
		// do nothing
	}
	
	public void SetCamera(int newCamera, float time){
		if (newCamera == currentCamera){
			// cannot change camera to self
			return;
		}
		
		if (time>0 && lastCamera==-1){
			maxInterpolationTimeInv = 1/time;
			interpolationTime = 0;
			maxInterpolationTime = time;
			lastCamera = currentCamera;
		} else {
			// if interpolation is already running, then do jumpcut
			lastCamera=-1;
		}
		currentCamera = newCamera;
		cameras[currentCamera].SetTarget(target);
		cameras[currentCamera].InitCamera();
		this.smoothLookAtEnabled = cameras[currentCamera].smoothLookAtEnabled;
		this.smoothLookAtDamping = cameras[currentCamera].smoothLookAtDamping;
	}
	
	public int GetCurrentCamera(){
		return currentCamera;
	}
	
	public void SetCamera(int newCamera){
		SetCamera(newCamera,0f);
	}
	
	// Use this for initialization
	void Start () {
		// disables all cameras (since UpdateCameraPosition() is called manually)
		for (int i=0;i<cameras.Length;i++){
			if (cameras[i]!=null){
				cameras[i].enabled = false;
				cameras[i].camera.enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	public override void UpdateCameraPosition () {
		cameras[currentCamera].UpdateCameraPosition();
		if (lastCamera != -1){
			interpolationTime += Time.deltaTime;
			cameras[lastCamera].UpdateCameraPosition();
			float interpol = curve.Evaluate(interpolationTime*maxInterpolationTimeInv);
			transform.position = Vector3.Lerp(cameras[lastCamera].transform.position,cameras[currentCamera].transform.position,interpol);
			if (interpolationTime>maxInterpolationTime){
				lastCamera = -1;
				Debug.Log("Done interpolating. Camera should now be "+currentCamera);
			}
		} else {
			transform.position = cameras[currentCamera].transform.position;
		}
	}
}

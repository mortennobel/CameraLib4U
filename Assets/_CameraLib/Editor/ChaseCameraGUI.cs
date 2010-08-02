using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (ChaseCamera))]
public class ChaseCameraGUI : Editor {
	
	private bool springFoldout = false;
	private bool lookAtDamping = false;
	private bool lookHorizontal = false;
	private bool lookVertical = false;
	private bool moveBackSpringFoldout = false;
	private bool debug = false;

	public override void OnInspectorGUI ()
	{
		ChaseCamera chaseCamera;

		chaseCamera = target as ChaseCamera;

		if (chaseCamera == null)
		{
			return;
		}
		EditorGUILayout.BeginHorizontal ();
    	EditorGUILayout.PrefixLabel ("Target");
    	chaseCamera.target = (Transform)EditorGUILayout.ObjectField(chaseCamera.target, typeof(Transform));
    	EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
    	chaseCamera.targetHeight = EditorGUILayout.FloatField("Target height",chaseCamera.targetHeight);
    	EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
    	chaseCamera.targetMinimumRenderingDistance = EditorGUILayout.FloatField("Min. distance", chaseCamera.targetMinimumRenderingDistance);
    	EditorGUILayout.EndHorizontal ();
		
		
		EditorGUILayout.BeginHorizontal ();
		chaseCamera.cameraType = (ChaseCamera.ChaseCameraType)EditorGUILayout.EnumPopup("Camera type", chaseCamera.cameraType);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
    	EditorGUILayout.PrefixLabel ("Distance");
    	chaseCamera.distance = EditorGUILayout.FloatField(chaseCamera.distance);
    	EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
    	
		EditorGUILayout.PrefixLabel ("Camera pitch");
    	chaseCamera.cameraPitch = EditorGUILayout.Slider(chaseCamera.cameraPitch*Mathf.Rad2Deg,0,90)*Mathf.Deg2Rad;
    	EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		string cameraHeight = ""+chaseCamera.GetCameraHeight();
		EditorGUILayout.LabelField("Camera height", cameraHeight);
    	EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal ();
        chaseCamera.virtualCameraCollisionTest = 
					EditorGUILayout.Toggle("Collision Test",chaseCamera.virtualCameraCollisionTest);
    		EditorGUILayout.EndHorizontal ();
		if (chaseCamera.virtualCameraCollisionTest){
			EditorGUILayout.BeginHorizontal ();
	    	EditorGUILayout.PrefixLabel ("CollisionRadius");
	    	chaseCamera.virtualCameraCollisionRadius = EditorGUILayout.FloatField(chaseCamera.virtualCameraCollisionRadius);
	    	EditorGUILayout.EndHorizontal ();
			
			moveBackSpringFoldout = EditorGUILayout.Foldout(moveBackSpringFoldout, "Moveback spring");
			if (moveBackSpringFoldout){
				EditorGUISpringDampingStat(ref chaseCamera.vccMoveBackSpringStiffness, 
			                                                  ref chaseCamera.vccMoveBackSpringDamping);
				EditorGUILayout.Separator();
			}
		}
		
		springFoldout =EditorGUILayout.Foldout(springFoldout, "Movement spring damping");
		if (springFoldout){
			EditorGUILayout.BeginHorizontal ();
        		chaseCamera.movementSpringDampingEnabled = 
					EditorGUILayout.Toggle("Enabled",chaseCamera.movementSpringDampingEnabled);
    		EditorGUILayout.EndHorizontal ();
		
			
			if (chaseCamera.movementSpringDampingEnabled){
				EditorGUISpringDampingStat(ref chaseCamera.movementSpringStiffness, 
			                                                  ref chaseCamera.movementSpringDamping);
			}
		}
		if (chaseCamera.cameraType==ChaseCamera.ChaseCameraType.StayBehind){
			lookVertical = EditorGUILayout.Foldout(lookVertical, "User look horizontal damping");
			if (lookVertical){
				EditorGUILayout.BeginHorizontal ();
	        	chaseCamera.lookHorizontalSpringDamped = 
					EditorGUILayout.Toggle("Enabled",chaseCamera.lookHorizontalSpringDamped);
	        	EditorGUILayout.EndHorizontal ();
					
				if (chaseCamera.movementSpringDampingEnabled){
					EditorGUISpringDampingStat(ref chaseCamera.lookHorizontalSpringStiffness, 
					                                                  ref chaseCamera.lookHorizontalSpringDamping);
				}
			}
			
			lookHorizontal = EditorGUILayout.Foldout(lookHorizontal, "User look vertical damping");
			if (lookHorizontal){
				EditorGUILayout.BeginHorizontal ();
	        	chaseCamera.lookVerticalSpringDamped = 
					EditorGUILayout.Toggle("Enabled",chaseCamera.lookVerticalSpringDamped);
	        	EditorGUILayout.EndHorizontal ();
					
				if (chaseCamera.lookVerticalSpringDamped){
					EditorGUISpringDampingStat(ref chaseCamera.lookHorizontalSpringStiffness, 
					                                                  ref chaseCamera.lookHorizontalSpringDamping);
				}
			}
		}
		
		lookAtDamping = EditorGUILayout.Foldout(lookAtDamping, "LookAt");
		if (lookAtDamping){
			ChaseCameraGUI.EditorGUISmoothLookAt(chaseCamera);
		}
		debug =EditorGUILayout.Foldout(debug, "Debug");
		if (debug){
			this.DrawDefaultInspector();
		}	
	}
	
	public static void EditorGUISpringDampingStat(ref float springStiffness, ref float springDamping){
		
		EditorGUILayout.BeginHorizontal ();
    	springStiffness = Mathf.Abs(EditorGUILayout.FloatField("Stiffness",springStiffness));
    	EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
    	springDamping = Mathf.Abs(EditorGUILayout.FloatField("Damping",springDamping));
    	EditorGUILayout.EndHorizontal ();
		
		float dampingRatio =Damping.GetSpringDampingRatio(springStiffness, springDamping);
		EditorGUILayout.BeginHorizontal ();
    	EditorGUILayout.LabelField("Damping ratio", ""+dampingRatio);
    	EditorGUILayout.EndHorizontal ();	
		EditorGUILayout.BeginHorizontal ();
		string dampingType = "";
		if (dampingRatio<0.99f){
			 dampingType ="Underdamped";
		} else if (dampingRatio>1.00f){
			dampingType ="Overdamped";
		} else {
			dampingType ="Critically damped";
		}
    	EditorGUILayout.LabelField("Damping type", ""+dampingType);
    	EditorGUILayout.EndHorizontal ();	
		EditorGUILayout.BeginHorizontal ();
	    bool res = GUILayout.Button("Compute critically damped from stiffness");
	    if (res){
			springDamping = 2*Mathf.Sqrt(springStiffness);	
		}
		EditorGUILayout.EndHorizontal ();
	}
	
	public static void EditorGUISmoothLookAt(AbstractCamera camera){
		EditorGUILayout.BeginHorizontal ();
    	camera.smoothLookAtEnabled = 
			EditorGUILayout.Toggle("Rotation damping Enabled",camera.smoothLookAtEnabled);
    	EditorGUILayout.EndHorizontal ();
		if (camera.smoothLookAtEnabled){
			EditorGUILayout.BeginHorizontal ();
	    	camera.smoothLookAtDamping = 
				EditorGUILayout.FloatField("Damping",camera.smoothLookAtDamping);
	    	EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.BeginHorizontal ();
    	camera.lookAtRotationOffset = 
			Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation offset",camera.lookAtRotationOffset.eulerAngles));
    	EditorGUILayout.EndHorizontal ();
	}
}
	
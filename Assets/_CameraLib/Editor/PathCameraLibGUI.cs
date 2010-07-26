using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor (typeof (PathBoundCamera))]
public class PathCameraLibGUI : Editor {
	
	bool springFoldout =false;
	bool jumpcutFoldout =false;
	bool lookAtDamping = false;

	public override void OnInspectorGUI ()
	{
		PathBoundCamera pathCamera;

		pathCamera = target as PathBoundCamera;

		if (pathCamera == null)
		{
			return;
		}
		
		EditorGUILayout.BeginHorizontal ();
    	EditorGUILayout.PrefixLabel ("Target");
    	pathCamera.target = EditorGUILayout.ObjectField(pathCamera.target, typeof(Transform)) as Transform;
    	EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
    	EditorGUILayout.PrefixLabel ("Spline");
    	pathCamera.cameraSpline = EditorGUILayout.ObjectField(pathCamera.cameraSpline, typeof(SplineComponent)) as SplineComponent;
    	EditorGUILayout.EndHorizontal ();
		
		
		EditorGUILayout.BeginHorizontal ();
    	pathCamera.maxDistanceToTarget = EditorGUILayout.FloatField("Max distance to target",pathCamera.maxDistanceToTarget);
    	EditorGUILayout.EndHorizontal ();
		
		jumpcutFoldout =EditorGUILayout.Foldout(jumpcutFoldout, "Jump-cut");
		if (jumpcutFoldout){
			EditorGUILayout.BeginHorizontal ();
	    	pathCamera.maxDistanceToJumpCut = EditorGUILayout.FloatField("Max distance",pathCamera.maxDistanceToJumpCut);
	    	EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.BeginHorizontal ();
	    	pathCamera.minTimeBetweenDistancebasedJumpcut = EditorGUILayout.FloatField("Min time between",pathCamera.minTimeBetweenDistancebasedJumpcut);
	    	EditorGUILayout.EndHorizontal ();
		}
		springFoldout =EditorGUILayout.Foldout(springFoldout, "Movement spring damping");
		if (springFoldout){
			EditorGUILayout.BeginHorizontal ();
        	pathCamera.movementSpringDampingEnabled = EditorGUILayout.Toggle("Enabled",pathCamera.movementSpringDampingEnabled);
        	EditorGUILayout.EndHorizontal ();
			
				
			if (pathCamera.movementSpringDampingEnabled){
				ChaseCameraGUI.EditorGUISpringDampingStat(ref pathCamera.movementSpringStiffness, 
				                                                  ref pathCamera.movementSpringDamping);
			}
		}
		lookAtDamping =EditorGUILayout.Foldout(lookAtDamping, "LookAt damping");
		if (lookAtDamping){
			ChaseCameraGUI.EditorGUISmoothLookAt(pathCamera);
		}
	}
}


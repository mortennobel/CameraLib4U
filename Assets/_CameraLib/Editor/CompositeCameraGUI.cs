using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor (typeof (CompositeCamera))]
public class CompositeCameraGUI : Editor {
	
	private bool camerasFoldout = false;
	private bool lookAtDamping = false;
	
	public override void OnInspectorGUI ()
	{
		CompositeCamera compositeCamera;

		compositeCamera = target as CompositeCamera;

		if (compositeCamera == null)
		{
			return;
		}
		
		camerasFoldout = EditorGUILayout.Foldout(camerasFoldout, "Cameras");
		if (camerasFoldout){
			EditorGUILayout.BeginHorizontal ();
			string sp = "    ";
			int newSize = EditorGUILayout.IntField(sp+"Size",compositeCamera.cameras.Length);
    		EditorGUILayout.EndHorizontal ();
    		 
			if (newSize != compositeCamera.cameras.Length){
				AbstractCamera[] newArray = new AbstractCamera[newSize];
				Array.Copy(compositeCamera.cameras, newArray, Math.Min(newSize, compositeCamera.cameras.Length));
				compositeCamera.cameras = newArray;
			}
			for (int i=0;i<compositeCamera.cameras.Length;i++){
				EditorGUILayout.BeginHorizontal ();
				compositeCamera.cameras[i] = (AbstractCamera)EditorGUILayout.ObjectField(sp+"Element "+i, compositeCamera.cameras[i], typeof(AbstractCamera));
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.Separator ();
		EditorGUILayout.BeginHorizontal ();
		compositeCamera.currentCamera = EditorGUILayout.IntSlider("Current camera", Math.Min( compositeCamera.currentCamera,compositeCamera.cameras.Length-1),0, compositeCamera.cameras.Length-1);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		compositeCamera.curve = EditorGUILayout.CurveField("Interpolation", compositeCamera.curve);
		EditorGUILayout.EndHorizontal ();
		
		lookAtDamping =EditorGUILayout.Foldout(lookAtDamping, "LookAt");
		if (lookAtDamping){
			ChaseCameraGUI.EditorGUISmoothLookAt(compositeCamera);
		}
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (SplineComponent))]
public class SplineLibGUI : Editor {
	bool editorGUiFoldout =false;
	
	[MenuItem ("GameObject/Create Other/Spline Curve")]
	public static void CreateSplineCurve ()
	{
		// Make a game object
    	GameObject splineCurve = new GameObject ("Spline Curve");
		// Add the splineComponent component
    	splineCurve.AddComponent("SplineComponent");
		
		for (int i=0;i<4;i++) {
			// Make a game object
    		GameObject controlPoint = new GameObject ("ControlPoint");
			// Add the splineComponent component
    		controlPoint.AddComponent("ControlPointComponent");
			controlPoint.transform.position = new Vector3(i,0,0);
			controlPoint.transform.parent = splineCurve.transform;
		} 
	}
	
	public void CreateNode(SplineComponent splineComponent){
		GameObject controlPoint = new GameObject ("ControlPoint");
		// Add the splineComponent component
    	controlPoint.AddComponent("ControlPointComponent");
		
		SplineCurve splineCurve = splineComponent.GetSplineObject();
		Vector3 targetPosition = Vector3.one;
		Vector3[] controlPoints = splineCurve.controlPoints;
		int cpLength = controlPoints.Length;
		int controlPointsPerSegment = splineComponent.GetControlPointsPerSegment();
		if (cpLength>=controlPointsPerSegment){
			// extrapolate the position from the last two points
			targetPosition = controlPoints[cpLength-1]+(controlPoints[cpLength-1]-controlPoints[cpLength-controlPointsPerSegment]);
		}
		controlPoint.transform.position = targetPosition;
		controlPoint.transform.parent = splineComponent.transform;
	}
	
	public override void OnInspectorGUI ()
	{
		SplineComponent splineComponent;

		splineComponent = target as SplineComponent;

		if (splineComponent == null)
		{
			return;
		}
		
		SplineComponent.SplineType newType = (SplineComponent.SplineType)EditorGUILayout.EnumPopup("Spline Type", splineComponent.splineType);
		if (newType!=splineComponent.splineType){
			splineComponent.splineType =newType;
			splineComponent.UpdateSpline();	
		}
		
		EditorGUILayout.BeginHorizontal ();
	    float newLenPre =EditorGUILayout.FloatField("Length precision",splineComponent.lengthPrecision);
	    if (newLenPre!=splineComponent.lengthPrecision){
			splineComponent.lengthPrecision =newLenPre;
			splineComponent.DoUpdateSpline();
		}
		EditorGUILayout.EndHorizontal ();
		if (splineComponent.splineType==SplineComponent.SplineType.KochanekBartel){
			float bias = splineComponent.bias;
			float tension = splineComponent.tension;
			float continuity = splineComponent.continuity;
			
			EditorGUILayout.BeginHorizontal ();
		    splineComponent.bias = EditorGUILayout.FloatField("Bias",splineComponent.bias);
		    EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.BeginHorizontal ();
		    splineComponent.tension = EditorGUILayout.FloatField("Tension",splineComponent.tension);
		    EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.BeginHorizontal ();
		    splineComponent.continuity = EditorGUILayout.FloatField("Continuity",splineComponent.continuity);
		    EditorGUILayout.EndHorizontal ();
			if (bias != splineComponent.bias || 
				tension != splineComponent.tension || 
				continuity != splineComponent.continuity){
				splineComponent.DoUpdateSpline();
			}
		}
			
		EditorGUILayout.BeginHorizontal();
    	EditorGUILayout.LabelField("Spline length",splineComponent.GetSplineObject().totalTime.ToString());
    	EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		bool update = GUILayout.Button("Update spline"); 
    	if (update){
			splineComponent.UpdateSpline();	
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator(); 
		editorGUiFoldout =EditorGUILayout.Foldout(editorGUiFoldout, "Editor GUI");
		if (editorGUiFoldout){
			splineComponent.alwaysDrawGizmo = EditorGUILayout.Toggle("Always draw gizmo", splineComponent.alwaysDrawGizmo);	
		
			EditorGUILayout.BeginHorizontal ();
		    splineComponent.sphereRadius = EditorGUILayout.FloatField("Sphere radius",splineComponent.sphereRadius);
		    EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator(); 
			
			splineComponent.splineColor = EditorGUILayout.ColorField("Spline Color", splineComponent.splineColor);	
			splineComponent.controlPointColor = EditorGUILayout.ColorField("ControlPoint Color", splineComponent.controlPointColor);	
			splineComponent.controlPointColorSelected = EditorGUILayout.ColorField("ControlPoint Color Selected", splineComponent.controlPointColorSelected);
		}
		EditorGUILayout.Separator(); 
		EditorGUILayout.BeginHorizontal ();
	    bool res = GUILayout.Button("Create node");
	    if (res){
			CreateNode(splineComponent);
		}
		EditorGUILayout.EndHorizontal ();
	}
}

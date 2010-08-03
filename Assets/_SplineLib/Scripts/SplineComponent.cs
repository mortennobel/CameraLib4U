using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplineComponent : MonoBehaviour {
	
	public enum SplineType { LinearSpline,BezierSpline,BezierSmoothSpline, CatmullRom };
	
	public SplineType splineType = SplineType.LinearSpline;
	public SplineCurve spline;
	public bool alwaysDrawGizmo = true;
	public Color splineColor = Color.white;
	public Color controlPointColor = Color.yellow;
	public Color controlPointColorSelected = Color.green;
	
	public float sphereRadius = 0.1f;
	public float lengthPrecision =0.001f;
	
	private bool updated = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	public void OnDrawGizmosSelected () {
		if (!alwaysDrawGizmo){
			DrawGizmo();
		}
	}
	
	public SplineCurve GetSplineObject(){
		if (spline==null){
			 DoUpdateSpline();
		}
		return spline;
	}
	
	public void UpdateSpline(){
		updated =false;
	}
	
	public void DoUpdateSpline(){
		List<Vector3> controlPoints = new List<Vector3>();
		List<Vector3> tangents = new List<Vector3>();
		List<float> time = new List<float>();
		float lastTime = 0;
		for (int i=0;i<transform.childCount;i++){
			ControlPointComponent controlPointComp = transform.GetChild(i).gameObject.GetComponent<ControlPointComponent>();
			if (controlPointComp!=null){
				controlPoints.Add(controlPointComp.position);
				tangents.Add(controlPointComp.tangent);
				// if illegal time detected, then autoadjust
				if (lastTime>=controlPointComp.time){
					controlPointComp.time += lastTime+1; 
				}
				lastTime = controlPointComp.time;
				time.Add(controlPointComp.time);
			}
		}
		
		switch (splineType){
		case SplineType.LinearSpline:{
			LinearSplineCurve lSpline = new LinearSplineCurve();
			spline = lSpline;
			spline.lengthPrecision =lengthPrecision;
			lSpline.Init(controlPoints.ToArray(), time.ToArray());
			}
			break;
		case SplineType.CatmullRom:{
			HermiteSplineCurve hSpline = new HermiteSplineCurve();
			spline = hSpline;
			spline.lengthPrecision =lengthPrecision;
			hSpline.InitCatmullRom(controlPoints.ToArray());
			}
			break;
		case SplineType.BezierSpline:
		case SplineType.BezierSmoothSpline:
			BezierSplineCurve bSpline = new BezierSplineCurve();
			spline = bSpline;
			spline.lengthPrecision =lengthPrecision;
			switch (splineType){
			case SplineType.BezierSpline:
				bSpline.Init(controlPoints.ToArray());
				break;
			case SplineType.BezierSmoothSpline:
				bSpline.InitSmoothTangents(controlPoints.ToArray());
				break;
			}
			break;
		}	
		
		// hack to make the editor update the obejct
		Vector3 p = transform.position;
		transform.position = Vector3.zero;
		if (p.sqrMagnitude > 0){
			transform.position = p;
		}
		
		updated = true;
	}
	
	private void DrawGizmo(){
		Gizmos.color = controlPointColor;
		for (int i=0;i<transform.childCount;i++){
			ControlPointComponent splineComponent = transform.GetChild(i).gameObject.GetComponent<ControlPointComponent>();
			if (splineComponent!=null){
				Gizmos.DrawSphere(splineComponent.position, sphereRadius);
			}
		}
		
		// only call when updates 
		if (!updated || spline ==null){
			DoUpdateSpline();
		}
		
		float[] fs = spline.GetRenderPoints();
		Vector3[] vs = new Vector3[fs.Length];
		for (int i=0;i<fs.Length;i++){
			vs[i] = spline.GetPosition(fs[i]);
		}
		for (int i=1;i<vs.Length;i++){
			Gizmos.color = splineColor;
			Gizmos.DrawLine(vs[i-1],vs[i]);
		}	
	}
	
	/// <summary>
	/// Returns the number of controlpoints for each segment.
	/// </summary>
	public int GetControlPointsPerSegment(){
		switch (splineType){
			case SplineType.LinearSpline:
			case SplineType.CatmullRom:
			return 2;
		case SplineType.BezierSpline:
			return 4;
		case SplineType.BezierSmoothSpline:
		default:
			return 3;	
		}
	}
	
	
	public void OnDrawGizmos(){
		if (alwaysDrawGizmo){
			DrawGizmo();
		}
	}
}

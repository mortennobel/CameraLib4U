using UnityEngine;
using System.Collections;
using System;

public class LinearSplineCurve : SplineCurve {
	public void Init(Vector3[] controlPoints) {
		if (Debug.isDebugBuild) {
			if (controlPoints.Length<2){
				throw new Exception("Minimum number of controlpoints is two");
			}			
		}
		this.controlPoints = new Vector3[controlPoints.Length];
		this.time = new float[controlPoints.Length];
		Array.Copy(controlPoints,this.controlPoints,controlPoints.Length);
		calculateSegmentLength();
	}
			
	public override Vector3 GetPosition(float time){
		int i =GetSegmentIndex(time);
		float t0 = this.time[i];
		float t1 = this.time[i+1];
		float u = (time-t0)/(t1-t0);
		
		return controlPoints[i]*(1-u)+controlPoints[i+1]*u;
	} 
	
	/// <summary>
	/// Note: this only returns the correct direction (for line segments) - may not return the correct length.
	/// </summary>
	public override Vector3 GetVelocity(float time){
		int i =GetSegmentIndex(time);
		return controlPoints[i+1]-controlPoints[i];
	}
}

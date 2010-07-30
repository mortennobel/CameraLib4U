using UnityEngine;
using System.Collections;
using System;

public class LinearSplineCurve : SplineCurve {
	public void Init(Vector3[] controlPoints, float[] time) {
		if (Debug.isDebugBuild) {
			if (controlPoints.Length<2){
				throw new Exception("Minimum number of controlpoints is two");
			}
			if (time.Length!=controlPoints.Length){
				throw new Exception("Time length should equal controlpoint length");
			}
			for (int i=1;i<time.Length;i++){
				if (time[i-1] >= time[i]){
					throw new Exception ("Time should be increasing");
				}
			}
		}
		this.controlPoints = new Vector3[controlPoints.Length];
		this.time = new float[controlPoints.Length];
		Array.Copy(controlPoints,this.controlPoints,controlPoints.Length);
		Array.Copy(time, this.time, time.Length);
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
		int i = GetSegmentIndex(time);
		return controlPoints[i+1]-controlPoints[i];
	}
}

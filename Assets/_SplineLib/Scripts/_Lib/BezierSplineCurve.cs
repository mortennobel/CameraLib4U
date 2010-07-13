using UnityEngine;
using System.Collections;
using System;

public class BezierSplineCurve : SplineCurve {
	
	private Vector3[] mApproximatingControlPoints;
	
	public Vector3[] approximatingControlPoints{
		get{
			return mApproximatingControlPoints;
		}
		protected set{
			mApproximatingControlPoints=value;
		}
	}
		
	public void Init(Vector3[] controlPoints, Vector3[] approximatingControlPoints){
		if (Debug.isDebugBuild) {
			if (controlPoints.Length<2){
				throw new Exception("Min 2 controlpoints must be defined");
			}
			if (approximatingControlPoints.Length!=controlPoints.Length/2+1){
				throw new Exception("Invalid number of controlPoints compared to number of approximatingControlPoints");
			}
		}
		this.controlPoints = new Vector3[controlPoints.Length];
		this.approximatingControlPoints = new Vector3[approximatingControlPoints.Length];
		this.time = new float[controlPoints.Length];
		Array.Copy(controlPoints, this.controlPoints, controlPoints.Length);
		Array.Copy(approximatingControlPoints, this.approximatingControlPoints, approximatingControlPoints.Length);
		Array.Copy(time, this.time, time.Length);
		calculateSegmentLength();
	}
	
	/// <summary>
	/// Uses the the inControlPoint = - outControlPoint 
	/// controlpoints must have the structure (note the first segment is 4 nodes - the rest is only three
	/// &lt;cp, acp, acp, cp, acp, cp, ... cp, acp, cp&gt;
	/// </summary>
	/// <param name='controlPoints'>
	/// Control points.
	/// </param>
	/// <param name='time'>
	/// Time.
	/// </param>
	public void InitSmoothTangents(Vector3[] controlPoints){
		int usableLength = controlPoints.Length-(controlPoints.Length%2);
		this.controlPoints = new Vector3[(usableLength)/2];
		this.time = new float[this.controlPoints.Length];
		this.approximatingControlPoints = new Vector3[(this.controlPoints.Length-1)*2];
		
		this.controlPoints[0] = controlPoints[0];
		this.approximatingControlPoints[0] = controlPoints[1];
		for (int i=3;i<usableLength;i=i+2){
			int index = i/2;
			int aIndex = (index-1)*2;
			
			this.controlPoints[index] = controlPoints[i];
			
			if (aIndex>0){
				this.approximatingControlPoints[aIndex] = controlPoints[i-2]-(controlPoints[i-3]-controlPoints[i-2]);
			}
			this.approximatingControlPoints[aIndex+1] = controlPoints[i-1];
		}
		calculateSegmentLength();
	}
	
	/// <summary>
	/// controlpoints must have the structure
	/// &lt;cp, acp, acp, cp, acp, acp, cp, ... ,cp&gt; and hence must have the length 1+3*controlPoints
	/// Time must either be specified for all controlpoints or the lenth of the final number of 
	/// controlpoints
	/// </summary>
	/// <param name='controlPoints'>
	/// Control points.
	/// </param>
	/// <param name='time'>
	/// Time.
	/// </param>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
	public void Init(Vector3[] controlPoints){
		if (Debug.isDebugBuild) {
			if (controlPoints.Length<4){
				throw new Exception("Min length is 4");
			}
		}
		int usableLength = controlPoints.Length-((controlPoints.Length-1)%3);
		this.controlPoints = new Vector3[(usableLength)/3+1];
		this.approximatingControlPoints = new Vector3[(this.controlPoints.Length-1)*2];
		this.time = new float[this.controlPoints.Length];
		this.controlPoints[0] = controlPoints[0];
		
		for (int i=1;i<usableLength;i=i+3){
			int index = i/3+1;
			int approxIndex = (i/3)*2;
			
			approximatingControlPoints[approxIndex] = controlPoints[i];
			approximatingControlPoints[approxIndex+1] = controlPoints[i+1];
			this.controlPoints[index] = controlPoints[i+2];
		}
		calculateSegmentLength();
	}
	
	public override Vector3 GetPosition(float time){
		if (time>=this.time[this.time.Length-1]){
			return controlPoints[controlPoints.Length-1];
		}
		else if (time<=this.time[0]){
			return controlPoints[0];
		}
		
		int i = GetSegmentIndex(time);
		
		float t0 = this.time[i];
		float t1 = this.time[i+1];
		float u = (time-t0)/(t1-t0);
		
    	Vector3 A = controlPoints[i+1]
                - 3.0f*approximatingControlPoints[2*i+1]
                + 3.0f*approximatingControlPoints[2*i]
                - controlPoints[i];
    	Vector3 B = 3.0f*approximatingControlPoints[2*i+1]
                - 6.0f*approximatingControlPoints[2*i]
                + 3.0f*controlPoints[i];
    	Vector3 C = 3.0f*approximatingControlPoints[2*i]
                - 3.0f*controlPoints[i];
    
    	return controlPoints[i] + u*(C + u*(B + u*A));
	}
	
	public override Vector3 GetVelocity(float time){
		if (time>this.time[this.time.Length-1]){
			time = this.time[this.time.Length-1];
		}
		else if (time<=this.time[0]){
			time = this.time[0];
		}
		
		int i = GetSegmentIndex(time);
		
		float t0 = this.time[i];
		float t1 = this.time[i+1];
		float u = (time-t0)/(t1-t0);

    	Vector3 A = controlPoints[i+1]
                - 3.0f*approximatingControlPoints[2*i+1]
                + 3.0f*approximatingControlPoints[2*i]
                - controlPoints[i];
    	Vector3 B = 6.0f*approximatingControlPoints[2*i+1]
                - 12.0f*approximatingControlPoints[2*i]
                + 6.0f*controlPoints[i];
    	Vector3 C = 3.0f*approximatingControlPoints[2*i]
                - 3.0f*controlPoints[i];
    
    	return C + u*(B + 3.0f*u*A);
	}
	
	public Vector3[] GetApproximatingControlPoints(){
		return approximatingControlPoints;
	}
}
using UnityEngine;
using System.Collections;
using System;

public class HermiteSplineCurve : SplineCurve {
	private Vector3[] tangent0Vectors; // first tangent at a line segment
	private Vector3[] tangent1Vectors; // last tangent at a line segment
	
	public void Init(Vector3[] controlPoints, Vector3[] tangent0Vectors, Vector3[] tangent1Vectors, float[] time){
		if (Debug.isDebugBuild) {
			if (controlPoints.Length-1!=tangent1Vectors.Length){
				throw new Exception("Invalid length of inTangentVectors. Must be one less than control points.");
			}
			if (controlPoints.Length-1!=tangent0Vectors.Length){
				throw new Exception("Invalid length of outTangentVectors. Must be one less than control points.");
			}
			if (time.Length!=controlPoints.Length){
				throw new Exception("Invalid length of time. Must equal controlpoints");
			}
		}
		this.controlPoints = new Vector3[controlPoints.Length];
		this.tangent1Vectors = new Vector3[tangent1Vectors.Length];
		this.tangent0Vectors = new Vector3[tangent0Vectors.Length];
		this.time = new float[time.Length];
		Array.Copy(controlPoints, this.controlPoints, controlPoints.Length);
		Array.Copy(tangent1Vectors, this.tangent1Vectors, tangent1Vectors.Length);
		Array.Copy(tangent0Vectors, this.tangent0Vectors, tangent0Vectors.Length);
		Array.Copy(time, this.time, time.Length);
		calculateSegmentLength();
	}
	
	/// <summary>
	/// Use linear distance between controlpoints as time estimate
	/// </summary>
	public void InitKochanekBartel(Vector3[] controlPoints, float bias, float tension, float continuity){
		float[] time = new float[controlPoints.Length];
		float totalTime = 0;
		time[0] = 0;
		for (int i=1;i<controlPoints.Length;i++){
			totalTime += (controlPoints[i]-controlPoints[i-1]).magnitude;
			time[i] = totalTime;
		}
		InitKochanekBartel(controlPoints,time,bias,tension,continuity);
	}
	
	public void InitKochanekBartel(Vector3[] controlPoints, float[] time, float bias, float tension, float continuity){
		Vector3[] tangent1Vectors = new Vector3[controlPoints.Length-1];
		Vector3[] tangent0Vectors = new Vector3[controlPoints.Length-1];
		for (int i=0;i<controlPoints.Length;i++){
			Vector3 pm1 = i>0?controlPoints[i-1]:controlPoints[i]+(controlPoints[i]-controlPoints[i+1]); // point minus one
			Vector3 p = controlPoints[i];
			Vector3 pp1 = i<controlPoints.Length-1?controlPoints[i+1]:controlPoints[i]+(controlPoints[i]-controlPoints[i-1]); // point plus one
			
			Vector3 si = ((1-bias)*(1+tension)*(1-continuity)/2)*(p-pm1)+((1-bias)*(1-tension)*(1+continuity)/2)*(pp1-p);		
			Vector3 di = ((1-bias)*(1+tension)*(1+continuity)/2)*(p-pm1)+((1-bias)*(1-tension)*(1-continuity)/2)*(pp1-p);		
			
			float Ni = time[Math.Min(i+1,controlPoints.Length-1)]-time[i];
			float Nim1 = time[i]-time[Math.Max(0,i-1)];
			
			if (i>0){
				//  adjust tangent length
				si = (2*Ni/(Nim1+Ni))*si;
			
				tangent1Vectors[i-1] = si;
			}
			if (i<controlPoints.Length-1){
				//  adjust tangent length
				di = (2*Nim1/(Nim1+Ni))*di;
			
				tangent0Vectors[i] = di;
			}
		}
		Init(controlPoints, tangent0Vectors, tangent1Vectors, time);
	}
	
	public void InitNatural(Vector3[] controlPoints){
		if (Debug.isDebugBuild) {
			if (controlPoints.Length<3){
				throw new Exception("Must use minimum three controlpoints");
			}
		}
		// build 
		int n = controlPoints.Length;
		float[,] A = new float[n,n];
			
		A[0,0] = 2.0f;
		A[0,1] = 1.0f;
		int i;
		for ( i = 1; i < n-1; ++i )
		{
	    	A[i , i - 1] = 1.0f;
	    	A[i , i] = 4.0f;
	    	A[i , i + 1] = 1.0f;
		}
		A[n-1,n-2] = 1.0f;
		A[n-1,n-1] = 2.0f;
		
		Debug.Log("Matrix is :\n"+Matrix.ToString(A));
		
		if (!Matrix.Invert(A)){
			throw new Exception("The linear system cannot be solved");
		}

		// set up arrays
		this.controlPoints = new Vector3[n];
		this.tangent0Vectors = new Vector3[n-1];
		this.tangent1Vectors = new Vector3[n-1];
		this.time = new float[n];
		
		// set up the tangents
		for ( i = 0; i < n; ++i )
		{
	    	// copy position
	    	this.controlPoints[i] = controlPoints[i];
	
	    	// multiply b by inverse of A to get tangents
	    	// compute count-1 incoming tangents
	    	if ( i < n-1 )
	    	{
	        	this.tangent0Vectors[i] = 3.0f*A[i,0]*(controlPoints[1]-controlPoints[0])
	                         + 3.0f*A[i ,n-n]*(controlPoints[n-1]-controlPoints[n-2]);
	        	for ( int j = 1; j < n-1; ++j )
	        	{
	            	Vector3 b_j = 3.0f*(controlPoints[j+1]-controlPoints[j-1]);
	            	this.tangent0Vectors[i] += A[i , j]*b_j;
	       		}
	        	// out tangent is in tangent of next segment
	        	if (i > 0)
	            	this.tangent1Vectors[i-1] = this.tangent0Vectors[i];
	    	}
	    	// compute final outgoing tangent
	    	else
	    	{
	        	this.tangent1Vectors[i-1] = 3.0f*A[i,0]*(controlPoints[1]-controlPoints[0])
	                         + 3.0f*A[i,n-n]*(controlPoints[n-1]-controlPoints[n-2]);
	        	for ( int j = 1; j < n-1; ++j )
	        	{
	            	Vector3 b_j = 3.0f*(controlPoints[j+1]-controlPoints[j-1]);
	            	this.tangent1Vectors[i-1] += A[i,j]*b_j;
	        	}
	    	}
		}
		calculateSegmentLength();
	}
	
	public override Vector3 GetPosition(float time){
		if (controlPoints.Length==1){
			return controlPoints[0];
		}
		int i =GetSegmentIndex(time);
		
		float t0 = this.time[i];
		float t1 = this.time[i+1];
		float u = (time-t0)/(t1-t0);
		
		// simplified matrix multiplication
		Vector3 A = 2.0f*controlPoints[i]
                - 2.0f*controlPoints[i+1]
                + tangent0Vectors[i]
                + tangent1Vectors[i];
    	Vector3 B = -3.0f*controlPoints[i]
                + 3.0f*controlPoints[i+1]
                - 2.0f*tangent0Vectors[i]
                - tangent1Vectors[i];
    
    	return controlPoints[i] + u*(tangent0Vectors[i] + u*(B + u*A));
	}
	
	public override Vector3 GetVelocity(float time){
		if (controlPoints.Length==1){
			return Vector3.zero;
		}
		int i =GetSegmentIndex(time);
		
		float t0 = this.time[i];
		float t1 = this.time[i+1];
		float u = (time-t0)/(t1-t0);
		
		// simplified matrix multiplication
		Vector3 A = 2.0f*controlPoints[i]
                - 2.0f*controlPoints[i+1]
                + tangent0Vectors[i]
                + tangent1Vectors[i];
    	Vector3 B = -3.0f*controlPoints[i]
                + 3.0f*controlPoints[i+1]
                - 2.0f*tangent0Vectors[i]
                - tangent1Vectors[i];
		
    	return tangent0Vectors[i] + u*(2.0f*B + 3.0f*u*A);
	}
}

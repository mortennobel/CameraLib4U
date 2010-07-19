using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class SplineCurve {
	private float[] mTime;
	private Vector3[] mControlPoints;
	public float lengthPrecision = 0.005f;
	public List<float> renderPoints = new List<float>();
	private static int MAX_RECURSION_DEPTH = 20;
	
	public Vector3[] controlPoints{
		get{
			return mControlPoints;
		}
		protected set{
			this.mControlPoints =value;
		}
	}
	
	public float[] time{
		get{
			return mTime;
		}
		protected set{
			this.mTime =value;
		}
	}
	
	public float totalTime{
		get{
			return mTime[mTime.Length-1];
		}
	}
	
	public int GetSegmentIndex(float time){
		int i = 0;
		// interpolate
		for (;i<this.time.Length-2;i++){
			if (time<=this.time[i+1]){
				break;
			}
		}		
		return i;
	}
	
	public float[] GetRenderPoints(){
		if (mTime.Length==0){
			return new float[0];
		}
		if (renderPoints.Count==0){
			renderPoints.Add(0f);
			for (int i=0;i<mTime.Length-1;i++){
				calcRenderPoints(mTime[i], mTime[i+1], 0, MAX_RECURSION_DEPTH);
			}
		}
		return renderPoints.ToArray();
	}
	
	public abstract Vector3 GetPosition(float f);
	
	/// <summary>
	/// Calculate derivative at f
	/// </summary>
	/// <returns>
	/// The velocity.
	/// </returns>
	/// <param name='f'>
	/// F.
	/// </param>
	public abstract Vector3 GetVelocity(float f);
	
	protected void calculateSegmentLength(){
		renderPoints.Clear();
		mTime[0] = 0;
		mTime[mTime.Length-1] = int.MaxValue;
		
		for (int i=0;i<mTime.Length-1;i++){
			// set value i+1 greater than i
			float newValue = (mTime[i]+1)*2;
			mTime[i+1] = newValue;
			
			// compute length
			float calculatedLength = SegmentArcLength(mTime[i], newValue,0, MAX_RECURSION_DEPTH);
			mTime[i+1] = mTime[i]+calculatedLength;
		}
	}
	
	/// <summary>
	/// Calculates the render points.
	/// Basically same algorithm as segment length
	/// </summary>
	private void calcRenderPoints (float u1, float u2, float epsilon, int max){
		Vector3 p1 = GetPosition(u1);
		Vector3 p2 = GetPosition(u2);
		Vector3 p1p2 = p2-p1;
		float p1p2Length =p1p2.magnitude;
		if (epsilon==0){
			epsilon = p1p2Length*lengthPrecision;
		}
		if (max==0){
			renderPoints.Add(u2);
			return;	 
		}
		
		float half = (u2-u1)*0.5f;
		Vector3 midPoint = GetPosition(u1+half);
		
		float quarter = half * 0.5f;
		Vector3 quarterPoint = GetPosition(u1+quarter);
		
		if (IsStraight(p1,midPoint,p2,epsilon) && IsStraight(p1,quarterPoint,midPoint,epsilon)){
			renderPoints.Add(u2);
			return;	  
		}
		max --;
		
		calcRenderPoints(u1, u1+half, epsilon, max-1);
		calcRenderPoints(u1+half, u2, epsilon, max-1);
	}
	
	/// <summary>
	/// Segments the length of the arc.
	/// The calculation uses midpoint subdivision (with an extra check to avoid false positive)
	/// </summary>
	/// <param name='i'>
	/// I. Segment index
	/// </param>
	/// <param name='u1'>
	/// U1. normalized (must be between mTime[i] and u2)
	/// </param>
	/// <param name='u2'>
	/// U2. normalized (must be between u1 and mTime[i+1])
	/// </param>
	private float SegmentArcLength(float u1, float u2, float epsilon, int max){
		Vector3 p1 = GetPosition(u1);
		Vector3 p2 = GetPosition(u2);
		Vector3 p1p2 = p2-p1;
		float p1p2Length =p1p2.magnitude;
		if (epsilon==0){
			epsilon = p1p2Length*lengthPrecision;
		}
		if (max==0){
			return p1p2Length;
		}
		
		float half = (u2-u1)*0.5f;
		Vector3 midPoint = GetPosition(u1+half);
		
		float quarter = half * 0.5f;
		Vector3 quarterPoint = GetPosition(u1+quarter);
		
		if (IsStraight(p1,midPoint,p2,epsilon) && IsStraight(p1,quarterPoint,midPoint,epsilon)){
			return p1p2Length;	  
		}
		max --;
		
		return SegmentArcLength(u1, u1+half,epsilon,max) + SegmentArcLength(u1+half, u2,epsilon, max);
	}
	
	/// <summary>
	/// Note that this method can be optimized if it is inlined in the code.
	/// So if it is used where performance matters this should be done.
	/// The main performance gain is that the same magnitude of the same vectors 
	/// are calculated twice. (SegmentArcLength's p1p2 and p1-midpoint)
	/// </summary>
	private bool IsStraight(Vector3 p1, Vector3 p2, Vector3 p3, float epsilon){
		float p1p3Length = (p1-p3).magnitude;
		float p1p2Length = (p1-p2).magnitude;
		float p2p3Length = (p2-p3).magnitude;
		return Mathf.Abs(p1p3Length-p1p2Length-p2p3Length)<=epsilon;
	}
}

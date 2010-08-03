using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public abstract class SplineCurve {
	private float[] mTime;
	private Vector3[] mControlPoints;
	public float lengthPrecision = 0.001f;
	public List<float> renderPoints = new List<float>();
	private static int MAX_RECURSION_DEPTH = 20;
	
	private struct ParameterValueToArcLength{
		public float parameterValue;
		public float archLength;
		public ParameterValueToArcLength(float parameterValue, float archLength){
			this.parameterValue = parameterValue;
			this.archLength = archLength;
		}
	}
	
	private ParameterValueToArcLength[] valueToLengthMapping;
	
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
	
	public float totalLength{
		get{
			if (valueToLengthMapping==null){
				InitValueToLengthMapping();
			}
			return valueToLengthMapping[valueToLengthMapping.Length-1].archLength;
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
				CalcRenderPoints(mTime[i], mTime[i+1], 0, MAX_RECURSION_DEPTH);
			}
		}
		return renderPoints.ToArray();
	}
	
	public Vector3 GetPositionByLength(float length){
		float time = GetTimeByLength(length);
		return GetPosition(time);
	}
	
	public float GetTimeByLength(float length){
		if (valueToLengthMapping==null){
			InitValueToLengthMapping();
		}
		float time = 0;
		if (length>0){
			if (length>=valueToLengthMapping[valueToLengthMapping.Length-1].archLength){
				time = this.time[this.time.Length-1];
			} else {
				// find the segment where time is between
				int segment = 0;
				// this can be improved by using binary search
				for (;valueToLengthMapping[segment+1].archLength<length;segment++){
					// do nothing
				}
				float fraction = (length-valueToLengthMapping[segment].archLength)/(valueToLengthMapping[segment+1].archLength-valueToLengthMapping[segment].archLength);
				time = Mathf.Lerp(valueToLengthMapping[segment].parameterValue,valueToLengthMapping[segment+1].parameterValue,fraction);
			}
		}
		return time;
	}
	
	public float GetLengthAtTime(float time){
		if (valueToLengthMapping==null){
			InitValueToLengthMapping();
		}
		
		float length = 0;
		if (time>this.time[0]){
			if (time>=this.time[this.time.Length-1]){
				length = valueToLengthMapping[valueToLengthMapping.Length-1].archLength;
			} else {
				// find the segment where time is between
				int segment = 0;
				// this can be improved by using binary search
				for (;valueToLengthMapping[segment+1].parameterValue<time;segment++){
					// do nothing
				}
				float fraction = (time-valueToLengthMapping[segment].parameterValue)/(valueToLengthMapping[segment+1].parameterValue-valueToLengthMapping[segment].parameterValue);
				length = Mathf.Lerp(valueToLengthMapping[segment].archLength,valueToLengthMapping[segment+1].archLength,fraction);
			}
		}
		
		return length;
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
	
	/*
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
	*/
	public void InitValueToLengthMapping(){
		// assertion check
		for (int i=1;i<mTime.Length;i++){
			if (mTime[i-1] >= mTime[i]){
				throw new Exception("Non increasing time parameter: {...,"+mTime[i-1] +","+ mTime[i]+",...} length: "+mTime.Length);
			}
		}
		
		float length = 0;
		List<ParameterValueToArcLength> res = new List<ParameterValueToArcLength>();
		res.Add(new ParameterValueToArcLength(mTime[0],length));
		for (int i=0;i<mTime.Length-1;i++){
			// compute length
			length = SegmentArcLength(mTime[i], mTime[i+1],length,0, MAX_RECURSION_DEPTH,res);
		}
		valueToLengthMapping = res.ToArray();
	}
	
	public string DebugValueToLength(){
		string res = "\nValueToLength map:\n";
		if (valueToLengthMapping==null){
			InitValueToLengthMapping();
		}
		foreach (ParameterValueToArcLength p in valueToLengthMapping){
			res += p.parameterValue+" = "+p.archLength+"\n";		
		} 
		return res;
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
	private float SegmentArcLength(float u1, float u2, float initLength, float epsilon, int max, List<ParameterValueToArcLength> res){
		Vector3 p1 = GetPosition(u1);
		Vector3 p2 = GetPosition(u2);
		Vector3 p1p2 = p2-p1;
		float p1p2Length =p1p2.magnitude;
		// make epsilon a fragment of segment length
		if (epsilon==0){
			epsilon = p1p2Length*lengthPrecision;
		}
		if (max==0){
			float totalLength = initLength+p1p2Length;
			res.Add(new ParameterValueToArcLength(u2,totalLength));
			return totalLength;
		}
		
		float half = (u2-u1)*0.5f;
		Vector3 midPoint = GetPosition(u1+half);
		
		float quarter = half * 0.5f;
		Vector3 quarterPoint = GetPosition(u1+quarter);
		
		if (IsStraight(p1,midPoint,p2,epsilon,true) && IsStraight(p1,quarterPoint,midPoint,epsilon,false)){
			float totalLength = initLength+p1p2Length;
			res.Add(new ParameterValueToArcLength(u2,totalLength));
			return totalLength;	  
		}
		max --;
		
		initLength = SegmentArcLength(u1, u1+half,initLength, epsilon,max,res); 
		return SegmentArcLength(u1+half, u2,initLength,epsilon, max,res);
	}
	
	/// <summary>
	/// Calculates the render points.
	/// Basically same algorithm as segment length
	/// </summary>
	private void CalcRenderPoints (float u1, float u2, float epsilon, int max){
		Vector3 p1 = GetPosition(u1);
		Vector3 p2 = GetPosition(u2);
		Vector3 p1p2 = p2-p1;
		float p1p2Length =p1p2.magnitude;
		
		// make epsilon a fragment of segment length
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
		
		if (IsStraight(p1,midPoint,p2,epsilon,false) && IsStraight(p1,quarterPoint,midPoint,epsilon,false)){
			renderPoints.Add(u2);
			return;	  
		}
		max --;
		
		CalcRenderPoints(u1, u1+half, epsilon, max-1);
		CalcRenderPoints(u1+half, u2, epsilon, max-1);
	}
	
	
	
	/// <summary>
	/// Note that this method can be optimized if it is inlined in the code.
	/// So if it is used where performance matters this should be done.
	/// The main performance gain is that the same magnitude of the same vectors 
	/// are calculated twice. (SegmentArcLength's p1p2 and p1-midpoint)
	/// </summary>
	private bool IsStraight(Vector3 p1, Vector3 p2, Vector3 p3, float epsilon, bool checkMidpointLength){
		float p1p3Length = (p1-p3).magnitude;
		float p1p2Length = (p1-p2).magnitude;
		float p2p3Length = (p2-p3).magnitude;
		if (checkMidpointLength && Mathf.Abs(p1p3Length*0.5f - p1p2Length) >epsilon){
			return false;
		}
		return p1p2Length+p2p3Length-p1p3Length<=epsilon;
	}
}

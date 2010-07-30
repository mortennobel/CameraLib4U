using UnityEngine;
using System.Collections;

public class BerzierSplineCurve : UUnitTestCase {
	public void TestBerzierSplineCurve ()
	{
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(0,1,0),
			new Vector3(1,1,0)
		};
		BezierSplineCurve bezier = new BezierSplineCurve();
		bezier.Init(p1); 
		float totalLength = bezier.totalLength;
		Assert.True(totalLength>1 && totalLength<2,"");
	}
	
	public void TestBerzierSplineCurveInitWithApproxPoints ()
	{
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(1,1,0)
		};
		Vector3[] aproxPoints ={
			new Vector3(0,1,0),
			new Vector3(0,1,0)			
		};
		BezierSplineCurve bezier = new BezierSplineCurve();
		bezier.Init(p1,aproxPoints); 
		float totalLength = bezier.totalLength;
		Assert.True(totalLength>1 && totalLength<2,"");
	}
	
	public void TestSimpleLength ()
	{
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(0,1,2)
		};
		Vector3[] aproxPoints ={
			new Vector3(0,0.5f,0),
			new Vector3(0,0.5f,0),
			new Vector3(0,1,1),
			new Vector3(0,1,1)
		};
		BezierSplineCurve bezier = new BezierSplineCurve();
		bezier.Init(p1,aproxPoints); 
		float totalLength = bezier.totalLength;
		Assert.Equals(1f+2f, totalLength,"");
	}
	
	public void TestBerzierSplineCurveLargeTest ()
	{
		Vector3[] p1 = {
			new Vector3(1.524025f,3.806607f,12.63667f),
			new Vector3(0.1744918f,2.554269f,10.28714f),
			new Vector3(-8.287139f,2.554269f,11.93816f),
			new Vector3(-6.715631f,3.240373f,9.715631f),
			new Vector3(-7.160138f,3.806607f,6.794591f),
			new Vector3(-6.207623f,3.806607f,5.778575f),
			new Vector3(0.396464f,3.806607f,5.778575f),
			new Vector3(3.571506f,3.806607f,5.778575f),
			new Vector3(5.032027f,3.806607f,5.842076f),
			new Vector3(5.095528f,3.806607f,8.064607f),
			new Vector3(5.032027f,3.806607f,11.43015f),
			new Vector3(4.968527f,3.806607f,14.6687f),
			new Vector3(7.318059f,3.806607f,13.97019f)
		};
		BezierSplineCurve bezier = new BezierSplineCurve();
		bezier.Init(p1); 
		Debug.Log("DebugValueToLength"+bezier.DebugValueToLength());
		Assert.Equals((p1.Length-1)/3+1, bezier.time.Length);
		float totalLength = bezier.totalLength;
		
		float linearLength = 0;
		for (int i=0;i<p1.Length-1;i=i+3){
			Vector3 linearSegment = p1[i]-p1[i+1];
			linearLength += linearSegment.magnitude;
		}
		
		
		
		string debug_t ="";
		foreach (float f in bezier.time){
			debug_t += f+" ";
		}
		Debug.Log("Bezier t: "+debug_t);
		
		Assert.True(totalLength>linearLength,"total length "+totalLength+ " should be longer than linearLength "+ linearLength);
		
		for (int i=0;i<bezier.time.Length;i++){
			float f = bezier.time[i];
			Assert.Equals(bezier.controlPoints[i], bezier.GetPosition(f), "Position "+i);
		}
	}
	
	public void TestBerzierSplineCurveRenderPoints ()
	{
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(0,1,0),
			new Vector3(1,1,0)
		};
		BezierSplineCurve bezier = new BezierSplineCurve();
		bezier.Init(p1); 
		float[] renderPoints = bezier.GetRenderPoints();
		foreach (float f in renderPoints){
			bezier.GetPosition(f);
		}
	}
	
	public void TestBerzierSplineVelocity ()
	{
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(0,1,0),
			new Vector3(0,2,0)
		};
		BezierSplineCurve bezier = new BezierSplineCurve();
		bezier.Init(p1); 
		Vector3 vel0 =bezier.GetVelocity(0);
		Vector3 vel1 =bezier.GetVelocity(1);
		Assert.True(Vector3.Dot(vel0,vel1)>0, "Vectors should not point in reverse directions");
		
	}
}

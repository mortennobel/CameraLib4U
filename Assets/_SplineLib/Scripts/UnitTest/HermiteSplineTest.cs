using UnityEngine;
using System.Collections;

public class HermiteSplineTest : UUnitTestCase {
	public void TestLinearPath ()
	{
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(1,2,1),
			new Vector3(2,2,2)
		};
		HermiteSplineCurve hermite = new HermiteSplineCurve();
		hermite.InitNatural(p1);
		float totalLength = hermite.totalLength;
		
		float linearLength = 0;
		for (int i=0;i<p1.Length-1;i++){
			Vector3 linearSegment = p1[i]-p1[i+1];
			linearLength += linearSegment.magnitude;
		}
		
		Assert.True(totalLength>linearLength, "Minimum is direct line between line segments");
	}		
	
	public void TestRenderpoints(){
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(0,0,0),
		};
		HermiteSplineCurve hermite = new HermiteSplineCurve();
		hermite.InitNatural(p1); 
		float[] renderPoints = hermite.GetRenderPoints();
		foreach (float f in renderPoints){
			hermite.GetPosition(f);
		}
	}
	
	public void TestCatmullRomVelocities(){
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(1,0,0),
			new Vector3(2,0,0),
			new Vector3(3,0,0),
			
		};
		float[] fs = {
			0,1,2,3
		};
		
		HermiteSplineCurve hermite = new HermiteSplineCurve();
		hermite.InitCatmullRom(p1,fs); 
		
		Vector3[] velocitySamples = {
			hermite.GetVelocity(0.0f),
			hermite.GetVelocity(0.5f),
			hermite.GetVelocity(1.0f),
			hermite.GetVelocity(1.5f),
			hermite.GetVelocity(2.0f),
			hermite.GetVelocity(3.0f),
			
		};
		string tangentDebug = "Tangent 0 vectors:\n";
		foreach (Vector3 v in hermite.Tangent0Vectors){
			tangentDebug += v+",";
		} 
		tangentDebug += "\nTangent 1 vectors:\n";
		foreach (Vector3 v in hermite.Tangent1Vectors){
			tangentDebug += v+",";
		} 
		Debug.Log(tangentDebug);
		
		string velcityDebug = "Velocity:\n";
		foreach (Vector3 v in velocitySamples){
			velcityDebug += v+",";
		}
		Debug.Log(velcityDebug);
		
		for (int i=1;i<velocitySamples.Length;i++){
			Assert.Equals(velocitySamples[i-1],velocitySamples[i], "Index "+i);
		}
	}
}

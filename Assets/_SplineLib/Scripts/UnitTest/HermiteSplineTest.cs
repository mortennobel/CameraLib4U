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
}

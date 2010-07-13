using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LinearTest : UUnitTestCase {

	public void TestLinearPath ()
	{
		Vector3[] p1 ={
			new Vector3(0,0,0),
			new Vector3(1,2,3),
		};
		LinearSplineCurve lp = new LinearSplineCurve();
		lp.Init(p1);
		
		Vector3 expectedResult = new Vector3(.5f, 1, 1.5f);
		Assert.Equals(expectedResult,lp.GetPosition(0.5f*lp.totalLength));
		
		expectedResult = new Vector3(.25f, .5f, 0.75f);
		Assert.Equals(expectedResult,lp.GetPosition(0.25f*lp.totalLength));	
		
		// test start and end conditions
		Assert.Equals(p1[0],lp.GetPosition(0.0f));	
		Assert.Equals(p1[1],lp.GetPosition(lp.totalLength));	
	}
		
	public void TestLinearPathExplicitDistances ()
	{
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(1,2,1)
		};
		LinearSplineCurve lp = new LinearSplineCurve();
		lp.Init(p1);
		Vector3 expectedResult = p1[1];
		Assert.Equals(expectedResult,lp.GetPosition(1f));
		
		expectedResult = new Vector3(0f, 0.5f, 0f);
		Assert.Equals(expectedResult,lp.GetPosition(0.5f));	
		
		// test start and end conditions
		Assert.Equals(Vector3.zero,lp.GetPosition(0.0f));
		Vector3 endpoint = new Vector3(1,2,1);
		Assert.Equals(endpoint,lp.GetPosition(lp.totalLength));
	}		
	
	public void TestLength(){
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0)
		};
		LinearSplineCurve lp = new LinearSplineCurve();
		lp.Init(p1);
		Assert.Equals(1.0f, lp.totalLength);
		
		Vector3[] p2 = {
			new Vector3(0,0,0),
			new Vector3(1,1,1)
		};
		lp = new LinearSplineCurve();
		lp.Init(p2);
		Assert.Equals(p2[1].magnitude, lp.totalLength);
	}
	
	public void TestLengthConCat1(){
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(0,0,0),
		};
		LinearSplineCurve lp = new LinearSplineCurve();
		lp.Init(p1);
		Assert.Equals(2, lp.totalLength);
	}
	
	public void TestLengthConCat2(){
		// more advanced test
		Vector3[] p2 = {
			new Vector3(0,0,0),
			new Vector3(1,1,1),
			new Vector3(0,1,1),
			new Vector3(0,4,1),
		};
		LinearSplineCurve lp = new LinearSplineCurve();
		lp.Init(p2);
		Assert.Equals(p2[1].magnitude+1+3, lp.totalLength);
	}
	
	public void TestRenderPoints(){
		Vector3[] p1 = {
			new Vector3(0,0,0),
			new Vector3(0,1,0),
			new Vector3(0,0,0),
		};
		LinearSplineCurve lp = new LinearSplineCurve();
		lp.Init(p1);
		float[] renderPoints = lp.GetRenderPoints();
		Assert.Equals(3, renderPoints.Length);
		Assert.Equals(0f, renderPoints[0]);
		Assert.Equals(1f, renderPoints[1]);
		Assert.Equals(2f, renderPoints[2]);
	}
}

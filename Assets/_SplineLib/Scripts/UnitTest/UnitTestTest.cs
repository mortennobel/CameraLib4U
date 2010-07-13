using UnityEngine;
using System.Collections;

public class UnitTestTest : UUnitTestCase {
	public void TestVector3(){
		Vector3 one = Vector3.one;
		Vector3 two = new Vector3(1.000001f, 1.000001f,1.000001f);
		Assert.Equals(one,two, 0.01f, ""); 
	}
}

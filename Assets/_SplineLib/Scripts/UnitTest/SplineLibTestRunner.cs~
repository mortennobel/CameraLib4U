using UnityEngine;
using System.Collections;

public class SplineLibTestRunner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UUnitTestSuite suite = new UUnitTestSuite();
		suite.Add(typeof(LinearTest));
		suite.Add(typeof(UnitTestTest));
		suite.Add(typeof(MatrixTest));
		suite.Add(typeof(HermiteSplineTest));
		suite.Add(typeof(BerzierSplineCurve));
		
		UUnitTestResult result = suite.RunAll();
		Debug.Log("Result: "+result.Summary());
	}
	
	// Update is called once per frame
	void Update () {
	 
	}
}

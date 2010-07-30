using UnityEngine;
using System.Collections;

public class UnitTestTest : UUnitTestCase {
	public void TestVector3(){
		Vector3 one = Vector3.one;
		Vector3 two = new Vector3(1.000001f, 1.000001f,1.000001f);
		Assert.Equals(one,two, 0.01f, ""); 
	}
	
	public void TestThrowException(){
		try{
			DoThrowException();
		}
		catch (System.Exception e){
			Debug.Log("Caught exception "+e);
		}
	}
	
	public void TestThrowCustomException(){
		try{
		// crashes Unity
		//	DoThrowCustomException();
		} catch (UUnitAssertException e){
			Debug.Log("Caught exception "+e);
		} 
	}
	
	private void DoThrowException(){
		if (Mathf.RoundToInt(0)==0){
			throw new System.Exception("");
		}
	}
	
	private void DoThrowCustomException(){
		if (Mathf.RoundToInt(0)==0){
			throw new UUnitAssertException("","","");
		}
	}
}

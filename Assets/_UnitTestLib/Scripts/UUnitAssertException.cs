using UnityEngine;
using System.Collections;
using System;

public class UUnitAssertException : Exception {
	//private T expected;
	//private T actual;
	private string msg;
	public UUnitAssertException(object expected, object actual, string msg){
		this.expected = expected;
		this.actual = actual;
		this.msg = msg;
	}	
	
	
	public object actual {
		get;
		set;
	}
	
	public object expected {
		get;
		set;
	}
	
	public string Details(){
		return "UUnitAssertException: Expected: "+expected+" actual "+actual+" msg: "+msg+" \n"+base.ToString();
	}
}

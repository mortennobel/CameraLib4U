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
		get{
			return actual;
		}
		set{
			actual =value;
		}
	}
	
	public object expected {
		get{
			return expected;
		}
		set{
			expected =value;
		}
	}
	
	public string Details(){
		return "UUnitAssertException: Expected: "+expected+" actual "+actual+" msg: "+msg+" \n"+base.ToString();
	}
}

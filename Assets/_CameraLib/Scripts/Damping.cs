using UnityEngine;
using System.Collections;

public static class Damping {
	public static float SimpleDamping(float actualValue, float destinationValue, float dampingFactor){
		return Mathf.Lerp(actualValue, destinationValue, dampingFactor*Time.deltaTime);
	}
	
	public static Vector3 SimpleDamping(Vector3 actualValue, Vector3 destinationValue, float dampingFactor){
		return Vector3.Lerp(actualValue, destinationValue, dampingFactor*Time.deltaTime);
	}
	
	public static float SpringDamping(float actualValue, float destinationValue, ref float velocity, 
	                                  float springStiffness, float springDamping){
		float displace = actualValue - destinationValue;
		float springAccel = (-springStiffness*displace)-(springDamping*velocity);
		velocity += springAccel *  Time.deltaTime;
		return actualValue + velocity * Time.deltaTime;
	}
	
	public static Vector3 SpringDamping(Vector3 actualValue, Vector3 destinationValue, ref Vector3 velocity, 
	                                  float springStiffness, float springDamping){
		Vector3 displace = actualValue - destinationValue;
		Vector3 springAccel = (-springStiffness*displace)-(springDamping*velocity);
		velocity += springAccel * Time.deltaTime;
		return actualValue + velocity *  Time.deltaTime;
	}
	
	/// <summary>
	/// 
	/// </summary>
	public static float GetSpringDampingRatio(float springStiffness, float springDamping){
		return springDamping/(2*Mathf.Sqrt(springStiffness));
	}
}

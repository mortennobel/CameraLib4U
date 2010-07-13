using UnityEngine;
using System.Collections;

public interface ICamera {
	Vector3 GetCameraTargetPosition();
	
	void SetTarget(Transform transform);
	Transform GetTarget();
	void InitCamera();
}

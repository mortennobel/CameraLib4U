using UnityEngine;
using System.Collections;

public class Boost : MonoBehaviour {
	
	int counter= 0;
	float time =0;
	
	// Use this for initialization
	void Start () {
		counter ++;
		
		Debug.Log("Start "+Time.frameCount);
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time>10){
			time =0;
			enabled =false;
			Debug.Log("Stop "+counter);
			SomeCoroutine ();
		}
		if (Input.GetKeyDown (KeyCode.Z)){
			rigidbody.velocity = new Vector3(1,0,0);
		} else {
			rigidbody.velocity = new Vector3(1,0,1);
		}
	}
	
	IEnumerator SomeCoroutine () {
        yield return new WaitForSeconds (10);
		enabled =true;
    }
}

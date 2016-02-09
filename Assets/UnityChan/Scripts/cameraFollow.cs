using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour {

	public Transform target;
	
	void Update(){
		transform.position = new Vector3(target.position.x, target.position.y+1, transform.position.z);
	}
	
}

using UnityEngine;
using System.Collections;

public class bulletBehavior : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		//Destroy(gameObject, 3);
	}
	
	void OnCollisionEnter(Collision col){
		if(!col.collider.CompareTag("Player")){
			//Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//print(GetComponent<Rigidbody>().velocity);
	}
}

using UnityEngine;
using System.Collections;

public class spinningBladeRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(-Vector3.forward * 70 * Time.deltaTime);
	}
}

using UnityEngine;
using System.Collections;

public class fallingPlatform : MonoBehaviour {

	public float fallTimer = 200;
	public bool isFalling;
	public float fallSpeed = 10000f;
	private float x; //used for shaking effect
	private float amplitude = 0.02f;
	private float speed = 300f;

	// Use this for initialization
	void Start () {
		isFalling = false;
		x = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		if (fallTimer <= 0) {
			transform.position = new Vector3(transform.position.x, transform.position.y-(Time.deltaTime*fallSpeed), transform.position.z);
		}
		else if (isFalling){
			fallTimer -= Time.deltaTime * 150f;
			transform.position = new Vector3(x + amplitude * Mathf.Sin(speed*Time.time), transform.position.y, 0);
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.CompareTag ("Player")) {
			print ("fall");
			isFalling = true;
		}
		if (col.CompareTag ("ground")) {
			Destroy (gameObject);
		}
	}
}

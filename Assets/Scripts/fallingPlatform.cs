using UnityEngine;
using System.Collections;

public class fallingPlatform : spawnable {

	public float fallTimer = 200;
	public bool isFalling;
	public float fallSpeed = 10000f;
	private float x; //used for shaking effect
	private float amplitude = 0.02f;
	private float speed = 300f;
	
	//respawn behavior
	public Vector3 originalPos;

	// Use this for initialization
	void Start () {
		isFalling = false;
		x = transform.position.x;
		spawn = true;
		originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//not spawned
		if(!spawn){
			timeLeft -= Time.deltaTime;
			if(timeLeft <= 0){
				spawn = true;
				gameObject.active = true;
			}
		}
		
		if (fallTimer <= 0) {
			transform.position = new Vector3(transform.position.x, transform.position.y-(Time.deltaTime*fallSpeed), transform.position.z);
		}
		else if (isFalling){
			fallTimer -= Time.deltaTime * 150f;
			transform.position = new Vector3(x + amplitude * Mathf.Sin(speed*Time.time), transform.position.y, 0);
		}
	}
}

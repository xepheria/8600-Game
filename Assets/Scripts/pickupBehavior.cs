using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
public class pickupBehavior : MonoBehaviour {

	public float gainAmt;
	public Player player;
	private float y; //used for floating effect
	private float amplitude = 0.06f;
	private float speed = 2f;
	public bool spawn;
	private float timeToRespawn = 3f;
	public float timeLeft;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player>();
		y = transform.position.y;
		spawn = true;
	}
	
	void OnTriggerEnter(Collider col){
		print("collision entered");
		if(col.CompareTag("Player")){
			print("frictionGain!");
			player.gain(gainAmt);
			gameObject.active = false;
			spawn = false;
			timeLeft = timeToRespawn;
		}
	}
	
	void Update(){
		//not spawned
		if(!spawn){
			timeLeft -= Time.deltaTime;
			if(timeLeft <= 0){
				spawn = true;
				gameObject.active = true;
			}
		}
		
		transform.position = new Vector3(transform.position.x, y + amplitude * Mathf.Sin(speed*Time.time), 0);
	}
}

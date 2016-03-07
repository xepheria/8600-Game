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

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player>();
		y = transform.position.y;
	}
	
	void OnTriggerEnter(Collider col){
		print("collision entered");
		if(col.CompareTag("Player")){
			print("frictionGain!");
			player.gain(gainAmt);
			Destroy (gameObject);
		}
	}
	
	void Update(){
		transform.position = new Vector3(transform.position.x, y + amplitude * Mathf.Sin(speed*Time.time), 0);
	}
}

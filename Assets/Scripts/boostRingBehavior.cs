using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Animator))]
public class boostRingBehavior : MonoBehaviour {

	public float boostAmt;
	public Player player;
	private float y; //used for floating effect
	private float amplitude = 0.06f;
	private float speed = 1f;
	
	public Animator anim;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player>();
		y = transform.position.y;
		GetComponent<Animator>();
	}
	
	void OnTriggerEnter(Collider col){
		print("collision entered");
		if(col.CompareTag("Player")){
			print("boost!");
			player.bounce(transform.up * boostAmt);
			anim.Play("Boost");
		}
	}
	
	void Update(){
		transform.position = new Vector3(transform.position.x, y + amplitude * Mathf.Sin(speed*Time.time), 0);
	}
}

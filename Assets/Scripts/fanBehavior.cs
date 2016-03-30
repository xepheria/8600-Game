using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]

public class fanBehavior : MonoBehaviour {

	public bool isFan;
	public float pushAmt;
	public float maxPush;
	public Player player;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player>();
	}
	
	void OnTriggerStay(Collider col){
		print("collision entered");
		if(col.CompareTag("Player")){
			player.forceField(isFan,transform.up*pushAmt,transform.up*maxPush,true);
		}
	}

	void OnTriggerExit(Collider col){
		print("collision entered");
		if(col.CompareTag("Player")){
			player.forceField(isFan,transform.up*pushAmt,transform.up*maxPush,false);
		}
	}

	
	void Update(){
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class springBounce : MonoBehaviour {

	public float bounceAmt;
	public Player player;

	void OnTriggerEnter(Collider col){
		print("collision entered");
		if(col.CompareTag("Player")){
			print("bounce it!");
			player.bounce(transform.up * bounceAmt);
		}
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class springBounce : MonoBehaviour {

	public float bounceAmt;
	public Player player;

	void OnCollisionEnter(Collision col){
		print("collision entered");
		if(col.gameObject.CompareTag("Player")){
			print("bounce it!");
			player.bounce(transform.up * bounceAmt);
		}
	}
}

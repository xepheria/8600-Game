using UnityEngine;
using System.Collections;

public class triggerParentFallingPlat : MonoBehaviour {

	public fallingPlatform parent;
	private Player player;

	void Start(){
		player = FindObjectOfType<Player>();
	}

	void OnTriggerEnter(Collider col){
		if (col.CompareTag ("Player")) {
			print ("fall");
			if(player.getMode() != 1)
				parent.isFalling = true;
		}
		if (col.CompareTag ("ground")) {
			parent.transform.position = parent.originalPos;
			parent.isFalling = false;
			parent.fallTimer = 150f;
			parent.gameObject.active = false;
			parent.spawn = false;
			parent.timeLeft = parent.timeToRespawn;
		}
	}
}

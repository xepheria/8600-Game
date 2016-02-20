using UnityEngine;
using System.Collections;

public class dangerousObject : MonoBehaviour {

	public Player player;
	
	void Start(){
		player = FindObjectOfType<Player>();
	}
	
	void OnTriggerEnter(Collider col){
		print("ya hit me");
		if(col.CompareTag("Player")){
			player.defeated();
		}
	}
}

using UnityEngine;
using System.Collections;

public class dangerousObject : MonoBehaviour {

	public Player player;
	
	void Start(){
		player = FindObjectOfType<Player>();
	}
	
	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			player.defeated();
		}
	}
}
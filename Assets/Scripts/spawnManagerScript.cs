using UnityEngine;
using System.Collections;

public class spawnManagerScript : MonoBehaviour {

	private spawnable[] spawnables;

	// Use this for initialization
	void Start () {
		spawnables = FindObjectsOfType<spawnable>();
	}
	
	// Update is called once per frame
	void Update () {
		foreach(spawnable obj in spawnables){
			//not spawned
			//pickupBehavior info = obj.GetComponent<pickupBehavior>();
			if(!obj.spawn){
				obj.timeLeft -= Time.deltaTime;
				if(obj.timeLeft <= 0){
					obj.spawn = true;
					obj.gameObject.active = true;
				}
			}
		}
	}
}

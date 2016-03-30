using UnityEngine;
using System.Collections;

public class spawnManagerScript : MonoBehaviour {

	private GameObject[] spawnables;

	// Use this for initialization
	void Start () {
		spawnables = (GameObject.FindGameObjectsWithTag("spawnable"));
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject obj in spawnables){
			//not spawned
			pickupBehavior info = obj.GetComponent<pickupBehavior>();
			if(!info.spawn){
				info.timeLeft -= Time.deltaTime;
				if(info.timeLeft <= 0){
					info.spawn = true;
					obj.active = true;
				}
			}
		}
	}
}

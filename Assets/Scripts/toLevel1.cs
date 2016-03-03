using UnityEngine;
using System.Collections;

public class toLevel1 : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			print("end of level");
			Application.LoadLevel("Level1");
		}
	}
}

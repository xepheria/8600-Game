using UnityEngine;
using System.Collections;

public class nextLevel : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		print("HWY");
		if(col.CompareTag("Player")){
			print("end of level");
			PlayerPrefs.SetInt ("currentLevel", PlayerPrefs.GetInt ("currentLevel") + 1); 
			print(PlayerPrefs.GetInt("currentLevel"));
			string nextLevel = "level" + PlayerPrefs.GetInt("currentLevel").ToString(); 
			Application.LoadLevel(nextLevel);
		}
	}
}

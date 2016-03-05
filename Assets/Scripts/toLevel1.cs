﻿using UnityEngine;
using System.Collections;

public class toLevel1 : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			print("end of level");
			PlayerPrefs.SetInt ("currentLevel", PlayerPrefs.GetInt ("currentLevel") + 1); 
			print(PlayerPrefs.GetInt("currentLevel"));
			string nextLevel = "level" + PlayerPrefs.GetInt("currentLevel").ToString(); 
			Application.LoadLevel(nextLevel);
		}
	}
}

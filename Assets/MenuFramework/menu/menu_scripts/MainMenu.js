#pragma strict

var menuSkin : GUISkin;
 
function OnGUI () {
GUI.skin = menuSkin;

	if(GUI.Button(Rect(0,Screen.height/3*2,Screen.width/3,Screen.height/3), "NEW GAME")){	
		PlayerPrefs.SetInt ("currentLevel", 0); 
		Application.LoadLevel("level0");
	}

	if(PlayerPrefs.HasKey("currentLevel")){
		if(GUI.Button(Rect(Screen.width/3*2,Screen.height/3*2,Screen.width/3,Screen.height/3), "Continue")){	
			var nextLevel:String = "level" + PlayerPrefs.GetInt("currentLevel"); 
			Application.LoadLevel(nextLevel);
		}
	}

	/*
	if(GUI.Button(Rect(Screen.width/3*2,Screen.height/3*2,Screen.width/3,Screen.height/3), "Options")){
		Application.LoadLevel("OptionsMenu");
	}
	*/

	if(GUI.Button(Rect(Screen.width/3*2,0,Screen.width/3,Screen.height/3), "Erase All Data")){
		PlayerPrefs.DeleteAll();
		Application.LoadLevel("MainMenu");
	}

}
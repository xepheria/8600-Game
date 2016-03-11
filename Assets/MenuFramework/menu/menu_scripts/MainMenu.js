#pragma strict

var menuSkin : GUISkin;
var bg : Texture;
function OnGUI () {
GUI.skin = menuSkin;
	if(Input.GetKeyDown("escape")){
		Application.Quit();
	}

	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),bg);
	GUI.Box(Rect(0,0,Screen.width,Screen.height),"MAGNETA");

	if(Input.GetButtonDown("Jump") || Input.GetButtonDown ("Start")){
		PlayerPrefs.SetInt ("currentLevel", 0); 
		Application.LoadLevel("level0");
	}

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
}
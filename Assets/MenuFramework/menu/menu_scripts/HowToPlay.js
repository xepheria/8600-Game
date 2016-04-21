#pragma strict

var menuSkin : GUISkin;
var bg : Texture;
var menuFont : GUIStyle;

function OnGUI () {
	GUI.skin = menuSkin;
	if(Input.GetKeyDown("escape")){
		Application.LoadLevel("MainMenu");
	}

	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),bg);
	GUI.Label(Rect(0,Screen.height-75,Screen.width,0),"Press Any Key to Continue", menuFont);
}
	
function Update(){
	if(Input.anyKey){
		if(PlayerPrefs.GetInt("storyOn")){
			Application.LoadLevel("introCutscene");
		}else{
			PlayerPrefs.SetInt ("currentLevel", 0); 
			Application.LoadLevel("level0");
		}
	}

	for (var i = 0;i < 20; i++) {
		if(Input.GetKeyDown("joystick 1 button "+i)){
			if(PlayerPrefs.GetInt("storyOn")){
				Application.LoadLevel("introCutscene");
			}else{
				PlayerPrefs.SetInt ("currentLevel", 0); 
				Application.LoadLevel("level0");
			}
		}
	}
}
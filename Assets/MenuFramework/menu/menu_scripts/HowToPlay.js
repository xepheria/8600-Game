#pragma strict

var menuSkin : GUISkin;
var bg : Texture;
function OnGUI () {
GUI.skin = menuSkin;
	if(Input.GetKeyDown("escape")){
		Application.LoadLevel("MainMenu");
	}

	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),bg);
	GUI.Label(Rect(0,0,Screen.width,Screen.height),"CONTROLS");


	if(Input.anyKey){
		PlayerPrefs.SetInt ("currentLevel", 0); 
		Application.LoadLevel("level0");
	}

		for (var i = 0;i < 20; i++) {
             if(Input.GetKeyDown("joystick 1 button "+i)){
               	PlayerPrefs.SetInt ("currentLevel", 0); 
				Application.LoadLevel("level0");
             }
         }
}
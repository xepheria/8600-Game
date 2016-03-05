#pragma strict

var menuSkin : GUISkin;
 
function OnGUI () {
GUI.skin = menuSkin;

	if(GUI.Button(Rect(0,Screen.height/3*2,Screen.width/3,Screen.height/3), "Erase All Data")){
		PlayerPrefs.DeleteAll();
		Application.LoadLevel("MainMenu");
	}

	if(GUI.Button(Rect(Screen.width/3*2,Screen.height/3*2,Screen.width/3,Screen.height/3), "Return to Title")){
		Application.LoadLevel("MainMenu");
	}
}
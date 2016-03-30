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

	var labelF: Rect = Rect(0,140,Screen.width,Screen.height);
	GUI.Label(labelF,"C: Lower Friction- Slide across surfaces quickly and gain energy \n \n X: Raise Friction- Use energy to stick to surfaces and climb slopes \n \n Space: Jump \n Arrow Keys: Move \n P: Pause ");

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
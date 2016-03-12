#pragma strict

var menuSkin : GUISkin;
var bg : Texture;
function OnGUI () {
GUI.skin = menuSkin;
	if(Input.GetKeyDown("escape")){
		Application.LoadLevel("MainMenu");
	}

	GUI.Label(Rect(0,20,Screen.width,Screen.height),"THANK YOU FOR PLAYING");
	GUI.Box(Rect(0,0,Screen.width,Screen.height),"CLEMSON DPA \n 8600 Spring 2016");


}

function Start(){
	//Chill();
}

function Update () {
	Chill();
}

function Chill() {
	
	yield WaitForSeconds (2);
	//Application.LoadLevel("MainMenu");
	if(Input.anyKey){
		PlayerPrefs.SetInt ("currentLevel", 0); 
		Application.LoadLevel("MainMenu");
	}

		for (var i = 0;i < 20; i++) {
             if(Input.GetKeyDown("joystick 1 button "+i)){
               	PlayerPrefs.SetInt ("currentLevel", 0); 
				Application.LoadLevel("MainMenu");
             }
         }
}

#pragma strict
var hudSkin : GUISkin;
private var isPaused:boolean;
function Start () {
	isPaused = false;
}

function Update () {

	if(Input.GetKeyDown("escape")){
		Application.Quit();
	}

	if(Input.GetButtonDown("Pause") && !isPaused){
      Time.timeScale = 0.0;
      isPaused = true;
   }
   else if(Input.GetButtonDown("Pause") && isPaused){
      Time.timeScale = 1.0;
      isPaused = false;    
   } 
}

function OnGUI () {
	GUI.skin = hudSkin;
	  var labelR: Rect = Rect((Screen.width/2)-180,(Screen.height/2)-80,Screen.width/2,Screen.height/3);
	if(isPaused){
		
		GUI.Label(labelR,"PAUSED!");
	}
}
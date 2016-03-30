#pragma strict
var hudSkin : GUISkin;
private var isPaused:boolean;
function Start () {
	isPaused = false;
}

var bg : Texture;

function Update () {
	if(isPaused){
		if(Input.GetKeyDown("escape")){
			Time.timeScale = 1.0;
      		isPaused = false;    
			Application.LoadLevel("MainMenu");
		}
		if(Input.GetKeyDown(KeyCode.R)){
			Application.LoadLevel(Application.loadedLevel);
	  		Time.timeScale = 1.0;
      		isPaused = false;    
		}
	}

	if(Input.GetButtonDown("Start") && !isPaused){
      Time.timeScale = 0.0;
      isPaused = true;
   }
   else if(Input.GetButtonDown("Start") && isPaused){
      Time.timeScale = 1.0;
      isPaused = false;    
   } 
}

function OnGUI () {


	GUI.skin = hudSkin;
	  var labelR: Rect = Rect(0,(Screen.height/3),Screen.width,Screen.height);
	if(isPaused){
		GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),bg);
		GUI.Label(labelR,"PAUSED! \n \n R to Retry \n ESC to return to menu");
	}
}
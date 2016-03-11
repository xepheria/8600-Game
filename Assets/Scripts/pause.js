#pragma strict
var hudSkin : GUISkin;
private var isPaused:boolean;
function Start () {
	isPaused = false;
}

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
	  var labelR: Rect = Rect((Screen.width/2)-180,(Screen.height/2)-80,Screen.width/2,Screen.height/3);
	  var labelE: Rect = Rect(20,80,Screen.width/2,Screen.height/3);
	  var labelF: Rect = Rect(20,140,Screen.width/2,Screen.height/3);
	if(isPaused){
		
		GUI.Label(labelR,"PAUSED!");
		GUI.Label(labelE,"R to Retry");
		GUI.Label(labelF,"ESC to return to menu");
	}
}
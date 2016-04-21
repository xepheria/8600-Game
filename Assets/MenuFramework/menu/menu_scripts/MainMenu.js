#pragma strict

var menuSkin : GUISkin;
var bg : Texture;

private var toggleMusic : boolean;
private var toggleStory : boolean;
private var toggleStyle : GUIStyle;

function Start(){
	if(PlayerPrefs.HasKey("musicOn")){
		toggleMusic = GetBool("musicOn");
	}
	else toggleMusic = true;
	
	if(PlayerPrefs.HasKey("storyOn")){
		toggleStory = GetBool("storyOn");
	}
	else toggleStory = true;
}

function OnGUI () {
	toggleStyle = GUIStyle(GUI.skin.toggle);
	toggleStyle.normal.background = GUI.skin.toggle.normal.background;
	toggleStyle.hover.background = GUI.skin.toggle.hover.background;
	toggleStyle.active.background = GUI.skin.toggle.active.background;
	toggleStyle.normal.textColor = Color(0.56,0.86,0.84);
	toggleStyle.onNormal.textColor = Color(0.0,1.0,.93);
	toggleStyle.hover.textColor = Color(.41,.73,.71);
	toggleStyle.onHover.textColor = Color(.41,.73,.71);
	toggleStyle.active.textColor = Color.cyan;
	toggleStyle.onActive.textColor = Color.cyan;
	GUI.skin = menuSkin;
	
	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),bg);
	
	//toggle switch for music
	toggleMusic = GUI.Toggle(Rect(20,10,150,30), toggleMusic, "Music On?", toggleStyle);
	//toggle switch for story
	toggleStory = GUI.Toggle(Rect(20,40,150,30), toggleStory, "Cutscenes On?", toggleStyle);

	if(GUI.Button(Rect(0,Screen.height/3*2,Screen.width/3,Screen.height/3), "NEW GAME")){
		SetBool("musicOn", toggleMusic);
		SetBool("storyOn", toggleStory);
		PlayerPrefs.SetInt ("currentLevel", 0); 
		Application.LoadLevel("HowToPlay");
	}

	if(PlayerPrefs.HasKey("currentLevel")){
		if(GUI.Button(Rect(Screen.width/3*2,Screen.height/3*2,Screen.width/3,Screen.height/3), "Continue")){
			SetBool("musicOn", toggleMusic);
			SetBool("storyOn", toggleStory);
			var nextLevel:String = "level" + PlayerPrefs.GetInt("currentLevel"); 
			Application.LoadLevel(nextLevel);
		}
	}
}

function Update(){
	if(Input.GetKeyDown("escape")){
		Application.Quit();
	}
	
	if(Input.GetButtonDown("Jump") || Input.GetButtonDown ("Start")){
		SetBool("musicOn", toggleMusic);
		SetBool("storyOn", toggleStory);
		PlayerPrefs.SetInt ("currentLevel", 0); 
		Application.LoadLevel("HowToPlay");
	}
}

static function SetBool (name : String, value : boolean) {
	PlayerPrefs.SetInt(name, value? 1 : 0);
}
 
static function GetBool (name : String) : boolean {
	return PlayerPrefs.GetInt(name) == 1;
}
 
static function GetBool (name : String, defaultValue : boolean) : boolean {
	if (PlayerPrefs.HasKey(name)) {
		return GetBool(name);
	}
	return defaultValue;
}
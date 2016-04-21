#pragma strict
var NewMusic: AudioClip; //Pick an audio track to play.
 
function Start (){
	if(PlayerPrefs.GetInt("musicOn")){
		var go = GameObject.Find("Music"); //Finds the game object called Game Music, if it goes by a different name, change this.
			if(go.GetComponent.<AudioSource>().clip != NewMusic){
				go.GetComponent.<AudioSource>().clip = NewMusic; //Replaces the old audio with the new one set in the inspector.
				go.GetComponent.<AudioSource>().Play(); //Plays the audio.
			}
	}
}
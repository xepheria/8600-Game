using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour {

	public GUISkin hudSkin;
	private bool isPaused;
	
	void Start () {
		isPaused = false;
	}

	public Texture bg;

	void Update () {
		if(isPaused){
			if(Input.GetKeyDown("escape")){
				Time.timeScale = 1.0f;
				isPaused = false;
				SceneManager.LoadScene("MainMenu");
				//Application.LoadLevel("MainMenu"); deprecated
			}
			if(Input.GetKeyDown(KeyCode.R)){
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				//Application.LoadLevel(Application.loadedLevel); deprecated
				Time.timeScale = 1.0f;
				isPaused = false;    
			}
		}

		if(Input.GetButtonDown("Start") && !isPaused){
		  Time.timeScale = 0.0f;
		  isPaused = true;
	   }
	   else if(Input.GetButtonDown("Start") && isPaused){
		  Time.timeScale = 1.0f;
		  isPaused = false;    
	   } 
	}

	void OnGUI () {
		GUI.skin = hudSkin;
		  Rect labelR = new Rect(0,(Screen.height/3),Screen.width,Screen.height);
		if(isPaused){
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),bg);
			GUI.Label(labelR,"PAUSED! \n \n R to Retry \n ESC to return to menu");
		}
	}
}
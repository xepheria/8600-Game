using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		Application.LoadLevel("MainMenu");
	}
	
	// Update is called once per frame
	void Update () {
		Application.LoadLevel("MainMenu");
	}
}

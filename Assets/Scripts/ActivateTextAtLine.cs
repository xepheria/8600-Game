using UnityEngine;
using System.Collections;

public class ActivateTextAtLine : MonoBehaviour {

	public TextAsset theText;
	
	public int startLine;
	public int endLine;
	
	public TextBoxManager theTextBox;
	
	public bool destroyWhenActivated;

	// Use this for initialization
	void Start () {
		theTextBox = FindObjectOfType<TextBoxManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col){
		print("collided with box");
		if(col.CompareTag("Player")){
			if(PlayerPrefs.GetInt("storyOn") == 1){
				theTextBox.ReloadScript(theText);
				theTextBox.currentLine = startLine;
				theTextBox.endAtLine = endLine;
				theTextBox.EnableTextBox();
			}
		
			if(destroyWhenActivated){
				Destroy(gameObject);
			}
		}
	}
}

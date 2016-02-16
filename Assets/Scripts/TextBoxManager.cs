using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

	public GameObject textBox;
	
	public Text theText;

	public TextAsset textFile;
	public string[] textLines;
	
	public int currentLine;
	public int endAtLine;
	
	public Player player;
	
	public bool isActive;
	public bool stopPlayerMovement;
	
	private bool isTyping = false;
	private bool cancelTyping = false;
	
	public float typeSpeed;
	
	public Image arrowDisplay;

	// Use this for initialization
	void Start () {
		arrowDisplay.gameObject.SetActive(false);
		
		player = FindObjectOfType<Player>();
		
		if(textFile != null){
			textLines = (textFile.text.Split('\n'));
		}
		
		if(endAtLine == 0){
			endAtLine = textLines.Length - 1;
		}
		
		if(isActive){
			EnableTextBox();
		}
		
		if(!isActive){
			DisableTextBox();
		}
	}
	
	void Update(){
		if(!isActive){
			return;
		}
				
		if(Input.GetKeyDown(KeyCode.Return)){
			arrowDisplay.gameObject.SetActive(false);
			if(!isTyping){
				currentLine++;
				if(currentLine > endAtLine){
					DisableTextBox();
				}
				else{
					StartCoroutine(TextScroll(textLines[currentLine]));
				}
			}
			else if(isTyping && !cancelTyping){
				cancelTyping = true;
			}
		}
	}
	
	private IEnumerator TextScroll (string lineOfText){
		int letter = 0;
		theText.text = "";
		isTyping = true;
		cancelTyping = false;
		while(isTyping && !cancelTyping && (letter < lineOfText.Length - 1)){
			theText.text += lineOfText[letter];
			letter++;
			yield return new WaitForSeconds(typeSpeed);
		}
		//if while loop is broken by player input, spit out all the text of the line into the box
		theText.text = lineOfText;
		arrowDisplay.gameObject.SetActive(true);
		isTyping = false;
		cancelTyping = false;
	}
	
	public void EnableTextBox(){
		textBox.SetActive(true);
		isActive = true;
		
		if(stopPlayerMovement){
			player.canMove = false;
		}
		
		StartCoroutine(TextScroll(textLines[currentLine]));
	}
	
	public void DisableTextBox(){
		textBox.SetActive(false);
		isActive = false;
		
		player.canMove = true;
	}
	
	public void ReloadScript(TextAsset theText){
		if(theText != null){
			textLines = new string[1];
			textLines = theText.text.Split('\n');
		}
		else
			print("invalid text file");
	}
}

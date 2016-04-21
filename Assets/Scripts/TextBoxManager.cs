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
	
	//public Image arrowDisplay;
	public Text continueText;
	
	public float tilNextLine = 0;
	private float waitTime = 3;
	private bool timerRunning = false;
	
	//used for cutscenes
	public float timeDone = 999999;

	// Use this for initialization
	void Start () {
		//arrowDisplay.gameObject.SetActive(false);
		continueText.gameObject.SetActive(false);
		
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
		
		if(!isActive || PlayerPrefs.GetInt("storyOn") == 0){
			DisableTextBox();
		}
	}
	
	void Update(){
		if(!isActive){
			return;
		}
		
		if(timerRunning){
			tilNextLine -= Time.deltaTime;
		}
				
		if(Input.GetButtonDown("Submit") && stopPlayerMovement){
			//arrowDisplay.gameObject.SetActive(false);
			continueText.gameObject.SetActive(false);
			if(!isTyping){
				currentLine++;
				if(currentLine > endAtLine){
					timeDone = Time.time;
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
		else if(!stopPlayerMovement && tilNextLine <= 0){
			//arrowDisplay.gameObject.SetActive(false);
			continueText.gameObject.SetActive(false);
			//reset timer, disable timer
			tilNextLine = waitTime;
			timerRunning = false;
			if(!isTyping){
				currentLine++;
				if(currentLine > endAtLine){
					DisableTextBox();
				}
				else{
					StartCoroutine(TextScroll(textLines[currentLine]));
				}
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
		//arrowDisplay.gameObject.SetActive(true);
		continueText.gameObject.SetActive(true);
		isTyping = false;
		cancelTyping = false;
		
		//if automated text box
		if(!stopPlayerMovement){
			timerRunning = true;
		}
	}
	
	public void EnableTextBox(){
		textBox.SetActive(true);
		isActive = true;
		
		if(stopPlayerMovement && player){
			player.canMove = false;
		}
		
		timeDone = 999999;
		
		StartCoroutine(TextScroll(textLines[currentLine]));
	}
	
	public void DisableTextBox(){
		textBox.SetActive(false);
		isActive = false;
		
		if(player)
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

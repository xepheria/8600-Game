using UnityEngine;
using System.Collections;

public class IntroCutscene : MonoBehaviour {

	public TextAsset theText;
	public TextBoxManager theTextBox;
	public GameObject lightFlash;
	public GameObject planet;
	public GameObject siren;
	private float x; //used for shaking effect
	private float amplitude = 0.02f;
	private float speed = 300f;
	public GameObject spacePod;
	
	private bool[] step;

	// Use this for initialization
	void Start () {
		theTextBox = FindObjectOfType<TextBoxManager>();
		lightFlash.SetActive(false);
		step = new bool[50];
		siren.SetActive(false);
		x = Camera.main.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		
		//first batch of text
		if(Time.time > 3 && !step[0]){
			step[0] = true;
			theTextBox.ReloadScript(theText);
			theTextBox.currentLine = 0;
			theTextBox.endAtLine = 3;
			theTextBox.EnableTextBox();
			return;
		}
		
		//second batch of text
		if(step[0] && !step[1] && Time.time - theTextBox.timeDone > 3){
			step[1] = true;
			theTextBox.currentLine = 4;
			theTextBox.endAtLine = 7;
			theTextBox.EnableTextBox();
			return;
		}
		
		//third batch of text, flash of light
		if(step[1] && !step[2] && Time.time - theTextBox.timeDone > 3){
			step[2] = true;
			StartCoroutine(lightFlashPlay(3));
			theTextBox.currentLine = 8;
			theTextBox.endAtLine = 8;
			theTextBox.EnableTextBox();
			return;
		}
		
		if(step[2] && !step[3] && Time.time - theTextBox.timeDone > 1){
			step[3] = true;
			return;
		}
		
		if(step[3]){
			planet.transform.position = Vector3.Lerp(planet.transform.position, new Vector3(8.53f, 1.74f, -0.44f), Time.deltaTime*0.2f);
		}
		
		if(step[3] && !step[4] && Time.time - theTextBox.timeDone > 5){
			step[4] = true;
			theTextBox.currentLine = 9;
			theTextBox.endAtLine = 12;
			theTextBox.EnableTextBox();
			return;
		}
		
		if(step[4] && !step[5] && Time.time - theTextBox.timeDone > 3){
			step[5] = true;
			theTextBox.currentLine = 13;
			theTextBox.endAtLine = 14;
			theTextBox.EnableTextBox();
			return;
		}
		
		if(step[5] && !step[6]){
			siren.SetActive(true);
			Color textureColor = siren.GetComponent<Renderer>().material.color;
			textureColor.a = Mathf.PingPong(Time.time, 1.0f) / 1.0f;
			siren.GetComponent<Renderer>().material.color = textureColor;
			Camera.main.transform.position = new Vector3(x + amplitude * Mathf.Sin(speed*Time.time), Camera.main.transform.position.y, Camera.main.transform.position.z);
		}
		
		if(step[5] && !step[6] && Time.time - theTextBox.timeDone > 1.5f){
			step[6] = true;
		}
		
		if(step[6]){
			Color textureColor = siren.GetComponent<Renderer>().material.color;
			textureColor.a = Mathf.Lerp(textureColor.a, 1.0f, Time.deltaTime) / 1.0f;
			siren.GetComponent<Renderer>().material.color = textureColor;
			spacePod.transform.position = Vector3.Lerp(spacePod.transform.position, planet.transform.position, Time.deltaTime);
			spacePod.transform.localScale = Vector3.Lerp(spacePod.transform.localScale, new Vector3(.3f, .3f, .3f), Time.deltaTime);
		}
		
		if(step[6] && !step[7] && Time.time - theTextBox.timeDone > 5){
			//code here to load the first level
		}
	}
	
	IEnumerator lightFlashPlay(int flashes){
		for(int i = 0; i<flashes; i++){
			lightFlash.SetActive(true);
			yield return new WaitForSeconds(.1f);
			lightFlash.SetActive(false);
			yield return new WaitForSeconds(.1f);
		}
	}
}

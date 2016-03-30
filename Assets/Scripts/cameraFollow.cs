using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour {
	// This script gets attached to the player. It will find the camera and make it follow. 
	// Set Dead Zone to 0 if you want the camera to follow the player exactly.

	public float deadZone;
	public bool followVertical = true;
	public bool followHorizontal = true;
	public float offsetY = 1;

	// The camera in the scene. It is private because it is dealt with in function Start()
	private Camera cam;
	private Vector3 newPos;
	
	void Start(){
		//The variable cam will look for the Main Camera in the scene before the scene starts running and make it become the variable cam.
		cam = Camera.main;
	}
	
	void Update(){
		newPos = cam.transform.position;
		//If Follow Horizontal is checked in inspector, the camera follows player horizonally with the deadzone.
		if(followHorizontal == true){
			if (cam.transform.position.x >= transform.position.x + deadZone){
				newPos.x = transform.position.x + deadZone;
			}
			if (cam.transform.position.x <= transform.position.x - deadZone){
				newPos.x = transform.position.x - deadZone;
			}
		}
	
		//If Follow Vertical is checked in inspector, the camera follows player vertically with the deadzone.
		if(followVertical == true){
			if (cam.transform.position.y >= transform.position.y + offsetY + deadZone){
				newPos.y = transform.position.y + offsetY + deadZone;
			}
			if (cam.transform.position.y <= transform.position.y + offsetY - deadZone){
				newPos.y = transform.position.y + offsetY - deadZone;
			}
		}
		
		cam.transform.position = newPos;
	}
}
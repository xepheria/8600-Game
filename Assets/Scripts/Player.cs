using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(Animator))]
public class Player : MonoBehaviour {
	
	public LayerMask collisionMask;

	public Animator anim;
	
	bool jumping;
	float velocityXSmoothing;
	
	bool descending;
	
	private float xsp, ysp;
	const float acc = 0.046875f;
	const float dec = 0.5f;
	const float frc = 0.046875f;
	const float top = 0.1f;
	const float air = 0.09375f;
	const float grv = -0.3f;
	const float jmp = .13f;
	const float slp = 0.15f;
	const float maxRotationDegrees = 10f;
	float oldSlideAngle;
	float bumpTimer, launchTimer;
	const float bumpTime = 0.6f; //how long to wait
	const float launchTime = 0.6f;
	
	bool showDebug;
	
	Controller2D controller;
	
	private MeshRenderer mesh;
	
	private float faceDir;
	
	void Start() {
		showDebug = true;
		
		controller = GetComponent<Controller2D> ();
		
		anim = GetComponent<Animator> ();

		jumping = false;

		controller.collisions.mode = 0;
		
		mesh = GetComponentInChildren<MeshRenderer>();
	}
	
	void OnGUI(){
		if(showDebug){
			GUI.Box(new Rect(Screen.width - 200, 0, 200, 200), "Angle: " + controller.collisions.slopeAngle);
			GUI.Label(new Rect(Screen.width - 50, 25, 100, 50), controller.collisions.mode.ToString());
			GUI.Label(new Rect(Screen.width - 100, 40, 100, 50), "xspeed: " + xsp.ToString());
			GUI.Label(new Rect(Screen.width - 100, 70, 100, 50), "yspeed: " + ysp.ToString());
			GUI.Label(new Rect(Screen.width - 175, 110, 100, 50), "below: " + controller.collisions.below.ToString());
			GUI.Label(new Rect(Screen.width - 175, 130, 100, 50), "ascending: " + controller.collisions.climbingSlope.ToString());
			GUI.Label(new Rect(Screen.width - 175, 150, 100, 50), "descending: " + controller.collisions.descendingSlope.ToString());
			GUI.Label(new Rect(Screen.width - 50, 150, 100, 50), bumpTimer.ToString());
			
			//demo instructions
			GUI.Box(new Rect(0, 0, 400, 200), "Demo\nMove left/right : arrow keys\t\tJump : Space\nHi-fric mode : Hold X\t\tLo-Fric mode : Hold C\n");
			GUI.Label(new Rect(20, 60, 375, 100), "Use hi-fric to climb steep hills. Use lo-fric to gain speed and launch off slopes. Press G to toggle instructions/debug information.");
		}
	}
	
	void Update(){
		
		if(Input.GetKeyDown(KeyCode.G)){
			showDebug = !showDebug;
		}
		
		float inputLR = Input.GetAxisRaw("Horizontal");
		int fricCtrl = 0;
		if(Input.GetKey(KeyCode.X) && bumpTimer <= 0 && controller.collisions.below)
			fricCtrl = -1;
		else if(Input.GetKey(KeyCode.C) && bumpTimer <= 0 && controller.collisions.below)
			fricCtrl = 1;
		else if(!Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.X))
			fricCtrl = 0;

		controller.collisions.mode = fricCtrl;
		
		//timer for using special mode after bumping into a wall
		bumpTimer -= Time.deltaTime;
		if(bumpTimer < 0) bumpTimer = 0;
		launchTimer -= Time.deltaTime;
		if(launchTimer < 0) launchTimer = 0;
		
		//low friction
		//accelerate based on slope, no user input
		//can't change direction
		if(controller.collisions.mode == 1 && bumpTimer <= 0){
			//if not on ground or speed is too low, bump out to normal mode
			if(!controller.collisions.below || Mathf.Abs(xsp) <= 0.01f){
				controller.collisions.mode = 0;
			}
			else{
				anim.SetBool("hiFricAnim", false); //stop hi-fric anim if playing
				anim.SetBool("jumping", false);
				jumping = false;
				anim.SetBool("sliding", true);
				//check raycasts of character
				RaycastHit leftRayInfo, rightRayInfo;
				if(doubleRaycastDown(out leftRayInfo, out rightRayInfo)){
					//both rays hit something. now move and rotate character
					slidePosition(leftRayInfo, rightRayInfo);
					xsp = Mathf.Lerp(xsp, xsp-(slp*Mathf.Sin(oldSlideAngle * Mathf.Deg2Rad)*1.8f), Time.deltaTime);
					print("moving " + transform.right);
				}
				else{
					//off the edge. launch off
					print("MISSed" + transform.right);
					controller.collisions.mode = 0;
					//set xsp and ysp based on direction of angle forward
					ysp = (xsp*transform.right).y;
					//xsp = (xsp*transform.right).x;
					print(xsp + "  " + ysp);
					bumpTimer = bumpTime;
					launchTimer = launchTime;
				}
				
			}
		}
		
		//normal mode
		if(controller.collisions.mode == 0){
			//reset rotation of transform
			transform.rotation = Quaternion.identity;
			anim.SetBool("sliding", false); //stop no-fric anim if playing
			anim.SetBool("hiFricAnim", false); //stop hi-fric anim if playing
			
			//face direction
			float moveDir = Input.GetAxisRaw("Horizontal");
			if(moveDir != 0)
				faceDir = (moveDir<0 ? 180 : 0);
			mesh.transform.rotation = Quaternion.Euler(0, faceDir, 0);
			
			//slope of ground beneath us
			RaycastHit hit;
			Debug.DrawRay(transform.position+(Vector3.up*0.5f), -Vector2.up * 1f, Color.red);
			float slopeAngle = 0;
			if(Physics.Raycast(transform.position+(Vector3.up*0.5f), -Vector2.up, out hit, 1f, collisionMask)){
				slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if(Vector3.Cross(hit.normal, Vector2.up).z > 0){
					slopeAngle = 360 - slopeAngle;
				}
				oldSlideAngle = slopeAngle;
			}
			//print(slopeAngle);
			
			//running into wall, 0 out xsp
			if(controller.collisions.below && (controller.collisions.left || controller.collisions.right)){
				xsp = 0;
			}
			
			//pressing left
			if(inputLR < 0){
				if(xsp > 0){
					xsp = Mathf.Lerp(xsp, xsp-dec, Time.deltaTime);
				}
				else if(xsp > -top){
					xsp = Mathf.Lerp(xsp, xsp-acc, Time.deltaTime);
				}
			}
			//pressing right
			else if(inputLR > 0){
				if(xsp < 0){
					xsp = Mathf.Lerp(xsp, xsp+dec, Time.deltaTime);
				}
				else if(xsp < top){
					xsp = Mathf.Lerp(xsp, xsp+acc, Time.deltaTime);
				}
			}
			//not pressing anything, friction kicks in
			else if(controller.collisions.below)
				xsp = Mathf.Lerp(xsp, xsp-(Mathf.Min(Mathf.Abs(xsp), frc)*Mathf.Sign(xsp)), Time.deltaTime);
			else if(ysp > 0 && ysp < 1){	//air drag
				if (Mathf.Abs(xsp) > 0.05f)
					xsp = xsp * 0.96875f;
			}
			
			//air/jump movement
			//nothing below us, add gravity
			if(!controller.collisions.below){
				ysp = Mathf.Lerp(ysp, ysp+grv, Time.deltaTime);
			}
			//if we're in collision with the ground and press "jump", we jump
			if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below){
				//ysp = jmp + Mathf.Abs(xsp)*.2f; //add a little bit of x-speed to jump
				xsp = xsp-jmp * Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
				ysp = jmp * Mathf.Cos(slopeAngle * Mathf.Deg2Rad);
				anim.SetBool("jumping", true);
				jumping = true;
			}
			else if(jumping && controller.collisions.below){
				anim.SetBool("jumping", false);
				jumping = false;
			}
			
			//if we let go of jump button early
			if(Input.GetKeyUp(KeyCode.Space) && ysp > jmp*.3f){
				ysp = jmp*.3f;
			}
			
			//accelerate going downhill, slow down going uphill
			xsp = Mathf.Lerp(xsp, xsp-(slp*Mathf.Sin(slopeAngle * Mathf.Deg2Rad)*(controller.collisions.climbingSlope?2f:1)), Time.deltaTime);
			anim.SetFloat("inputH", Mathf.Abs(xsp));
			anim.SetFloat("inputV", ysp);
			
			//cap to max speed
			if(controller.collisions.below && launchTimer <= 0){
				xsp = Mathf.Clamp(xsp, -top, top);
			}

			controller.Move(new Vector3(xsp, ysp, 0));
		}
		
		//high friction
		//takes player input
		//grip to surface
		//low max speed
		else if(controller.collisions.mode == -1 && bumpTimer <= 0){
			const float hiFricSpCap = 0.03f;
			
			//cap speed
			if(Mathf.Abs(xsp) > hiFricSpCap){
				xsp = hiFricSpCap * Mathf.Sign(xsp);
			}
			//reset animations of transform
			anim.SetBool("sliding", false); //stop low-fric anim if playing
			anim.SetBool("jumping", false);
			jumping = false;
			anim.SetBool("hiFricAnim", true); //no-fric anim
			
			//face direction
			float moveDir = Input.GetAxisRaw("Horizontal");
			if(moveDir != 0)
				faceDir = (moveDir<0 ? 180 : 0);
			mesh.transform.eulerAngles = new Vector3(0, faceDir, 0);
			
			if(!controller.collisions.below){
				controller.collisions.mode = 0;
			}
			else{
				//pressing left
				if(inputLR < 0){
					if(xsp > 0){
						xsp = Mathf.Lerp(xsp, xsp-dec, Time.deltaTime);
					}
					else if(xsp > -hiFricSpCap){
						xsp = Mathf.Lerp(xsp, xsp-acc, Time.deltaTime);
					}
				}
				//pressing right
				else if(inputLR > 0){
					if(xsp < 0){
						xsp = Mathf.Lerp(xsp, xsp+dec, Time.deltaTime);
					}
					else if(xsp < hiFricSpCap){
						xsp = Mathf.Lerp(xsp, xsp+acc, Time.deltaTime);
					}
				}
				//not pressing anything, friction kicks in
				else if(!jumping)
					xsp = Mathf.Lerp(xsp, xsp-(Mathf.Min(Mathf.Abs(xsp), frc)*Mathf.Sign(xsp)), Time.deltaTime);
				else if(ysp > 0 && ysp < 1){	//air drag
					if (Mathf.Abs(xsp) > 0.05f)
						xsp = xsp * 0.96875f;
				}
				
				//check raycasts of character
				RaycastHit leftRayInfo, rightRayInfo;
				if(doubleRaycastDown(out leftRayInfo, out rightRayInfo)){
					//both rays hit something. now move and rotate character
					slidePosition(leftRayInfo, rightRayInfo);
				}
				else{
					//off the edge. bump to normal mode
					print("MISSed");
					controller.collisions.mode = 0;
					bumpTimer = bumpTime;
				}
			}
		}
	}
	
	bool doubleRaycastDown(out RaycastHit leftRayInfo, out RaycastHit rightRayInfo){
		
		float rayLength = 2f;
		float centerY = transform.GetComponent<BoxCollider>().bounds.center.y;
		
		//make sure rays shoot from right place
		controller.UpdateRaycastOrigins();
		
		Vector2 slideOffset = xsp * transform.right;
		Vector2 updatedBottomLeft = new Vector2(controller.raycastOrigins.bottomLeft.x + slideOffset.x, centerY + slideOffset.y);
		Vector2 updatedBottomRight = new Vector2(controller.raycastOrigins.bottomRight.x + slideOffset.x, centerY + slideOffset.y);
		
		//shoot one from bottomleft, one from bottomright
		Ray leftRay = new Ray(updatedBottomLeft, -transform.up);
		Ray rightRay = new Ray(updatedBottomRight, -transform.up);
		
		//debug
		Debug.DrawRay(updatedBottomLeft, -transform.up * rayLength, Color.red);
		Debug.DrawRay(updatedBottomRight, -transform.up * rayLength, Color.red);
		
		//check for walls
		//if moving left
		if(xsp < 0){
			Debug.DrawRay(transform.position+transform.up*0.2f, -transform.right * 0.5f, Color.red);
			if(Physics.Raycast(transform.position+transform.up*0.2f, -transform.right, 0.5f, collisionMask)){
				//hit a wall
				xsp = 0;
				rightRayInfo = new RaycastHit();
				leftRayInfo = new RaycastHit();
				return false;
			}
			Debug.DrawRay(transform.position+transform.up, -transform.right * 0.5f, Color.red);
			if(Physics.Raycast(transform.position+transform.up, -transform.right, 0.5f, collisionMask)){
				//hit a wall
				xsp = 0;
				rightRayInfo = new RaycastHit();
				leftRayInfo = new RaycastHit();
				return false;
			}
		}
		//if moving right
		else if(xsp > 0){
			Debug.DrawRay(transform.position+transform.up*0.2f, transform.right * 0.5f, Color.red);
			if(Physics.Raycast(transform.position+transform.up*0.2f, transform.right, 0.5f, collisionMask)){
				//hit a wall
				xsp = 0;
				rightRayInfo = new RaycastHit();
				leftRayInfo = new RaycastHit();
				return false;
			}
			Debug.DrawRay(transform.position+transform.up, transform.right * 0.5f, Color.red);
			if(Physics.Raycast(transform.position+transform.up, transform.right, 0.5f, collisionMask)){
				//hit a wall
				xsp = 0;
				rightRayInfo = new RaycastHit();
				leftRayInfo = new RaycastHit();
				return false;
			}
		}
		
		return Physics.Raycast(leftRay, out leftRayInfo, rayLength, collisionMask) && Physics.Raycast(rightRay, out rightRayInfo, rayLength, collisionMask);
	}
	
	void slidePosition(RaycastHit leftRayInfo, RaycastHit rightRayInfo){
		Vector3 averageNormal = (leftRayInfo.normal + rightRayInfo.normal) / 2;
		Vector3 averagePoint = (leftRayInfo.point + rightRayInfo.point) / 2;
		
		Debug.DrawRay(leftRayInfo.point, leftRayInfo.normal, Color.magenta);
		Debug.DrawRay(rightRayInfo.point, rightRayInfo.normal, Color.magenta);
		Debug.DrawRay(averagePoint, averageNormal, Color.white);
		
		Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
		Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationDegrees);
		transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);
	
		if(controller.collisions.mode == -1){
			//face direction
			float moveDir = Input.GetAxisRaw("Horizontal");
			if(moveDir != 0)
				faceDir = (moveDir<0 ? 180 : 0);
			mesh.transform.rotation = Quaternion.Euler(0, faceDir, 0);
		}
		
		float slopeAngle = Vector2.Angle(averageNormal, Vector2.up);
				if(Vector3.Cross(averageNormal, Vector2.up).z > 0){
					slopeAngle = 360 - slopeAngle;
				}
				
		transform.position = averagePoint + transform.up*.2f;
		print("old angle: " + oldSlideAngle.ToString() + "     new angle: " + slopeAngle.ToString());
		oldSlideAngle = slopeAngle;
	}
	
	public void bounce(Vector3 bounceAmt){
		print(bounceAmt);
		controller.collisions.mode = 0;
		xsp = bounceAmt.x;
		ysp = bounceAmt.y;
		bumpTimer = bumpTime;
	}
}
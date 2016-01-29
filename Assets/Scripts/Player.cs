using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof(Animator))]
public class Player : MonoBehaviour {
	
	public LayerMask collisionMask;

	public Animator anim;
	
	public float jumpHeight, timeToJumpApex, moveSpeed;
	bool jumping;
	private float gravity, jumpVelocity, originalGravity;
	float accelerationTimeAirborne = .2f, accelerationTimeGrounded = .2f;
	//Vector3 velocity;
	float velocityXSmoothing;
	
	private float xsp, ysp;
	const float acc = 0.046875f;
	const float dec = 0.5f;
	const float frc = 0.046875f;
	const float top = 6f;
	const float air = 0.09375f;
	const float grv = -0.21875f;
	const float jmp = .1f;
	
	private int fricLvl;
	
	Controller2D controller;
	
	void Start() {
		controller = GetComponent<Controller2D> ();
		
		anim = GetComponent<Animator> ();
		
		originalGravity = gravity = -(2 * jumpHeight)/Mathf.Pow(timeToJumpApex,2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		jumping = false;
		
		fricLvl = 0;
		
		print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
	}
	
	void OnGUI(){
		GUI.Box(new Rect(Screen.width - 200, 0, 200, 200), "Angle");
		GUI.Label(new Rect(Screen.width - 100, 20, 100, 50), controller.maxClimbAngle.ToString());
		GUI.Label(new Rect(Screen.width - 100, 30, 100, 50), moveSpeed.ToString());
		GUI.Label(new Rect(Screen.width - 50, 25, 100, 50), fricLvl.ToString());
		GUI.Label(new Rect(Screen.width - 100, 40, 100, 50), "xspeed: " + xsp.ToString());
		GUI.Label(new Rect(Screen.width - 100, 70, 100, 50), "yspeed: " + ysp.ToString());
		GUI.Label(new Rect(Screen.width - 100, 110, 100, 50), "collisions\nbelow: " + controller.collisions.below.ToString());
	}
	
	void Update(){
		//if(controller.collisions.above || controller.collisions.below){
			//ysp = 0;
		//}
		
		float inputLR = Input.GetAxisRaw("Horizontal");
		int fricCtrl = 0;
		if(Input.GetKeyDown("down"))
			fricCtrl = -1;
		else if(Input.GetKeyDown("up"))
			fricCtrl = 1;
		
		if (fricCtrl != 0){
			print(fricCtrl);
			fricLvl = Mathf.Clamp(fricLvl + fricCtrl, -1, 1);

			switch(fricLvl){
				case -1:
					controller.collisions.mode = -1;
					break;
				case 0:
					controller.collisions.mode = 0;
					break;
				case 1:
					controller.collisions.mode = 1;
					break;
				default:
					break;
			}
		}
		
		/*if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below){
			velocity.y = jumpVelocity;
			anim.SetBool("jumping", true);
			jumping = true;
		}
		else if(jumping && controller.collisions.below){
			anim.SetBool("jumping", false);
			jumping = false;
		}*/
		
		//normal mode
		if(controller.collisions.mode == 0){
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
			else
				xsp = Mathf.Lerp(xsp, xsp-(Mathf.Min(Mathf.Abs(xsp), frc)*Mathf.Sign(xsp)), Time.deltaTime);
			
			//air/jump movement
			//nothing below us, add gravity
			if(!controller.collisions.below){
				ysp = Mathf.Lerp(ysp, ysp+grv, Time.deltaTime);
			}
			//if we're in collision with the ground and press "jump", we jump
			//sometimes we're not colliding with ground when we're obviously touching the ground. why is this??
			if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below){
				ysp = jmp;
			}
		}
		
		//high friction
		else if(controller.collisions.mode == 1){

		}
		
		//no friction
		else if(controller.collisions.mode == -1){
			
		}
		
		anim.SetFloat("inputH", Mathf.Abs(xsp));
		anim.SetFloat("inputV", ysp);
		controller.Move(new Vector3(xsp, ysp, 0));
	}
}
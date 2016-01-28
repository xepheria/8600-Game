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
	Vector3 velocity;
	float velocityXSmoothing;
	
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
		GUI.Box(new Rect(Screen.width - 100, 0, 100, 50), "Angle");
		GUI.Label(new Rect(Screen.width - 100, 20, 100, 50), controller.maxClimbAngle.ToString());
		GUI.Label(new Rect(Screen.width - 100, 30, 100, 50), moveSpeed.ToString());
		GUI.Label(new Rect(Screen.width - 50, 25, 100, 50), fricLvl.ToString());
	}
	
	void Update(){
		if(controller.collisions.above || controller.collisions.below){
			velocity.y = 0;
		}
		
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		int fricCtrl = 0;
		if(Input.GetKeyDown("down"))
			fricCtrl = -1;
		else if(Input.GetKeyDown("up"))
			fricCtrl = 1;
		
		if (fricCtrl != 0){
			print(fricCtrl);
			fricLvl = Mathf.Clamp(fricLvl + fricCtrl, 0, 1);
		}
		
		if(fricCtrl != 0){
			switch(fricLvl){
				case 0:
					controller.maxClimbAngle = 90;
					moveSpeed = 2;
					gravity = originalGravity;
					break;
				case 1:
					controller.maxClimbAngle = 75;
					moveSpeed = 15;
					gravity = originalGravity;
					break;
				default:
					break;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below){
			velocity.y = jumpVelocity;
			anim.SetBool("jumping", true);
			jumping = true;
		}
		else if(jumping && controller.collisions.below){
			anim.SetBool("jumping", false);
			jumping = false;
		}
		
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
		
		anim.SetFloat("inputH", Mathf.Abs(velocity.x));
		anim.SetFloat("inputV", velocity.y);
		controller.Move(velocity * Time.deltaTime);
	}
}
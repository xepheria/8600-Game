﻿using UnityEngine;
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
	
	BoxCollider2D boxCollider;
	bool descending;
	
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
	
	private MeshRenderer mesh;
	
	private float faceDir;
	
	void Start() {
		controller = GetComponent<Controller2D> ();
		
		anim = GetComponent<Animator> ();
		
		originalGravity = gravity = -(2 * jumpHeight)/Mathf.Pow(timeToJumpApex,2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		jumping = false;
		
		fricLvl = 0;
		
		print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
		
		mesh = GetComponentInChildren<MeshRenderer>();
		
		boxCollider = GetComponent<BoxCollider2D>();
	}
	
	void OnGUI(){
		GUI.Box(new Rect(Screen.width - 200, 0, 200, 200), "Angle: " + controller.collisions.slopeAngle);
		GUI.Label(new Rect(Screen.width - 50, 25, 100, 50), fricLvl.ToString());
		GUI.Label(new Rect(Screen.width - 100, 40, 100, 50), "xspeed: " + xsp.ToString());
		GUI.Label(new Rect(Screen.width - 100, 70, 100, 50), "yspeed: " + ysp.ToString());
		GUI.Label(new Rect(Screen.width - 175, 110, 100, 50), "below: " + controller.collisions.below.ToString());
		GUI.Label(new Rect(Screen.width - 175, 130, 100, 50), "ascending: " + controller.collisions.climbingSlope.ToString());
		GUI.Label(new Rect(Screen.width - 175, 150, 100, 50), "descending: " + controller.collisions.descendingSlope.ToString());
		GUI.Label(new Rect(Screen.width - 50, 150, 100, 50), faceDir.ToString());
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
			if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below){
				ysp = jmp;
				anim.SetBool("jumping", true);
				jumping = true;
			}
			else if(jumping && controller.collisions.below){
				anim.SetBool("jumping", false);
				jumping = false;
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
		
		//face direction
		RaycastHit2D hit = Physics2D.Raycast(transform.position,
							-Vector2.up, collisionMask);
		float slopeAngle = 0;
		if(hit){
			slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(Vector3.Cross(hit.normal, Vector2.up).z > 0){
				slopeAngle = 360 - slopeAngle;
			}
		}
		print(slopeAngle);
		float moveDir = Input.GetAxisRaw("Horizontal");
		
		if(moveDir != 0)
			faceDir = (moveDir<0 ? 180 : 0);
		
		//this one almost works
		//mesh.transform.rotation = Quaternion.Euler(0, faceDir, 0);
		//mesh.transform.rotation = collisions.descendingSlope ? Quaternion.Euler(0, faceDir, collisions.slopeAngle*-1) : Quaternion.Euler(0, faceDir, collisions.slopeAngle);
		mesh.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * Quaternion.Euler(0, faceDir, 0);
		if(slopeAngle > 0){
			if(slopeAngle>180)
				mesh.transform.position = new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.min.y, 0);
			else
				mesh.transform.position = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y, 0);
		}
	}
}
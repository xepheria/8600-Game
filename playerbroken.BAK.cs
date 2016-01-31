using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof(Animator))]
public class Player : MonoBehaviour {
	public LayerMask collisionMask;
	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	
	public float maxClimbAngle = 90;
	float maxDescendAngle = 75;
	
	float horizontalRaySpacing;
	float verticalRaySpacing;
	
	public Animator anim;
	
	private MeshRenderer mesh;

	bool jumping, falling;
	private float gravity, jumpVelocity, originalGravity;
	float accelerationTimeAirborne = .2f, accelerationTimeGrounded = .2f;
	//Vector3 velocity;
	float velocityXSmoothing;
	
	private float gsp = 0, xsp = 0, ysp = 0;
	const float acc = 0.046875f;
	const float dec = 0.5f;
	const float frc = 0.046875f;
	const float top = 6f;
	const float air = 0.09375f;
	const float grv = -0.21875f;
	const float jmp = .15f;
	
	private int fricLvl;
	
	//Controller2D controller;
	
	BoxCollider2D boxCollider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;
	
	void Start() {
		
		boxCollider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
		mesh = GetComponentInChildren<MeshRenderer>();
		
		//controller = GetComponent<Controller2D> ();
		
		anim = GetComponent<Animator> ();
		
		jumping = false;
		
		fricLvl = 0;
	}
	
	void UpdateRaycastOrigins() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);
		
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	
	void CalculateRaySpacing() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);
		
		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);
		
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
	void OnGUI(){
		GUI.Box(new Rect(Screen.width - 200, 0, 200, 200), "Info");
		GUI.Label(new Rect(Screen.width - 50, 25, 100, 50), fricLvl.ToString());
		GUI.Label(new Rect(Screen.width - 100, 40, 100, 50), "xspeed: " + xsp.ToString());
		GUI.Label(new Rect(Screen.width - 100, 70, 100, 50), "yspeed: " + ysp.ToString());
		GUI.Label(new Rect(Screen.width - 100, 100, 100, 50), "gsp: " + gsp.ToString());
		GUI.Label(new Rect(Screen.width - 100, 120, 100, 50), "collisions\nbelow: " + collisions.below.ToString());
	}
	
	void Update(){
		//update where our rays are shooting from
		UpdateRaycastOrigins ();
		collisions.Reset();

		
		
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
					collisions.mode = -1;
					break;
				case 0:
					collisions.mode = 0;
					break;
				case 1:
					collisions.mode = 1;
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
	
		//check collisions
		UnderTileCollisions();
		
		//normal mode
		if(collisions.mode == 0){
			if(ysp < 0) falling = true;
			
			//pressing left
			if(inputLR < 0){
				if(gsp > 0){
					gsp = Mathf.Lerp(gsp, gsp-dec, Time.deltaTime);
				}
				else if(gsp > -top){
					gsp = Mathf.Lerp(gsp, gsp-acc, Time.deltaTime);
				}
			}
			//pressing right
			else if(inputLR > 0){
				if(gsp < 0){
					gsp = Mathf.Lerp(gsp, gsp+dec, Time.deltaTime);
				}
				else if(gsp < top){
					gsp = Mathf.Lerp(gsp, gsp+acc, Time.deltaTime);
				}
			}
			//not pressing anything, friction kicks in
			else
				gsp = Mathf.Lerp(gsp, gsp-(Mathf.Min(Mathf.Abs(gsp), frc)*Mathf.Sign(gsp)), Time.deltaTime);
			
			//air/jump movement
			//nothing below us, add gravity
			if(!collisions.below){
				ysp = Mathf.Lerp(ysp, ysp+grv, Time.deltaTime);
			}
			
			/*
			//if we're in collision with the ground and press "jump", we jump
			//sometimes we're not colliding with ground when we're obviously touching the ground. why is this??
			if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below){
				ysp = jmp;
			}
			//if we let go of the jump button before jump reaches apex, stop increasing height
			if(Input.GetKeyUp(KeyCode.Space) && !controller.collisions.below && ysp > .04f){
				ysp = .04f;
			}*/
		}
		
		//high friction
		else if(collisions.mode == 1){

		}
		
		//no friction
		else if(collisions.mode == -1){
			
		}
		
		xsp = gsp*Mathf.Cos(collisions.slopeAngle);
		//ysp = gsp*(-1)*Mathf.Sin(collisions.slopeAngle);
		
		anim.SetFloat("inputH", Mathf.Abs(xsp));
		anim.SetFloat("inputV", ysp);
		
		transform.Translate(new Vector3(xsp, ysp, 0),Space.World);
		
		//controller.Move(new Vector3(xsp, ysp, 0));
	}
	
	void UnderTileCollisions(){
		float directionY = Mathf.Sign(ysp);
		if(ysp == 0) directionY = -1;
		float rayLength = Mathf.Abs(ysp) + skinWidth;
		
		for(int i = 0; i < verticalRayCount; i++){
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + xsp);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
			
			if(hit){
				rayLength = hit.distance;
								
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}
}



struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
	
public struct CollisionInfo{
	//-1: no friction
	// 0: normal
	// 1: high friction
	public int mode;
	public bool above, below, left, right, climbingSlope, descendingSlope;
	public float slopeAngle, slopeAngleOld;
	public Vector3 velocityOld;
	public void Reset(){
		descendingSlope = climbingSlope = above = below = left = right = false;
		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
}
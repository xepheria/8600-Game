using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {
	public LayerMask collisionMask;
	public const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	
	public float maxClimbAngle = 60;
	float maxDescendAngle = 75;
	
	float horizontalRaySpacing;
	float verticalRaySpacing;
	
	BoxCollider boxCollider;
	public RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;
		
	void Start() {
		boxCollider = GetComponent<BoxCollider> ();
		CalculateRaySpacing ();
	}

	public void Move(Vector3 velocity){
		//update where our rays are shooting from
		UpdateRaycastOrigins ();
		collisions.Reset();
		collisions.velocityOld = velocity;
		if(velocity.y < 0){
			DescendSlope(ref velocity);
		}
		if(velocity.x != 0){
			HorizontalCollisions(ref velocity);
		}
		if(velocity.y != 0){
			VerticalCollisions(ref velocity);
		}
		transform.Translate(velocity,Space.World);
	}
	
	void HorizontalCollisions(ref Vector3 velocity){
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		
		for (int i = 0; i < horizontalRayCount; i++) {
			Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector3.up * (horizontalRaySpacing * i);
			RaycastHit hit;
						
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
			
			if(Physics.Raycast(rayOrigin, Vector2.right * directionX, out hit, rayLength, collisionMask)){
				//get angle of the surface we hit
				//using the surface's normal
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if(i == 0 && slopeAngle <= maxClimbAngle){
					if(collisions.descendingSlope){
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if(slopeAngle != collisions.slopeAngleOld){
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}
				
				//set x velocity
				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle){
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;
					
					if(collisions.climbingSlope){
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}
					
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}
	
	void VerticalCollisions(ref Vector3 velocity){
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;
		
		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit hit;
			
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
			
			if(Physics.Raycast(rayOrigin, Vector2.up * directionY, out hit, rayLength, collisionMask)){
				//set y velocity
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;
				
				if(collisions.climbingSlope){
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}
								
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
		
		if(collisions.climbingSlope){
			//check for new slope
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit hit;

			if(Physics.Raycast(rayOrigin, Vector2.right * directionX, out hit, rayLength, collisionMask)){
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if(slopeAngle != collisions.slopeAngle){
					//new slope
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}
	
	void ClimbSlope(ref Vector3 velocity, float slopeAngle){
		//using how much distance we expect to move from velocity
		//and the angle of the slope, create a new velocity
		float moveDistance = Mathf.Abs(velocity.x);
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
		
		if(velocity.y <= climbVelocityY){
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}
	
	void DescendSlope(ref Vector3 velocity){
		//which way are we facing
		float directionX = Mathf.Sign(velocity.x);
		//ray origin is bottom right if facing left
		Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomRight:raycastOrigins.bottomLeft;
		//fire the ray straight down until we hit the surface.
		RaycastHit hit;
		
		if(Physics.Raycast(rayOrigin, -Vector2.up, out hit, Mathf.Infinity, collisionMask)){			
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle){
				if(Mathf.Sign(hit.normal.x) == directionX){
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)){
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;
						
						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}
	
	public void UpdateRaycastOrigins() {
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
	
	public struct RaycastOrigins {
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
	
}
using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class boxPhysics : MonoBehaviour {

	public bool isSticky;
	

	// Use this for initialization
	void Start () {
		changeFriction(isSticky);
	}		
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.G)){
			isSticky = !isSticky;
			changeFriction(isSticky);
		}
	}
	
		
	void changeFriction(bool Sticky){
		Renderer rend = GetComponent<Renderer>();
		PhysicMaterial material = 	GetComponent<Collider>().material;
		
		if(Sticky){
			material.dynamicFriction = 1;
			material.staticFriction = 1;
			rend.material.shader = Shader.Find ("Standard");
			rend.material.SetColor ("_Color", Color.red);
		} else {
			material.dynamicFriction = 0;
			material.staticFriction = 0;
			rend.material.shader = Shader.Find ("Standard");
			rend.material.SetColor ("_Color", Color.blue);
		}
	}
	
	public float bounceAmt;
	public Player player;
	public float velocityDecay = 0.9f;
	
	void OnTriggerStay(Collider col){
	
		Rigidbody rb = GetComponent<Rigidbody>();

		print("collision entered");
		if(col.CompareTag("Player")){
			if(!isSticky){
				//rb.velocity = ;
			}
		}
		if(col.CompareTag("ground")){
			if(isSticky){
				rb.velocity = rb.velocity*velocityDecay;
				rb.angularVelocity  = rb.angularVelocity*velocityDecay;
			}
		}
	}
}


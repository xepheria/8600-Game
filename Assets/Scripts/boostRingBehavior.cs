using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Animator))]
//[RequireComponent (typeof(ParticleSystem))]
public class boostRingBehavior : MonoBehaviour {

	public float boostAmt;
	public Player player;
	private float y; //used for floating effect
	private float amplitude = 0.06f;
	private float speed = 1f;

	private ParticleSystem ps;
	
	public Animator anim;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player>();
		y = transform.position.y;
		GetComponent<Animator>();

		GameObject child = this.gameObject.transform.GetChild (0).gameObject;
		//print ("HELLO");
		//print ("HI: " + child);
		//psArray = GetComponentsInChildren<ParticleSystem> ();
		ps = child.GetComponent<ParticleSystem> ();

	}
	
	void OnTriggerEnter(Collider col){
		print("collision entered");
		if(col.CompareTag("Player")){
			var em = ps.emission;
			em.enabled = true;
			ps.Emit (20);


			player.bounce(transform.up * boostAmt);
			anim.Play("Boost");
			//ps.emission.enabled = true;
			//ps.emission.type = ParticleSystemEmissionType.Time;

		}
	}
	
	void Update(){
		transform.position = new Vector3(transform.position.x, y + amplitude * Mathf.Sin(speed*Time.time), 0);
	}
}

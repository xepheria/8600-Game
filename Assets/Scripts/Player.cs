﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(Animator))]
public class Player : MonoBehaviour {
	
	float fpsCounter = 0.0f;
	
	public LayerMask collisionMask;

	public Animator anim;
	
	bool jumping;
	float velocityXSmoothing;
	
	bool descending;
	
	Vector3 originalBodyPosition, originalCapePosition;
	
	private float xsp, ysp;
	const float acc = 0.08f;
	const float hiAcc = .8f; //Acceleration at high friction (instant or close to it)
	const float hiFricSpCap = .05f; //I moved this variable up here because I suck at finding things
	const float dec = 0.5f;
	const float frc = 0.046875f;
	const float top = 0.11f;
	const float aboveTopDec = 6f; //This is multiplied by delta time in a lerp for if normal mode is above max speed
	const float air = 0.09375f;
	const float grv = -0.28f;
	const float jmp = .145f;
	const float slp = 0.15f;
	const float maxRotationDegrees = 20f;
	float oldSlideAngle;
	float bumpTimer, launchTimer;
	const float bumpTime = 0.2f; //how long to wait
	const float launchTime = 0.2f;
	const float barTime = 1.0f; //if we run out of energy
	
	float angleToShoot;
	int angleIncSign;
	Vector3 shootDir;
	public GameObject shot;
	
	bool showDebug;
	
	Controller2D controller;
	BoxCollider boxCollider;
	
	public Transform bodyMesh;
	//public Transform capeMesh;
	public GameObject loFricStuff, hiFricStuff;
	public GameObject[] bootButtons = new GameObject[4];
	public Material blueGlow, redGlow, originalButtonShader;

	private TrailRenderer trail;
	public float trailLifetime;
	private float trailOffTime;	
	private float startTime;
	public float spawnDelay;
	private float spawnTimer = 0;
	public Transform deathFX;
	GameObject deathFXInstance;
	
	private float faceDir;
	
	public bool canMove;
	private bool gameOver;
	public bool onBelt;
	//public Image gameOverOverlay;
	//public Text gameOverText;
	
	//power meter
	public Texture2D barHigh,barLow,barNorm, barFull;
	float hiFricEnergy;
	private Vector2 barSize = new Vector2(214, 11);
	float hiFricEnergyInc = 0.05f;
	float hiFricEnergyDec = 0.15f;
	
	//audio stuff
	public AudioClip climbingSFX, fricDownSFX, fricUpSFX, slidingSFX, fricModeOffSFX, pickupSFX, spawnSFX, despawnSFX;
	private AudioSource audioClimbing, audioFricDown, audioFricUp, audioSliding, audioPickup, audioSpawn, audioDespawn;

	public void resetSpawnTimer(){
		this.spawnTimer = 0;
	}

	public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol){
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip;
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol;
		return newAudio;
	}
	public void Awake(){
		//add audiosources to Player
		audioClimbing = AddAudio(climbingSFX, true, false, 0.08f);
		audioFricDown = AddAudio(fricDownSFX, false, false, 0.5f);
		audioFricUp = AddAudio(fricUpSFX, false, false, 0.5f);
		audioSliding = AddAudio(slidingSFX, true, false, 0.08f);
		audioPickup = AddAudio (pickupSFX, false, false, 0.5f);
		audioSpawn = AddAudio (spawnSFX, false, false, 0.5f);
		audioDespawn = AddAudio (despawnSFX, false, false, 0.5f);
	}
	
	void Start() {
		hiFricEnergy = 1;
		
		gameOver = false;
		
		showDebug = false;
		
		controller = GetComponent<Controller2D> ();
		boxCollider = GetComponent<BoxCollider>();
		
		anim = GetComponent<Animator> ();

		jumping = false;

		controller.collisions.mode = 0;

		
		angleToShoot = 0;
		angleIncSign = 1;
		
		//gameOverOverlay.color = new Color(0,0,0,0);
		//gameOverOverlay.gameObject.SetActive(false);
		//gameOverText.gameObject.SetActive(false);

		originalBodyPosition = bodyMesh.transform.localPosition;

		trail = GameObject.Find("LowFricTrail").GetComponent<TrailRenderer>();
		print ("LOW FRIC TRAIL: " + trail);
		trail.enabled = true;
		trail.time = trailLifetime;
		trailOffTime = 0;
		startTime = Time.time;
		this.spawnTimer = 0;
		
	}
	
	void OnGUI(){
		if(showDebug){
			GUI.Box(new Rect(Screen.width - 200, 0, 200, 200), "Angle: " + oldSlideAngle);
			GUI.Label(new Rect(Screen.width - 50, 25, 100, 50), controller.collisions.mode.ToString());
			GUI.Label(new Rect(Screen.width - 100, 40, 100, 50), "xspeed: " + xsp.ToString());
			GUI.Label(new Rect(Screen.width - 100, 70, 100, 50), "yspeed: " + ysp.ToString());
			GUI.Label(new Rect(Screen.width - 175, 110, 100, 50), "below: " + controller.collisions.below.ToString());
			GUI.Label(new Rect(Screen.width - 175, 130, 100, 50), "ascending: " + controller.collisions.climbingSlope.ToString());
			GUI.Label(new Rect(Screen.width - 175, 150, 100, 50), "descending: " + controller.collisions.descendingSlope.ToString());
			GUI.Label(new Rect(Screen.width - 50, 150, 100, 50), bumpTimer.ToString());
			
			//demo instructions
			float msec = fpsCounter * 1000.0f;
			float fps = 1.0f / fpsCounter;
			string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
			GUI.Box(new Rect(0, 0, 400, 40), "Framerate: " + text);
		}
		
		//power gauge
		GUI.BeginGroup(new Rect(Screen.width - 300, Screen.height - 100, 300, 100));
			//GUI.Box(new Rect(0, 0, 200, 100), "POWER");
			if (controller.collisions.mode == 1) {
				GUI.DrawTexture (new Rect (10, 30, 300, 80), barLow, ScaleMode.StretchToFill);
			} else if (controller.collisions.mode == -1) {
				GUI.DrawTexture (new Rect (10, 30, 300, 80), barHigh, ScaleMode.StretchToFill);
			} else {
				GUI.DrawTexture (new Rect (10, 30, 300, 80), barNorm, ScaleMode.StretchToFill);
			}
			//filled-in bar
			GUI.BeginGroup(new Rect(10+43, 30+33, barSize.x * hiFricEnergy, barSize.y));
				GUI.DrawTexture(new Rect(0, 0, barSize.x, barSize.y), barFull, ScaleMode.StretchToFill);
			GUI.EndGroup();
		GUI.EndGroup();
	}

	void Update(){
		//update fpsCounter
		fpsCounter += (Time.deltaTime - fpsCounter) * 0.1f;
		
		//Delay our player's spawn to align with the spawn portal.  The delay is a public variable in player and is NOT linked directly to the spawn animation.
		if (this.spawnTimer < this.spawnDelay) {
			if(!audioSpawn.isPlaying)	audioSpawn.Play();
			this.spawnTimer += Time.deltaTime;
			Renderer[] renderers = GetComponentsInChildren<Renderer> ();
			foreach(Renderer r in renderers){
				r.enabled = false;
			}
			//Make sure we only have to turn on the renderers once
			if (this.spawnTimer >= this.spawnDelay) {
				foreach(Renderer r in renderers){
					r.enabled = true;
				}
				faceDir = 90;
			}
		}
		else{
		//for paused
			if (Time.timeScale > .5f) {
			
				if (Input.GetKeyDown (KeyCode.G)) {
					showDebug = !showDebug;
				}
		
				//game over stuff, reset scene
				if (gameOver) {
					return;
				}
		
				float inputLR = Input.GetAxisRaw ("Horizontal");
				int fricCtrl = 0;
				if (controller.collisions.mode == 2)
					fricCtrl = 2;

				if (Input.GetButton ("HiFric") && bumpTimer <= 0 && controller.collisions.below && hiFricEnergy > 0) {
					fricCtrl = -1;
					if (Input.GetButtonDown ("HiFric")) {
						hiFricEnergy -= .1f;
					}
					if (controller.collisions.mode == 0) {
						transform.rotation = Quaternion.Euler (0, 0, oldSlideAngle);
					}
				} else if (Input.GetButton ("LoFric") && bumpTimer <= 0 && controller.collisions.below) {
					fricCtrl = 1;
					if (controller.collisions.mode == 0) {
						transform.rotation = Quaternion.Euler (0, 0, oldSlideAngle);
					}
				}

				if (controller.collisions.mode != fricCtrl) {
					if (fricCtrl == 0) {
						//audioFricModeOff.Play();
						//Too much!
					} else if (fricCtrl == -1) {
						audioFricUp.Play ();
						if (audioSliding.isPlaying)
							audioSliding.Stop ();
						audioClimbing.Play ();
					} else if (fricCtrl == 1) {
						audioFricDown.Play ();
						if (audioClimbing.isPlaying)
							audioClimbing.Stop ();
						audioSliding.Play ();
						trail.time = trailLifetime;
						trailOffTime = trailLifetime;
					}
				}
				controller.collisions.mode = fricCtrl;
		
				//refill hifric energy bar
				if (controller.collisions.mode != -1) {
					if (controller.collisions.mode == 1) {
						hiFricEnergy = Mathf.Clamp (hiFricEnergy + Time.deltaTime * (hiFricEnergyInc*3 + (3 * Mathf.Abs (xsp))), 0, 1);
					} else if (controller.collisions.mode != 2){
						hiFricEnergy = Mathf.Clamp (hiFricEnergy + Time.deltaTime * hiFricEnergyInc, 0, 1);
					}
				}
		
				if (oldSlideAngle >= 20 && oldSlideAngle <= 340 && (controller.collisions.mode == 0 || (controller.collisions.mode == -1 && hiFricEnergy <= 0)) && controller.collisions.below) {
					controller.collisions.mode = 2;
					transform.rotation = Quaternion.Euler (0, 0, oldSlideAngle);
				}
		
				//if can't move, set xsp to 0, friction to normal
				if (!canMove) {
					xsp = 0;
					controller.collisions.mode = 0;
					inputLR = 0;
				}
		
				//timer for using special mode after bumping into a wall
				bumpTimer -= Time.deltaTime;
				if (bumpTimer < 0)
					bumpTimer = 0;
				launchTimer -= Time.deltaTime;
				if (launchTimer < 0)
					launchTimer = 0;
				if (bumpTimer > 0 || launchTimer > 0)
					controller.collisions.mode = 0;

				//low friction
				//accelerate based on slope, no user input
				//can't change direction
				if (controller.collisions.mode == 1) {
					//if not on ground, bump out to normal mode
					if (!controller.collisions.below) {
						controller.collisions.mode = 0;
					} else {
						foreach (GameObject but in bootButtons) {
							but.GetComponent<Renderer> ().material = blueGlow;
						}
				
						//face direction
						float moveDir = Input.GetAxisRaw ("Horizontal");
						if (inputLR != 0)
							faceDir = (moveDir < 0 ? 270 : 90);

						bodyMesh.transform.rotation = Quaternion.RotateTowards (bodyMesh.transform.rotation, Quaternion.Euler (0, faceDir, 0), 300 * Time.deltaTime);
				
						anim.SetBool ("hiFricAnim", false); //stop hi-fric anim if playing
						anim.SetBool ("jumping", false);
						anim.SetBool ("tumblingAnim", false);
						jumping = false;
						if ((inputLR > 0 && xsp < 0) || (inputLR < 0 && xsp > 0)) {
							anim.SetBool ("sliding", false);
						} else {
							anim.SetBool ("sliding", true);
						}

						loFricStuff.active = true;
						hiFricStuff.active = false;
				
						anim.SetFloat ("inputH", Mathf.Abs (xsp));
				
						//check raycasts of character
						RaycastHit leftRayInfo, rightRayInfo;
						if (doubleRaycastDown (out leftRayInfo, out rightRayInfo)) {
							//both rays hit something. now move and rotate character
					
							//if we press jump, do SLIDE JUMP
							if (Input.GetButtonDown("Jump")) {
								controller.collisions.mode = 0;
								ysp = Mathf.Clamp ((jmp * transform.up).y, 0, jmp * 2);
								xsp = Mathf.Clamp ((jmp * transform.up).x + (xsp * transform.right).x, -top * 2, top * 2);
						
								anim.SetBool ("jumping", true);
								anim.SetBool ("sliding", false);
								jumping = true;
								controller.Move (new Vector3 (xsp, ysp, 0));
								launchTimer = launchTime;

							} else {
								slidePosition (leftRayInfo, rightRayInfo);

								//Falling Upside down
								if (oldSlideAngle > 100 && oldSlideAngle < 260 && Mathf.Abs (xsp) < .06f) {
									ysp = 0;
									xsp = 0;
									controller.collisions.mode = 0;
								}
								xsp = Mathf.Lerp (xsp, xsp - (slp * Mathf.Sin (oldSlideAngle * Mathf.Deg2Rad) * 1.8f), Time.deltaTime);
							}
						} else {
							//off the edge. launch off
							print ("MISSed" + transform.right);
							controller.collisions.mode = 0;
							//set xsp and ysp based on direction of angle forward
							ysp = Mathf.Clamp ((xsp * transform.right).y, 0, jmp * 2);
							xsp = Mathf.Clamp ((xsp * transform.right).x, -top * 2, top * 2);
							print (xsp + "  " + ysp);
							bumpTimer = bumpTime;
							launchTimer = launchTime;
						}
					}
				}
		
				//tumble mode
				if (controller.collisions.mode == 2) {
					trailOffTime -= Time.deltaTime * 3f;
					trail.time = Mathf.Lerp (0, trailLifetime, trailOffTime / (trailLifetime));	
					foreach (GameObject but in bootButtons) {
						but.GetComponent<Renderer> ().material = originalButtonShader;
					}
						
					//check current slope of ground beneath us
					RaycastHit hit;
					Debug.DrawRay (transform.position + (transform.up * 0.5f), -transform.up * 1.5f, Color.red);
					float slopeAngle = 0;
					if (Physics.Raycast (transform.position + (transform.up * 0.5f), -transform.up, out hit, 1.5f, collisionMask)) {
						slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
						if (Vector3.Cross (hit.normal, Vector2.up).z > 0) {
							slopeAngle = 360 - slopeAngle;
						}
						oldSlideAngle = slopeAngle;
					}
					if (slopeAngle < 20 || slopeAngle > 340 || (slopeAngle > 91 && slopeAngle < 270))
						controller.collisions.mode = 0;
			
			
					anim.SetBool ("jumping", false);
					anim.SetBool ("sliding", false);
					anim.SetBool ("hiFricAnim", false);
					jumping = false;
					anim.SetBool ("tumblingAnim", true);

					loFricStuff.active = false;
					hiFricStuff.active = false;
			
					//check raycasts of character
					RaycastHit leftRayInfo, rightRayInfo;
					if (doubleRaycastDown (out leftRayInfo, out rightRayInfo)) {
						//both rays hit something. now move and rotate character
						if (Input.GetButtonDown("Jump")) {
							controller.collisions.mode = 0;
							ysp = Mathf.Clamp ((jmp * transform.up).y, 0, jmp * 2);
							xsp = Mathf.Clamp ((jmp * transform.up).x + (xsp * transform.right).x, -top * 2, top * 2);
						
							anim.SetBool ("jumping", true);
							anim.SetBool ("tumblingAnim", false);
							jumping = true;
							controller.Move (new Vector3 (xsp, ysp, 0));
							launchTimer = launchTime;
					
						} else {
							slidePosition (leftRayInfo, rightRayInfo);
							xsp = Mathf.Lerp (xsp, xsp - (slp * Mathf.Sin (oldSlideAngle * Mathf.Deg2Rad) * 1.8f), Time.deltaTime);
					
							if (xsp > top) {
								xsp = Mathf.Lerp (xsp, top, Time.deltaTime * aboveTopDec);
							}
							if (xsp < -top) {
								xsp = Mathf.Lerp (xsp, -top, Time.deltaTime * aboveTopDec);	
							}
						}
					} else {
						//off the edge. launch off
						print ("MISSed" + transform.right);
						controller.collisions.mode = 0;
						//set xsp and ysp based on direction of angle forward
						ysp = Mathf.Clamp ((xsp * transform.right).y, -jmp, jmp * 2);
						xsp = Mathf.Clamp ((xsp * transform.right).x, -top * 2, top * 2);
						print (xsp + "  " + ysp);
						bumpTimer = bumpTime;
						launchTimer = launchTime;
					}
				}
		
				//normal mode
				if (controller.collisions.mode == 0) {
					//trail off the low friction trail		
					trailOffTime -= Time.deltaTime * 3f;
					trail.time = Mathf.Lerp (0, trailLifetime, trailOffTime / (trailLifetime));	
					bodyMesh.transform.localPosition = originalBodyPosition;

					foreach (GameObject but in bootButtons) {
						but.GetComponent<Renderer> ().material = originalButtonShader;
					}
			
					if (audioSliding.isPlaying)
						audioSliding.Stop ();
					if (audioClimbing.isPlaying)
						audioClimbing.Stop ();
			
					//reset rotation of transform
					if (launchTimer == 0 && transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270) {
						xsp = -xsp;
					}
					transform.rotation = Quaternion.identity;
					anim.SetBool ("sliding", false); //stop no-fric anim if playing
					anim.SetBool ("hiFricAnim", false); //stop hi-fric anim if playing
					anim.SetBool ("tumblingAnim", false);

					loFricStuff.active = false;
					hiFricStuff.active = false;
			
					//face direction
					float moveDir = Input.GetAxisRaw ("Horizontal");
					if (inputLR != 0)
						faceDir = (moveDir < 0 ? 270 : 90);
			
					bodyMesh.transform.rotation = Quaternion.RotateTowards (bodyMesh.transform.rotation, Quaternion.Euler (0, faceDir, 0), 15);
			
					//slope of ground beneath us
					RaycastHit hit;
					Debug.DrawRay (transform.position + (Vector3.up * 0.5f), -Vector2.up * 1.5f, Color.red);
					float slopeAngle = 0;
					if (Physics.Raycast (transform.position + (Vector3.up * 0.5f), -Vector2.up, out hit, 1.5f, collisionMask)) {
						slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
						if (Vector3.Cross (hit.normal, Vector2.up).z > 0) {
							slopeAngle = 360 - slopeAngle;
						}
						oldSlideAngle = slopeAngle;
					}
			
					//running into wall, 0 out xsp
					if (controller.collisions.below && (controller.collisions.left || controller.collisions.right)) {
						xsp = 0;
					}
			
					//pressing left
					if (inputLR < 0) {
						if (xsp > 0) {
							xsp = Mathf.Lerp (xsp, xsp - dec, Time.deltaTime);
						} else if (xsp > -top) {
							xsp = Mathf.Lerp (xsp, xsp - acc, Time.deltaTime);
						}
					}
					//pressing right
					else if (inputLR > 0) {
						if (xsp < 0) {
							xsp = Mathf.Lerp (xsp, xsp + dec, Time.deltaTime);
						} else if (xsp < top) {
							xsp = Mathf.Lerp (xsp, xsp + acc, Time.deltaTime);
						}
					}
					//not pressing anything, gravity kicks in
					else if (controller.collisions.below) {
						xsp = Mathf.Lerp (xsp, xsp - (Mathf.Min (Mathf.Abs (xsp), frc) * Mathf.Sign (xsp)), Time.deltaTime);
					} 
					else if (ysp > 0 && ysp < 1) {	//air drag
						if (Mathf.Abs (xsp) > 0.05f)
							xsp = xsp * 0.995f;
					}
			
					//air/jump movement
					//nothing below us, add gravity
					if (!controller.collisions.below) {
						ysp = Mathf.Lerp (ysp, ysp + grv, Time.deltaTime);
						if (ysp <= -0.1) {
							anim.SetBool ("jumping", true);
							jumping = true;
						}
					}
					//if we're in collision with the ground and press "jump", we jump
					if (Input.GetButtonDown("Jump") && controller.collisions.below && canMove) {
						ysp = (jmp + Mathf.Abs (xsp) * .1f) * Mathf.Cos (slopeAngle * Mathf.Deg2Rad); //add a little bit of x-speed to jump
						xsp = xsp - jmp * Mathf.Sin (slopeAngle * Mathf.Deg2Rad);
						anim.SetBool ("jumping", true);
						jumping = true;
					
					} else if (jumping && controller.collisions.below) {
						anim.SetBool ("jumping", false);
						jumping = false;
						ysp = 0;
					}
			
					//if we let go of jump button early
					if (Input.GetButtonUp ("Jump") && ysp > jmp * .3f && ysp < jmp) {
						ysp = jmp * .3f;
					}
			
					//accelerate going downhill, slow down going uphill
					if (slopeAngle > 30 && slopeAngle < 330) {
						xsp = Mathf.Lerp (xsp, xsp - (slp * Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * (controller.collisions.climbingSlope ? 2f : 1)), Time.deltaTime);
					}
			
					anim.SetFloat ("inputH", Mathf.Abs (xsp));
					anim.SetFloat ("inputV", ysp);
			
					//cap to max speed
					if (controller.collisions.below && launchTimer <= 0) {
						if (xsp > top) {
							xsp = Mathf.Lerp (xsp, top, Time.deltaTime * aboveTopDec);
						}
						if (xsp < -top) {
							xsp = Mathf.Lerp (xsp, -top, Time.deltaTime * aboveTopDec);	
						}
					} else if (!controller.collisions.below) {
						if (xsp > top * 3) {
							xsp = Mathf.Lerp (xsp, top * 3, Time.deltaTime * aboveTopDec);
						}
						if (xsp < -top * 3) {
							xsp = Mathf.Lerp (xsp, -top * 3, Time.deltaTime * aboveTopDec);	
						}
					}

					controller.Move (new Vector3 (xsp, ysp, 0));
				}
			
				//high friction
				//takes player input
				//grip to surface
				//low max speed
				else if (controller.collisions.mode == -1 && bumpTimer <= 0) {
					trailOffTime -= Time.deltaTime * 3f;
					trail.time = Mathf.Lerp (0, trailLifetime, trailOffTime / (trailLifetime));	
					foreach (GameObject but in bootButtons) {
						but.GetComponent<Renderer> ().material = redGlow;
					}
			
					//cap speed
					if (Mathf.Abs (xsp) > hiFricSpCap) {
						if (!onBelt)
							xsp *= .98f; //highfricDecay
					}
					//reset animations of transform
					anim.SetBool ("sliding", false); //stop low-fric anim if playing
					anim.SetBool ("jumping", false);
					anim.SetBool ("tumblingAnim", false);
					jumping = false;
					anim.SetBool ("hiFricAnim", true);

					loFricStuff.active = false;
					hiFricStuff.active = true;

					//face direction
					float moveDir = Input.GetAxisRaw ("Horizontal");
					if (moveDir != 0)
						faceDir = (moveDir < 0 ? 270 : 90);

					bodyMesh.transform.rotation = Quaternion.RotateTowards (bodyMesh.transform.rotation, Quaternion.Euler (0, faceDir, 0), 300 * Time.deltaTime);

					if (!controller.collisions.below) {
						controller.collisions.mode = 0;
					} else {
						//pressing left
						if (inputLR < 0) {
							if (xsp > 0 && xsp < hiFricSpCap) {
								xsp = Mathf.Lerp (xsp, xsp - hiAcc, Time.deltaTime);
							} else if (xsp > -hiFricSpCap) {
								xsp = Mathf.Lerp (xsp, xsp - hiAcc, Time.deltaTime);
							}
						}
						//pressing right
						else if (inputLR > 0) {
							if (xsp < 0 && xsp > -hiFricSpCap) {
								xsp = Mathf.Lerp (xsp, xsp + hiAcc, Time.deltaTime);
							} else if (xsp < hiFricSpCap) {
								xsp = Mathf.Lerp (xsp, xsp + hiAcc, Time.deltaTime);
							}
						}
						//not pressing anything, friction kicks in
						else if (!jumping && !onBelt) {
							xsp *= .95f; //highfriction decay no input
						} else if (ysp > 0 && ysp < 1 && !onBelt) {	//air drag
							xsp = 0;
						}
				
						anim.SetFloat ("inputH", Mathf.Abs (xsp));
				
						//jumping
						if (Input.GetButtonDown("Jump") && controller.collisions.below) {
							controller.collisions.mode = 0;
							ysp = Mathf.Clamp ((jmp * transform.up).y, -jmp, jmp * 2);
							xsp = Mathf.Clamp ((jmp * transform.up).x + (xsp * transform.right).x, -top * 2, top * 2);
							anim.SetBool ("jumping", true);
							anim.SetBool ("hiFricAnim", false);
							jumping = true;
							controller.Move (new Vector3 (xsp, ysp, 0));
							launchTimer = launchTime;
						} else {
							//check raycasts of character
							RaycastHit leftRayInfo, rightRayInfo;
							if (doubleRaycastDown (out leftRayInfo, out rightRayInfo)) {
								//both rays hit something. now move and rotate character
								slidePosition (leftRayInfo, rightRayInfo);
						
								//Falling Upside down
								if (oldSlideAngle > 90 && oldSlideAngle < 270) {
									ysp = 0;
								}
							} else {
								//off the edge. bump to normal mode
								print ("MISSed");
								controller.collisions.mode = 0;
								bumpTimer = bumpTime;
							}
						}
				
						//decrement energy
						hiFricEnergy = Mathf.Clamp (hiFricEnergy - Time.deltaTime * hiFricEnergyDec, 0, 1);
						if (hiFricEnergy == 0)
							//bumpTimer = barTime;
							controller.collisions.mode = 2;
					}
				}
			}
		}
	}
	
	bool doubleRaycastDown(out RaycastHit leftRayInfo, out RaycastHit rightRayInfo){
		
		float rayLength = 1.5f;
		Vector2 centerBox = boxCollider.bounds.center;
		Vector2 transformRight = transform.right;
		
		//make sure rays shoot from right place
		controller.UpdateRaycastOrigins();
		
		Vector2 slideOffset = xsp * transform.right;
		Vector2 updatedBottomLeft = centerBox - ((boxCollider.size.x * .5f) * transformRight) + slideOffset;
		Vector2 updatedBottomRight = centerBox + ((boxCollider.size.x * .5f) * transformRight) + slideOffset;
		
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
			Debug.DrawRay(transform.position+transform.up, -transform.right * 0.4f, Color.red);
			if(Physics.Raycast(transform.position+transform.up, -transform.right, 0.4f, collisionMask)){
				//hit a wall
				xsp = acc;
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
			Debug.DrawRay(transform.position+transform.up, transform.right * 0.4f, Color.red);
			if(Physics.Raycast(transform.position+transform.up, transform.right, 0.4f, collisionMask)){
				//hit a wall
				xsp = -acc;
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
		
		//*********
		if(Mathf.Abs(leftRayInfo.normal.x) + Mathf.Abs(rightRayInfo.normal.y) == 2 || Mathf.Abs(leftRayInfo.normal.y) + Mathf.Abs(rightRayInfo.normal.x) == 2){
			print("NOT ALLOWED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			controller.collisions.mode = 0;
			return;
		}
		
		Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
		Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationDegrees);
		transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);

		float moveDir = Input.GetAxisRaw("Horizontal");
		if(moveDir != 0 && controller.collisions.mode != 2)
			faceDir = (moveDir<0 ? 270 : 90);
			
		bodyMesh.transform.rotation = Quaternion.Euler(faceDir==90? 360-finalRotation.eulerAngles.z : finalRotation.eulerAngles.z, faceDir, 0);

		float slopeAngle = Vector2.Angle(averageNormal, Vector2.up);
				if(Vector3.Cross(averageNormal, Vector2.up).z > 0){
					slopeAngle = 360 - slopeAngle;
				}
		
		//actual movement
		if(xsp != 0){
			transform.position = averagePoint + transform.up*.15f;
		}
		oldSlideAngle = slopeAngle;
	}
	
	public void bounce(Vector3 bounceAmt){
		// can't use on the ground
		//controller.collisions.mode = 0;
		//bumpTimer = bumpTime;
		if (controller.collisions.mode != -1) {
			xsp = bounceAmt.x;
			ysp = bounceAmt.y;
		}
	}

	public void forceField(bool isFan, Vector3 pushAmt, Vector3 maxPush, bool isTouching){
		if (isFan) {
			//not high fric mode
			if (controller.collisions.mode != -1) {
				if(xsp < top*4f)
					xsp += pushAmt.x;
				if (maxPush.y > ysp){
					ysp += pushAmt.y;
				}
			}
		} else {
			if (controller.collisions.below) {
				if (controller.collisions.mode != 1) {
					if (Mathf.Abs (maxPush.x) > Mathf.Abs (xsp)) {
						xsp += pushAmt.x;
					}
				}
			}
			onBelt = isTouching;
		}
	}
		

	public void gain(float gainAmt){
		hiFricEnergy += gainAmt;
		audioPickup.Play();
	}
	
	//called when out of life
	public void defeated(){
		gameOver = true;
		audioDespawn.Play();
		//gameOverOverlay.gameObject.SetActive(true);
		//gameOverText.gameObject.SetActive(true);

		//Make our character invisible
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		for(int i = 0; i < renderers.Length; i++){
			renderers[i].enabled = false;
		}

		//Instantiate our death plane
		Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z - 1.5f);
		deathFXInstance = (GameObject)Instantiate (deathFX, pos, Quaternion.Euler (270, 0, 0));

		//anim.Play("Defeated");
		print("game over");
	}
	
	public int getMode(){
		return controller.collisions.mode;
	}
}

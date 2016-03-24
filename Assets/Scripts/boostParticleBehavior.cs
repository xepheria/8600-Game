using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class boostParticleBehavior : MonoBehaviour {

	private ParticleSystem ps;
	private ParticleSystem.Particle[] p;
	public float vortexHeight;
	public float vacHeight;
	public float velocityScale;
	public float yMaxVel;
	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		p = new ParticleSystem.Particle[ps.maxParticles];

	
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 vortexCenter = new Vector3 (0, vacHeight + vHeight / 2f, 0);
		int pCount = ps.GetParticles (p);

		for (int i = 0; i < pCount; i++) {
			float vScale = 1;
			Vector3 pPos = p [i].position;


			//If it is below the vortex, accelerate in
			if (pPos.y < vacHeight) {
				vScale = -(vacHeight - pPos.y) / vacHeight;

			//if it is in the vortex, accelerate only upwards.
			} else if (pPos.y > vacHeight && pPos.y < (vacHeight + vortexHeight)) {
				vScale = 0;

			//If it is above the vortex, accelerate out
			} else {
				vScale = (pPos.y - (vacHeight + vortexHeight)) / vacHeight;
			}
			//y velocity scales from 3 to 0.0

			//Get distance from vortex center and normalize it
			float yVel = Mathf.Abs(((vortexHeight/2f)+vacHeight - pPos.y)/((vortexHeight/2f)+vacHeight));
			yVel = yMaxVel -  yMaxVel * yVel;
			if (yVel < 0.3f) {
				yVel = 0.3f;
			}

			p [i].velocity = new Vector3 (pPos.x, 0,pPos.z);	
			p [i].velocity *= vScale * velocityScale;
			p [i].velocity += new Vector3 (0, yVel, 0);
			Vector3.ClampMagnitude (p [i].velocity, 2);



		}
		//Vortex
		//Clamp our final velocity
		ps.SetParticles(p, pCount);
	}
}

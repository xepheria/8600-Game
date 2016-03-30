using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class cloudsBehavior : MonoBehaviour {

	private ParticleSystem particleSystem;
	private ParticleSystem.Particle[] particles;
	// Use this for initialization
	void Start () {
		particleSystem = GetComponent<ParticleSystem> ();
		particles = new ParticleSystem.Particle[particleSystem.maxParticles];
	}

	// Update is called once per frame
	void Update () {		
		int pCount = particleSystem.GetParticles (particles);

		float containerLength = particleSystem.shape.box.x;
		//print("pCount: " + pCount);
		for (int i = 0; i < pCount; i++) {
			//print ("Particles Start-----------------------");
			if (particles [i].position.x > (containerLength/2f)) {
				print ("pos: " + particles [i].position.x);
				//particles [i].position.Set(-1 * (containerLength / 2), particles[i].position.y, particles[i].position.z);
				//Vector3 diff = new Vector3(-containerLength, 0, 0);
				particles [i].position -= new Vector3(containerLength, 0, 0);
				print ("NEW pos: " + particles [i].position.x);

			}
		}
		particleSystem.SetParticles (particles, pCount);
	
	}
}

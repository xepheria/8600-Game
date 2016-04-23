using UnityEngine;
using System.Collections;

public class deathSequenceScript : MonoBehaviour {


	public string ResourceFolder;
	public int FPS;
	private Object[] objects;
	private Texture[] textures;
	private Renderer r;
	private	Material mat;

	private int frameCounter = 0;
	private float startTime;
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		this.objects = Resources.LoadAll (ResourceFolder, typeof(Texture));
		//print (objects.Length);
		this.textures = new Texture[objects.Length];
		//this.mat = GetComponent<Material> ();
		this.r = GetComponent<Renderer>();
		this.mat = r.material;
		//this.sprites = new Sprite[objects.Length];
		//sr =  GetComponent<SpriteRenderer> ();
		//mainSprite = sr.sprite;


		for (int i = 0; i < objects.Length; i++) {
			this.textures [i] = (Texture)this.objects [i];
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		if (frameCounter < this.objects.Length) {
			StartCoroutine ("PlayLoop", 0.00f);
			mat.mainTexture = textures [frameCounter];
			//frameCounter = (++frameCounter) % textures.Length;	
		} else {
			Destroy (this.gameObject);
			Application.LoadLevel(Application.loadedLevel);

		}

	}

	IEnumerator PlayLoop(float delay){
		yield return new WaitForSeconds (delay);
		frameCounter += 2;	
		StopCoroutine ("PlayLoop");
	}
		
}

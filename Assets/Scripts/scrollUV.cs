using UnityEngine;
using System.Collections;

public class scrollUV : MonoBehaviour {

	private MeshRenderer mr;
	
	void Start(){
		mr = GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update () {
		Material mat = mr.material;
		Vector2 offset = mat.mainTextureOffset;
		offset.x -= Time.deltaTime*0.05f;
		mat.mainTextureOffset = offset;
	}
}

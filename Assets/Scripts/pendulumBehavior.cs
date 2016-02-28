using UnityEngine;
using System.Collections;

public class pendulumBehavior : MonoBehaviour {

	private float angle = 90f;
	public float speed = 1.5f;
	Quaternion pLeft, pRight;

	// Use this for initialization
	void Start () {
		pLeft = Quaternion.AngleAxis(-angle, Vector3.forward);
		pRight = Quaternion.AngleAxis(angle, Vector3.forward);
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Lerp(pLeft, pRight, (Mathf.Sin(speed * Time.time + Mathf.PI/2) + 1.0f)/2.0f);
	}
}

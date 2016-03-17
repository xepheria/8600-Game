using UnityEngine;
using System.Collections;

public class swivelSpaceShip : MonoBehaviour {

	private float angle = 5f;
	public float speed = .5f;
	Vector3 pLeft, pRight;

	// Use this for initialization
	void Start () {
		pLeft = transform.rotation.eulerAngles + new Vector3(-angle, 0, 0);
		pRight = transform.rotation.eulerAngles + new Vector3(angle, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler(Vector3.Lerp(pLeft, pRight, (Mathf.Sin(speed * Time.time + Mathf.PI/2) + 1.0f)/2.0f));
	}
}

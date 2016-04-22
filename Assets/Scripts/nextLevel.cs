using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ParticleSystem))]
public class nextLevel : MonoBehaviour {
	
	GameObject teleporter;
	bool activated = false;
	Player player;
	public AnimationClip teleportAnim;
	public Transform floatTowards;
	private ParticleSystem ps;

	void Start(){
		ps = transform.Find("beamParticles").GetComponent<ParticleSystem> ();
		ps.Stop ();
		ps.Clear ();
		teleporter = GameObject.Find("teleporter");
		player = FindObjectOfType<Player>();
	}
	
	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			ps.Play ();
			teleporter.GetComponent<Animator>().SetTrigger("active");
			player.GetComponent<Animator>().SetTrigger("teleport");
			activated = true;
			player.canMove = false;
			player.enabled = false;
			StartCoroutine(LoadAfterAnim());
		}
	}
	
	void Update(){
		if(activated){
			player.transform.position = Vector3.Lerp(player.transform.position, floatTowards.position+Vector3.up, Time.deltaTime*5f);
		}
	}
	
	IEnumerator LoadAfterAnim(){
		//print(teleportAnim.length);
		yield return new WaitForSeconds(teleportAnim.length - 0.05f);
		player.resetSpawnTimer ();
		foreach (Renderer r in player.GetComponentsInChildren<Renderer>()) {
			r.enabled = false;
		}
		yield return new WaitForSeconds (0.05f);
		PlayerPrefs.SetInt ("currentLevel", PlayerPrefs.GetInt ("currentLevel") + 1); 
		//print(PlayerPrefs.GetInt("currentLevel"));
		string nextLevel = "level" + PlayerPrefs.GetInt("currentLevel").ToString(); 
		if (PlayerPrefs.GetInt ("currentLevel") > 8) {
			PlayerPrefs.SetInt ("currentLevel", 0);
			SceneManager.LoadScene ("Credits");
		} else {

			SceneManager.LoadScene (nextLevel);
		}
	}
}

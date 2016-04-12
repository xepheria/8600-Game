using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class nextLevel : MonoBehaviour {
	
	GameObject teleporter;
	bool activated = false;
	Player player;
	public AnimationClip teleportAnim;
	public Transform floatTowards;
	
	void Start(){
		teleporter = GameObject.Find("teleporter");
		player = FindObjectOfType<Player>();
	}
	
	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
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
		yield return new WaitForSeconds(teleportAnim.length);
		player.resetSpawnTimer ();
		PlayerPrefs.SetInt ("currentLevel", PlayerPrefs.GetInt ("currentLevel") + 1); 
		//print(PlayerPrefs.GetInt("currentLevel"));
		string nextLevel = "level" + PlayerPrefs.GetInt("currentLevel").ToString(); 
		if (PlayerPrefs.GetInt ("currentLevel") > 6) {
			PlayerPrefs.SetInt ("currentLevel", 0);
			SceneManager.LoadScene ("Credits");
		} else {

			SceneManager.LoadScene (nextLevel);
		}
	}
}

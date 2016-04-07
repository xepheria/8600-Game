using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class nextLevel : MonoBehaviour {
	
	GameObject teleporter;
	Player player;
	public AnimationClip teleportAnim;
	
	void Start(){
		teleporter = GameObject.Find("teleporter");
		player = FindObjectOfType<Player>();
	}
	
	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			teleporter.GetComponent<Animator>().SetTrigger("active");
			player.GetComponent<Animator>().SetTrigger("teleport");
			player.canMove = false;
			StartCoroutine(LoadAfterAnim());
		}
	}
	
	IEnumerator LoadAfterAnim(){
		print(teleportAnim.length);
		yield return new WaitForSeconds(teleportAnim.length);

		PlayerPrefs.SetInt ("currentLevel", PlayerPrefs.GetInt ("currentLevel") + 1); 
		print(PlayerPrefs.GetInt("currentLevel"));
		string nextLevel = "level" + PlayerPrefs.GetInt("currentLevel").ToString(); 
		if (PlayerPrefs.GetInt ("currentLevel") > 6) {
			PlayerPrefs.SetInt ("currentLevel", 0);
			SceneManager.LoadScene ("Credits");
		} else {
			SceneManager.LoadScene (nextLevel);
		}
	}
}

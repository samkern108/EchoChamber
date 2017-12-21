using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public GameObject deathExplosion;
	// Totally not working. CharacterController2D is the suspect.
	/*public void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Projectile" || coll.gameObject.tag == "EnemyProjectile" || coll.gameObject.tag == "Ghost") {
			Die ();
		}
	}*/

	// Player collides fatally with Enemy, EnemyProjectile, and Ghost.
	// Player collides notfatally with Exit.
	public void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.layer != LayerMask.NameToLayer ("Exit")) {
			Die ();
		}
	}

	private void Die() {
		NotificationMaster.SendPlayerDeathNotification ();
		AudioManager.PlayPlayerDeath ();
		Instantiate (deathExplosion, transform.position, Quaternion.identity);
		gameObject.SetActive (false);
	}
}

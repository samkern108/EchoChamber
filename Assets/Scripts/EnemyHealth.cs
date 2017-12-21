using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	public GameObject deathExplosion;

	// Enemy collides fatally with Projectile.
	public void OnTriggerEnter2D(Collider2D coll) {
		Die ();
	}

	private void Die() {
		AudioManager.PlayEnemyDeath ();
		Instantiate (deathExplosion, transform.position, Quaternion.identity);
		GameObject.Destroy (this.gameObject);
	}
}

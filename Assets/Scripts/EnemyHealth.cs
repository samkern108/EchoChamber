using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	// Enemy collides fatally with Projectile.
	public void OnTriggerEnter2D(Collider2D coll) {
		Die ();
	}

	private void Die() {
		AudioManager.PlayEnemyDeath ();
		GameObject.Destroy (this.gameObject);
	}
}

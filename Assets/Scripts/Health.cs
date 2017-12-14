using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	private LayerMask layerMask;

	public void Start() {
		layerMask = 1 << LayerMask.NameToLayer ("Enemy") | LayerMask.NameToLayer("Projectile") | LayerMask.NameToLayer("EnemyProjectile");
	}

	public void OnCollisionEnter2D(Collision2D coll) {
		Debug.Log ("Collision " + LayerMask.LayerToName(coll.gameObject.layer));
		if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Projectile" || coll.gameObject.tag == "EnemyProjectile") {
			Debug.Log ("Collided with " + coll.gameObject.name); 
			Die ();
		}
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		Debug.Log ("Trigger Collision " + LayerMask.LayerToName(coll.gameObject.layer));
		if (coll.tag == "Enemy" || coll.tag == "Projectile" || coll.tag == "EnemyProjectile") {
			Debug.Log ("Trigger collided with " + coll.gameObject.name);
			Die ();
		}
	}

	private void Die() {
		if (gameObject.tag == "Player") {
			AudioManager.PlayPlayerDeath ();
			UIManager.instance.Stop ();
		} else {
			AudioManager.PlayEnemyDeath ();
		}
		GameObject.Destroy (this.gameObject);
	}
}

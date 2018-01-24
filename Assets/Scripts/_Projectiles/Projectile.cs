﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	protected Vector3 moveVector;
	public GameObject p_explosion;

	public void Initialize(Vector3 direction, float speed) {
		moveVector = direction * speed;
	}

	void Update () {
		transform.position += (moveVector * Time.deltaTime);
	}

	public void OnBecameInvisible() {
		Destroy (gameObject);
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (LayerMask.LayerToName (collider.gameObject.layer) == "Wall") {
			Explode ();
		}
		Destroy (gameObject);
	}

	private void Explode() {
		// Make sure we're including all things projectiles can hit
		RaycastHit2D hit = Physics2D.Raycast(transform.position, moveVector.normalized, 2f, 1 << LayerMask.NameToLayer("Wall"));
		if (hit.collider) {
			GameObject explosion = Instantiate (p_explosion);

			float size;
			if (GetComponent <PolygonCollider2D> () != null) {
				size = GetComponent<PolygonCollider2D> ().bounds.size.x;
			} else {
				size = GetComponent<BoxCollider2D> ().bounds.size.x;
			}

			explosion.transform.position = transform.position - (moveVector.normalized * 2 * size/3);

			Vector2 reflection = Vector2.Reflect (moveVector.normalized, hit.normal);
			explosion.transform.rotation = Quaternion.LookRotation(reflection,Vector3.up);

			AudioManager.PlayProjectileExplode ();
		}

		Destroy (gameObject);
	}
}

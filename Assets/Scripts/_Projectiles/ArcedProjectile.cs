using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcedProjectile : Projectile {

	private float angle;

	public void Initialize(Vector3 direction, float speed, float angle) {
		moveVector = direction * speed;
	}

	void Update () {
		transform.position += (moveVector * Time.deltaTime);
	}
}

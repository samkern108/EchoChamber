using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class GhostAttack : MonoBehaviour {

	private GameObject projectile;

	private bool shooting = false;
	private float detectionRampUp = .7f;
	private float shootCooldown = 1.0f;
	private float projectileSpeed = 7.0f;
	private Animate animate;

	public void Start() {
		animate = GetComponent<Animate> ();
		projectile = ResourceLoader.LoadProjectile("EnemyMissile");
	}

	public void StartShooting() {
		CancelInvoke ();
		Invoke ("Shoot", detectionRampUp);
	}

	public void StopShooting() {
		CancelInvoke ();
	}

	private void Shoot() {
		AudioManager.PlayEnemyShoot ();
		animate.AnimateToColorAndBack (Palette.EnemyColor, Color.red, .2f);
		Vector3 direction = (PlayerController.PlayerPosition - transform.position).normalized;
		GameObject missile = Instantiate (projectile, ProjectileManager.myTransform);
		missile.transform.position = transform.position;
		missile.GetComponent <Missile>().Initialize(direction, projectileSpeed);

		Invoke ("Shoot", shootCooldown);
	}
}
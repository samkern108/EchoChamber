using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBrain : MonoBehaviour {

	public GhostAIStats stats;
	private GhostMovement movement;
	private GhostAttack attack;

	public void Start() {
		movement = GetComponent <GhostMovement>();
		attack = GetComponent <GhostAttack>();
	}

}

public class GhostMovement : MonoBehaviour {

	public void Wander() {
	}

	public void HuntPlayer() {
	}

	public void RunAway() {
	}

	public void ChasePlayer() {
	}
}
	
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
		if (!shooting)
			StartCoroutine ("Co_Shoot");
	}

	public void StopShooting() {
		shooting = false;
	}

	private IEnumerator Co_Shoot()
	{
		shooting = true;
		yield return new WaitForSeconds (detectionRampUp);
		while (shooting) {
			Shoot ();
			yield return new WaitForSeconds (shootCooldown);
		}
	}

	private void Shoot() {
		AudioManager.PlayEnemyShoot ();
		animate.AnimateToColorAndBack (Palette.EnemyColor, Color.red, .2f);
		Vector3 direction = (PlayerController.PlayerPosition - transform.position).normalized;
		GameObject missile = Instantiate (projectile, ProjectileManager.myTransform);
		missile.transform.position = transform.position;
		missile.GetComponent <Missile>().Initialize(direction, projectileSpeed);
	}
}
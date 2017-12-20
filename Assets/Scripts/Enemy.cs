using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public GameObject projectile;

	private LayerMask layerMask;
	private Bounds floor;

	private bool detectedPlayer = false;
	private float detectionRadius = 4.0f;

	private Vector3 startPosition, persistentTargetPosition;

	private Animate animate;
	private Vector3 size;

	void Awake() {
		GetComponent <SpriteRenderer> ().color = Palette.Invisible;
	}

	void Start () {
		size = GetComponent <SpriteRenderer>().bounds.extents;

		startPosition = Room.GetRandomPointOnFloorAvoidingPoints (new Vector2[]{PlayerController.PlayerPosition}, size);
		transform.position = startPosition;

		RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.down, 20.0f, 1 << LayerMask.NameToLayer("Wall"));
		floor = hit.collider.bounds;

		layerMask = 1 << LayerMask.NameToLayer ("Wall") | 1 << LayerMask.NameToLayer("Player");

		animate = GetComponent <Animate>();
		animate.AnimateToColor (Palette.Invisible, Color.yellow, .3f);
	}

	void Update () {
		RaycastHit2D hit = Physics2D.Linecast (transform.position, PlayerController.PlayerPosition, layerMask);
		if (hit.collider.tag == "Wall" || (hit.collider.tag == "Player" && hit.distance > detectionRadius)) {
			//MoveToStart ();
			if (detectedPlayer) {
				detectedPlayer = false;
				animate.AnimateToColor (Palette.EnemyColor, Color.yellow, .3f);
			}
			return;
		} else {
			//MoveToPlayer ();
			if (!detectedPlayer) {
				detectedPlayer = true;
				animate.AnimateToColor (Color.yellow, Palette.EnemyColor, detectionRampUp);

				if (!shooting) {
					StartCoroutine ("Co_Shoot");
				}
			}
		}
	}

	public float smoothDampTime = 0.2f;
	private void MoveToPlayer() {
		Vector3 targetPosition = PlayerController.PlayerPosition;
		targetPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		targetPosition.y = transform.position.y;
		Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, 2.0f * Time.deltaTime);
		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;
	}

	private void MoveToStart() {
		Vector3 targetPosition = startPosition;
		targetPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		targetPosition.y = transform.position.y;
		Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, .5f * Time.deltaTime);
		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;
	}

	private bool shooting = false;
	private float detectionRampUp = .3f;
	private float shootCooldown = 1.0f;
	private float projectileSpeed = 7.0f;
	private IEnumerator Co_Shoot()
	{
		shooting = true;
		yield return new WaitForSeconds (detectionRampUp);
		while (detectedPlayer) {
			Shoot ();
			yield return new WaitForSeconds (shootCooldown);
		}
		shooting = false;
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
